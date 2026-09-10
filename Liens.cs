
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETAPE2;

namespace ETAPE2
{
    /// <summary>
    /// la classe Lien va permettre d'établir la liaison enyre deux noeuds
    /// </summary>
    public class Lien
    {
        public string Ligne { get; set; }
        public Noeud<int> Depart { get; set; }
        public Noeud<int> Arrivee { get; set; }
        public int Temps { get; set; }

        /// <summary>
        /// le constructeur de la classe
        /// </summary>
        /// <param name="ligne">la ligne de métro que prend ce lien</param>
        /// <param name="depart">le noeud de départ du lien</param>
        /// <param name="arrivee">le noeud d'arrivée du lien</param>
        /// <param name="temps">la pondération du lien</param>
        public Lien(string ligne, Noeud<int> depart, Noeud<int> arrivee, int temps)
        {
            this.Ligne = ligne;
            this.Depart = depart;
            this.Arrivee = arrivee;
            this.Temps = temps;
        }
    }
}

