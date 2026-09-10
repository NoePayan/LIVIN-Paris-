
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ETAPE2
{
    /// <summary>
    /// ici sont regroupées l'ensemble des méthodes attenant au graphe
    /// </summary>
    public class Graphe
    {

        public List<Noeud<int>> Noeuds { get; set; }
        public List<Lien> Liens { get; set; }
        private int INDEXGLOBAL { get; set; }
        /// <summary>
        /// le constructeur du graphe
        /// </summary>
        public Graphe()
        {
            Noeuds = new List<Noeud<int>>();
            INDEXGLOBAL = 1;
            Liens = new List<Lien>();
        }
        /// <summary>
        /// on veut creer les neouds du Graphe à partir d'un fichier
        /// 1- on ignore la ligne des titres
        /// 2- on dit que les tabulations sont la séparation des chaines de caractère
        /// 3- pour chaque ligne, si il y a le bon nombre de colonnes on extrait les données
        /// 4-si un noeud avec le meme Nom existe déjà on ne crée pas de nouveau noeud mais on ajoute la ligne et l'id au noeud existant
        ///   sinon on crée un nouveau noeud avec les données extraites, la pondération sera modifiée lors de la création des liens
        ///   on ajoute un INDEXGLOBAL qui sera utilisé pour repertorier le nombre de noeuds et les différencier
        /// 5-on a deux messages d'erreurs dans le cas ou il y a un problème avec la création d'un noeud
        /// </summary>
        /// <param name="cheminFichier">la localisation du fichier</param>
        public void InitialiserNoeuds(string cheminFichier)
        {
            var lignes = File.ReadLines(cheminFichier).Skip(1);
            foreach (var ligne in lignes)
            {
                var colonnes = ligne.Split('\t');
                if (colonnes.Length == 7)
                {
                    try
                    {

                        var idStation = int.Parse(colonnes[0]);
                        var libelleLine = colonnes[1];
                        string libelleStation = colonnes[2];
                        double longitude = double.Parse(colonnes[3]);
                        double latitude = double.Parse(colonnes[4]);
                        string communeNom = colonnes[5];
                        int communeCodeInsee = int.Parse(colonnes[6]);


                        var noeudExistant = Noeuds.Find(n => n.Nom == libelleStation);
                        if (noeudExistant != null)
                        {
                            noeudExistant.identifiantsStation.Add(idStation);
                            noeudExistant.ListeLignes.Add(libelleLine);
                        }
                        else
                        {

                            var noeud = new Noeud<int>(
                                libelleStation,
                                0.0,
                                idStation,
                                (float)latitude,
                                (float)longitude,
                                communeCodeInsee,
                                libelleLine,
                                INDEXGLOBAL++
                            );
                            Noeuds.Add(noeud);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur à la ligne : " + ligne + ",  " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Pas le bon nombre de colonnes à la ligne " + ligne);
                }
            }
        }
        /// <summary>
        /// La méthode pour afficher les noeuds dans la console (sans utilité pour le projet mais utile pour tester)
        /// </summary>
        public void AfficherNoeuds()
        {
            Console.WriteLine("Liste des Noeuds :\n");
            if (Noeuds.Count == 0)
            {
                Console.WriteLine("Pas de Noeuds");
                return;
            }

            foreach (Noeud<int> noeud in Noeuds)
            {
                Console.WriteLine("Index Global : " + noeud.INEXGLOBALNOEUD);
                Console.WriteLine("Station : " + noeud.Nom);
                Console.WriteLine("ID Station(s) : " + string.Join(", ", noeud.identifiantsStation));
                Console.WriteLine("Ligne(s) : " + string.Join(", ", noeud.ListeLignes));
                Console.WriteLine("Longitude : " + noeud.longitude);
                Console.WriteLine("Latitude : " + noeud.latitude);
                Console.WriteLine("Commune : " + noeud.codecommune);
                Console.WriteLine("Pondération : " + noeud.Pondération);
                Console.WriteLine("------------------------------------------------------------");
            }
        }
        /// <summary>
        /// on veut creer les liens àn partir d'un deuxieme fichier'
        /// 1- on ignore la ligne des titres
        /// 2- on dit que les tabulations sont la séparation des chaines de caractère
        /// 3- pour chaque ligne, si il y a le bon nombre de colonnes on extrait les données
        /// 4-on cherche les noeuds de départ et d'arrivée pour vérifier si ils existent via leur INDEXGLOBAL
        /// 5-si ils existent on crée un lien entre eux avec la pondération, les stations et la ligne
        ///   puis on vérifie si la pondération du noeud (qui se trouve dans ce fichier) est nulle, si ce n'est pas le cas 
        ///   on la modifie dans les parametres du noeuds pour celui d'arrivée (comme nous le donne le format de l'excel)
        /// </summary>
        /// <param name="cheminFichier">la localisation fichier</param>
        public void CreerLiensDepuisFichier(string cheminFichier)
        {

            var lignes = File.ReadAllLines(cheminFichier);


            for (int i = 1; i < lignes.Length; i++)
            {
                var columns = lignes[i].Split('\t');
                if (columns.Length >= 7)
                {
                    try
                    {
                        var stationId = !string.IsNullOrWhiteSpace(columns[0]) ? int.Parse(columns[0]) : 0;
                        var stationNom = columns[1];
                        var precedentId = !string.IsNullOrWhiteSpace(columns[2]) ? int.Parse(columns[2]) : 0;
                        var suivantId = !string.IsNullOrWhiteSpace(columns[3]) ? int.Parse(columns[3]) : 0;
                        var tempsEntreStations = !string.IsNullOrWhiteSpace(columns[4]) ? int.Parse(columns[4]) : 0;
                        var tempsDeChangement = !string.IsNullOrWhiteSpace(columns[5]) ? int.Parse(columns[5]) : 0;
                        var metro = columns[6];


                        var depart = Noeuds.Find(n => n.identifiantsStation.Contains(precedentId));
                        var arrivee = Noeuds.Find(n => n.identifiantsStation.Contains(stationId));


                        if (depart != null && arrivee != null)
                        {

                            var lien = new Lien(metro, depart, arrivee, tempsEntreStations);
                            Liens.Add(lien);
                            depart.VoisinsSortants.Add(arrivee);

                            if (tempsDeChangement != 0)
                            {
                                arrivee.Pondération = tempsDeChangement;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur à la ligne : " + lignes[i] + ", " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Pas le bon nombre de colonnes à la ligne " + lignes[i]);
                }
            }
        }
        /// <summary>
        /// La méthode pour afficher les liens dans la console (sans utilité pour le projet mais utile pour tester)
        /// </summary>
        public void AfficherLiens()
        {
            Console.WriteLine("Liste des Liens :\n");
            if (Liens.Count == 0)
            {
                Console.WriteLine("Pas de liens");
                return;
            }

            foreach (Lien lien in Liens)
            {
                Console.WriteLine("Ligne: " + lien.Ligne + ", " + "Départ: " + lien.Depart.Nom + " (ID: " + lien.Depart.INEXGLOBALNOEUD + "), " +
                  "Arrivée: " + lien.Arrivee.Nom + " (ID: " + lien.Arrivee.INEXGLOBALNOEUD + "), " +
                  "Temps: " + lien.Temps + " min");
            }
        }
        /// <summary>
        /// méthode qui vérife si le graphe est connexe en verifiant via le parcours en profondeur si tout les neodus sont atteignables depuis un noeud
        /// </summary>
        /// <returns></returns>
        public bool EstConnexe()
        {
            if (Noeuds.Count == 0)
                return false;

            HashSet<Noeud<int>> visites = new HashSet<Noeud<int>>();


            DFS(Noeuds.First(), visites);


            return visites.Count == Noeuds.Count;
        }
        /// <summary>
        /// la méthode récursive qui réalise un parcours en profondeur
        /// </summary>
        /// <param name="noeud"></param>
        /// <param name="visites"></param>
        private void DFS(Noeud<int> noeud, HashSet<Noeud<int>> visites)
        {
            if (visites.Contains(noeud))
                return;

            visites.Add(noeud);


            foreach (var voisin in noeud.VoisinsSortants)
            {
                if (!visites.Contains(voisin))
                {
                    DFS(voisin, visites);
                }
            }
        }
        /// <summary>
        /// La méthode qui vérifie si le Graphe est complet en verifiant pour tout les noeuds si ils sont
        /// liés à tout les autres
        /// </summary>
        /// <returns>si il est complet ou non</returns>
        public bool EstComplet()
        {
            int nompbrenoeuds = Noeuds.Count;
            if (nompbrenoeuds <= 1) return true;

            foreach (var noeud in Noeuds)
            {
                HashSet<Noeud<int>> liaisons = new HashSet<Noeud<int>>(noeud.VoisinsSortants);
                liaisons.Add(noeud);

                if (liaisons.Count != nompbrenoeuds)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// la méthode qui va comparer de toute les stations pour trouver la plus proche 
        /// </summary>
        /// <param name="lat">la latitude du point</param>
        /// <param name="lon">la longitude du point</param>
        /// <returns></returns>
        private Noeud<int> TrouverStation(double lat, double lon)
        {
            Noeud<int> stationLaPlusProche = null;
            double distanceMin = double.MaxValue;

            foreach (var noeud in Noeuds)
            {
                double distance = CalculerDistanceGps(lat, lon, noeud.latitude, noeud.longitude);
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    stationLaPlusProche = noeud;
                }
            }

            return stationLaPlusProche;
        }
        /// <summary>
        /// methode qui calcule la distance entre deux coordonées GPS via la formule de Haversine 
        /// Nous l'avons séparé de celle du temps pour clarifiier où se situaient certaines erreurs
        /// </summary>
        /// <param name="lat1">latitude du premier point</param>
        /// <param name="lon1">longitude du premier point</param>
        /// <param name="lat2">latitude du deuxième point</param>
        /// <param name="lon2">lobgitude du deuxième point</param>
        private double CalculerDistanceGps(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371;
            double lat1R = lat1 * Math.PI / 180;
            double lat2R = lat2 * Math.PI / 180;
            double lon1R = lon1 * Math.PI / 180;
            double lon2R = lon2 * Math.PI / 180;

            double distancelatitude = lat2R - lat1R;
            double distancelongitude = lon2R - lon1R;

            double c = R * 2 * Math.Asin(Math.Sqrt(Math.Sin(distancelatitude / 2) * Math.Sin(distancelatitude / 2) + Math.Cos(lat1R) * Math.Cos(lat2R) * Math.Sin(distancelongitude / 2) * Math.Sin(distancelongitude / 2)));

            return c;
        }
        /// <summary>
        /// methode qui le temps entre deux coordonées GPS. Nous avons mis 9 km/h car nous ne marchons pas en ligne droite entre un point et la station
        /// </summary>
        /// <param name="lat1">latitude du premier point</param>
        /// <param name="lon1">longitude du premier point</param>
        /// <param name="station">la station de metro avec laquelle on veut savoir la distance</param>
        /// <returns>renvoie le temps entre les deux points</returns>
        private double CalculerTempsGps(double lat1, double lon1, Noeud<int> station)
        {
            double distance = CalculerDistanceGps(lat1, lon1, station.latitude, station.longitude);
            double vitesseMoyenne = 9;
            return (distance / vitesseMoyenne) * 60;
        }
        /// <summary>
        /// cette méthode va trouver le plus court chemin via l'algo de BellmanFord
        /// </summary>
        /// <param name="latDepart">latitude du point de départ</param>
        /// <param name="longDepart">longitude du point de départ</param>
        /// <param name="latArrivee">latitude du point d'arrivée</param>
        /// <param name="longArrivee">longitude du point d'arrivée</param>
        /// <returns>le temps, le chemin et les lignes</returns>
        public (double tempsTotal, List<string> chemin, List<string> lignes) Dijkstra(double latDepart, double longDepart, double latArrivee, double longArrivee)
        {
            /// <summary>
            /// on cherche les stations les plus proches du point de depart et d'arrivee
            /// </summary>
            var depart = TrouverStation(latDepart, longDepart);
            var arrivee = TrouverStation(latArrivee, longArrivee);

            /// <summary>
            /// Initialisation des structures de données pour l'algorithme de Dijkstra.
            /// distances contient chaque point et sa distance qui s'actualise au long de l'algo
            /// predecesseurs contient les les predecesseurs de chaque noeud pour sa distance minimale
            /// priorites gere l'ordre de traitement des noeuds selon leur distance
            /// lignesutilisees donne la ligne utilisee pour rejoindre une station
            /// </summary>
            var distances = new Dictionary<Noeud<int>, double>();
            var predecesseurs = new Dictionary<Noeud<int>, Noeud<int>>();
            var Priorites = new PriorityQueue<Noeud<int>, double>();
            var lignesUtilisees = new Dictionary<Noeud<int>, string>();
            /// <summary>
            /// va initialiser es distances de chaque noeud au maximum, les predecesseurs et lignes a null et et creer la liste de priorite
            /// puis initialiser le noeud initial
            /// </summary>
            foreach (var noeud in Noeuds)
            {
                distances[noeud] = double.MaxValue;
                predecesseurs[noeud] = null;
                Priorites.Enqueue(noeud, double.MaxValue);
                lignesUtilisees[noeud] = "";
            }
            /// <summary>
            /// on initialise la station de depart
            /// </summary>
            distances[depart] = 0;
            Priorites.Enqueue(arrivee, 0);
            /// <summary>
            /// tant qu'il reste des noeuds à traiter, on compare la distance des voisins du premier noeud de la file de priorite 
            /// à celle du noeud en question plus l'arc a son voisin et on actualise si necessaire
            /// </summary>
            while (Priorites.Count > 0)
            {
                var noeudact = Priorites.Dequeue();

                foreach (var voisin in noeudact.VoisinsSortants)
                {
                    var lien = Liens.FirstOrDefault(l => (l.Depart == noeudact && l.Arrivee == voisin) ||
                                                         (l.Depart == voisin && l.Arrivee == noeudact));
                    var pondlien = lien?.Temps ?? double.MaxValue;

                    var nouvdist = distances[noeudact] + pondlien;
                    var nouvligne = voisin.ListeLignes.FirstOrDefault();

                    if (!string.IsNullOrEmpty(nouvligne) && lignesUtilisees[noeudact] != nouvligne)
                    {
                        nouvdist += noeudact.Pondération;
                    }

                    if (nouvdist < distances[voisin])
                    {
                        distances[voisin] = nouvdist;
                        predecesseurs[voisin] = noeudact;
                        lignesUtilisees[voisin] = nouvligne;
                        Priorites.Enqueue(voisin, nouvdist);
                    }
                }
            }

            /// <summary>
            /// on part du noeud d'arrivée puis on reconstitue le chemin à partir des predecesseurs jusqu'a atteindre le départ et on trouve le temps de métro
            /// </summary>
            var cheminStations = new List<Noeud<int>>();
            var lignesUtiliseesFinal = new List<string>();
            var stationActuelle = arrivee;

            while (stationActuelle != null && stationActuelle != depart)
            {
                cheminStations.Insert(0, stationActuelle);
                if (predecesseurs[stationActuelle] != null)
                {
                    var lien = Liens.FirstOrDefault(l => (l.Depart == predecesseurs[stationActuelle] && l.Arrivee == stationActuelle) ||
                                                         (l.Depart == stationActuelle && l.Arrivee == predecesseurs[stationActuelle]));
                    if (lien != null)
                    {
                        lignesUtiliseesFinal.Insert(0, lien.Ligne);
                    }
                }
                stationActuelle = predecesseurs[stationActuelle];
            }

            cheminStations.Insert(0, depart);
            /// <summary>
            ///on ajoute au temps total les temps de marche
            /// </summary>

            double tempsTotal = distances[arrivee] +
                                CalculerTempsGps(latDepart, longDepart, depart) +
                                CalculerTempsGps(latArrivee, longArrivee, arrivee);

            return (tempsTotal, cheminStations.Select(s => s.Nom).ToList(), lignesUtiliseesFinal);
        }
        /// <summary>
        /// cette méthode va trouver le plus court chemin via l'algo de Dijkstra
        /// </summary>
        /// <param name="latDepart">latitude du point de départ</param>
        /// <param name="longDepart">longitude du point de départ</param>
        /// <param name="latArrivee">latitude du point d'arrivée</param>
        /// <param name="longArrivee">longitude du point d'arrivée</param>
        /// <returns>le temps, le chemin et les lignes</returns>
        public (double tempsTotal, List<string> chemin, List<string> lignes) BellmanFord(double latDepart, double longDepart, double latArrivee, double longArrivee)
        {
            /// <summary>
            /// On cherche les stations les plus proches du point de départ et d'arrivée.
            /// </summary>
            var stationDepart = TrouverStation(latDepart, longDepart);
            var stationArrivee = TrouverStation(latArrivee, longArrivee);

            /// <summary>
            /// Initialisation des données pour l'algorithme de Bellman-Ford
            /// distances stocke la distance minimale connue pour chaque noeud.
            /// predecesseurs stocke le noeud précédent dans le chemin optimal.
            /// lignesUtilisees stocke la ligne utilisée pour atteindre chaque station.
            /// </summary>
            var distances = new Dictionary<Noeud<int>, double>();
            var predecesseurs = new Dictionary<Noeud<int>, Noeud<int>>();
            var lignesUtilisees = new Dictionary<Noeud<int>, string>();

            /// <summary>
            /// On initialise les distances à l'infini pour tous les noeud sauf le point de départ.
            /// Le départ est initialisé à 0, et sa ligne est récupérée si disponible.
            /// </summary>
            foreach (var noeud in Noeuds)
            {
                distances[noeud] = double.MaxValue;
                predecesseurs[noeud] = null;
                lignesUtilisees[noeud] = "";
            }
            distances[stationDepart] = 0;
            lignesUtilisees[stationDepart] = stationDepart.ListeLignes.FirstOrDefault() ?? "";

            /// <summary>
            /// Application de l'algorithme 
            /// On met à jour les distances en parcourant tous les arcs du graphe N-1 fois.
            /// Si un changement est détecté, on continue l'itération, sinon on s'arrête.
            /// </summary>
            for (int i = 0; i < Noeuds.Count - 1; i++)
            {
                bool changement = false;

                foreach (var lien in Liens)
                {
                    var depart = lien.Depart;
                    var arrivee = lien.Arrivee;
                    double pond = lien.Temps;

                    /// <summary>
                    /// On vérifie si une distance plus courte est trouvée et si un changement de ligne implique une pénalité.
                    /// </summary>
                    if (distances[depart] != double.MaxValue)
                    {
                        double nouvdist = distances[depart] + pond;
                        string ligneDepart = lignesUtilisees[depart];
                        string ligneArrivee = lien.Ligne;

                        if (!string.IsNullOrEmpty(ligneDepart) && !string.IsNullOrEmpty(ligneArrivee) && ligneDepart != ligneArrivee && depart != stationDepart)
                        {
                            nouvdist += depart.Pondération; // Pénalité de changement de ligne
                        }

                        if (nouvdist < distances[arrivee])
                        {
                            distances[arrivee] = nouvdist;
                            predecesseurs[arrivee] = depart;
                            lignesUtilisees[arrivee] = ligneArrivee;
                            changement = true;
                        }
                    }
                }

                /// <summary>
                /// Si aucune mise à jour n'a été effectuée, l'algorithme peut s'arrêter avant
                /// </summary>
                if (!changement)
                    break;
            }

            /// <summary>
            /// Reconstitution du chemin optimal à partir des prédécesseurs.
            /// On part du nœud d'arrivée et on remonte jusqu'au départ.
            /// </summary>
            var cheminStations = new List<Noeud<int>>();
            var lignesUtiliseesFinal = new List<string>();
            var stationActuelle = stationArrivee;

            while (stationActuelle != null && stationActuelle != stationDepart)
            {
                cheminStations.Insert(0, stationActuelle);
                if (predecesseurs[stationActuelle] != null)
                {
                    var lignePrecedente = lignesUtilisees[stationActuelle];
                    var lien = Liens.Where(l =>
                        (l.Depart == predecesseurs[stationActuelle] && l.Arrivee == stationActuelle) ||
                        (l.Depart == stationActuelle && l.Arrivee == predecesseurs[stationActuelle]))
                        .OrderByDescending(l => l.Ligne == lignePrecedente) // Prioriser les liens de la même ligne
                        .FirstOrDefault();

                    if (lien != null)
                    {
                        lignesUtiliseesFinal.Insert(0, lien.Ligne);
                    }
                }
                stationActuelle = predecesseurs[stationActuelle];
            }

            cheminStations.Insert(0, stationDepart);

            /// <summary>
            /// Calcul du temps total du trajet, incluant les temps de marche GPS.
            /// </summary>
            double tempsTotal = distances[stationArrivee] +
                                CalculerTempsGps(latDepart, longDepart, stationDepart) +
                                CalculerTempsGps(latArrivee, longArrivee, stationArrivee);

            return (tempsTotal, cheminStations.Select(s => s.Nom).ToList(), lignesUtiliseesFinal);
        }
        /// <summary>
        /// cette méthode va trouver le plus court chemin via l'algo de Floyd-Warshall
        /// </summary>
        /// <param name="latDepart">latitude du point de départ</param>
        /// <param name="longDepart">longitude du point de départ</param>
        /// <param name="latArrivee">latitude du point d'arrivée</param>
        /// <param name="longArrivee">longitude du point d'arrivée</param>
        /// <returns>le temps, le chemin et les lignes</returns>
        public (double tempsTotal, List<string> chemin, List<string> lignes) FloydWarshall(double latDepart, double longDepart, double latArrivee, double longArrivee)
        {
            /// <summary>
            /// on cherche les stations les plus proches du point de depart et d'arrivee
            /// </summary>
            var stationDepart = TrouverStation(latDepart, longDepart);
            var stationArrivee = TrouverStation(latArrivee, longArrivee);

            /// <summary>
            /// Initialisation des matrices de Floyd-Warshall.
            /// distances stocke le poids minimal entre chaque paire de noeuds
            /// prochains stocke les indices des noeuds permettant de reconstruire le chemin optimal
            /// lignesUtilisees stocke la ligne empruntée entre chaque paire de noeuds
            /// </summary>
            int n = Noeuds.Count;
            double[,] distances = new double[n, n];
            int[,] prochains = new int[n, n];
            string[,] lignesUtilisees = new string[n, n];

            /// <summary>
            /// Initialisation des matrices :
            /// - On met 0 sur la diagonale car un noeud est à distance 0 de lui-même
            /// - On met l'infini ailleurs pour marquer l'absence de liaison initiale
            /// - On met -1 dans prochains pour signaler qu'aucun chemin n'est défini
            /// </summary>
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    distances[i, j] = (i == j) ? 0 : double.MaxValue;
                    prochains[i, j] = -1;
                    lignesUtilisees[i, j] = "";
                }
            }

            /// <summary>
            /// Remplissage de la matrice avec les données des liens existants
            /// </summary>
            foreach (var lien in Liens)
            {
                int depart = Noeuds.IndexOf(lien.Depart);
                int arrivee = Noeuds.IndexOf(lien.Arrivee);

                distances[depart, arrivee] = lien.Temps;
                prochains[depart, arrivee] = arrivee;
                lignesUtilisees[depart, arrivee] = lien.Ligne;

                distances[arrivee, depart] = lien.Temps;
                prochains[arrivee, depart] = depart;
                lignesUtilisees[arrivee, depart] = lien.Ligne;
            }

            /// <summary>
            /// Application de l'algorithme de Floyd-Warshall :
            /// On vérifie si en passant par un noeud intermédiaire k, on peut réduire la distance entre i et j.
            /// Si c'est le cas, on met à jour la distance et le chemin.
            /// </summary>
            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (distances[i, k] != double.MaxValue && distances[k, j] != double.MaxValue)
                        {
                            double nouvelleDistance = distances[i, k] + distances[k, j];
                            if (nouvelleDistance < distances[i, j])
                            {
                                distances[i, j] = nouvelleDistance;
                                prochains[i, j] = prochains[i, k];
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Reconstruction du chemin optimal à partir de la matrice prochains
            /// </summary>
            int idxStationDepart = Noeuds.IndexOf(stationDepart);
            int idxStationArrivee = Noeuds.IndexOf(stationArrivee);

            if (distances[idxStationDepart, idxStationArrivee] == double.MaxValue)
            {
                throw new Exception("Aucun chemin n'existe entre les stations de départ et d'arrivée.");
            }

            var cheminStations = new List<Noeud<int>>();
            var lignesUtiliseesFinal = new List<string>();
            cheminStations.Add(stationDepart);

            int u = idxStationDepart;
            while (u != idxStationArrivee)
            {
                if (prochains[u, idxStationArrivee] == -1)
                {
                    throw new Exception("Erreur lors de la reconstruction du chemin.");
                }

                int v = prochains[u, idxStationArrivee];
                lignesUtiliseesFinal.Add(lignesUtilisees[u, v]);
                cheminStations.Add(Noeuds[v]);
                u = v;
            }

            /// <summary>
            /// Calcul du temps total en ajoutant le temps de marche jusqu'aux stations
            /// </summary>
            double tempsTotal = distances[idxStationDepart, idxStationArrivee] +
                                CalculerTempsGps(latDepart, longDepart, stationDepart) +
                                CalculerTempsGps(latArrivee, longArrivee, stationArrivee);

            return (tempsTotal, cheminStations.Select(s => s.Nom).ToList(), lignesUtiliseesFinal);
        }



        /// <summary>
        /// Affiche tous les noeuds du graphe avec leur index global et leur nom, triés par ligne,
        /// puis demande à l'utilisateur de choisir un noeud par son index global
        /// </summary>
        /// <returns>L'index global du noeud choisi</returns>
        public int AfficherEtChoisirNoeud()
        {
            Dictionary<string, List<Noeud<int>>> noeudsByLigne = new Dictionary<string, List<Noeud<int>>>();

            foreach (var noeud in Noeuds)
            {
                foreach (var ligne in noeud.ListeLignes)
                {
                    if (!noeudsByLigne.ContainsKey(ligne))
                    {
                        noeudsByLigne[ligne] = new List<Noeud<int>>();
                    }
                    noeudsByLigne[ligne].Add(noeud);
                }
            }

            Console.WriteLine("Liste des noeuds par ligne :");
            Console.WriteLine("===========================");

            Dictionary<Noeud<int>, int> indexGlobalByNoeud = new Dictionary<Noeud<int>, int>();
            int indexGlobal = 0;

            foreach (var ligne in noeudsByLigne.Keys.OrderBy(l => l))
            {
                Console.WriteLine($"\nLigne {ligne} :");
                Console.WriteLine("------------------");

                var noeudsTriés = noeudsByLigne[ligne].OrderBy(n => n.Nom).ToList();

                foreach (var noeud in noeudsTriés)
                {
                    if (!indexGlobalByNoeud.ContainsKey(noeud))
                    {
                        indexGlobalByNoeud[noeud] = indexGlobal++;
                    }

                    Console.WriteLine($"[{indexGlobalByNoeud[noeud]}] {noeud.Nom}");
                }
            }

            Console.WriteLine("\nEntrez l'index global du noeud choisi :");
            int choix;

            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out choix) && choix >= 0 && choix < indexGlobal)
                {
                    break;
                }
                Console.WriteLine("Veuillez entrer un nombre entre 0 et " + (indexGlobal - 1));
            }

            return choix;
        }
    }

}