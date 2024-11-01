using iTextSharp.text.pdf;
using iTextSharp.text;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Data;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;

namespace DealsForYou.Models {
    public class DB {
        private static readonly string constr = "server=localhost;uid=Part3User;pwd=Password1234567890;database=part3asp";

        public static (int, bool) Login(string username, string password) {
            try {
                using MySqlConnection con = new(constr);
                con.Open();

                string qry = @"SELECT id, isadmin FROM user WHERE username = @un AND password = @pwd";
                using MySqlCommand cmd = new(qry, con);
                cmd.Parameters.AddWithValue("@un", username);
                cmd.Parameters.AddWithValue("@pwd", password);

                using MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();

                return (reader.GetInt32("id"), reader.GetBoolean("isadmin"));

            } catch (Exception ex) {
                Console.WriteLine("An error occurred: " + ex.Message);
                return (0, false);
            }
        }

        public static User GetUserDetails(int id) {
            try {
                using MySqlConnection con = new(constr);
                con.Open();

                string qry = @"SELECT username, firstname, lastname, email, cell FROM user WHERE id = @id";
                using MySqlCommand cmd = new(qry, con);
                cmd.Parameters.AddWithValue("@id", id);

                using MySqlDataReader reader = cmd.ExecuteReader();

                reader.Read();

                User newUser = new() {
                    Username = reader.GetString("username"),
                    FirstName = reader.GetString("firstname"),
                    LastName = reader.GetString("lastname"),
                    Email = reader.GetString("email"),
                    Cell = reader.GetString("cell")
                };

                return newUser;
            } catch (Exception ex) {
                Console.WriteLine("An error occurred: " + ex.Message);
                return new();
            }
        }

        public static bool CreateUser(RegisterModel model, bool isAdmin) {
            try {
                using MySqlConnection con = new(constr);
                con.Open();

                string qry = @"INSERT INTO user (username, password, firstname, lastname, email, cell, isAdmin) 
                       VALUES (@un, @pwd, @fn, @ln, @mail, @cell, @ia)";
                using MySqlCommand cmd = new(qry, con);
                cmd.Parameters.AddWithValue("@un", model.Username);
                cmd.Parameters.AddWithValue("@pwd", model.Password);
                cmd.Parameters.AddWithValue("@fn", model.FirstName);
                cmd.Parameters.AddWithValue("@ln", model.LastName);
                cmd.Parameters.AddWithValue("@mail", model.Email);
                cmd.Parameters.AddWithValue("@cell", model.Cell);
                cmd.Parameters.AddWithValue("@ia", isAdmin);

                cmd.ExecuteNonQuery();
                return true;
            } catch (Exception) {
                return false;
            }
        }


