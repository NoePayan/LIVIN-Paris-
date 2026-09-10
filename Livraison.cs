using System;

namespace LivInParis.Models
{
    /// <summary>
    /// Classe qui represente une livraison dans l'application
    /// </summary>
    public class Livraison
    {
        public int IdLivraison { get; set; }
        public DateTime DateLivraison { get; set; }
        public string AdresseLivraison { get; set; } = string.Empty;
        public string StatutLivraison { get; set; } = string.Empty;
        public string CommentaireLivraison { get; set; } = string.Empty;

        /// <summary>
        /// Constructeur par defaut
        /// </summary>
        public Livraison()
        {
            DateLivraison = DateTime.Now.AddHours(1); /// Par défaut, livraison dans 1 heure
            StatutLivraison = "En préparation";
        }

        /// <summary>
        /// Constructeur avec adresse et date
        /// </summary>
        /// <param name="adresse">L'adresse de livraison</param>
        /// <param name="dateLivraison">La date de livraison</param>
        public Livraison(string adresse, DateTime dateLivraison)
        {
            AdresseLivraison = adresse;
            DateLivraison = dateLivraison;
            StatutLivraison = "En préparation";
        }

        /// <summary>
        /// Renvoie une chaine avec les infos de la livraison
        /// </summary>
        /// <returns>Les infos de la livraison</returns>
        public override string ToString()
        {
            return "Livraison prévue le " + DateLivraison.ToString("dd/MM/yyyy à HH:mm") + " - " + StatutLivraison + " - " + AdresseLivraison;
        }
    }
}