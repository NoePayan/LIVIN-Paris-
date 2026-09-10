using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LivInParis.Models;
using LivInParis.Data;

namespace LivInParis.Services
{
    /// <summary>
    /// Service qui gere les historiques dans la bdd
    /// </summary>
    public class HistoriqueService
    {
        /// <summary>
        /// Ajoute un historique dans la bdd
        /// </summary>
        /// <param name="historique">L'historique a ajouter</param>
        /// <returns>L'id de l'historique ajouté</returns>
        public static int AjouterHistorique(Historique historique)
        {
            string query = @"INSERT INTO Historique 
                           (date_Historique, action_Historique, details_Historique) 
                           VALUES 
                           (@date, @action, @details);
                           SELECT LAST_INSERT_ID();";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@date", historique.DateHistorique);
                    command.Parameters.AddWithValue("@action", historique.ActionHistorique);
                    command.Parameters.AddWithValue("@details", historique.DetailsHistorique);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Recupere un historique par son ID
        /// </summary>
        /// <param name="idHistorique">L'id de l'historique a chercher</param>
        /// <returns>L'historique ou null si pas trouvé</returns>
        public static Historique? ObtenirHistoriqueParId(int idHistorique)
        {
            string query = "SELECT * FROM Historique WHERE id_Historique = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idHistorique);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Historique
                            {
                                IdHistorique = Convert.ToInt32(reader["id_Historique"]),
                                DateHistorique = Convert.ToDateTime(reader["date_Historique"]),
                                ActionHistorique = reader["action_Historique"].ToString() ?? string.Empty,
                                DetailsHistorique = reader["details_Historique"].ToString() ?? string.Empty
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}