        public static bool AddStock(StockModel model) {
            try {
                using var con = new MySqlConnection(constr);
                con.Open();

                // Prepare image data
                using var memoryStream = new MemoryStream();
                model.UploadedFile.CopyTo(memoryStream);
                byte[] imageData = memoryStream.ToArray();

                string qry = @"INSERT INTO car (make, model, year, vin, license, price, image, image_name, file_type, available) 
                               VALUES (@make, @model, @year, @vin, @license, @price, @image, @imageName, @fileType, @available)";

                using var cmd = new MySqlCommand(qry, con);
                cmd.Parameters.AddWithValue("@make", model.Make);
                cmd.Parameters.AddWithValue("@model", model.Model);
                cmd.Parameters.AddWithValue("@year", model.Year);
                cmd.Parameters.AddWithValue("@vin", model.Vin);
                cmd.Parameters.AddWithValue("@license", model.License);
                cmd.Parameters.AddWithValue("@price", Convert.ToInt32(model.Price));
                cmd.Parameters.AddWithValue("@image", imageData);
                cmd.Parameters.AddWithValue("@imageName", model.UploadedFile.FileName);
                cmd.Parameters.AddWithValue("@fileType", model.UploadedFile.ContentType);
                cmd.Parameters.AddWithValue("@available", true);

                cmd.ExecuteNonQuery();
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public static List<CurrentStock> GetCurrent() {
            List<CurrentStock> currentStockList = [];

            using MySqlConnection con = new(constr);
            con.Open();

            string qry = "SELECT id, make, model, year, vin, license, price, image, image_name, file_type FROM available_cars";
            using MySqlCommand cmd = new(qry, con);
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                ImageModel img = new() {
                    image_data = reader["image"] as byte[] ?? [],
                    FileName = reader.GetString("image_name"),
                    FileType = reader.GetString("file_type")
                };

                CurrentStock stock = new() {
                    ID = reader.GetInt32("id"),
                    Make = reader.GetString("make"),
                    Model = reader.GetString("model"),
                    Year = reader.GetInt32("year"),
                    Vin = reader.GetString("vin"),
                    License = reader.GetString("license"),
                    Price = reader.GetInt32("price"),
                    Image = img
                };

                currentStockList.Add(stock);
            }

            return currentStockList;
        }
        public static CurrentStock GetStockByID(int id) {
            MySqlConnection con = new(constr);
            con.Open();

            string qry = "SELECT id, make, model, year, vin, license, price, image, image_name, file_type FROM car WHERE id = @id";
            MySqlCommand cmd = new(qry, con);
            cmd.Parameters.AddWithValue("@id", id);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            ImageModel img = new() {
                image_data = reader["image"] as byte[] ?? [],
                FileName = reader.GetString("image_name"),
                FileType = reader.GetString("file_type")
            };

            CurrentStock stock = new() {
                ID = reader.GetInt32("id"),
                Make = reader.GetString("make"),
                Model = reader.GetString("model"),
                Year = reader.GetInt32("year"),
                Vin = reader.GetString("vin"),
                License = reader.GetString("license"),
                Price = reader.GetInt32("price"),
                Image = img
            };

            return stock;
        }

        public static bool EditPriceById(int id, int price) {
            MySqlConnection con = new(constr);
            con.Open();

            string qry = "UPDATE car SET price = @price WHERE id = @id";
            MySqlCommand cmd = new(qry, con);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@id", id);
            try {
                cmd.ExecuteNonQuery();
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public static bool DeleteItemById(int id) {
            MySqlConnection con = new(constr);
            con.Open();

            string qry = "DELETE FROM car WHERE id = @id";
            MySqlCommand cmd = new(qry, con);
            cmd.Parameters.AddWithValue("@id", id);
            try {
                cmd.ExecuteNonQuery();
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public static bool CreateOffer(int user_id, int car_id, int price, int offer, int months, int interest, int monthly, int total) {
            try {
                MySqlConnection con = new(constr);
                con.Open();

                string qry = @" INSERT INTO offer (user_id, car_id, price, offer, months, interest, monthly, total) VALUES (@ui, @ci, @p, @o, @m, @i, @mp, @ta)";
                MySqlCommand cmd = new(qry, con);
                cmd.Parameters.AddWithValue("@ui", user_id);
                cmd.Parameters.AddWithValue("@ci", car_id);
                cmd.Parameters.AddWithValue("@p", price);
                cmd.Parameters.AddWithValue("@o", offer);
                cmd.Parameters.AddWithValue("@m", months);
                cmd.Parameters.AddWithValue("@i", interest);
                cmd.Parameters.AddWithValue("@mp", monthly);
                cmd.Parameters.AddWithValue("@ta", total);

                cmd.ExecuteNonQuery();

                return true;

            } catch (Exception) {
                return false;
            }
        }

        public static List<OfferPreview> GetAllPreviews() {
            List<OfferPreview> offers = [];

            MySqlConnection con = new(constr);
            try {
                con.Open();
                string qry = "SELECT * FROM OfferPreviewView";

                MySqlCommand cmd = new MySqlCommand(qry, con);
                var reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    ImageModel img = new() {
                        image_data = reader["image_data"] as byte[] ?? [],
                        FileName = reader.GetString("image_name"),
                        FileType = reader.GetString("FileType")
                    };

                    OfferPreview offerPreview = new() {
                        ID = reader.GetInt32("ID"),
                        UserId = reader.GetInt32("UserId"),
                        CarId = reader.GetInt32("CarId"),
                        Amount = reader.GetInt32("Offer"),
                        Total = reader.GetInt32("Total"),
                        FirstName = reader.GetString("firstname"),
                        LastName = reader.GetString("lastname"),
                        Username = reader.GetString("username"),
                        Year = reader.GetInt32("Year"),
                        Make = reader.GetString("Make"),
                        Model = reader.GetString("Model"),
                        Image = img
                    };

                    offers.Add(offerPreview);
                }
            } catch (Exception) {
                return [];
            }

            return offers;
        }

        public static FullOffer GetFullOffer(int offerId, int userId, int carId) {
            FullOffer fullOffer;

            try {
                MySqlConnection con = new(constr);
                con.Open();

                string qry = @"SELECT * FROM FullOfferView WHERE OfferId = @offerId AND UserId = @userId AND CarId = @carId";

                MySqlCommand cmd = new(qry, con);
                cmd.Parameters.AddWithValue("@offerId", offerId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@carId", carId);

                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();

                fullOffer = new FullOffer {
                    User = new OUser {
                        ID = reader.GetInt32("UserId"),
                        Username = reader.GetString("username"),
                        FirstName = reader.GetString("firstname"),
                        LastName = reader.GetString("lastname"),
                        Email = reader.GetString("email"),
                        Cell = reader.GetString("cell")
                    },
                    Offer = new OOffer {
                        ID = reader.GetInt32("OfferId"),
                        UserId = reader.GetInt32("UserId"),
                        CarId = reader.GetInt32("CarId"),
                        Price = reader.GetInt32("price"),
                        OfferAmount = reader.GetInt32("offer"),
                        Months = reader.GetInt32("months"),
                        Interest = reader.GetInt32("interest"),
                        Monthly = reader.GetInt32("monthly"),
                        Total = reader.GetInt32("total")
                    },
                    Car = new OCar {
                        ID = reader.GetInt32("CarId"),
                        Make = reader.GetString("make"),
                        Model = reader.GetString("model"),
                        Year = reader.GetInt32("year"),
                        Vin = reader.GetString("vin"),
                        License = reader.GetString("license"),
                        Price = reader.GetInt32("CarPrice"),
                        Image = new ImageModel {
                            image_data = reader["ImageData"] as byte[] ?? [],
                            FileName = reader.GetString("ImageName"),
                            FileType = reader.GetString("FileType")
                        }
                    }
                };
                return fullOffer;

            } catch (Exception) {
                return new();
            }
        }

        public static int AcceptOffer(int carId, int userId, int offerId, int adminId) {
            using MySqlConnection con = new MySqlConnection(constr);
            con.Open();

            MySqlCommand cmd = new MySqlCommand("AcceptOffer", con) {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("p_car_id", carId);
            cmd.Parameters.AddWithValue("p_user_id", userId);
            cmd.Parameters.AddWithValue("p_offer_id", offerId);
            cmd.Parameters.AddWithValue("p_admin_id", adminId);

            // OUT parameter for transaction ID
            MySqlParameter transactionIdParam = new MySqlParameter("p_transaction_id", MySqlDbType.Int32) {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(transactionIdParam);

            cmd.ExecuteNonQuery();

            // Retrieve the transaction ID from the output parameter
            int transactionId = (int)transactionIdParam.Value;
            return transactionId;
        }

        public static void RejectOffer(int offerId) {

            using (MySqlConnection connection = new MySqlConnection(constr)) {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction()) {
                    try {
                        string getOfferQuery = @"SELECT user_id, car_id, price, offer, months, interest, monthly, total 
                                         FROM offer 
                                         WHERE id = @OfferId";

                        MySqlCommand getOfferCmd = new MySqlCommand(getOfferQuery, connection, transaction);
                        getOfferCmd.Parameters.AddWithValue("@OfferId", offerId);

                        OOffer offer = null;
                        using (MySqlDataReader reader = getOfferCmd.ExecuteReader()) {
                            if (reader.Read()) {
                                offer = new OOffer {
                                    UserId = reader.GetInt32("user_id"),
                                    CarId = reader.GetInt32("car_id"),
                                    Price = reader.GetInt32("price"),
                                    OfferAmount = reader.GetInt32("offer"),
                                    Months = reader.GetInt32("months"),
                                    Interest = reader.GetInt32("interest"),
                                    Monthly = reader.GetInt32("monthly"),
                                    Total = reader.GetInt32("total")
                                };
                            }
                        }

                        if (offer == null) {
                            throw new Exception("Offer not found.");
                        }

                        string archiveOfferQuery = @"INSERT INTO archived_offer (user_id, car_id, price, offer, months, interest, monthly, total) 
                                             VALUES (@UserId, @CarId, @Price, @Offer, @Months, @Interest, @Monthly, @Total)";

                        MySqlCommand archiveOfferCmd = new MySqlCommand(archiveOfferQuery, connection, transaction);
                        archiveOfferCmd.Parameters.AddWithValue("@UserId", offer.UserId);
                        archiveOfferCmd.Parameters.AddWithValue("@CarId", offer.CarId);
                        archiveOfferCmd.Parameters.AddWithValue("@Price", offer.Price);
                        archiveOfferCmd.Parameters.AddWithValue("@Offer", offer.OfferAmount);
                        archiveOfferCmd.Parameters.AddWithValue("@Months", offer.Months);
                        archiveOfferCmd.Parameters.AddWithValue("@Interest", offer.Interest);
                        archiveOfferCmd.Parameters.AddWithValue("@Monthly", offer.Monthly);
                        archiveOfferCmd.Parameters.AddWithValue("@Total", offer.Total);

                        archiveOfferCmd.ExecuteNonQuery();

                        string deleteOfferQuery = @"DELETE FROM offer WHERE id = @OfferId";

                        MySqlCommand deleteOfferCmd = new MySqlCommand(deleteOfferQuery, connection, transaction);
                        deleteOfferCmd.Parameters.AddWithValue("@OfferId", offerId);

                        deleteOfferCmd.ExecuteNonQuery();

                        transaction.Commit();
                    } catch {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }



    public static InvoiceDetails GetInvoiceDetails(int transactionId) {
            InvoiceDetails invoiceDetails = new();

            try {
                using (MySqlConnection con = new MySqlConnection(constr)) {
                    con.Open();

                    // Retrieve invoice details based on transaction ID
                    string qry = "SELECT * FROM InvoiceDetails WHERE OfferId = @transactionId"; // Ensure correct column name

                    using (MySqlCommand cmd = new MySqlCommand(qry, con)) {
                        cmd.Parameters.AddWithValue("@transactionId", transactionId);

                        using (MySqlDataReader reader = cmd.ExecuteReader()) {
                            if (reader.Read()) {
                                invoiceDetails = new InvoiceDetails {
                                    UserId = reader.GetInt32("UserId"),
                                    Username = reader.GetString("username"),
                                    FirstName = reader.GetString("firstname"),
                                    LastName = reader.GetString("lastname"),
                                    Email = reader.GetString("email"),
                                    Cell = reader.GetString("cell"),
                                    OfferId = reader.GetInt32("OfferId"),
                                    Price = reader.GetInt32("price"),
                                    OfferAmount = reader.GetInt32("offer"),
                                    Months = reader.GetInt32("months"),
                                    Interest = reader.GetInt32("interest"),
                                    Monthly = reader.GetInt32("monthly"),
                                    Total = reader.GetInt32("total"),
                                    CarId = reader.GetInt32("CarId"),
                                    Make = reader.GetString("make"),
                                    Model = reader.GetString("model"),
                                    Year = reader.GetInt32("year"),
                                    Vin = reader.GetString("vin"),
                                    License = reader.GetString("license"),
                                    CarPrice = reader.GetInt32("CarPrice"),
                                    ImageData = reader.IsDBNull("ImageData") ? null : (byte[])reader["ImageData"], // Check for null
                                    ImageName = reader.GetString("ImageName"),
                                    TransactionID = transactionId,
                                    FileType = reader.GetString("FileType"),
                                    AdminId = reader.GetInt32("AdminId")
                                };
                            }
                        }
                    }

                    // Get the last inserted invoice ID
                    string lastIdQuery = "SELECT LAST_INSERT_ID()";
                    using (MySqlCommand idCommand = new MySqlCommand(lastIdQuery, con)) {
                        var lastId = idCommand.ExecuteScalar();
                        if (lastId != null && Convert.ToInt32(lastId) > 0) {
                            invoiceDetails.ID = Convert.ToInt32(lastId);
                        } else {
                            invoiceDetails.ID = 1; // Handle as needed
                        }
                    }
                }
            } catch (Exception ex) {
                // Log the exception for debugging
                Console.WriteLine($"Error retrieving invoice details: {ex}");
                return new InvoiceDetails(); // Return an empty object or handle as needed
            }

            return invoiceDetails;
        }

        public static void SaveInvoicePdf(InvoiceDetails invoiceDetails, byte[] pdfData) {
            using (MySqlConnection con = new MySqlConnection(constr)) {
                con.Open();
                string qry = "INSERT INTO invoice (trans_id, inv_name, file, file_type) VALUES (@transId, @invName, @file, @fileType)";

                MySqlCommand cmd = new MySqlCommand(qry, con);
                cmd.Parameters.AddWithValue("@transId", invoiceDetails.TransactionID);
                cmd.Parameters.AddWithValue("@invName", $"DFYINV{invoiceDetails.ID}.pdf");
                cmd.Parameters.AddWithValue("@file", pdfData);
                cmd.Parameters.AddWithValue("@fileType", "application/pdf");

                cmd.ExecuteNonQuery();
            }
        }


        public static List<AddedInvoices> GetAllInvoices() {
            List<AddedInvoices> invoices = new List<AddedInvoices>();

            using (MySqlConnection conn = new MySqlConnection(constr)) {
                conn.Open();
                string query = "SELECT c.make, c.model, c.year, i.id AS InvoiceID, i.inv_name, i.file, i.file_type " +
                               "FROM car c " +
                               "JOIN transaction t ON c.id = t.car_id " +
                               "JOIN invoice i ON t.id = i.trans_id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn)) {
                    using (MySqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            invoices.Add(new AddedInvoices {
                                ID = reader.GetInt32("InvoiceID"),
                                Make = reader.GetString("make"),
                                Model = reader.GetString("model"),
                                Year = reader.GetInt32("year"),
                                InvName = reader.GetString("inv_name"),
                                File = reader["file"] as byte[], // Assuming the file is stored as a BLOB
                                FileType = reader.GetString("file_type")
                            });
                        }
                    }
                }
            }

            return invoices;
        }

        public static ViewSpecificInvoice GetSpecificInvoice(int id) {
            ViewSpecificInvoice invoice = new();

            using (MySqlConnection conn = new MySqlConnection(constr)) {
                conn.Open();
                string query = "SELECT i.id AS InvoiceID, i.inv_name, i.file, i.file_type " +
                               "FROM invoice i WHERE i.id = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn)) {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader()) {
                        if (reader.Read()) {
                            invoice = new ViewSpecificInvoice {
                                ID = reader.GetInt32("InvoiceID"),
                                FileName = reader.GetString("inv_name"),
                                File = reader["file"] as byte[] ?? [],
                                FileType = reader.GetString("file_type")
                            };
                        }
                    }
                }
            }

            return invoice;
        }

        public static List<TransactionViewModel> GetAllTransactions(int user_id) {
            var transactions = new List<TransactionViewModel>();

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();
                var command = new MySqlCommand("SELECT t.id, t.balance_id, c.year AS CarYear, c.make AS CarMake, c.model AS CarModel " +
                                                "FROM transaction t " +
                                                "JOIN car c ON t.car_id = c.id " +
                                                "WHERE t.user_id = @user_id", connection);
                command.Parameters.AddWithValue("@user_id", user_id);

                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        transactions.Add(new TransactionViewModel {
                            ID = reader.GetInt32("id"),
                            BalanceID = reader.GetInt32("balance_id"),
                            CarYear = reader.GetInt32("CarYear"),
                            CarMake = reader.GetString("CarMake"),
                            CarModel = reader.GetString("CarModel")
                        });
                    }
                }
            }

            return transactions;
        }

        public static SpecificBalanceModel GetSpecificBalance(int balance_id) {
            SpecificBalanceModel balance = null;

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();
                var command = new MySqlCommand("SELECT b.id, b.current, b.total, c.year AS CarYear, c.make AS CarMake, c.model AS CarModel " +
                                                "FROM balance b " +
                                                "JOIN transaction t ON b.id = t.balance_id " +
                                                "JOIN car c ON t.car_id = c.id " +
                                                "WHERE b.id = @balance_id", connection);
                command.Parameters.AddWithValue("@balance_id", balance_id);

                using (var reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        balance = new SpecificBalanceModel {
                            ID = reader.GetInt32("id"),
                            Current = reader.GetInt32("current"),
                            Total = reader.GetInt32("total"),
                            CarYear = reader.GetInt32("CarYear"),
                            CarMake = reader.GetString("CarMake"),
                            CarModel = reader.GetString("CarModel")
                        };
                    }
                }
            }

            return balance;
        }

