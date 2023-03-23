using CleanDDTest.Data;
using CleanDDTest.Models;
using Microsoft.AspNetCore.Mvc;

namespace CleanDDTest.Controllers
{
    public class DBTestController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DBTestController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<AdminUser> objUserList = _db.AdminUsers;


            return View(objUserList);
        }
    }
}
