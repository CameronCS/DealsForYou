using DealsForYou.Models;
using Microsoft.AspNetCore.Mvc;

namespace DealsForYou.Controllers {
    public class AdminController : Controller {
        public IActionResult Index() {
            List<CurrentStock> current = DB.GetCurrent();
            return View(current);
        }

        [HttpGet]
        public IActionResult AddStock() {
            return View();
        }
        
        [HttpPost]
        public IActionResult AddStock(StockModel m) {
            return View(m);
        }

        public IActionResult Profile() {
            int id = HttpContext.Session.GetInt32("user_id") ?? 0;

            User newUser = DB.GetUserDetails(id);

            return View(newUser);
        }

        [HttpPost]
        public IActionResult AddToSystem(StockModel mymod) {
            if (ModelState.IsValid && mymod.UploadedFile != null) {
                if (DB.AddStock(mymod)) {
                    return Redirect("Index"); // Return to the AddStock view with the model for validation errors
                    
                }
                return View("AddStock", mymod); // Return to the AddStock view with the model for validation errors
            }
            return View("AddStock", mymod); // Return to the AddStock view with the model for validation errors
        }



        public IActionResult LogOut() {
            AppStateModel.State = 0;
            return RedirectToAction("Index", "Home");
        }
    }
}
