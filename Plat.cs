using System;

namespace LivInParis.Models
{
    /// <summary>
    /// Classe qui represente un plat dans l'application
    /// </summary>
    public class Plat
    {
        public int IdPlat { get; set; }
        public string NomPlat { get; set; } = string.Empty;
        public string DescriptionPlat { get; set; } = string.Empty;
        public decimal PrixPlat { get; set; }
        public string CategoriePlat { get; set; } = string.Empty;
        public int IdCuisinier { get; set; }
        public DateTime DateFabrication { get; set; }
        public DateTime DatePeremption { get; set; }
        public string IngredientPlat { get; set; } = string.Empty;
        public string TypePlat { get; set; } = string.Empty;
        public string RegimePlat { get; set; } = string.Empty;
        public string NationalitePlat { get; set; } = string.Empty;
        public string PhotoPlat { get; set; } = string.Empty;
        public int NombrePlat { get; set; }

        /// <summary>
        /// Propriété de navigation vers le cuisinier
        /// </summary>
        public Cuisinier? Cuisinier { get; set; }

        /// <summary>
        /// Constructeur par defaut
        /// </summary>
        public Plat()
        {
            DateFabrication = DateTime.Now;
            DatePeremption = DateTime.Now.AddDays(3);
        }

        /// <summary>
        /// Constructeur avec tous les parametres
        /// </summary>
        /// <param name="nom">Le nom du plat</param>
        /// <param name="description">La description du plat</param>
        /// <param name="prix">Le prix du plat</param>
        /// <param name="categorie">La categorie du plat</param>
        /// <param name="idCuisinier">L'id du cuisinier qui a fait le plat</param>
        /// <param name="ingredients">Les ingredients du plat</param>
        /// <param name="type">Le type de cuisine</param>
        /// <param name="regime">Le regime alimentaire</param>
        /// <param name="nationalite">La nationalité du plat</param>
        /// <param name="nombre">Le nombre de portions disponibles</param>
        public Plat(string nom, string description, decimal prix, string categorie, int idCuisinier,
                   string ingredients, string type, string regime, string nationalite, int nombre)
        {
            NomPlat = nom;
            DescriptionPlat = description;
            PrixPlat = prix;
            CategoriePlat = categorie;
            IdCuisinier = idCuisinier;
            DateFabrication = DateTime.Now;
            DatePeremption = DateTime.Now.AddDays(3);
            IngredientPlat = ingredients;
            TypePlat = type;
            RegimePlat = regime;
            NationalitePlat = nationalite;
            NombrePlat = nombre;
        }

        /// <summary>
        /// Renvoie une chaine avec les infos du plat
        /// </summary>
        /// <returns>Le nom, prix et categorie du plat</returns>
        public override string ToString()
        {
            return NomPlat + " - " + PrixPlat.ToString("C") + " - " + CategoriePlat;
        }

        /// <summary>
        /// Affiche les details complets du plat
        /// </summary>
        /// <returns>Les details du plat</returns>
        public string AfficherDetails()
        {
            return "Nom: " + NomPlat + "\n" +
                   "Description: " + DescriptionPlat + "\n" +
                   "Prix: " + PrixPlat.ToString("C") + "\n" +
                   "Catégorie: " + CategoriePlat + "\n" +
                   "Type: " + TypePlat + "\n" +
                   "Régime: " + RegimePlat + "\n" +
                   "Nationalité: " + NationalitePlat + "\n" +
                   "Ingrédients: " + IngredientPlat + "\n" +
                   "Fabriqué le: " + DateFabrication.ToString("dd/MM/yyyy") + "\n" +
                   "À consommer avant le: " + DatePeremption.ToString("dd/MM/yyyy") + "\n" +
                   "Quantité disponible: " + NombrePlat;
        }
    }
}