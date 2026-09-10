using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LivInParis.Models;
using LivInParis.Data;

namespace LivInParis.Services
{
    /// <summary>
    /// Service qui gere les plats dans la bdd
    /// </summary>
    public class PlatService
    {
        /// <summary>
        /// Ajoute un plat dans la bdd
        /// </summary>
        /// <param name="plat">Le plat a ajouter</param>
        /// <returns>L'id du plat ajouté</returns>
        public static int AjouterPlat(Plat plat)
        {
            string query = @"INSERT INTO Plat 
                           (nom_Plat, description_Plat, prix_Plat, categorie_Plat, id_Cuisinier,
                            date_Fabrication, date_Peremption, ingredient_Plat, type_Plat, 
                            regime_Plat, nationalite_Plat, photo_Plat, nombre_Plat) 
                           VALUES 
                           (@nom, @description, @prix, @categorie, @idCuisinier,
                            @dateFabrication, @datePeremption, @ingredient, @type,
                            @regime, @nationalite, @photo, @nombre);
                           SELECT LAST_INSERT_ID();";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nom", plat.NomPlat);
                    command.Parameters.AddWithValue("@description", plat.DescriptionPlat);
                    command.Parameters.AddWithValue("@prix", plat.PrixPlat);
                    command.Parameters.AddWithValue("@categorie", plat.CategoriePlat);
                    command.Parameters.AddWithValue("@idCuisinier", plat.IdCuisinier);
                    command.Parameters.AddWithValue("@dateFabrication", plat.DateFabrication);
                    command.Parameters.AddWithValue("@datePeremption", plat.DatePeremption);
                    command.Parameters.AddWithValue("@ingredient", plat.IngredientPlat);
                    command.Parameters.AddWithValue("@type", plat.TypePlat);
                    command.Parameters.AddWithValue("@regime", plat.RegimePlat);
                    command.Parameters.AddWithValue("@nationalite", plat.NationalitePlat);
                    command.Parameters.AddWithValue("@photo", plat.PhotoPlat);
                    command.Parameters.AddWithValue("@nombre", plat.NombrePlat);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Met a jour un plat dans la bdd
        /// </summary>
        /// <param name="plat">Le plat avec les nouvelles infos</param>
        /// <returns>True si ca a marché</returns>
        public static bool MettreAJourPlat(Plat plat)
        {
            string query = @"UPDATE Plat 
                           SET nom_Plat = @nom, 
                               description_Plat = @description, 
                               prix_Plat = @prix, 
                               categorie_Plat = @categorie,
                               date_Fabrication = @dateFabrication,
                               date_Peremption = @datePeremption,
                               ingredient_Plat = @ingredient,
                               type_Plat = @type,
                               regime_Plat = @regime,
                               nationalite_Plat = @nationalite,
                               photo_Plat = @photo,
                               nombre_Plat = @nombre
                           WHERE id_Plat = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", plat.IdPlat);
                    command.Parameters.AddWithValue("@nom", plat.NomPlat);
                    command.Parameters.AddWithValue("@description", plat.DescriptionPlat);
                    command.Parameters.AddWithValue("@prix", plat.PrixPlat);
                    command.Parameters.AddWithValue("@categorie", plat.CategoriePlat);
                    command.Parameters.AddWithValue("@dateFabrication", plat.DateFabrication);
                    command.Parameters.AddWithValue("@datePeremption", plat.DatePeremption);
                    command.Parameters.AddWithValue("@ingredient", plat.IngredientPlat);
                    command.Parameters.AddWithValue("@type", plat.TypePlat);
                    command.Parameters.AddWithValue("@regime", plat.RegimePlat);
                    command.Parameters.AddWithValue("@nationalite", plat.NationalitePlat);
                    command.Parameters.AddWithValue("@photo", plat.PhotoPlat);
                    command.Parameters.AddWithValue("@nombre", plat.NombrePlat);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Supprime un plat de la bdd
        /// </summary>
        /// <param name="idPlat">L'id du plat a supprimer</param>
        /// <returns>True si ca a marché</returns>
        public static bool SupprimerPlat(int idPlat)
        {
            string query = "DELETE FROM Plat WHERE id_Plat = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idPlat);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Recupere un plat par son ID
        /// </summary>
        /// <param name="idPlat">L'id du plat a chercher</param>
        /// <returns>Le plat ou null si pas trouvé</returns>
        public static Plat? ObtenirPlatParId(int idPlat)
        {
            string query = "SELECT * FROM Plat WHERE id_Plat = @id";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", idPlat);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Plat
                            {
                                IdPlat = Convert.ToInt32(reader["id_Plat"]),
                                NomPlat = reader["nom_Plat"].ToString() ?? string.Empty,
                                DescriptionPlat = reader["description_Plat"].ToString() ?? string.Empty,
                                PrixPlat = Convert.ToDecimal(reader["prix_Plat"]),
                                CategoriePlat = reader["categorie_Plat"].ToString() ?? string.Empty,
                                IdCuisinier = Convert.ToInt32(reader["id_Cuisinier"]),
                                DateFabrication = reader["date_Fabrication"] != DBNull.Value ? Convert.ToDateTime(reader["date_Fabrication"]) : DateTime.Now,
                                DatePeremption = reader["date_Peremption"] != DBNull.Value ? Convert.ToDateTime(reader["date_Peremption"]) : DateTime.Now.AddDays(3),
                                IngredientPlat = reader["ingredient_Plat"] != DBNull.Value ? reader["ingredient_Plat"].ToString() ?? string.Empty : string.Empty,
                                TypePlat = reader["type_Plat"] != DBNull.Value ? reader["type_Plat"].ToString() ?? string.Empty : string.Empty,
                                RegimePlat = reader["regime_Plat"] != DBNull.Value ? reader["regime_Plat"].ToString() ?? string.Empty : string.Empty,
                                NationalitePlat = reader["nationalite_Plat"] != DBNull.Value ? reader["nationalite_Plat"].ToString() ?? string.Empty : string.Empty,
                                PhotoPlat = reader["photo_Plat"] != DBNull.Value ? reader["photo_Plat"].ToString() ?? string.Empty : string.Empty,
                                NombrePlat = reader["nombre_Plat"] != DBNull.Value ? Convert.ToInt32(reader["nombre_Plat"]) : 0
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Recupere tous les plats de la bdd
        /// </summary>
        /// <returns>Liste de tous les plats</returns>
        public static List<Plat> ObtenirTousLesPlats()
        {
            List<Plat> plats = new List<Plat>();
            string query = "SELECT * FROM Plat";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            plats.Add(new Plat
                            {
                                IdPlat = Convert.ToInt32(reader["id_Plat"]),
                                NomPlat = reader["nom_Plat"].ToString() ?? string.Empty,
                                DescriptionPlat = reader["description_Plat"].ToString() ?? string.Empty,
                                PrixPlat = Convert.ToDecimal(reader["prix_Plat"]),
                                CategoriePlat = reader["categorie_Plat"].ToString() ?? string.Empty,
                                IdCuisinier = Convert.ToInt32(reader["id_Cuisinier"]),
                                DateFabrication = reader["date_Fabrication"] != DBNull.Value ? Convert.ToDateTime(reader["date_Fabrication"]) : DateTime.Now,
                                DatePeremption = reader["date_Peremption"] != DBNull.Value ? Convert.ToDateTime(reader["date_Peremption"]) : DateTime.Now.AddDays(3),
                                IngredientPlat = reader["ingredient_Plat"] != DBNull.Value ? reader["ingredient_Plat"].ToString() ?? string.Empty : string.Empty,
                                TypePlat = reader["type_Plat"] != DBNull.Value ? reader["type_Plat"].ToString() ?? string.Empty : string.Empty,
                                RegimePlat = reader["regime_Plat"] != DBNull.Value ? reader["regime_Plat"].ToString() ?? string.Empty : string.Empty,
                                NationalitePlat = reader["nationalite_Plat"] != DBNull.Value ? reader["nationalite_Plat"].ToString() ?? string.Empty : string.Empty,
                                PhotoPlat = reader["photo_Plat"] != DBNull.Value ? reader["photo_Plat"].ToString() ?? string.Empty : string.Empty,
                                NombrePlat = reader["nombre_Plat"] != DBNull.Value ? Convert.ToInt32(reader["nombre_Plat"]) : 0
                            });
                        }
                    }
                }
            }

            return plats;
        }

