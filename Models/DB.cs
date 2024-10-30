using MySql.Data.MySqlClient;
using System;

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
                // Log the exception or handle it as necessary
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
                // Log the exception or handle it as necessary
                Console.WriteLine("An error occurred: " + ex.Message);
#pragma warning disable CS8603 // Possible null reference return.
                return null;
#pragma warning restore CS8603 // Possible null reference return.
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

                string qry = @"INSERT INTO car (make, model, year, vin, license, price, image, image_name, file_type) 
                               VALUES (@make, @model, @year, @vin, @license, @price, @image, @imageName, @fileType)";

                using var cmd = new MySqlCommand(qry, con);
                cmd.Parameters.AddWithValue("@make", model.Make);
                cmd.Parameters.AddWithValue("@model", model.Model);
                cmd.Parameters.AddWithValue("@year", model.Year);
                cmd.Parameters.AddWithValue("@vin", model.Vin);
                cmd.Parameters.AddWithValue("@license", model.License);
                cmd.Parameters.AddWithValue("@price", int.Parse(model.Price)); // Convert price to int
                cmd.Parameters.AddWithValue("@image", imageData);
                cmd.Parameters.AddWithValue("@imageName", model.UploadedFile.FileName);
                cmd.Parameters.AddWithValue("@fileType", model.UploadedFile.ContentType);

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

            string qry = "SELECT id, make, model, year, vin, license, price, image, image_name, file_type FROM car";
            using MySqlCommand cmd = new(qry, con);
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                ImageModel img = new ImageModel() {
                    image_data = reader["image"] as byte[], // Assuming the image is stored as a byte array
                    FileName = reader.GetString("image_name"),
                    FileType = reader.GetString("file_type")
                };

                CurrentStock stock = new CurrentStock {
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

            ImageModel img = new ImageModel() {
                image_data = reader["image"] as byte[], // Assuming the image is stored as a byte array
                FileName = reader.GetString("image_name"),
                FileType = reader.GetString("file_type")
            };

            CurrentStock stock = new CurrentStock {
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
            } catch(Exception) {
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
    }

}
