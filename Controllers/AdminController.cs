using CleanDDTest.Data;
using CleanDDTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using CleanDDTest.Services;

namespace CleanDDTest.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AdminController> _logger;
        //public AdminController(ApplicationDbContext db)
        //{
        //    _db = db;
        //}

        // reset password email address
        const string emailContact = "wchandl5@utk.edu";


        public AdminController(ILogger<AdminController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
//            _dataService = dataservice;

        }

        // max number of logins and max password size
        const int maxLogins = 3;
        const int maxPwordSize = 15;

        // =================  LOGIN CHECKER -------------------------
        public bool IsLoggedIn()
        {
            bool loggedIn = false;
            // see if we are already logged in
            ViewBag.ShowMenu = false;

            var sessionAdminLevel = HttpContext.Session.GetString("SessionKeyAccessLevel");
            int loginTries = Convert.ToInt32(HttpContext.Session.GetInt32("SessionKeyLoginTries"));

            // check and see if session logged in started login process
            if (loginTries == 0 && string.IsNullOrEmpty(sessionAdminLevel))
            {
                //no session so set defaults (or reset)
                HttpContext.Session.SetString("SessionKeyUserName", "");
                HttpContext.Session.SetString("SessionKeySessionId", Guid.NewGuid().ToString());
                HttpContext.Session.SetString("SessionKeyAccessLevel", "");
                HttpContext.Session.SetString("SessionKeyFirstLogin", "No");
                HttpContext.Session.SetInt32("SessionKeyLockedOut", 0);
                HttpContext.Session.SetInt32("SessionKeyLoginTries", 0);
                return false;
            }

            if (string.IsNullOrEmpty(sessionAdminLevel))
            {
                return false;
            }

            //// get session info
            //var sessionUsername = HttpContext.Session.GetString("SessionKeyUsername");
            //var sessionId = HttpContext.Session.GetString("SessionKeySessionId");
            //sessionAdminLevel = HttpContext.Session.GetString("SessionKeyAccessLevel");
            //var sessionFirstLogin = HttpContext.Session.GetString("SessionKeyFirstLogin");
            loggedIn = true;

            // check 
            return loggedIn;
        }
        // -----------------------  end of login checker 

        
        // =================  LOGIN -------------------------------
        public IActionResult Login()
        {
            // check and see if user is allowed into the area

            ViewBag.LockedOut = Convert.ToInt32(HttpContext.Session.GetInt32("SessionKeyLockedOut"));
            ViewBag.EmailContact = emailContact;
            return View();
        }

        //POST
        [HttpPost, ActionName("Login")]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel obj)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Invalid application state.  Contact technical support.";
                return View();
            }

            // check login status
            int loginTries = Convert.ToInt32(HttpContext.Session.GetInt32("SessionKeyLoginTries"));
            ViewBag.LockedOut = 0;
            //if (loginTries >= maxLogins)
            //{
            //    HttpContext.Session.SetInt32("SessionKeyLockedOut", 1);
            //    ViewBag.LockedOut = 1;
            //    return View();
            //}

            // GET Login
            string loginEmail = obj.UserEmail;
            string userPword = obj.UserPword;

            // check for invalid lengths
            if (loginEmail.Length > 50 || userPword.Length > maxPwordSize)
            {
                // set lengths to 0 to trigger invalid login
                loginEmail = string.Empty; userPword = string.Empty;
            }


            //// **************   UPDATE AFTER ADD AUTHENTICATION   ***********************
            //// check for authentication login here
            //// if not same person fail login
            // if (authenticated email != loginEmail){
            //  set loginEmail 
            //}


            userPword = EncryptString(userPword);

            if (!string.IsNullOrEmpty(loginEmail) && !string.IsNullOrEmpty(userPword))
            {
                var userFromDb = _db.AdminUsers.FirstOrDefault(u => u.UserEmail == loginEmail && u.UserPword == userPword && u.UserStatus == 1);
                // check login
                if (userFromDb != null)
                {
                    // matched!  Login successful - update variables and let the user in
                    HttpContext.Session.SetString("SessionKeyUserName", loginEmail);
                    HttpContext.Session.SetString("SessionKeyAccessLevel", userFromDb.UserRole);
                    HttpContext.Session.SetInt32("SessionKeyLockedOut", 0);
                    HttpContext.Session.SetInt32("SessionKeyLoginTries", 0);

                    // write cookies

                    if (userFromDb.PasswordReset)
                    {
                        HttpContext.Session.SetString("SessionKeyFirstLogin", "Yes");
                        ViewBag.ErrorMessage = "This is your first login.  Please change your password.";
                        // goto reset password screen

                        return RedirectToAction("ChangePassword");
                    }
                    else
                    {
                        HttpContext.Session.SetString("SessionKeyFirstLogin", "No");
                        return RedirectToAction("Index");
                    }
                }
            }
            // no match found...
            //invalid login - email not found, update logintries before lockout
            loginTries = Convert.ToInt32(HttpContext.Session.GetInt32("SessionKeyLoginTries"));

            ViewBag.EmailContact = emailContact;

            //lock after three tries
            loginTries++;
            if (loginTries >= maxLogins)
            {
                HttpContext.Session.SetInt32("SessionKeyLoginTries", loginTries);
                ViewBag.ErrorMessage = "Invalid login credentials.  You have exceeded the login attempts and have been temporarily locked out.";
                HttpContext.Session.SetInt32("SessionKeyLockedOut", 1);
                ViewBag.LockedOut = 1;
                return View();
            }
            int triesLeft = maxLogins -loginTries;

            HttpContext.Session.SetInt32("SessionKeyLoginTries", loginTries);
            ViewBag.ErrorMessage = "Invalid login credentials.  Please try again. You have (" + triesLeft + ") attempts left.";
            return View();
        }
        // ------------------------- end of login ------------------


        // -===================  LOG OUT  --------------------------
        public IActionResult Logout()
        {
            // clean session and cookies
            HttpContext.Session.SetString("SessionKeyUserName", "");
            HttpContext.Session.SetString("SessionKeySessionId", "");
            HttpContext.Session.SetString("SessionKeyAccessLevel", "");
            HttpContext.Session.SetString("SessionKeyFirstLogin", "");
            HttpContext.Session.SetInt32("SessionKeyLockedOut", 0);
            //            HttpContext.Session.SetInt32("SessionKeyLoginTries", 0);
            return View();
        }


        // =====================   CHANGE PASSWORD   ----------------------------
        public IActionResult ChangePassword()
        {
            ////verify user is looged in and can change password
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }
            ViewBag.userEmail = HttpContext.Session.GetString("SessionKeyUserName");
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePassword obj, string? ValidateUserPword, string? newUserPword, string? userPword)
        {

            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            string userEmail = HttpContext.Session.GetString("SessionKeyUserName");
            // validate input information
            if (string.IsNullOrEmpty(ValidateUserPword) || string.IsNullOrEmpty(newUserPword) || string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userPword) || string.IsNullOrEmpty(ValidateUserPword))
            {
                ViewBag.userEmail = userEmail;
                ViewBag.ErrorMessage = "Please comlete all of the fields.";
                return View();
            }

            string passWord1 = newUserPword;
            string passWord2 = ValidateUserPword;

            // check for invalid lengths
            if (passWord1.Length > maxPwordSize || passWord2.Length > maxPwordSize)
            {
                // set lengths to 0 to trigger invalid login
                ViewBag.ErrorMessage = "Password is to long.  Max password size = " + maxPwordSize + ".";
                return View();
            }

            // check login
            var userFromDb = _db.AdminUsers.FirstOrDefault(u => u.UserEmail == userEmail && u.UserPword == EncryptString(userPword) && u.UserStatus == 1);
            if (userFromDb == null)
            {

                //invalid login - email not found, update logintries before lockout
                int loginTries = Convert.ToInt32(HttpContext.Session.GetInt32("SessionKeyLoginTries"));

                loginTries++;
                if (loginTries >= maxLogins)
                {
                    HttpContext.Session.Clear();
                    HttpContext.Session.SetInt32("SessionKeyLoginTries", loginTries);
                    //                    ViewBag.ErrorMessage = "Invalid login credentials.  You have exceeded the login attempts and have been temporarily locked out.";
                    HttpContext.Session.SetInt32("SessionKeyLockedOut", 1);
                    ViewBag.LockedOut = 1;
                    return RedirectToAction("Login");
                }
                HttpContext.Session.SetInt32("SessionKeyLoginTries", loginTries);


                ViewBag.ErrorMessage = "Invalid login in.";

                return View();
            }

            if (ModelState.IsValid)
            {
                //encrypt password
                userFromDb.UserPword = EncryptString(passWord1);

                //remove reset flag
                userFromDb.PasswordReset = false;

                //save data
                _db.AdminUsers.Update(userFromDb);
                _db.SaveChanges();
                TempData["success"] = "User updated successfully.";

                // remove changepassword session flag
                HttpContext.Session.SetString("SessionKeyFirstLogin", "No");


                return RedirectToAction("Index");
            }
                ViewBag.ModelErrors = "Invalid model.";
            return View();
        }
        // --------------- end of change password  ---------------------


        public IActionResult Index()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }
            //check and see if we need to change the password, if so redirect
            if (HttpContext.Session.GetString("SessionKeyFirstLogin") == "Yes")
            {
                return RedirectToAction("ChangePassword");
            }

            return View();
        }
        public IActionResult DDReview(string viewtype, string searchString)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }
            //check and see if we need to change the password, if so redirect
            if (HttpContext.Session.GetString("SessionKeyFirstLogin") == "Yes")
            {
                return RedirectToAction("ChangePassword");
            }


            var ddreviewViewModel = from ubank in _db.Bankaccounts
                                    join upeeps in _db.Users on ubank.EmpId equals upeeps.EmpId
                                    join ucheck in _db.Voidedchecks on ubank.Id equals ucheck.BankAcctId
                                    into ucheckInfo
                                    from ucheckall in ucheckInfo.DefaultIfEmpty()
                                    select new DDReviewViewModel
                                    {
                                        firstName = upeeps.FirstName,
                                        lastName = upeeps.LastName,
                                        middleInitial = upeeps.MiddleInit,
                                        employeeID = upeeps.EmpId,
                                        netID = upeeps.NetId,
                                        payPeriod = upeeps.PayPeriod,
                                        routingNumber = ubank.RoutingNumber,
                                        accountType = ubank.AccountType,
                                        bankName = ubank.BankName,
                                        cityState = ubank.CityState,
                                        checkingOrSavings = ubank.Account,
                                        accountNum = ubank.AccountNum,
                                        dollarAmount = ubank.DollarAmount,
                                        dateReceived = ubank.DateReceived,
                                        dateProcessed = ubank.DateProccessed,
                                        bankAccountID = ubank.Id,
                                        checkImage = ucheckall.FileName
                                    };


            switch (viewtype)
            {
                case "completed":
                    ViewBag.ViewInfo = "completed";
                    //(all three tables) with new model
                    ddreviewViewModel = ddreviewViewModel.Where(ubank => ubank.dateProcessed != null);
                    break;
                case "all":
                    ViewBag.ViewInfo = "all";
                    // get active records (dateprocessed = null)
                    //(all three tables) with new model
                    // ddreviewViewModel = ddreviewViewModel.Where(ubank => ubank.dateProcessed == null);
                    break;
                default:
                    // get active records
                    ViewBag.ViewInfo = "pending";
                    ddreviewViewModel = ddreviewViewModel.Where(ubank => ubank.dateProcessed == null);
                    break;
            }

            return View(ddreviewViewModel);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DDReview(IEnumerable<DDReviewViewModel> dDReviewViewModels)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            //check and see if we need to change the password, if so redirect
            if (HttpContext.Session.GetString("SessionKeyFirstLogin") == "Yes")
            {
                return RedirectToAction("ChangePassword");
            }

            if (ModelState.IsValid)
            {
                int i = 0;
                var rowID = HttpContext.Request.Form["rowID"];
                int totalSelected = rowID.Count;
                if (totalSelected > 0)
                {
                    foreach (string id in rowID)
                    {
                        var dateProcessed = "";
                        string accountNum = "";
                        string netID = "";
                        string empID = "";
                        string formBankID = "";
                        int bankID;
                        dateProcessed = HttpContext.Request.Form["ddInfo[" + id + "].dateProcessed"];
                        // if not empty date, then update record
                        if (!string.IsNullOrEmpty(dateProcessed.ToString()))
                        {
                            // =================  THIS SHOULD JUST NEED TO GET THE PRIMARY ID OF THE BANKACCOUNT TABLE!! 
                            // =================   NEED TO ADD A PRIMARY KEY TO THIS....
                            // get accountNum
                            accountNum = HttpContext.Request.Form["ddInfo[" + id + "].accoutnNum"];
                            // get netID
                            netID = HttpContext.Request.Form["ddInfo[" + id + "].netID"];
                            // get employee id
                            empID = HttpContext.Request.Form["ddInfo[" + id + "].employeeID"];
                            // get bankID
                            formBankID = HttpContext.Request.Form["ddInfo[" + id + "].ID"];

                            // make sure we have a valid bankID
                            if (string.IsNullOrEmpty(formBankID))
                            {
                                // give invalid bank id 
                                ViewBag.errorMessage = "Invalid bank account ID";
                                ViewBag.accountNum = accountNum;
                                ViewBag.empID = empID;
                                return View();
                            }
                            else
                            {
                                //convert formbankid to int
                                if (Int32.TryParse(formBankID, out bankID))
                                {
                                    var userAccounts = _db.Bankaccounts.FirstOrDefault(u => u.EmpId == empID && u.Id == bankID);
                                    //update data
                                    if (userAccounts != null)
                                    {
                                        userAccounts.DateProccessed = DateTime.Now;
                                        try
                                        {
                                            _db.SaveChanges();
                                        }
                                        catch (Exception)
                                        {
                                            // get specifics and see
                                            throw;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return Redirect("DDReview");
            }
            else
            {
                return View(dDReviewViewModels);
            }
        }

        // ======  USER MANAGEMENT AREA  -----------------------------------------
        public IActionResult Users()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }
            //var objUserList = _db.AdminUsers.ToList();

            //check and see if we have permission to be here
            if (HttpContext.Session.GetString("SessionKeyAccessLevel") is not "Admin" and not "SuperAdmin")
            {
                return RedirectToAction("Index");
            }

            //check and see if we need to change the password, if so redirect
            if (HttpContext.Session.GetString("SessionKeyFirstLogin") == "Yes")
            {
                return RedirectToAction("ChangePassword");
            }

            IEnumerable<AdminUser> objUserList = _db.AdminUsers;


            return View(objUserList);
        }

        //GET
        public IActionResult CreateUser()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            //check and see if we have permission to be here
            if (HttpContext.Session.GetString("SessionKeyAccessLevel") is not "Admin" and not "SuperAdmin")
            {
                return RedirectToAction("Index");
            }
            //check and see if we need to change the password, if so redirect
            if (HttpContext.Session.GetString("SessionKeyFirstLogin") == "Yes")
            {
                return RedirectToAction("ChangePassword");
            }

            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(AdminUser obj, string ValidateUserPword)
        {
            // check for matching passwords
            string passWord1 = obj.UserPword;
            string passWord2 = ValidateUserPword;

            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }
            //check and see if we have permission to be here
            if (HttpContext.Session.GetString("SessionKeyAccessLevel") is not "Admin" and not "SuperAdmin")
            {
                return RedirectToAction("Index");
            }
            // ADD VALIDaTION to check if user exists ....
            var userFromDb = _db.AdminUsers.FirstOrDefault(u => u.UserEmail == obj.UserEmail);

            if (userFromDb != null)
            {
                ViewBag.ErrorMessage = "The user (email) currently exists as an admin.";
                return View(obj);
            }

            // validate that we don't have blank fields
            if (string.IsNullOrEmpty(obj.FirstName) || string.IsNullOrEmpty(obj.LastName) || string.IsNullOrEmpty(obj.UserEmail) || string.IsNullOrEmpty(obj.UserPword) || string.IsNullOrEmpty(obj.UserRole) || string.IsNullOrEmpty(ValidateUserPword))
            {
                ViewBag.ErrorMessage = "Please complete all of the fields.";
                return View(obj);
            }


            // verify if passwords patch
            if (passWord1 != passWord2)
            {
                ViewBag.ErrorMessage = "The passwords do not match.";
                return View(obj);
            }

            //set password
            // if user does not exist, add user
            if (ModelState.IsValid)
            {
                // encryp new password 
                obj.UserPword = EncryptString(obj.UserPword);

                // set default password Rest 
                obj.PasswordReset = true;

                // set default status to active
                obj.UserStatus = 1;


                _db.AdminUsers.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "User created successfully.";
                return RedirectToAction("Users");
            }
            ViewBag.ModelErrors = "Invalid model.";

            return View(obj);
        }

        //GET  ================  EDIT USER
        public IActionResult EditUser(int? id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }
            //check and see if we have permission to be here
            if (HttpContext.Session.GetString("SessionKeyAccessLevel") is not "Admin" and not "SuperAdmin")
            {

                return RedirectToAction("Index");
            }
            //check and see if we need to change the password, if so redirect
            if (HttpContext.Session.GetString("SessionKeyFirstLogin") == "Yes")
            {
                return RedirectToAction("ChangePassword");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }


            //var userFromDbFirst = _db.AdminUsers.FirstOrDefault(u => u.Id == id);
            //var userFromDbSingle = _db.AdminUsers.SingleOrDefault(u => u.Id == id);

            var userFromDb = _db.AdminUsers.Find(id);

            if (userFromDb == null)
            {
                return NotFound();
            }


            //// check and see if this is superadmin and do we have permissions
            if (userFromDb.UserRole == "SuperAdmin" && HttpContext.Session.GetString("SessionKeyAccessLevel") != "SuperAdmin")
            {
                return RedirectToAction("Users");
            }

            return View(userFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult EditUser(AdminUser obj, string? resetUserPassword, string? ValidateUserPword, string? newUserPword)
        public IActionResult EditUser(AdminUser obj, string? resetUserPassword, string? ValidateUserPword)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }
            //check and see if we have permission to be here
            if (HttpContext.Session.GetString("SessionKeyAccessLevel") is not "Admin" and not "SuperAdmin")
            {

                return RedirectToAction("Index");
            }
            //check and see if we need to change the password, if so redirect
            if (HttpContext.Session.GetString("SessionKeyFirstLogin") == "Yes")
            {
                return RedirectToAction("ChangePassword");
            }

            // VALIDATION
            // validate that we don't have blank fields
            if (string.IsNullOrEmpty(obj.FirstName) || string.IsNullOrEmpty(obj.LastName) || string.IsNullOrEmpty(obj.UserEmail) || string.IsNullOrEmpty(obj.UserRole))
            {
                ViewBag.ErrorMessage = "Please complete all of the required (*) fields.";
                return View(obj);
            }

            // get current user pword reset information
            // use AsNoTracking() here. we don't want to track this model for saving/updating.  We just want to get information.
            var userInDb = _db.AdminUsers.AsNoTracking().FirstOrDefault(u => u.UserEmail == obj.UserEmail && u.Id== obj.Id);
            string passWord1 = string.Empty;

            if (resetUserPassword == "yes")
            {
                // check for matching passwords
                passWord1 = obj.UserPword;
                string passWord2 = ValidateUserPword;


                if (string.IsNullOrEmpty(passWord1) || string.IsNullOrEmpty(passWord2))
                {
                    ViewBag.ErrorMessage = "Please enter in the new password and verify password.";
                    return View();
                }

                if (passWord1 != passWord2)
                {
                    ViewBag.ErrorMessage = "Passwords do not match.";
                    return View();
                }
            }


            if (ModelState.IsValid)
            {
                // validate passwords if need to change if so get new one and encrypt it
                if (resetUserPassword == "yes")
                {
                    //encrypt password
                    obj.UserPword = EncryptString(passWord1);

                    // set reset flag
                    obj.PasswordReset = true;
                }
                else
                {
                    // password not changed so keep same password
                    obj.UserPword = userInDb.UserPword;
                }
                //save data
                _db.AdminUsers.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "User updated successfully.";
                return RedirectToAction("Users");
            }
            ViewBag.ModelErrors = "Invalid model.";

            return View(obj);
        }

        //GET
        public IActionResult DeleteUser(int? id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }
            //check and see if we have permission to be here
            if (HttpContext.Session.GetString("SessionKeyAccessLevel") is not "Admin" and not "SuperAdmin")
            {

                return RedirectToAction("Index");
            }

            //check and see if we need to change the password, if so redirect
            if (HttpContext.Session.GetString("SessionKeyFirstLogin") == "Yes")
            {
                return RedirectToAction("ChangePassword");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var userFromDb = _db.AdminUsers.Find(id);

            // check and see if this is a super admin and have permissinos


            //var userFromDbFirst = _db.AdminUsers.FirstOrDefault(u => u.Id == id);
            //var userFromDbSingle = _db.AdminUsers.SingleOrDefault(u => u.Id == id);

            if (userFromDb == null)
            {
                return NotFound();
            }

            //// check and see if this is superadmin and do we have permissions
            if (userFromDb.UserRole == "SuperAdmin" && HttpContext.Session.GetString("SessionKeyAccessLevel") != "SuperAdmin")
            {
                return View(userFromDb);
            }

            return View(userFromDb);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login");
            }

            //check and see if we have permission to be here
            if (HttpContext.Session.GetString("SessionKeyAccessLevel") is not "Admin" and not "SuperAdmin")
            {

                return RedirectToAction("Index");
            }
            //check and see if we need to change the password, if so redirect
            if (HttpContext.Session.GetString("SessionKeyFirstLogin") == "Yes")
            {
                return RedirectToAction("ChangePassword");
            }


            var obj = _db.AdminUsers.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            _db.AdminUsers.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "User deleted successfully.";
            return RedirectToAction("Users");
        }
        // --------------  end of user management area

        // =========================================  ENCRYPTION 
        private string EncryptString(string clearText)
        {
            string encryptionKey = "PsP1N1022TU2BRO";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return clearText;
        }
        // -------------------------  end of encryption
    }
}
