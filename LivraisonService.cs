using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LivInParis.Models;
using LivInParis.Data;

namespace LivInParis.Services
{
    /// <summary>
    /// Service qui gere les livraisons dans la bdd
    /// </summary>
    public class LivraisonService
    {
        /// <summary>
        /// Ajoute une livraison dans la bdd
        /// </summary>
        /// <param name="livraison">La livraison a ajouter</param>
        /// <returns>L'id de la livraison ajoutée</returns>
        public static int AjouterLivraison(Livraison livraison)
        {
            string query = @"INSERT INTO Livraison 
                           (date_Livraison, adresse_Livraison, statut_Livraison, commentaire_Livraison) 
                           VALUES 
                           (@date, @adresse, @statut, @commentaire);
                           SELECT LAST_INSERT_ID();";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@date", livraison.DateLivraison);
                    command.Parameters.AddWithValue("@adresse", livraison.AdresseLivraison);
                    command.Parameters.AddWithValue("@statut", livraison.StatutLivraison);
                    command.Parameters.AddWithValue("@commentaire", livraison.CommentaireLivraison);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Met a jour le statut d'une livraison
        /// </summary>
        /// <param name="idLivraison">L'id de la livraison a modifier</param>
        /// <param name="statut">Le nouveau statut</param>
        /// <returns>True si ca a marché</returns>
        public static bool MettreAJourStatutLivraison(int idLivraison, string statut)
        {
            string query = "UPDATE Livraison SET statut_Livraison = @statut WHERE id_Livraison = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idLivraison);
                    command.Parameters.AddWithValue("@statut", statut);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Recupere une livraison par son ID
        /// </summary>
        /// <param name="idLivraison">L'id de la livraison a chercher</param>
        /// <returns>La livraison ou null si pas trouvée</returns>
        public static Livraison? ObtenirLivraisonParId(int idLivraison)
        {
            string query = "SELECT * FROM Livraison WHERE id_Livraison = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idLivraison);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Livraison
                            {
                                IdLivraison = Convert.ToInt32(reader["id_Livraison"]),
                                DateLivraison = Convert.ToDateTime(reader["date_Livraison"]),
                                AdresseLivraison = reader["adresse_Livraison"].ToString() ?? string.Empty,
                                StatutLivraison = reader["statut_Livraison"].ToString() ?? string.Empty,
                                CommentaireLivraison = reader["commentaire_Livraison"].ToString() ?? string.Empty
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}