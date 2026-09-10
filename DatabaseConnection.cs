using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace LivInParis.Data
{
    /// <summary>
    /// la classe qui connecte le SQL et le projet
    /// </summary>
    public class DatabaseConnection
    {
        private static string connectionString = "Server=localhost;Database=livinparis;Uid=root;Pwd=g$MViF6iX;";

        public static MySqlConnection GetConnection()
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de connexion à la base de données: {ex.Message}");
                throw;
            }
        }

        public static bool TestConnection()
        {
            try
            {
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();
                    connection.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}