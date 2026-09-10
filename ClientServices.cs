using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LivInParis.Models;
using LivInParis.Data;

namespace LivInParis.Services
{
    /// <summary>
    /// Service qui gere les clients dans la bdd
    /// </summary>
    public class ClientService
    {
        /// <summary>
        /// Ajoute un client dans la bdd
        /// </summary>
        /// <param name="client">Le client a ajouter</param>
        /// <returns>L'id du client ajouté</returns>
        public static int AjouterClient(Client client)
        {
            string query = @"INSERT INTO Client 
               (nom_Client, prenom_Client, email_Client, mot_de_passe_Client, telephone_Client, adresse_Client, station_metro_proche) 
               VALUES 
               (@nom, @prenom, @email, @motDePasse, @telephone, @adresse, @stationMetro);
               SELECT LAST_INSERT_ID();";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nom", client.NomClient);
                    command.Parameters.AddWithValue("@prenom", client.PrenomClient);
                    command.Parameters.AddWithValue("@email", client.EmailClient);
                    command.Parameters.AddWithValue("@motDePasse", client.MotDePasseClient);
                    command.Parameters.AddWithValue("@telephone", client.TelephoneClient);
                    command.Parameters.AddWithValue("@adresse", client.AdresseClient);
                    command.Parameters.AddWithValue("@stationMetro", client.StationMetroProche);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Met a jour un client dans la bdd
        /// </summary>
        /// <param name="client">Le client avec les nouvelles infos</param>
        /// <returns>True si ca a marché</returns>
        public static bool MettreAJourClient(Client client)
        {
            string query = @"UPDATE Client 
                           SET nom_Client = @nom, 
                               prenom_Client = @prenom, 
                               email_Client = @email, 
                               mot_de_passe_Client = @motDePasse, 
                               telephone_Client = @telephone, 
                               adresse_Client = @adresse,
                               station_metro_proche = @stationMetro
                           WHERE id_Client = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", client.IdClient);
                    command.Parameters.AddWithValue("@nom", client.NomClient);
                    command.Parameters.AddWithValue("@prenom", client.PrenomClient);
                    command.Parameters.AddWithValue("@email", client.EmailClient);
                    command.Parameters.AddWithValue("@motDePasse", client.MotDePasseClient);
                    command.Parameters.AddWithValue("@telephone", client.TelephoneClient);
                    command.Parameters.AddWithValue("@adresse", client.AdresseClient);
                    command.Parameters.AddWithValue("@stationMetro", client.StationMetroProche);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Supprime un client de la bdd
        /// </summary>
        /// <param name="idClient">L'id du client a supprimer</param>
        /// <returns>True si ca a marché</returns>
        public static bool SupprimerClient(int idClient)
        {
            string query = "DELETE FROM Client WHERE id_Client = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idClient);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Recupere un client par son ID
        /// </summary>
        /// <param name="idClient">L'id du client a chercher</param>
        /// <returns>Le client ou null si pas trouvé</returns>
        public static Client? ObtenirClientParId(int idClient)
        {
            string query = "SELECT * FROM Client WHERE id_Client = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idClient);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Client
                            {
                                IdClient = Convert.ToInt32(reader["id_Client"]),
                                NomClient = reader["nom_Client"].ToString() ?? string.Empty,
                                PrenomClient = reader["prenom_Client"].ToString() ?? string.Empty,
                                EmailClient = reader["email_Client"].ToString() ?? string.Empty,
                                MotDePasseClient = reader["mot_de_passe_Client"].ToString() ?? string.Empty,
                                TelephoneClient = reader["telephone_Client"].ToString() ?? string.Empty,
                                AdresseClient = reader["adresse_Client"].ToString() ?? string.Empty,
                                StationMetroProche = reader["station_metro_proche"] != DBNull.Value ? Convert.ToInt32(reader["station_metro_proche"]) : 0
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Recupere un client par son email
        /// </summary>
        /// <param name="email">L'email du client a chercher</param>
        /// <returns>Le client ou null si pas trouvé</returns>
        public static Client? ObtenirClientParEmail(string email)
        {
            string query = "SELECT * FROM Client WHERE email_Client = @email";

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
                            return new Client
                            {
                                IdClient = Convert.ToInt32(reader["id_Client"]),
                                NomClient = reader["nom_Client"].ToString() ?? string.Empty,
                                PrenomClient = reader["prenom_Client"].ToString() ?? string.Empty,
                                EmailClient = reader["email_Client"].ToString() ?? string.Empty,
                                MotDePasseClient = reader["mot_de_passe_Client"].ToString() ?? string.Empty,
                                TelephoneClient = reader["telephone_Client"].ToString() ?? string.Empty,
                                AdresseClient = reader["adresse_Client"].ToString() ?? string.Empty,
                                StationMetroProche = reader["station_metro_proche"] != DBNull.Value ? Convert.ToInt32(reader["station_metro_proche"]) : 0
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Authentifie un client avec son email et mdp
        /// </summary>
        /// <param name="email">L'email du client</param>
        /// <param name="motDePasse">Le mot de passe du client</param>
        /// <returns>Le client ou null si auth échouée</returns>
        public static Client? AuthentifierClient(string email, string motDePasse)
        {
            string query = "SELECT * FROM Client WHERE email_Client = @email AND mot_de_passe_Client = @motDePasse";

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
                            return new Client
                            {
                                IdClient = Convert.ToInt32(reader["id_Client"]),
                                NomClient = reader["nom_Client"].ToString() ?? string.Empty,
                                PrenomClient = reader["prenom_Client"].ToString() ?? string.Empty,
                                EmailClient = reader["email_Client"].ToString() ?? string.Empty,
                                MotDePasseClient = reader["mot_de_passe_Client"].ToString() ?? string.Empty,
                                TelephoneClient = reader["telephone_Client"].ToString() ?? string.Empty,
                                AdresseClient = reader["adresse_Client"].ToString() ?? string.Empty,
                                StationMetroProche = reader["station_metro_proche"] != DBNull.Value ? Convert.ToInt32(reader["station_metro_proche"]) : 0
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Recupere tous les clients de la bdd
        /// </summary>
        /// <returns>Liste de tous les clients</returns>
        public static List<Client> ObtenirTousLesClients()
        {
            List<Client> clients = new List<Client>();
            string query = "SELECT * FROM Client";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clients.Add(new Client
                            {
                                IdClient = Convert.ToInt32(reader["id_Client"]),
                                NomClient = reader["nom_Client"].ToString() ?? string.Empty,
                                PrenomClient = reader["prenom_Client"].ToString() ?? string.Empty,
                                EmailClient = reader["email_Client"].ToString() ?? string.Empty,
                                MotDePasseClient = reader["mot_de_passe_Client"].ToString() ?? string.Empty,
                                TelephoneClient = reader["telephone_Client"].ToString() ?? string.Empty,
                                AdresseClient = reader["adresse_Client"].ToString() ?? string.Empty,
                                StationMetroProche = reader["station_metro_proche"] != DBNull.Value ? Convert.ToInt32(reader["station_metro_proche"]) : 0
                            });
                        }
                    }
                }
            }

            return clients;
        }

        /// <summary>
        /// Verifie si un email existe deja dans la bdd
        /// </summary>
        /// <param name="email">L'email a verifier</param>
        /// <returns>True si l'email existe deja</returns>
        public static bool EmailExiste(string email)
        {
            string query = "SELECT COUNT(*) FROM Client WHERE email_Client = @email";

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