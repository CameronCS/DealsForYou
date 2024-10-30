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

        [HttpGet]
        public IActionResult Edit(int ID) {
            CurrentStock current = DB.GetStockByID(ID);
            return View(current);
        }

        [HttpPost]
        public IActionResult EditPrice(int id, int price) {
            if (DB.EditPriceById(id, price)) {
                return Redirect("Index");
            } else {
                return RedirectToAction("Edit", id);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id) {
            CurrentStock stock = DB.GetStockByID(id);
            if (DB.DeleteItemById(id)) {
                return View(stock);
            } else {
                return Redirect("Index");
            }
        }

        public IActionResult LogOut() {
            AppStateModel.State = 0;
            return RedirectToAction("Index", "Home");
        }
    }
}
