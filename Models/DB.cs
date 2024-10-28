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
                return null;
            }
        }
    }
}
