
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LivInParis.Models;
using LivInParis.Data;

namespace LivInParis.Services
{
    public class CuisinierService
    {
        
        public static int AjouterCuisinier(Cuisinier cuisinier)
        {
            string query = @"INSERT INTO Cuisinier 
                           (nom_Cuisinier, prenom_Cuisinier, email_Cuisinier, mot_de_passe_Cuisinier, 
                            telephone_Cuisinier, specialite_Cuisinier, adresse_Cuisinier, station_metro_proche) 
                           VALUES 
                           (@nom, @prenom, @email, @motDePasse, @telephone, @specialite, @adresse, @stationMetro);
                           SELECT LAST_INSERT_ID();";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nom", cuisinier.NomCuisinier);
                    command.Parameters.AddWithValue("@prenom", cuisinier.PrenomCuisinier);
                    command.Parameters.AddWithValue("@email", cuisinier.EmailCuisinier);
                    command.Parameters.AddWithValue("@motDePasse", cuisinier.MotDePasseCuisinier);
                    command.Parameters.AddWithValue("@telephone", cuisinier.TelephoneCuisinier);
                    command.Parameters.AddWithValue("@specialite", cuisinier.SpecialiteCuisinier);
                    command.Parameters.AddWithValue("@adresse", cuisinier.AdresseCuisinier);
                    command.Parameters.AddWithValue("@stationMetro", cuisinier.StationMetroProche);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// met a jour le cuisinier
        /// </summary>
        /// <param name="cuisinier"></param>
        /// <returns>si ca a ete fait ou non</returns>
        public static bool MettreAJourCuisinier(Cuisinier cuisinier)
        {
            string query = @"UPDATE Cuisinier 
                           SET nom_Cuisinier = @nom, 
                               prenom_Cuisinier = @prenom, 
                               email_Cuisinier = @email, 
                               mot_de_passe_Cuisinier = @motDePasse, 
                               telephone_Cuisinier = @telephone, 
                               specialite_Cuisinier = @specialite,
                               adresse_Cuisinier = @adresse,
                               station_metro_proche = @stationMetro
                           WHERE id_Cuisinier = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", cuisinier.IdCuisinier);
                    command.Parameters.AddWithValue("@nom", cuisinier.NomCuisinier);
                    command.Parameters.AddWithValue("@prenom", cuisinier.PrenomCuisinier);
                    command.Parameters.AddWithValue("@email", cuisinier.EmailCuisinier);
                    command.Parameters.AddWithValue("@motDePasse", cuisinier.MotDePasseCuisinier);
                    command.Parameters.AddWithValue("@telephone", cuisinier.TelephoneCuisinier);
                    command.Parameters.AddWithValue("@specialite", cuisinier.SpecialiteCuisinier);
                    command.Parameters.AddWithValue("@adresse", cuisinier.AdresseCuisinier);
                    command.Parameters.AddWithValue("@stationMetro", cuisinier.StationMetroProche);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// supprime un cuisinier
        /// </summary>
        /// <param name="idCuisinier">l'id du cuisinier a supprimer</param>
        /// <returns>si ca a ete fait ou non</returns>
        public static bool SupprimerCuisinier(int idCuisinier)
        {
            string query = "DELETE FROM Cuisinier WHERE id_Cuisinier = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idCuisinier);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// obtenir lke cuisinier via son ID
        /// </summary>
        /// <param name="idCuisinier"></param>
        /// <returns></returns>
        public static Cuisinier? ObtenirCuisinierParId(int idCuisinier)
        {
            string query = "SELECT * FROM Cuisinier WHERE id_Cuisinier = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idCuisinier);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Cuisinier
                            {
                                IdCuisinier = Convert.ToInt32(reader["id_Cuisinier"]),
                                NomCuisinier = reader["nom_Cuisinier"].ToString() ?? string.Empty,
                                PrenomCuisinier = reader["prenom_Cuisinier"].ToString() ?? string.Empty,
                                EmailCuisinier = reader["email_Cuisinier"].ToString() ?? string.Empty,
                                MotDePasseCuisinier = reader["mot_de_passe_Cuisinier"].ToString() ?? string.Empty,
                                TelephoneCuisinier = reader["telephone_Cuisinier"].ToString() ?? string.Empty,
                                SpecialiteCuisinier = reader["specialite_Cuisinier"].ToString() ?? string.Empty,
                                AdresseCuisinier = reader["adresse_Cuisinier"].ToString() ?? string.Empty,
                                StationMetroProche = reader["station_metro_proche"] != DBNull.Value ? Convert.ToInt32(reader["station_metro_proche"]) : 0
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// recuperer un cuisinier via son mail
        /// </summary>
        /// <param name="email"></param>
        /// <returns>le cuisinier</returns>
        public static Cuisinier? ObtenirCuisinierParEmail(string email)
        {
            string query = "SELECT * FROM Cuisinier WHERE email_Cuisinier = @email";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Cuisinier
                            {
                                IdCuisinier = Convert.ToInt32(reader["id_Cuisinier"]),
                                NomCuisinier = reader["nom_Cuisinier"].ToString() ?? string.Empty,
                                PrenomCuisinier = reader["prenom_Cuisinier"].ToString() ?? string.Empty,
                                EmailCuisinier = reader["email_Cuisinier"].ToString() ?? string.Empty,
                                MotDePasseCuisinier = reader["mot_de_passe_Cuisinier"].ToString() ?? string.Empty,
                                TelephoneCuisinier = reader["telephone_Cuisinier"].ToString() ?? string.Empty,
                                SpecialiteCuisinier = reader["specialite_Cuisinier"].ToString() ?? string.Empty,
                                AdresseCuisinier = reader["adresse_Cuisinier"].ToString() ?? string.Empty,
                                StationMetroProche = reader["station_metro_proche"] != DBNull.Value ? Convert.ToInt32(reader["station_metro_proche"]) : 0
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// authentification
        /// </summary>
        /// <param name="email"></param>
        /// <param name="motDePasse"></param>
        /// <returns>le cuisinier connecte</returns>
        public static Cuisinier? AuthentifierCuisinier(string email, string motDePasse)
        {
            string query = "SELECT * FROM Cuisinier WHERE email_Cuisinier = @email AND mot_de_passe_Cuisinier = @motDePasse";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@motDePasse", motDePasse);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Cuisinier
                            {
                                IdCuisinier = Convert.ToInt32(reader["id_Cuisinier"]),
                                NomCuisinier = reader["nom_Cuisinier"].ToString() ?? string.Empty,
                                PrenomCuisinier = reader["prenom_Cuisinier"].ToString() ?? string.Empty,
                                EmailCuisinier = reader["email_Cuisinier"].ToString() ?? string.Empty,
                                MotDePasseCuisinier = reader["mot_de_passe_Cuisinier"].ToString() ?? string.Empty,
                                TelephoneCuisinier = reader["telephone_Cuisinier"].ToString() ?? string.Empty,
                                SpecialiteCuisinier = reader["specialite_Cuisinier"].ToString() ?? string.Empty,
                                AdresseCuisinier = reader["adresse_Cuisinier"].ToString() ?? string.Empty,
                                StationMetroProche = reader["station_metro_proche"] != DBNull.Value ? Convert.ToInt32(reader["station_metro_proche"]) : 0
                            };
                        }
                    }
                }
            }

            return null;
        }

       /// <summary>
       /// donne tout les cuisiniers
       /// </summary>
       /// <returns>la liste des cuisiniers de l'application</returns>
        public static List<Cuisinier> ObtenirTousLesCuisiniers()
        {
            List<Cuisinier> cuisiniers = new List<Cuisinier>();
            string query = "SELECT * FROM Cuisinier";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cuisiniers.Add(new Cuisinier
                            {
                                IdCuisinier = Convert.ToInt32(reader["id_Cuisinier"]),
                                NomCuisinier = reader["nom_Cuisinier"].ToString() ?? string.Empty,
                                PrenomCuisinier = reader["prenom_Cuisinier"].ToString() ?? string.Empty,
                                EmailCuisinier = reader["email_Cuisinier"].ToString() ?? string.Empty,
                                MotDePasseCuisinier = reader["mot_de_passe_Cuisinier"].ToString() ?? string.Empty,
                                TelephoneCuisinier = reader["telephone_Cuisinier"].ToString() ?? string.Empty,
                                SpecialiteCuisinier = reader["specialite_Cuisinier"].ToString() ?? string.Empty,
                                AdresseCuisinier = reader["adresse_Cuisinier"].ToString() ?? string.Empty,
                                StationMetroProche = reader["station_metro_proche"] != DBNull.Value ? Convert.ToInt32(reader["station_metro_proche"]) : 0
                            });
                        }
                    }
                }
            }

            return cuisiniers;
        }


        /// <summary>
        /// verifie si le mail a deja ete utilise
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool EmailExiste(string email)
        {
            string query = "SELECT COUNT(*) FROM Cuisinier WHERE email_Cuisinier = @email";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }
    }
}
