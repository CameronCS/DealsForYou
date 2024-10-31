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

        [HttpGet]
        public IActionResult ViewDetails(int id) {
            CurrentStock selected = DB.GetStockByID(id);
            return View(selected);
        }

        public IActionResult Payment(int id) {
            CurrentStock select = DB.GetStockByID(id);
            return View(select);
        }

        public IActionResult OfferSubmitted() {
            int id = HttpContext.Session.GetInt32("user_id") ?? 0;
            User user = DB.GetUserDetails(id);
            return View(user);
        }

        public IActionResult SubmitOffer(int id, int price, int offer, int months, string interest, int monthly, int total) {
            int userid = HttpContext.Session.GetInt32("user_id") ?? 0;

            int interest_int = Convert.ToInt32(interest.Remove(interest.IndexOf('%')));

            if (DB.CreateOffer(userid, id, price, offer, months, interest_int, monthly, total)) {
                return RedirectToAction("OfferSubmitted");
            } else {
                return RedirectToAction("Payment", id);
            }
        }

        public IActionResult LogOut() {
            HttpContext.Session.Clear();
            AppStateModel.State = 0;
            return RedirectToAction("Index", "Home");
        }
    }
}
