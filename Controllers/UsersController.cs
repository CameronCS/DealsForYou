using DealsForYou.Models;
using Microsoft.AspNetCore.Mvc;

namespace DealsForYou.Controllers {
    public class UsersController : Controller {
        public IActionResult Index() {
            List<CurrentStock> current = DB.GetCurrent();
            return View(current);
        }

        public IActionResult Profile() {
            int id = HttpContext.Session.GetInt32("user_id") ?? 0;

            User newUser = DB.GetUserDetails(id);

            return View(newUser);
        }

        public IActionResult LogOut() {
            HttpContext.Session.Clear();
            AppStateModel.State = 0;
            return RedirectToAction("Index", "Home");
        }
    }
}
