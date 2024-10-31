using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Runtime.ConstrainedExecution;

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
                string qry = @"SELECT o.id AS ID, o.user_id AS UserId, o.car_id AS CarId, o.total as Total, o.offer as Offer, c.year AS Year, c.make AS Make, c.model AS Model, c.image AS image_data, c.image_name, c.file_type AS FileType, u.firstname, u.lastname, u.username FROM offer o JOIN car c ON o.car_id = c.id JOIN user u ON o.user_id = u.id ORDER BY o.car_id, o.total ASC";

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
            FullOffer fullOffer = new();

            try {
                MySqlConnection con = new(constr);
                con.Open();

                string qry = @"SELECT  u.id AS UserId, u.username, u.firstname, u.lastname, u.email, u.cell, o.id AS OfferId, o.price, o.offer, o.months, o.interest, o.monthly, o.total, c.id AS CarId, c.make, c.model, c.year, c.vin, c.license, c.price AS CarPrice, c.image AS ImageData, c.image_name AS ImageName, c.file_type AS FileType FROM offer o JOIN user u ON o.user_id = u.id JOIN car c ON o.car_id = c.id WHERE o.id = @offerId AND o.user_id = @userId AND o.car_id = @carId";

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
    }
}