        /// <summary>
        /// Recupere les plats d'un cuisinier
        /// </summary>
        /// <param name="idCuisinier">L'id du cuisinier</param>
        /// <returns>Liste des plats du cuisinier</returns>
        public static List<Plat> ObtenirPlatsParCuisinier(int idCuisinier)
        {
            List<Plat> plats = new List<Plat>();
            string query = "SELECT * FROM Plat WHERE id_Cuisinier = @idCuisinier";

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
                            plats.Add(new Plat
                            {
                                IdPlat = Convert.ToInt32(reader["id_Plat"]),
                                NomPlat = reader["nom_Plat"].ToString() ?? string.Empty,
                                DescriptionPlat = reader["description_Plat"].ToString() ?? string.Empty,
                                PrixPlat = Convert.ToDecimal(reader["prix_Plat"]),
                                CategoriePlat = reader["categorie_Plat"].ToString() ?? string.Empty,
                                IdCuisinier = Convert.ToInt32(reader["id_Cuisinier"]),
                                DateFabrication = reader["date_Fabrication"] != DBNull.Value ? Convert.ToDateTime(reader["date_Fabrication"]) : DateTime.Now,
                                DatePeremption = reader["date_Peremption"] != DBNull.Value ? Convert.ToDateTime(reader["date_Peremption"]) : DateTime.Now.AddDays(3),
                                IngredientPlat = reader["ingredient_Plat"] != DBNull.Value ? reader["ingredient_Plat"].ToString() ?? string.Empty : string.Empty,
                                TypePlat = reader["type_Plat"] != DBNull.Value ? reader["type_Plat"].ToString() ?? string.Empty : string.Empty,
                                RegimePlat = reader["regime_Plat"] != DBNull.Value ? reader["regime_Plat"].ToString() ?? string.Empty : string.Empty,
                                NationalitePlat = reader["nationalite_Plat"] != DBNull.Value ? reader["nationalite_Plat"].ToString() ?? string.Empty : string.Empty,
                                PhotoPlat = reader["photo_Plat"] != DBNull.Value ? reader["photo_Plat"].ToString() ?? string.Empty : string.Empty,
                                NombrePlat = reader["nombre_Plat"] != DBNull.Value ? Convert.ToInt32(reader["nombre_Plat"]) : 0
                            });
                        }
                    }
                }
            }

            return plats;
        }

        /// <summary>
        /// Recherche des plats par nom, categorie ou ingredients
        /// </summary>
        /// <param name="terme">Le terme de recherche</param>
        /// <returns>Liste des plats correspondants</returns>
        public static List<Plat> RechercherPlats(string terme)
        {
            List<Plat> plats = new List<Plat>();
            string query = @"SELECT * FROM Plat 
                           WHERE nom_Plat LIKE @terme 
                           OR categorie_Plat LIKE @terme 
                           OR ingredient_Plat LIKE @terme
                           OR type_Plat LIKE @terme
                           OR regime_Plat LIKE @terme
                           OR nationalite_Plat LIKE @terme";

            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@terme", "%" + terme + "%");
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            plats.Add(new Plat
                            {
                                IdPlat = Convert.ToInt32(reader["id_Plat"]),
                                NomPlat = reader["nom_Plat"].ToString() ?? string.Empty,
                                DescriptionPlat = reader["description_Plat"].ToString() ?? string.Empty,
                                PrixPlat = Convert.ToDecimal(reader["prix_Plat"]),
                                CategoriePlat = reader["categorie_Plat"].ToString() ?? string.Empty,
                                IdCuisinier = Convert.ToInt32(reader["id_Cuisinier"]),
                                DateFabrication = reader["date_Fabrication"] != DBNull.Value ? Convert.ToDateTime(reader["date_Fabrication"]) : DateTime.Now,
                                DatePeremption = reader["date_Peremption"] != DBNull.Value ? Convert.ToDateTime(reader["date_Peremption"]) : DateTime.Now.AddDays(3),
                                IngredientPlat = reader["ingredient_Plat"] != DBNull.Value ? reader["ingredient_Plat"].ToString() ?? string.Empty : string.Empty,
                                TypePlat = reader["type_Plat"] != DBNull.Value ? reader["type_Plat"].ToString() ?? string.Empty : string.Empty,
                                RegimePlat = reader["regime_Plat"] != DBNull.Value ? reader["regime_Plat"].ToString() ?? string.Empty : string.Empty,
                                NationalitePlat = reader["nationalite_Plat"] != DBNull.Value ? reader["nationalite_Plat"].ToString() ?? string.Empty : string.Empty,
                                PhotoPlat = reader["photo_Plat"] != DBNull.Value ? reader["photo_Plat"].ToString() ?? string.Empty : string.Empty,
                                NombrePlat = reader["nombre_Plat"] != DBNull.Value ? Convert.ToInt32(reader["nombre_Plat"]) : 0
                            });
                        }
                    }
                }
            }

            return plats;
        }
    }
}