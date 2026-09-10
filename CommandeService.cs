using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LivInParis.Models;
using LivInParis.Data;

namespace LivInParis.Services
{
    /// <summary>
    /// Service qui gere les commandes dans la bdd
    /// </summary>
    public class CommandeService
    {
        /// <summary>
        /// Ajoute une commande dans la bdd
        /// </summary>
        /// <param name="commande">La commande a ajouter</param>
        /// <returns>L'id de la commande ajouter</returns>
        public static int AjouterCommande(Commande commande)
        {
            string query = @"INSERT INTO Commande 
                           (numero_Commande, id_Client, id_Cuisinier, date_Commande, prix_Total, prix_Commande, id_Livraison, id_Historique, statut_Commande) 
                           VALUES 
                           (@numero, @idClient, @idCuisinier, @date, @prixTotal, @prixCommande, @idLivraison, @idHistorique, @statut);
                           SELECT LAST_INSERT_ID();";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int idCommande;
                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@numero", commande.NumeroCommande);
                            command.Parameters.AddWithValue("@idClient", commande.IdClient);
                            command.Parameters.AddWithValue("@idCuisinier", commande.IdCuisinier);
                            command.Parameters.AddWithValue("@date", commande.DateCommande);
                            command.Parameters.AddWithValue("@prixTotal", commande.PrixTotal);
                            command.Parameters.AddWithValue("@prixCommande", commande.PrixCommande);
                            command.Parameters.AddWithValue("@idLivraison", commande.IdLivraison.HasValue ? (object)commande.IdLivraison.Value : DBNull.Value);
                            command.Parameters.AddWithValue("@idHistorique", commande.IdHistorique.HasValue ? (object)commande.IdHistorique.Value : DBNull.Value);
                            command.Parameters.AddWithValue("@statut", commande.StatutCommande);

                            idCommande = Convert.ToInt32(command.ExecuteScalar());
                        }

