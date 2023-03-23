using CleanDDTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CleanDDTest.Data;
using Microsoft.EntityFrameworkCore;
using CleanDDTest.Services;
using System.Security.Cryptography;
using System;

namespace DirectDeposit_local.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataService _dataService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IDataService dataservice)
        {
            _context = context;
            _dataService = dataservice;

        }

        // ================  THANK YOU PAGE  
        public IActionResult ThankYou()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Index()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> OnPost(ViewModel mymodel, string? newOrAdd)
        {
            mymodel.User.EmpId = "1984";
            var empId = mymodel.User.EmpId;
            mymodel.Account.EmpId = mymodel.User.EmpId;
            //mymodel.Account.DateReceived = DateTime.Now;
            mymodel.SecondaryAccount.EmpId = mymodel.Account.EmpId;
            mymodel.TravelAccount.EmpId = mymodel.Account.EmpId;

            bool gotErrors = false;
            string errorMessages = string.Empty;
            //validate if user information is person info is here - fn, ln, netid, TNID


            //validate if user selected payperiod
            string payPeriod = mymodel.User.PayPeriod;
            if (string.IsNullOrEmpty(payPeriod))
            {
                gotErrors = true;
                errorMessages += "<li>Please select your pay period.</li>";
            }

            //validate if user selected new or update bank information
            
            if (string.IsNullOrEmpty(newOrAdd)) {
                gotErrors= true;
                errorMessages += "<li>Please select if this is your first time or your are updating your direct deposit.</li>";
            }

            //--------  BANK INFORMAITON VALIDATION   -------------------
            // PRIMARY  *****
            if (HttpContext.Request.Form["select-primary-bank"] == "1")
            {
                // -- validate routing numbers not blank
                if (string.IsNullOrEmpty(mymodel.Account.RoutingNumber))
                {
                    gotErrors = true;
                    errorMessages += "<li>Please enter your primary bank's routing number.</li>";
                }
                // - validate verify routing
                string routingNumber2 = HttpContext.Request.Form["verify-primary-routing-number"];
                if (string.IsNullOrEmpty(routingNumber2))
                {
                    gotErrors = true;
                    errorMessages += "<li>Please verify your primary bank's routing number.</li>";
                }

                // - compare routing numbers
                if (mymodel.Account.RoutingNumber != routingNumber2)
                {
                    gotErrors = true;
                    errorMessages += "<li>The primary bank routing numbers do not match.</li>";
                }

                // - validate bank account not blank
                if (string.IsNullOrEmpty(mymodel.Account.AccountNum))
                {
                    gotErrors = true;
                    errorMessages += "<li>Please enter your primary bank's account number.</li>";
                }
                // - validate account number
                string accountNumber2 = HttpContext.Request.Form["verify-primary-account-number"];
                if (string.IsNullOrEmpty(accountNumber2))
                {
                    gotErrors = true;
                    errorMessages += "<li>Please verify your primary bank's account number.</li>";
                }
                // - compare acocunt numbers
                if (mymodel.Account.AccountNum != accountNumber2)
                {
                    gotErrors = true;
                    errorMessages += "<li>The primary account numbers do not match.</li>";

                }


                // validate bank name not blank
                if (string.IsNullOrEmpty(mymodel.Account.BankName))
                {
                    // missing 
                    gotErrors = true;
                    errorMessages += "<li>Please enter your primary bank's name.</li>";
                }

                // validate city/state not blank
                if (string.IsNullOrEmpty(mymodel.Account.CityState))
                {
                    // missing 
                    gotErrors = true;
                    errorMessages += "<li>Please enter your primary bank's city and state.</li>";
                }

                // validate checking or savings
                if (string.IsNullOrEmpty(mymodel.Account.Account))
                {
                    // missing 
                    gotErrors = true;
                    errorMessages += "<li>Please select if this primary account is a checking or savings account.</li>";
                }
                // image validation?
                // image validation?
                var primaryImageName = HttpContext.Request.Form["FileModel.ImageFileP"];
                foreach (var file in primaryImageName)
                {
                    if (file.Length == 0)
                    {
                        gotErrors = true;
                        errorMessages += "<li>Please upload a picture of your primary account bank routing/account information.</li>";
                    }
                }

            }
            // --------------  end of primary validation

            // if secondary account
            if (HttpContext.Request.Form["select-secondary-bank"] == "1")
            {
                // -- validate routing numbers not blank
                if (string.IsNullOrEmpty(mymodel.SecondaryAccount.RoutingNumber))
                {
                    gotErrors = true;
                    errorMessages += "<li>Please enter your secondary bank's routing number.</li>";
                }
                // - validate verify routing
                string routingNumber2 = HttpContext.Request.Form["verify-secondary-routing-number"];
                if (string.IsNullOrEmpty(routingNumber2))
                {
                    gotErrors = true;
                    errorMessages += "<li>Please verify your secondary bank's routing number.</li>";
                }

                // - compare routing numbers
                if (mymodel.SecondaryAccount.RoutingNumber != routingNumber2)
                {
                    gotErrors = true;
                    errorMessages += "<li>The secondary routing numbers do not match.</li>";
                }

                // - validate bank account not blank
                if (string.IsNullOrEmpty(mymodel.SecondaryAccount.AccountNum))
                {
                    gotErrors = true;
                    errorMessages += "<li>Please enter your secondary bank's account number.</li>";
                }
                // - validate account number
                string accountNumber2 = HttpContext.Request.Form["verify-secondary-account-number"];
                if (string.IsNullOrEmpty(accountNumber2))
                {
                    gotErrors = true;
                    errorMessages += "<li>Please verify your secondary bank's account number.</li>";
                }
                // - compare acocunt numbers
                if (mymodel.SecondaryAccount.AccountNum != accountNumber2)
                {
                    gotErrors = true;
                    errorMessages += "<li>The secondary bank account numbers do not match.</li>";

                }


                // validate bank name not blank
                if (string.IsNullOrEmpty(mymodel.SecondaryAccount.BankName))
                {
                    // missing 
                    gotErrors = true;
                    errorMessages += "<li>Please enter your secondary bank's name.</li>";
                }

                // validate city/state not blank
                if (string.IsNullOrEmpty(mymodel.SecondaryAccount.CityState))
                {
                    // missing 
                    gotErrors = true;
                    errorMessages += "<li>Please enter your secondary bank's city and state.</li>";
                }

                // validate checking or savings
                if (string.IsNullOrEmpty(mymodel.SecondaryAccount.Account))
                {
                    // missing 
                    gotErrors = true;
                    errorMessages += "<li>Please select if this secondary account is a checking or savings account.</li>";
                }

                // image validation?
                var secondaryImageName = HttpContext.Request.Form["FileModel.ImageFileS"];
                foreach (var file in secondaryImageName)
                {
                    if (file.Length == 0)
                    {
                        gotErrors = true;
                        errorMessages += "<li>Please upload a picture of your secondary account bank routing/account information.</li>";
                    }
                }
            }
            // ---- end of secondary validation

            // if travel
            if (HttpContext.Request.Form["select-travel-bank"] == "1")
            {
                // -- validate routing numbers not blank
                if (string.IsNullOrEmpty(mymodel.TravelAccount.RoutingNumber))
                {
                    gotErrors = true;
                    errorMessages += "<li>Please enter your travel bank's routing number.</li>";
                }
                // - validate verify routing
                string routingNumber2 = HttpContext.Request.Form["verify-travel-routing-number"];
                if (string.IsNullOrEmpty(routingNumber2))
                {
                    gotErrors = true;
                    errorMessages += "<li>Please verify your travel bank's routing number.</li>";
                }

                // - compare routing numbers
                if (mymodel.TravelAccount.RoutingNumber != routingNumber2)
                {
                    gotErrors = true;
                    errorMessages += "<li>The travel bank routing numbers do not match.</li>";
                }

                // - validate bank account not blank
                if (string.IsNullOrEmpty(mymodel.TravelAccount.AccountNum))
                {
                    gotErrors = true;
                    errorMessages += "<li>Please enter your travel bank's account number.</li>";
                }
                // - validate account number
                string accountNumber2 = HttpContext.Request.Form["verify-travel-account-number"];
                if (string.IsNullOrEmpty(accountNumber2))
                {
                    gotErrors = true;
                    errorMessages += "<li>Please verify your travel bank's account number.</li>";
                }
                // - compare acocunt numbers
                if (mymodel.TravelAccount.AccountNum != accountNumber2)
                {
                    gotErrors = true;
                    errorMessages += "<li>The travel account numbers do not match.</li>";

                }


                // validate bank name not blank
                if (string.IsNullOrEmpty(mymodel.TravelAccount.BankName))
                {
                    // missing 
                    gotErrors = true;
                    errorMessages += "<li>Please enter your travel bank's name.</li>";
                }

                // validate city/state not blank
                if (string.IsNullOrEmpty(mymodel.TravelAccount.CityState))
                {
                    // missing 
                    gotErrors = true;
                    errorMessages += "<li>Please enter your travel bank's city and state.</li>";
                }

                // validate checking or savings
                if (string.IsNullOrEmpty(mymodel.TravelAccount.Account))
                {
                    // missing 
                    gotErrors = true;
                    errorMessages += "<li>Please select if this travel account is a checking or savings account.</li>";
                }
                // IMAGE VALIDATION
                var travelImageName = HttpContext.Request.Form["FileModel.ImageFileT"];
                foreach (var file in travelImageName)
                {
                    if (file.Length == 0)
                    {
                        gotErrors = true;
                        errorMessages += "<li>Please upload a picture of your travel account bank routing/account information.</li>";
                    }
                }

            }
            // ---- end of travel validation


            // --------   return if we have errors and alert user........
            if (gotErrors == true)
            {
                ViewBag.errorMessages = "<ul>"+errorMessages+"</ul>";
                return View("Index");
            }



            // ----------------  Check if user exists in DB if so, update, if not, add new
            var userExists = _context.Users.AsNoTracking().FirstOrDefault(x => x.EmpId == empId);
            if (userExists != null)
            {
                //update userinfo
                userExists.PayPeriod = mymodel.User.PayPeriod;
                userExists.LastName = mymodel.User.LastName;
                userExists.FirstName = mymodel.User.FirstName;
                userExists.NetId = mymodel.User.NetId;


                _context.Users.Update(userExists);
            }
            else
            {
                //add user info
                _context.Users.Add(mymodel.User);
            }


            if (mymodel.Account.AccountNum != null || mymodel.Account.RoutingNumber != null)
            {
                mymodel.Account.DateReceived = DateTime.Now;
                mymodel.Account.AccountType = "Primary";
                await _context.Bankaccounts.AddAsync(mymodel.Account);


            }
            if (mymodel.SecondaryAccount.AccountNum != null || mymodel.SecondaryAccount.RoutingNumber != null)
            {
                if (mymodel.SecondaryAccount.DollarAmount == null)
                {
                    mymodel.SecondaryAccount.DollarAmount = 0;
                }
                mymodel.SecondaryAccount.DateReceived = DateTime.Now;
                mymodel.SecondaryAccount.AccountType = "Secondary";
                await _context.Bankaccounts.AddAsync(mymodel.SecondaryAccount);


            }
            if (mymodel.TravelAccount.AccountNum != null || mymodel.TravelAccount.RoutingNumber != null)
            {
                if (mymodel.TravelAccount.DollarAmount == null)
                {
                    mymodel.TravelAccount.DollarAmount = 0;
                }
                mymodel.TravelAccount.DateReceived = DateTime.Now;
                mymodel.TravelAccount.AccountType = "Travel";
                await _context.Bankaccounts.AddAsync(mymodel.TravelAccount);


            }


            _context.SaveChanges();



            await SaveVoidedCheckAsync(mymodel);
            return View("ThankYou");
        }


        public async Task SaveVoidedCheckAsync(ViewModel mymodel)
        {
            Voidedcheck voidcheckP = new Voidedcheck();
            Voidedcheck voidcheckS = new Voidedcheck();
            Voidedcheck voidcheckT = new Voidedcheck();

            if (mymodel.Account.AccountNum != null)
            {
                voidcheckP.AccountNum = mymodel.Account.AccountNum;
                voidcheckP.FileName = _dataService.GetImage(mymodel.FileModel.ImageFileP);
                voidcheckP.BankAcctId = mymodel.Account.Id;
                voidcheckP.EmpId = mymodel.User.EmpId;
                _context.Voidedchecks.Add(voidcheckP);
            }
            if (mymodel.SecondaryAccount.AccountNum != null)
            {
                voidcheckS.AccountNum = mymodel.SecondaryAccount.AccountNum;
                voidcheckS.FileName = _dataService.GetImage(mymodel.FileModel.ImageFileS);
                voidcheckS.BankAcctId = mymodel.SecondaryAccount.Id;
                voidcheckS.EmpId = mymodel.User.EmpId;
                _context.Voidedchecks.Add(voidcheckS);
            }
            if (mymodel.TravelAccount.AccountNum != null)
            {
                voidcheckT.AccountNum = mymodel.TravelAccount.AccountNum;
                voidcheckT.FileName = _dataService.GetImage(mymodel.FileModel.ImageFileT);
                voidcheckT.BankAcctId = mymodel.TravelAccount.Id;
                voidcheckT.EmpId = mymodel.User.EmpId;
                _context.Voidedchecks.Add(voidcheckT);
            }
            await _context.SaveChangesAsync();


        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }

}