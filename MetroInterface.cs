
using ETAPE2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Collections.Specialized.BitVector32;
using System.Linq;

namespace ETAPE2
{
    /// <summary>
    /// ici toutes les différentes fonctionnalités quant au graphe sont centralisées. 
    /// </summary>
    public class LivinParisMetro
    {




        /// <summary>
        /// pour cette méthode que nous avions vu lors de la première année, 
        /// nous avons demandé à mistral comment faire en sorte qu'elle soit adaptable à un maximum de fonctions
        /// Elle permet de mesurer le temps d'exécution d'une fonction générique.
        /// Le temps d'exécution est mesuré en millisecondes et affiché dans la console.
        /// </summary>
        /// <typeparam name="T">Le type de retour de la fonction. nous l'avons normalisé afin de s'adapter à tout type de fonction</typeparam>
        /// <param name="action">La fonction à exécuter.</param>
        /// <param name="nomAlgo">Le nom de retour pour la console</param>
        /// <returns>renvoie le temps en ms</returns>
        /// 

        static T MesurerTemps<T>(Func<T> action, string nomAlgo)
        {

            Stopwatch chrono = new Stopwatch();


            chrono.Start();


            T resultat = action();


            chrono.Stop();


            double tempsEcoule = chrono.ElapsedMilliseconds;
            Console.WriteLine("Temps d'exécution de l'algorithme : " + nomAlgo + " : " + tempsEcoule + "ms");

            return resultat;
        }

        /// <summary>
        /// Ici, il s'agit de la methode qui va afficher le résultat d'un chemin dans la console
        /// Elle affiche le temps total du trajet, la liste des stations traversées et les lignes utilisées.
        /// </summary>
        /// <param name="tempsTotal">Le temps du trajet</param>
        /// <param name="chemin">Les stations de métro utilisées</param>
        /// <param name="lignes">La liste des lignes utilisees</param>
        static void AfficherResultat(double tempsTotal, List<string> chemin, List<string> lignes)
        {
            tempsTotal = Math.Round(tempsTotal);
            Console.WriteLine("Temps total du trajet : " + tempsTotal + " minutes");

            Console.WriteLine("Trajet :");

            for (int i = 0; i < chemin.Count - 1; i++)
            {
                Console.Write(chemin[i] + "--->" + "(" + lignes[i] + ")" + "-->");
            }

            Console.WriteLine(chemin[chemin.Count - 1]);


        }





        public static void PresGraphe()
        {
            var cheminFichierNoeuds = "feuillenoeuds3.txt";
            var cheminFichierLiens = "feuilleliens3.txt";

            var graphe = new Graphe();

            graphe.InitialiserNoeuds(cheminFichierNoeuds);

            graphe.CreerLiensDepuisFichier(cheminFichierLiens);
            graphe.AfficherNoeuds();


            graphe.AfficherLiens();


            if (graphe.EstConnexe())
            {
                Console.WriteLine("Le graphe est connexe !");
            }
            else
            {
                Console.WriteLine("Le graphe n'est pas connexe.");
            }

            Console.WriteLine("Le graphe est complet ? " + graphe.EstComplet());


            double latDepart = 48.829513;
            double longDepart = 2.349668;

            double latArrivee = 48.841112;
            double longArrivee = 2.325174;

            Console.WriteLine("Algorithme de Dijkstra");
            

            var resultatDijkstra = MesurerTemps(
                delegate {
                    return graphe.Dijkstra(latDepart, longDepart, latArrivee, longArrivee);
                },
                "Dijkstra"
            );


            double tempsTotal1 = resultatDijkstra.tempsTotal;
            List<string> chemin1 = resultatDijkstra.chemin;
            List<string> lignes1 = resultatDijkstra.lignes;
            AfficherResultat(tempsTotal1, chemin1, lignes1);

            Console.WriteLine("Algorithme de Bellman-Ford");

            var resultatBellman = MesurerTemps(
                delegate {
                    return graphe.BellmanFord(latDepart, longDepart, latArrivee, longArrivee);
                },
                "Bellman-Ford"
            );
            double tempsTotal2 = resultatBellman.tempsTotal;
            List<string> chemin2 = resultatBellman.chemin;
            List<string> lignes2 = resultatBellman.lignes;
            AfficherResultat(tempsTotal2, chemin2, lignes2);







            Console.WriteLine("Floyd-Warshall");
            var resultatFloyd = MesurerTemps(
                delegate {
                    return graphe.Dijkstra(latDepart, longDepart, latArrivee, longArrivee);
                },
                "Floyd-Warshall"
            );
            double tempsTotal = resultatFloyd.tempsTotal;
            List<string> chemin = resultatFloyd.chemin;
            List<string> lignes = resultatFloyd.lignes;
            AfficherResultat(tempsTotal, chemin, lignes);


            string loc1 = "metro_path.png";
            var pathVisualizer = new RepresentationChemin(graphe, chemin, lignes);
            pathVisualizer.DessinerChemin(loc1);
            Console.WriteLine("Chemin enregistré a " + loc1);
            Process.Start(new ProcessStartInfo(loc1) { UseShellExecute = true });




            string loc = "metro_graph.png";
            var graphVisualizer = new RepresentationGraphique(graphe);
            graphVisualizer.DessinerGraphe(loc);
            Console.WriteLine("Graph enregistré à " + loc);
            Process.Start(new ProcessStartInfo(loc) { UseShellExecute = true });
        }

