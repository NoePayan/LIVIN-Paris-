using System;
using System.Collections.Generic;

namespace LivInParis.Models
{
    /// <summary>
    /// Classe qui represente un cuisinier dans l'application
    /// </summary>
    public class Cuisinier
    {
        public int IdCuisinier { get; set; }
        public string NomCuisinier { get; set; } = string.Empty;
        public string PrenomCuisinier { get; set; } = string.Empty;
        public string EmailCuisinier { get; set; } = string.Empty;
        public string MotDePasseCuisinier { get; set; } = string.Empty;
        public string TelephoneCuisinier { get; set; } = string.Empty;
        public string SpecialiteCuisinier { get; set; } = string.Empty;
        public string AdresseCuisinier { get; set; } = string.Empty;
        public int StationMetroProche { get; set; } 

        /// <summary>
        /// Constructeur par defaut
        /// </summary>
        public Cuisinier()
        {
        }

        /// <summary>
        /// Constructeur avec tout les parametres
        /// </summary>
        /// <param name="nom">le nom du cuisinier</param>
        /// <param name="prenom">le prenom du cuisinier</param>
        /// <param name="email">l'email pour le contacter</param>
        /// <param name="motDePasse">son mot de passe securisé</param>
        /// <param name="telephone">son numero de tel</param>
        /// <param name="specialite">sa specialité culinaire</param>
        /// <param name="adresse">son adresse de domicile</param>
        /// <param name="stationMetro">la station de metro la plus proche</param>
        public Cuisinier(string nom, string prenom, string email, string motDePasse, string telephone, string specialite, string adresse, int stationMetro)
        {
            NomCuisinier = nom;
            PrenomCuisinier = prenom;
            EmailCuisinier = email;
            MotDePasseCuisinier = motDePasse;
            TelephoneCuisinier = telephone;
            SpecialiteCuisinier = specialite;
            AdresseCuisinier = adresse;
            StationMetroProche = stationMetro;
        }

        /// <summary>
        /// Renvoie une chaine avec les infos du cuisinier
        /// </summary>
        /// <returns>Le nom et la specialité</returns>
        public override string ToString()
        {
            return PrenomCuisinier + " " + NomCuisinier + " - Spécialité: " + SpecialiteCuisinier;
        }
    }
}