                        /// <summary>
                        /// Ajoute les lignes de commande
                        /// </summary>
                        if (commande.LignesCommande != null && commande.LignesCommande.Count > 0)
                        {
                            foreach (var ligne in commande.LignesCommande)
                            {
                                /// <summary>
                                /// Modifié pour inclure prix_Unitaire dans la requete
                                /// </summary>
                                string ligneQuery = @"INSERT INTO Ligne_Commande 
                                                   (id_Commande, id_Livraison, id_Plat, quantite_Ligne_Commande, 
                                                    date_livraison_Ligne_Commande, lieu_livraison_Ligne_Commande, prix_Unitaire) 
                                                   VALUES 
                                                   (@idCommande, @idLivraison, @idPlat, @quantite, @dateLivraison, @lieuLivraison, @prixUnitaire)";

                                using (MySqlCommand ligneCommand = new MySqlCommand(ligneQuery, connection, transaction))
                                {
                                    ligneCommand.Parameters.AddWithValue("@idCommande", idCommande);
                                    ligneCommand.Parameters.AddWithValue("@idLivraison", commande.IdLivraison ?? 0);
                                    ligneCommand.Parameters.AddWithValue("@idPlat", ligne.IdPlat);
                                    ligneCommand.Parameters.AddWithValue("@quantite", ligne.Quantite);
                                    ligneCommand.Parameters.AddWithValue("@dateLivraison", DateTime.Now.AddHours(1).Date);
                                    ligneCommand.Parameters.AddWithValue("@prixUnitaire", ligne.PrixUnitaire); /// Ajout du prix unitaire

                                    string lieuLivraison = string.Empty;
                                    if (commande.Livraison != null)
                                    {
                                        lieuLivraison = commande.Livraison.AdresseLivraison;
                                    }
                                    ligneCommand.Parameters.AddWithValue("@lieuLivraison", lieuLivraison);

                                    ligneCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                        return idCommande;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Erreur lors de l'ajout de la commande: " + ex.Message);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Met a jour le statut d'une commande
        /// </summary>
        /// <param name="idCommande">L'id de la commande a modifier</param>
        /// <param name="statut">Le nouveau statut</param>
        /// <returns>True si ca a marché</returns>
        public static bool MettreAJourStatutCommande(int idCommande, string statut)
        {
            string query = "UPDATE Commande SET statut_Commande = @statut WHERE id_Commande = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idCommande);
                    command.Parameters.AddWithValue("@statut", statut);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Recupere une commande par son ID
        /// </summary>
        /// <param name="idCommande">L'id de la commande a chercher</param>
        /// <returns>La commande ou null si pas trouvé</returns>
        public static Commande? ObtenirCommandeParId(int idCommande)
        {
            string query = "SELECT * FROM Commande WHERE id_Commande = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idCommande);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Commande commande = new Commande
                            {
                                IdCommande = Convert.ToInt32(reader["id_Commande"]),
                                NumeroCommande = reader["numero_Commande"].ToString() ?? string.Empty,
                                IdClient = Convert.ToInt32(reader["id_Client"]),
                                IdCuisinier = reader["id_Cuisinier"] != DBNull.Value ? Convert.ToInt32(reader["id_Cuisinier"]) : 0,
                                DateCommande = Convert.ToDateTime(reader["date_Commande"]),
                                PrixTotal = Convert.ToDecimal(reader["prix_Total"]),
                                PrixCommande = Convert.ToDecimal(reader["prix_Commande"]),
                                IdLivraison = reader["id_Livraison"] != DBNull.Value ? (int?)Convert.ToInt32(reader["id_Livraison"]) : null,
                                IdHistorique = reader["id_Historique"] != DBNull.Value ? (int?)Convert.ToInt32(reader["id_Historique"]) : null,
                                StatutCommande = reader["statut_Commande"].ToString() ?? string.Empty
                            };

                            /// <summary>
                            /// Charge les lignes de commande
                            /// </summary>
                            commande.LignesCommande = ObtenirLignesCommande(idCommande);

                            return commande;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Recupere toutes les commandes de la bdd
        /// </summary>
        /// <returns>Liste de toutes les commandes</returns>
        public static List<Commande> ObtenirToutesLesCommandes()
        {
            List<Commande> commandes = new List<Commande>();
            string query = "SELECT * FROM Commande";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Commande commande = new Commande
                            {
                                IdCommande = Convert.ToInt32(reader["id_Commande"]),
                                NumeroCommande = reader["numero_Commande"].ToString() ?? string.Empty,
                                IdClient = Convert.ToInt32(reader["id_Client"]),
                                IdCuisinier = reader["id_Cuisinier"] != DBNull.Value ? Convert.ToInt32(reader["id_Cuisinier"]) : 0,
                                DateCommande = Convert.ToDateTime(reader["date_Commande"]),
                                PrixTotal = Convert.ToDecimal(reader["prix_Total"]),
                                PrixCommande = Convert.ToDecimal(reader["prix_Commande"]),
                                IdLivraison = reader["id_Livraison"] != DBNull.Value ? (int?)Convert.ToInt32(reader["id_Livraison"]) : null,
                                IdHistorique = reader["id_Historique"] != DBNull.Value ? (int?)Convert.ToInt32(reader["id_Historique"]) : null,
                                StatutCommande = reader["statut_Commande"].ToString() ?? string.Empty
                            };

                            commandes.Add(commande);
                        }
                    }
                }

                /// <summary>
                /// Charge les lignes de commande pour chaque commande
                /// </summary>
                foreach (var commande in commandes)
                {
                    commande.LignesCommande = ObtenirLignesCommande(commande.IdCommande);
                }
            }

            return commandes;
        }

        /// <summary>
        /// Recupere les lignes d'une commande
        /// </summary>
        /// <param name="idCommande">L'id de la commande</param>
        /// <returns>Liste des lignes de commande</returns>
        private static List<LigneCommande> ObtenirLignesCommande(int idCommande)
        {
            List<LigneCommande> lignes = new List<LigneCommande>();
            string query = "SELECT * FROM Ligne_Commande WHERE id_Commande = @idCommande";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idCommande", idCommande);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lignes.Add(new LigneCommande
                            {
                                IdLigneCommande = Convert.ToInt32(reader["id_Ligne_Commande"]),
                                IdCommande = Convert.ToInt32(reader["id_Commande"]),
                                IdLivraison = Convert.ToInt32(reader["id_Livraison"]),
                                IdPlat = Convert.ToInt32(reader["id_Plat"]), /// Lecture de l'ID du plat
                                Quantite = Convert.ToInt32(reader["quantite_Ligne_Commande"]),
                                DateLivraison = reader["date_livraison_Ligne_Commande"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["date_livraison_Ligne_Commande"]) : null,
                                LieuLivraison = reader["lieu_livraison_Ligne_Commande"] != DBNull.Value ?
                                    reader["lieu_livraison_Ligne_Commande"].ToString() ?? string.Empty : string.Empty,
                                /// <summary>
                                /// On a pas le prix unitaire dans la bdd, faut le récup du plat
                                /// </summary>
                                PrixUnitaire = 0 /// Sera mis à jour plus tard
                            });
                        }
                    }
                }
            }

            /// <summary>
            /// Charge les plats pour chaque ligne et met a jour le prix unitaire
            /// </summary>
            foreach (var ligne in lignes)
            {
                ligne.Plat = PlatService.ObtenirPlatParId(ligne.IdPlat);
                if (ligne.Plat != null)
                {
                    ligne.PrixUnitaire = ligne.Plat.PrixPlat;
                }
            }

            return lignes;
        }

        /// <summary>
        /// Recupere les commandes d'un client
        /// </summary>
        /// <param name="idClient">L'id du client</param>
        /// <returns>Liste des commandes du client</returns>
        public static List<Commande> ObtenirCommandesParClient(int idClient)
        {
            List<Commande> commandes = new List<Commande>();
            string query = "SELECT * FROM Commande WHERE id_Client = @idClient";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idClient", idClient);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Commande commande = new Commande
                            {
                                IdCommande = Convert.ToInt32(reader["id_Commande"]),
                                NumeroCommande = reader["numero_Commande"].ToString() ?? string.Empty,
                                IdClient = Convert.ToInt32(reader["id_Client"]),
                                IdCuisinier = reader["id_Cuisinier"] != DBNull.Value ? Convert.ToInt32(reader["id_Cuisinier"]) : 0,
                                DateCommande = Convert.ToDateTime(reader["date_Commande"]),
                                PrixTotal = Convert.ToDecimal(reader["prix_Total"]),
                                PrixCommande = Convert.ToDecimal(reader["prix_Commande"]),
                                IdLivraison = reader["id_Livraison"] != DBNull.Value ? (int?)Convert.ToInt32(reader["id_Livraison"]) : null,
                                IdHistorique = reader["id_Historique"] != DBNull.Value ? (int?)Convert.ToInt32(reader["id_Historique"]) : null,
                                StatutCommande = reader["statut_Commande"].ToString() ?? string.Empty
                            };

                            commandes.Add(commande);
                        }
                    }
                }

                /// <summary>
                /// Charge les lignes de commande pour chaque commande
                /// </summary>
                foreach (var commande in commandes)
                {
                    commande.LignesCommande = ObtenirLignesCommande(commande.IdCommande);
                }
            }

            return commandes;
        }

        /// <summary>
        /// Recupere les commandes d'un cuisinier
        /// </summary>
        /// <param name="idCuisinier">L'id du cuisinier</param>
        /// <returns>Liste des commandes du cuisinier</returns>
        public static List<Commande> ObtenirCommandesParCuisinier(int idCuisinier)
        {
            List<Commande> commandes = new List<Commande>();
            string query = "SELECT * FROM Commande WHERE id_Cuisinier = @idCuisinier";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Commande commande = new Commande
                            {
                                IdCommande = Convert.ToInt32(reader["id_Commande"]),
                                NumeroCommande = reader["numero_Commande"].ToString() ?? string.Empty,
                                IdClient = Convert.ToInt32(reader["id_Client"]),
                                IdCuisinier = reader["id_Cuisinier"] != DBNull.Value ? Convert.ToInt32(reader["id_Cuisinier"]) : 0,
                                DateCommande = Convert.ToDateTime(reader["date_Commande"]),
                                PrixTotal = Convert.ToDecimal(reader["prix_Total"]),
                                PrixCommande = Convert.ToDecimal(reader["prix_Commande"]),
                                IdLivraison = reader["id_Livraison"] != DBNull.Value ? (int?)Convert.ToInt32(reader["id_Livraison"]) : null,
                                IdHistorique = reader["id_Historique"] != DBNull.Value ? (int?)Convert.ToInt32(reader["id_Historique"]) : null,
                                StatutCommande = reader["statut_Commande"].ToString() ?? string.Empty
                            };

                            commandes.Add(commande);
                        }
                    }
                }

                /// <summary>
                /// Charge les lignes de commande pour chaque commande
                /// </summary>
                foreach (var commande in commandes)
                {
                    commande.LignesCommande = ObtenirLignesCommande(commande.IdCommande);
                }
            }

            return commandes;
        }
    }
}