        public static int RecupererStation()
        {
            var cheminFichierNoeuds = "feuillenoeuds3.txt";
            var cheminFichierLiens = "feuilleliens3.txt";

            var graphe = new Graphe();

            graphe.InitialiserNoeuds(cheminFichierNoeuds);

            graphe.CreerLiensDepuisFichier(cheminFichierLiens);

            int ig = graphe.AfficherEtChoisirNoeud();
            return ig;
        }
        /// <summary>
        /// cree le chemin entre 2 stations
        /// </summary>
        /// <param name="id1">l'index de la station 1</param>
        /// <param name="id2">l'index de la station 2</param>
        public static void CreerChemin(int id1, int id2)
        {
            var cheminFichierNoeuds = "feuillenoeuds3.txt";
            var cheminFichierLiens = "feuilleliens3.txt";

            var graphe = new Graphe();

            graphe.InitialiserNoeuds(cheminFichierNoeuds);

            graphe.CreerLiensDepuisFichier(cheminFichierLiens);
            var (latD, longD) = ExtraireCoordonneesNoeud(graphe, id1);
            var (latA, longA) = ExtraireCoordonneesNoeud(graphe, id2);
            var resultatFloyd = MesurerTemps(
                delegate {
                    return graphe.Dijkstra(latD, longD, latA, longA);
                },
                "Floyd-Warshall"
            );


            double tempsTotal = resultatFloyd.tempsTotal;
            List<string> chemin = resultatFloyd.chemin;
            List<string> lignes = resultatFloyd.lignes;
            AfficherResultat(tempsTotal, chemin, lignes);
            string loc1 = "metro_path.png";
            var pathVisualizer = new RepresentationChemin(graphe, chemin, lignes);
            pathVisualizer.DessinerChemin(loc1);
            Console.WriteLine("Chemin enregistré a " + loc1);
            Process.Start(new ProcessStartInfo(loc1) { UseShellExecute = true });
            Console.WriteLine(" souhaitez vous voir la carte de métro de Paris, \nOui : 0\nnOn : 1 ");
            int a = Convert.ToInt32(Console.ReadLine());
            if (a == 0)
            {
                string loc = "metro_graph.png";
                var graphVisualizer = new RepresentationGraphique(graphe);
                graphVisualizer.DessinerGraphe(loc);
                Console.WriteLine("Graph enregistré à " + loc);
                Process.Start(new ProcessStartInfo(loc) { UseShellExecute = true });
            }
        }
        /// <summary>
        /// Extrait les coordonnées d'un noeud à partir de son index global
        /// </summary>
        /// <param name="graphe">Le graphe contenant les noeuds</param>
        /// <param name="indexGlobal">L'index global du noeud</param>
        /// <returns>Un tuple contenant la latitude et la longitude du noeud</returns>
        public static (float latitude, float longitude) ExtraireCoordonneesNoeud(Graphe graphe, int indexGlobal)
        {
            Dictionary<Noeud<int>, int> indexGlobalByNoeud = new Dictionary<Noeud<int>, int>();
            Dictionary<int, Noeud<int>> noeudByIndexGlobal = new Dictionary<int, Noeud<int>>();
            int index = 0;

            
            foreach (var ligne in graphe.Noeuds
                .SelectMany(delegate (Noeud<int> n) { return n.ListeLignes; })
                .Distinct()
                .OrderBy(delegate (string l) { return l; }))
            {
                foreach (var noeud in graphe.Noeuds
                    .Where(delegate (Noeud<int> n) { return n.ListeLignes.Contains(ligne); })
                    .OrderBy(delegate (Noeud<int> n) { return n.Nom; }))
                {
                    if (!indexGlobalByNoeud.ContainsKey(noeud))
                    {
                        indexGlobalByNoeud[noeud] = index;
                        noeudByIndexGlobal[index] = noeud;
                        index++;
                    }
                }
            }


            if (!noeudByIndexGlobal.ContainsKey(indexGlobal))
            {
                throw new ArgumentException("Aucun noeud trouvé avec l'index global "+indexGlobal);
            }

            var noeudChoisi = noeudByIndexGlobal[indexGlobal];

            return (noeudChoisi.latitude, noeudChoisi.longitude);
        }


    }
}