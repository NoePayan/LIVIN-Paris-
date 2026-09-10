using System;

namespace LivInParis.Models
{
    /// <summary>
    /// Classe qui represente un historique DE COMMANDE dans l'application
    /// </summary>
    public class Historique
    {
        public int IdHistorique { get; set; }
        public DateTime DateHistorique { get; set; }
        public string ActionHistorique { get; set; } = string.Empty;
        public string DetailsHistorique { get; set; } = string.Empty;

        /// <summary>
        /// Constructeur par defaut
        /// </summary>
        public Historique()
        {
            DateHistorique = DateTime.Now;
        }

        /// <summary>
        /// Constructeur avec action et details
        /// </summary>
        /// <param name="action">L'action effectuée</param>
        /// <param name="details">Les détails de l'action</param>
        public Historique(string action, string details)
        {
            DateHistorique = DateTime.Now;
            ActionHistorique = action;
            DetailsHistorique = details;
        }

        /// <summary>
        /// Renvoie une chaine avec les infos de l'historique
        /// </summary>
        /// <returns>Les infos de l'historique</returns>
        public override string ToString()
        {
            return DateHistorique.ToString("dd/MM/yyyy HH:mm:ss") + " - " + ActionHistorique + " - " + DetailsHistorique;
        }
    }
}