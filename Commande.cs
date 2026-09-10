using System;
using System.Collections.Generic;
using System.Linq;

namespace LivInParis.Models
{
    /// <summary>
    /// Classe qui represente une commande dans l'application
    /// </summary>
    public class Commande
    {
        public int IdCommande { get; set; }
        public string NumeroCommande { get; set; } = string.Empty;
        public int IdClient { get; set; }
        public int IdCuisinier { get; set; } /// Changé de int? à int
        public DateTime DateCommande { get; set; }
        public decimal PrixTotal { get; set; }
        public decimal PrixCommande { get; set; }
        public int? IdLivraison { get; set; }
        public int? IdHistorique { get; set; }
        public string StatutCommande { get; set; } = string.Empty;

        /// <summary>
        /// Propriétés de navigation vers les autres objets
        /// </summary>
        public Client? Client { get; set; }
        public Cuisinier? Cuisinier { get; set; }
        public Livraison? Livraison { get; set; }
        public Historique? Historique { get; set; }
        public List<LigneCommande> LignesCommande { get; set; } = new List<LigneCommande>();

        /// <summary>
        /// Constructeur par defaut
        /// </summary>
        public Commande()
        {
            DateCommande = DateTime.Now;
            NumeroCommande = GenererNumeroCommande();
            StatutCommande = "En attente";
            IdCuisinier = 0; /// Valeur par défaut
        }

        /// <summary>
        /// Constructeur avec client et lignes de commande
        /// </summary>
        /// <param name="idClient">L'id du client qui commande</param>
        /// <param name="lignesCommande">Les lignes de la commande</param>
        public Commande(int idClient, List<LigneCommande> lignesCommande)
        {
            IdClient = idClient;
            LignesCommande = lignesCommande;
            DateCommande = DateTime.Now;
            NumeroCommande = GenererNumeroCommande();
            StatutCommande = "En attente";
            IdCuisinier = 0; /// Sera mis à jour dans PasserCommande
            CalculerPrixTotal();
        }

        /// <summary>
        /// Genere un numero de commande unique
        /// </summary>
        /// <returns>Le numero de commande</returns>
        private string GenererNumeroCommande()
        {
            return "CMD-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        /// <summary>
        /// Calcule le prix total de la commande
        /// </summary>
        public void CalculerPrixTotal()
        {
            PrixTotal = LignesCommande.Sum(l => l.PrixTotal);
            PrixCommande = PrixTotal; /// Par défaut, le prix de la commande est égal au prix total
        }

        /// <summary>
        /// Ajoute une ligne a la commande
        /// </summary>
        /// <param name="ligne">La ligne a ajouter</param>
        public void AjouterLigneCommande(LigneCommande ligne)
        {
            LignesCommande.Add(ligne);
            CalculerPrixTotal();
        }

        /// <summary>
        /// Supprime une ligne de la commande
        /// </summary>
        /// <param name="idLigne">L'id de la ligne a supprimer</param>
        public void SupprimerLigneCommande(int idLigne)
        {
            var ligne = LignesCommande.FirstOrDefault(l => l.IdLigneCommande == idLigne);
            if (ligne != null)
            {
                LignesCommande.Remove(ligne);
                CalculerPrixTotal();
            }
        }

        /// <summary>
        /// Renvoie une chaine avec les infos de la commande
        /// </summary>
        /// <returns>Les infos de la commande</returns>
        public override string ToString()
        {
            string clientInfo = Client != null ? Client.ToString() : "Client #" + IdClient;
            return "Commande #" + NumeroCommande + " - " + DateCommande.ToString("dd/MM/yyyy HH:mm") + " - " + PrixTotal.ToString("C") + " - " + StatutCommande + " - " + clientInfo;
        }

        /// <summary>
        /// Affiche les details complets de la commande
        /// </summary>
        /// <returns>Les details de la commande</returns>
        public string AfficherDetails()
        {
            string result = "Commande #" + NumeroCommande + "\n" +
                           "Date: " + DateCommande.ToString("dd/MM/yyyy HH:mm") + "\n" +
                           "Client: " + (Client != null ? Client.ToString() : "Client #" + IdClient) + "\n" +
                           "Statut: " + StatutCommande + "\n" +
                           "Prix total: " + PrixTotal.ToString("C") + "\n\n" +
                           "Détails de la commande:\n";

            foreach (var ligne in LignesCommande)
            {
                result += "- " + ligne + "\n";
            }

            if (Livraison != null)
            {
                result += "\nLivraison: " + Livraison + "\n";
            }

            return result;
        }
    }
}