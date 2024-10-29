using DealsForYou.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DealsForYou.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public IActionResult LogUserIn(LoginModel model) {
            if (!ModelState.IsValid) {
                return View();
            }

            string username = model.Username;
            string password = model.Password;

            int id;
            bool admin;

            (id, admin) = DB.Login(username, password);
            if (id == 0) {
                return View();
            }

            HttpContext.Session.SetInt32("user_id", id);

            if (admin) {
                AppStateModel.State = 2;
                return RedirectToAction("Index", "Admin");

            } else {
                AppStateModel.State = 1;
                return RedirectToAction("Index", "Users");
            }

        }

        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterUser(RegisterModel model) {
            if (ModelState.IsValid) {
                try {
                    if (DB.CreateUser(model, false)) {
                        return Redirect("Login");
                    } else {
                        return Redirect("Register");
                    }
                
                } catch(Exception) {
                    return Redirect("Register");
                }
            }
            return Redirect("Register");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
