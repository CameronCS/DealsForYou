using DealsForYou.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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


        // Action to view all transactions
        public IActionResult ViewTransaction() {
            int user_id = HttpContext.Session.GetInt32("user_id") ?? 0;
            var transactions = DB.GetAllTransactions(user_id); // Assume you have a way to get the user_id
            return View(transactions);
        }


        [HttpGet]
        public IActionResult ViewSpecific(int id) {
            var balanceModel = DB.GetSpecificBalance(id);
            return View(balanceModel);
        }

        // Action to show update balance form
        [HttpGet]
        public IActionResult UpdateBalance(int id) {
            var balanceModel = DB.GetSpecificBalance(id); // Fetch balance details for the specified id
            return View(balanceModel);
        }

        // Action to update the transaction balance
        [HttpPost]
        public IActionResult UpdateTransaction(int id, int current) {
            DB.UpdateTransaction(id, current); // Update the current balance in the database

            // Redirect to ViewSpecific to see the updated balance
            return RedirectToAction("ViewSpecific", new {
                id
            });
        }


        public IActionResult ViewAllInvoices() {
            int userId = HttpContext.Session.GetInt32("user_id") ?? 0;
            List<UserInvoiceDetails> invoices = DB.GetAllInvoices(userId);
            return View(invoices);
        }

        [HttpGet]
        public IActionResult ViewInvoice(int invoice_id) {
            UserInvoice userInvoice = DB.GetUserInvoice(invoice_id);
            if (userInvoice == null) {
                return Redirect("ViewAllInvoices");
            }
            return View(userInvoice);
        }

        public IActionResult ViewInvoicePdf(int id) {
            UserInvoice invoice = DB.GetUserInvoice(id);
            if (invoice == null) {
                return Redirect("ViewAllInvoices");
            }

            // Serve the PDF inline for viewing
            return File(invoice.File, invoice.FileType);
        }


        public IActionResult DownloadInvoice(int id) {
            UserInvoice invoice = DB.GetUserInvoice(id);
            if (invoice == null) {
                return Redirect("ViewInvoice");
            }
            return File(invoice.File, invoice.FileType, invoice.InvoiceName + ".pdf");
        }


        public IActionResult LogOut() {
            HttpContext.Session.Clear();
            AppStateModel.State = 0;
            return RedirectToAction("Index", "Home");
        }
    }
}