        public static void UpdateTransaction(int balance_id, int current) {
            using (var connection = new MySqlConnection(constr)) {
                connection.Open();
                var command = new MySqlCommand("UPDATE balance SET current = @current WHERE id = @balance_id", connection);
                command.Parameters.AddWithValue("@current", current);
                command.Parameters.AddWithValue("@balance_id", balance_id);
                command.ExecuteNonQuery();
            }
        }

        public static List<UserInvoiceDetails> GetAllInvoices(int user_id) {
            List<UserInvoiceDetails> invoices = new List<UserInvoiceDetails>();

            using (MySqlConnection conn = new MySqlConnection(constr)) {
                conn.Open();
                string query = @"SELECT i.id AS InvoiceId, c.year AS CarYear, c.make AS CarMake, 
                             c.model AS CarModel, i.inv_name AS InvoiceName
                             FROM invoice i
                             JOIN transaction t ON i.trans_id = t.id
                             JOIN car c ON t.car_id = c.id
                             WHERE t.user_id = @userId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn)) {
                    cmd.Parameters.AddWithValue("@userId", user_id);
                    using (MySqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            invoices.Add(new UserInvoiceDetails {
                                InvoiceId = reader.GetInt32("InvoiceId"),
                                CarYear = reader.GetInt32("CarYear"),
                                CarMake = reader.GetString("CarMake"),
                                CarModel = reader.GetString("CarModel"),
                                InvoiceName = reader.GetString("InvoiceName")
                            });
                        }
                    }
                }
            }
            return invoices;
        }

        public static UserInvoice GetUserInvoice(int invoice_id) {
            UserInvoice userInvoice = null;

            using (MySqlConnection conn = new MySqlConnection(constr)) {
                conn.Open();
                string query = "SELECT id, inv_name, file, file_type FROM invoice WHERE id = @invoiceId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn)) {
                    cmd.Parameters.AddWithValue("@invoiceId", invoice_id);
                    using (MySqlDataReader reader = cmd.ExecuteReader()) {
                        if (reader.Read()) {
                            userInvoice = new UserInvoice {
                                Id = reader.GetInt32("id"),
                                InvoiceName = reader.GetString("inv_name"),
                                File = (byte[])reader["file"],
                                FileType = reader.GetString("file_type")
                            };
                        }
                    }
                }
            }
            return userInvoice;
        }
    }
}