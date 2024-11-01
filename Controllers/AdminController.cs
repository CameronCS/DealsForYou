using DealsForYou.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

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

        public IActionResult Offers() {
            List<OfferPreview> previews = DB.GetAllPreviews();
            return View(previews);
        }

        public IActionResult Offer(int id, int user_id, int car_id) {
            FullOffer offer = DB.GetFullOffer(id, user_id, car_id);
            return View(offer);
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

        public IActionResult AcceptOffer(int car_id, int user_id, int offer_id) {
            try {
                int admin_id = HttpContext.Session.GetInt32("user_id") ?? 0;
                if (admin_id == 0) {
                    byte[] stashed = [(byte)car_id, (byte)user_id, (byte)offer_id];
                    HttpContext.Session.Set("stashed", stashed);
                    return RedirectToAction("Index", "Home");
                }
                int transaction_id = DB.AcceptOffer(car_id, user_id, offer_id, admin_id);
                return RedirectToAction("GenerateInvoice", new {
                    id = transaction_id
                });
            } catch (Exception) {
                return Redirect("Offers");
            }
        }

        [HttpGet]
        public IActionResult RejectOffer(int car_id, int user_id, int offer_id) {
            DB.RejectOffer(offer_id);
            return Redirect("Offers");
        }

        public IActionResult GenerateInvoice(int id) {
            var invoiceDetails = DB.GetInvoiceDetails(id); // Assume this method retrieves the details
            if (invoiceDetails == null) {
                byte[] stashed = HttpContext.Session.Get("stashed") ?? new byte[0];
                return RedirectToAction("AcceptOffer", new {
                    car_id = (int)stashed[0],
                    user_id = (int)stashed[1],
                    offer_id = (int)stashed[2]
                });
            }
            invoiceDetails.TransactionID = id;
            HttpContext.Session.Remove("stashed");
            return View(invoiceDetails);
        }

        [HttpPost]
        public IActionResult GeneratePdf(int invoiceId) {
            var invoiceDetails = DB.GetInvoiceDetails(invoiceId);
            if (invoiceDetails == null) {
                return RedirectToAction("GenerateInvoice", "Admin", new {
                    id = invoiceDetails.OfferId
                });
            }

            byte[] pdfData = this.GenerateInvoicePdf(invoiceDetails);
            DB.SaveInvoicePdf(invoiceDetails, pdfData);

            return RedirectToAction("Index", "Admin");
        }

        public IActionResult ViewAllInvoices() {
            List<AddedInvoices> added = DB.GetAllInvoices();
            return View(added);
        }

        public IActionResult ViewSpecific(int id) {
            ViewSpecificInvoice invoice = DB.GetSpecificInvoice(id);
            return View(invoice);
        }

        public IActionResult DownloadInvoice(int id) {
            var invoice = DB.GetSpecificInvoice(id);
            if (invoice == null) {
                return NotFound();
            }

            return File(invoice.File, invoice.FileType, invoice.FileName);
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

        public byte[] GenerateInvoicePdf(InvoiceDetails invoiceDetails) {
            MemoryStream memoryStream = new();
            iTextSharp.text.Document document = new(PageSize.A4, 36, 36, 54, 54); // Set page margins
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            // Title Styling
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
            var sectionHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.DARK_GRAY);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);

            // Add title
            Paragraph title = new Paragraph($"Invoice ID: DFYINV{invoiceDetails.ID}", titleFont) {
                Alignment = Element.ALIGN_CENTER
            };
            document.Add(title);
            document.Add(new Paragraph(" ")); // Spacer

            // User Details Section
            document.Add(new Paragraph("User Details", sectionHeaderFont));
            PdfPTable userDetailsTable = new PdfPTable(2);
            userDetailsTable.WidthPercentage = 100;
            userDetailsTable.SetWidths(new float[] { 1, 2 }); // Set column widths

            // Add user details with styled cells
            AddStyledCell(userDetailsTable, "Username");
            AddStyledCell(userDetailsTable, invoiceDetails.Username);
            AddStyledCell(userDetailsTable, "Name");
            AddStyledCell(userDetailsTable, $"{invoiceDetails.FirstName} {invoiceDetails.LastName}");
            AddStyledCell(userDetailsTable, "Email");
            AddStyledCell(userDetailsTable, invoiceDetails.Email);
            AddStyledCell(userDetailsTable, "Cell");
            AddStyledCell(userDetailsTable, invoiceDetails.Cell);

            document.Add(userDetailsTable);
            document.Add(new Paragraph(" ")); // Spacer

            // Car Details Section
            document.Add(new Paragraph("Car Details", sectionHeaderFont));
            PdfPTable carDetailsTable = new PdfPTable(2);
            carDetailsTable.WidthPercentage = 100;
            carDetailsTable.SetWidths(new float[] { 1, 2 });

            // Add car details with styled cells
            AddStyledCell(carDetailsTable, "Make");
            AddStyledCell(carDetailsTable, invoiceDetails.Make);
            AddStyledCell(carDetailsTable, "Model");
            AddStyledCell(carDetailsTable, invoiceDetails.Model);
            AddStyledCell(carDetailsTable, "Year");
            AddStyledCell(carDetailsTable, invoiceDetails.Year.ToString());
            AddStyledCell(carDetailsTable, "VIN");
            AddStyledCell(carDetailsTable, invoiceDetails.Vin);
            AddStyledCell(carDetailsTable, "License");
            AddStyledCell(carDetailsTable, invoiceDetails.License);
            AddStyledCell(carDetailsTable, "Price");
            AddStyledCell(carDetailsTable, invoiceDetails.CarPrice.ToString("C"));

            document.Add(carDetailsTable);
            document.Add(new Paragraph(" ")); // Spacer

            // Invoice Details Section
            document.Add(new Paragraph("Invoice Details", sectionHeaderFont));
            PdfPTable invoiceDetailsTable = new PdfPTable(2);
            invoiceDetailsTable.WidthPercentage = 100;
            invoiceDetailsTable.SetWidths(new float[] { 1, 2 });

            AddStyledCell(invoiceDetailsTable, "Offer Amount");
            AddStyledCell(invoiceDetailsTable, invoiceDetails.OfferAmount.ToString("C"));

            document.Add(invoiceDetailsTable);
            document.Add(new Paragraph(" ")); // Spacer

            // Monthly Payments Section
            document.Add(new Paragraph("Monthly Payments", sectionHeaderFont));
            if (invoiceDetails.Months == 0) {
                // One-off payment
                document.Add(new Paragraph("This is a one-time payment.", normalFont));
            } else {
                // Regular monthly payments
                PdfPTable paymentTable = new PdfPTable(2);
                paymentTable.WidthPercentage = 100;
                paymentTable.SetWidths(new float[] { 1, 2 });

                for (int i = 0; i < invoiceDetails.Months; i++) {
                    AddStyledCell(paymentTable, $"Month {i + 1}");
                    AddStyledCell(paymentTable, invoiceDetails.Monthly.ToString("C"));
                }
                document.Add(paymentTable);
            }
            document.Add(new Paragraph(" ")); // Spacer

            // Total Amount Section
            document.Add(new Paragraph("Total Amount", sectionHeaderFont));
            document.Add(new Paragraph($"Total: {invoiceDetails.Total.ToString("C")}", titleFont));

            document.Close();
            return memoryStream.ToArray();
        }

        // Helper method to add styled cells to a table
        private void AddStyledCell(PdfPTable table, string text) {
            PdfPCell cell = new PdfPCell(new Phrase(text, FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK)));
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.BackgroundColor = BaseColor.WHITE;
            cell.Padding = 8; // Padding for a nicer look
            table.AddCell(cell);
        }
    }
}
