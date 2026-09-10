using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETAPE2
{
    /// <summary>
    /// lors de la création de cette classe, nous avons mis les parametres demandés et donnés dans le tableau et avons rajouté
    /// INDEXGLOBAL qui va representer une station en entier. On stockera les identifiants et les lignes des difféerentes sous-stations dans le metro dans des listes
    /// </summary>
    /// <typeparam name="T">la normalisation du noeux</typeparam>
    public class Noeud<T>
    {

        public int INEXGLOBALNOEUD { get; set; }
        public string Nom { get; set; }
        public double Pondération { get; set; }
        public List<T> identifiantsStation { get; set; }
        public List<Noeud<T>> VoisinsSortants { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }
        public int codecommune { get; set; }
        public List<string> ListeLignes { get; set; }

        /// <summary>
        /// le constructeur de la classe
        /// </summary>
        /// <param name="Nom">le nom du noeud</param>
        /// <param name="Ponderation">son eventuelle pondération si il compte plusieurs lignes qui passent par lui</param>
        /// <param name="Numero">l'identifiant de la "sous station"</param>
        /// <param name="latitude">position de la station</param>
        /// <param name="longitude">position de la station</param>
        /// <param name="codecommune">code postal</param>
        /// <param name="ligne">numéro de la ligne de la sous station</param>
        /// <param name="INDEXGLOBALNOEUD">le numéro atribué à une sttion lors de sa création</param>
        public Noeud(string Nom, double Ponderation, T Numero, float latitude, float longitude, int codecommune, string ligne, int INDEXGLOBALNOEUD)
        {
            this.identifiantsStation = new List<T>();
            this.Nom = Nom;
            this.Pondération = Ponderation;
            this.VoisinsSortants = new List<Noeud<T>>();
            this.latitude = latitude;
            this.longitude = longitude;
            this.codecommune = codecommune;
            this.ListeLignes = new List<string> { ligne };
            this.INEXGLOBALNOEUD = INDEXGLOBALNOEUD;
            this.identifiantsStation.Add(Numero);
        }
    }
}