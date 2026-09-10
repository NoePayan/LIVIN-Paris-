using System;

namespace LivInParis.Models
{
    /// <summary>
    /// la classe qui regroupe les informations concernant un client, les attributs quye l'on met en Empty sont ceux qu'ik rempli, les autres sont automatiques ou a choisir
    /// </summary>
    public class Client
    {
        /// <summary>
        /// identification du client
        /// </summary>
        public int IdClient { get; set; }

        public string NomClient { get; set; } = string.Empty;
        public string PrenomClient { get; set; } = string.Empty;
        public string EmailClient { get; set; } = string.Empty;
        public string MotDePasseClient { get; set; } = string.Empty;
        public string TelephoneClient { get; set; } = string.Empty;
        public string AdresseClient { get; set; } = string.Empty;
        public int StationMetroProche { get; set; } 

        /// <summary>
        /// le constructeur initial
        /// </summary>
        public Client()
        {
            
        }
        /// <summary>
        /// le constructeur donnant les infos
        /// </summary>
        /// <param name="nom">son Nom</param>
        /// <param name="prenom">son prenom</param>
        /// <param name="email">son mail</param>
        /// <param name="motDePasse">son mot de passe</param>
        /// <param name="telephone">son telephone</param>
        /// <param name="adresse">son adresse</param>
        /// <param name="stationMetro">sa station de metro</param>

        public Client(string nom, string prenom, string email, string motDePasse, string telephone, string adresse, int stationMetro)
        {
            NomClient = nom;
            PrenomClient = prenom;
            EmailClient = email;
            MotDePasseClient = motDePasse;
            TelephoneClient = telephone;
            AdresseClient = adresse;
            StationMetroProche = stationMetro;
        }

        public override string ToString()
        {
            return PrenomClient+NomClient+", "+EmailClient;
        }
    }
}