using System;

namespace LivInParis.Models
{
    /// <summary>
    /// Classe qui represente une ligne de commande dans l'application
    /// </summary>
    public class LigneCommande
    {
        public int IdLigneCommande { get; set; }
        public int IdCommande { get; set; }
        public int IdPlat { get; set; } 
        public int IdLivraison { get; set; }
        public int Quantite { get; set; }
        /// <summary>
        /// Cette propriété n'est pas dans la table mais est utilisée dans le code
        /// </summary>
        public decimal PrixUnitaire { get; set; } 
        public DateTime? DateLivraison { get; set; }
        public string LieuLivraison { get; set; } = string.Empty;

        /// <summary>
        /// Propriété de navigation vers le plat
        /// </summary>
        public Plat? Plat { get; set; }

        /// <summary>
        /// Calcule le prix total de la ligne
        /// </summary>
        public decimal PrixTotal {get { return Quantite * PrixUnitaire; }}

        /// <summary>
        /// Constructeur par defaut
        /// </summary>
        public LigneCommande()
        {
            DateLivraison = DateTime.Now.AddHours(1);
        }

        /// <summary>
        /// Constructeur avec plat, quantité et prix
        /// </summary>
        /// <param name="idPlat">L'id du plat</param>
        /// <param name="quantite">La quantité commandée</param>
        /// <param name="prixUnitaire">Le prix unitaire du plat</param>
        public LigneCommande(int idPlat, int quantite, decimal prixUnitaire)
        {
            IdPlat = idPlat;
            Quantite = quantite;
            PrixUnitaire = prixUnitaire;
            DateLivraison = DateTime.Now.AddHours(1);
        }

        /// <summary>
        /// Renvoie une chaine avec les infos de la ligne
        /// </summary>
        /// <returns>Les infos de la ligne de commande</returns>
        public override string ToString()
        {
            string nomPlat = Plat != null ? Plat.NomPlat : "Plat #" + IdPlat;
            return Quantite + " x " + nomPlat + " (" + PrixUnitaire.ToString("C") + ") = " + PrixTotal.ToString("C");
        }
    }
}