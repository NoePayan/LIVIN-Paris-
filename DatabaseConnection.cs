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
        /// <summary>
        /// Chaîne de connexion lue dans la variable d'environnement LIVINPARIS_DB
        /// (voir .env.example). Aucun mot de passe n'est stocké dans le code.
        /// </summary>
        private static string ConnectionString =>
            Environment.GetEnvironmentVariable("LIVINPARIS_DB")
            ?? throw new InvalidOperationException(
                "La variable d'environnement LIVINPARIS_DB n'est pas définie. " +
                "Exemple : Server=localhost;Database=livinparis;Uid=root;Pwd=VOTRE_MOT_DE_PASSE;");

        public static MySqlConnection GetConnection()
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(ConnectionString);
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