using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace LivInParis.Models
{
    /// <summary>
    /// la classe qui cree un trajet entre une station de depart et une d'arrivee
    /// </summary>
    public class Trajet
    {
        public int IdTrajet { get; set; }
        public string MetroTrajet { get; set; }
        public int DureeTrajet { get; set; } 
        public string StationDepartTrajet { get; set; }
        public string StationArriveeTrajet { get; set; }
        /// <summary>
        /// le constructeur de base
        /// </summary>
        public Trajet()
        {
            
        }

        public Trajet(string stationDepart, string stationArrivee, int duree)
        {
            StationDepartTrajet = stationDepart;
            StationArriveeTrajet = stationArrivee;
            DureeTrajet = duree;
        }
        /// <summary>
        /// la methode qui donne les infos sur un trajet
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "De "+StationDepartTrajet+" à "+StationArriveeTrajet+" - Durée: "+DureeTrajet+" min";
        }
    }
}