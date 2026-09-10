/*using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LivInParis.Models;
using LivInParis.Services;

namespace LivInParis.Visualization
{
    /// <summary>
    /// Classe qui analyse les relations entre clients et cuisiniers
    /// </summary>
    public class GraphAnalyzer
    {
        private Dictionary<int, List<int>> _adjacencyList; /// <summary>Représentation du graphe (client -> liste de cuisiniers)</summary>
        private Dictionary<int, string> _clientNames; /// <summary>ID client -> nom</summary>
        private Dictionary<int, string> _chefNames; /// <summary>ID cuisinier -> nom</summary>
        private Dictionary<int, int> _nodeColors; /// <summary>ID nœud -> couleur</summary>
        private int _numColors; /// <summary>Nombre de couleurs utilisées</summary>
        private string _outputPath; /// <summary>Chemin de l'image générée</summary>

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="outputPath">Le chemin ou sauvegarder l'image</param>
        public GraphAnalyzer(string outputPath = null)
        {
            _adjacencyList = new Dictionary<int, List<int>>();
            _clientNames = new Dictionary<int, string>();
            _chefNames = new Dictionary<int, string>();
            _nodeColors = new Dictionary<int, int>();
            _numColors = 0;
            _outputPath = outputPath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "graph_analysis.png");
        }

        /// <summary>
        /// Methode principale qui execute toute l'analyse
        /// </summary>
        public void AnalyzeAndVisualize()
        {
            try
            {
                Console.WriteLine("=== ANALYSE DE GRAPHE DES RELATIONS CLIENTS-CUISINIERS ===\n");

                Console.WriteLine("1. Construction du graphe à partir des commandes...");
                BuildGraphFromOrders();
                Console.WriteLine("   Graphe construit avec " + _adjacencyList.Count + " clients et " + CountChefs() + " cuisiniers.\n");

                Console.WriteLine("2. Application de l'algorithme de coloration de Welsh-Powell...");
                ApplyWelshPowellColoring();
                Console.WriteLine("   Coloration terminée. Nombre de couleurs utilisées: " + _numColors + "\n");

                Console.WriteLine("3. Analyse des propriétés du graphe...");
                bool isBipartite = IsBipartite();
                bool isPlanar = IsPlanar();
                Console.WriteLine("   Le graphe est-il biparti? " + (isBipartite ? "Oui" : "Non"));
                Console.WriteLine("   Le graphe est-il planaire? " + (isPlanar ? "Oui" : "Non") + "\n");

                Console.WriteLine("4. Identification des groupes indépendants...");
                Dictionary<int, List<int>> groups = GetIndependentGroups();
                foreach (var pair in groups)
                {
                    int color = pair.Key;
                    List<int> nodes = pair.Value;
                    Console.WriteLine("   Groupe " + (color + 1) + ": " + nodes.Count + " nœuds");
                }
                Console.WriteLine();

                Console.WriteLine("5. Génération de l'image du graphe...");
                DrawGraph(_outputPath);
                Console.WriteLine("   Image générée et sauvegardée à: " + _outputPath + "\n");

                Console.WriteLine("6. Ouverture de l'image...");
                Process.Start(new ProcessStartInfo(_outputPath) { UseShellExecute = true });
                Console.WriteLine("   Image ouverte.\n");

                Console.WriteLine("=== RAPPORT DÉTAILLÉ ===\n");
                Console.WriteLine(GenerateReport());

                Console.WriteLine("\nAnalyse terminée avec succès!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Construit le graphe a partir des données de commandes
        /// </summary>
        private void BuildGraphFromOrders()
        {
            // Récupère toutes les commandes
            List<Commande> commandes = CommandeService.ObtenirToutesLesCommandes();

            // Récupère tous les clients et cuisiniers pour les noms
            List<Client> clients = ClientService.ObtenirTousLesClients();
            List<Cuisinier> cuisiniers = CuisinierService.ObtenirTousLesCuisiniers();

            // Remplit les dictionnaires de noms
            foreach (var client in clients)
            {
                _clientNames[client.IdClient] = client.PrenomClient + " " + client.NomClient;
            }

            foreach (var chef in cuisiniers)
            {
                _chefNames[chef.IdCuisinier] = chef.PrenomCuisinier + " " + chef.NomCuisinier;
            }

            // Construit la liste d'adjacence
            foreach (var commande in commandes)
            {
                // Ignore les commandes sans cuisinier assigné
                if (commande.IdCuisinier <= 0)
                    continue;

                int clientId = commande.IdClient;
                int chefId = commande.IdCuisinier;

                // Ajoute le client s'il n'existe pas encore dans le graphe
                if (!_adjacencyList.ContainsKey(clientId))
                {
                    _adjacencyList[clientId] = new List<int>();
                }

                // Ajoute le cuisinier à la liste des cuisiniers du client
                if (!_adjacencyList[clientId].Contains(chefId))
                {
                    _adjacencyList[clientId].Add(chefId);
                }
            }
        }

        /// <summary>
        /// Compte le nombre de cuisiniers uniques
        /// </summary>
        /// <returns>Le nombre de cuisiniers</returns>
        private int CountChefs()
        {
            HashSet<int> chefs = new HashSet<int>();
            foreach (var clientChefs in _adjacencyList.Values)
            {
                foreach (var chef in clientChefs)
                {
                    chefs.Add(chef);
                }
            }
            return chefs.Count;
        }

        /// <summary>
        /// Algorithme de coloration de Welsh-Powell
        /// </summary>
        private void ApplyWelshPowellColoring()
        {
            // Crée une liste de tous les nœuds (clients et cuisiniers)
            List<int> allNodes = new List<int>();

            // Ajoute tous les clients
            allNodes.AddRange(_adjacencyList.Keys);

            // Ajoute tous les cuisiniers (sans doublons)
            HashSet<int> chefs = new HashSet<int>();
            foreach (var clientChefs in _adjacencyList.Values)
            {
                foreach (var chef in clientChefs)
                {
                    chefs.Add(chef);
                }
            }
            allNodes.AddRange(chefs);

            // Trie les nœuds par degré décroissant (nombre de connexions)
            Dictionary<int, int> nodeDegrees = new Dictionary<int, int>();
            foreach (var node in allNodes)
            {
                int degree = 0;

                // Si c'est un client, son degré est le nombre de cuisiniers
                if (_adjacencyList.ContainsKey(node))
                {
                    degree = _adjacencyList[node].Count;
                }
                // Si c'est un cuisinier, son degré est le nombre de clients qui l'ont
                else
                {
                    degree = _adjacencyList.Count(pair => pair.Value.Contains(node));
                }

                nodeDegrees[node] = degree;
            }

            // Trie les nœuds par degré décroissant
            List<int> sortedNodes = allNodes.OrderByDescending(node => nodeDegrees[node]).ToList();

            // Initialise les couleurs
            _nodeColors.Clear();
            _numColors = 0;

            // Colore les nœuds
            foreach (var node in sortedNodes)
            {
                // Trouve les voisins du nœud
                List<int> neighbors = new List<int>();

                // Si c'est un client, ses voisins sont les cuisiniers
                if (_adjacencyList.ContainsKey(node))
                {
                    neighbors.AddRange(_adjacencyList[node]);
                }
                // Si c'est un cuisinier, ses voisins sont les clients qui l'ont
                else
                {
                    foreach (var pair in _adjacencyList)
                    {
                        if (pair.Value.Contains(node))
                        {
                            neighbors.Add(pair.Key);
                        }
                    }
                }

                // Trouve la première couleur disponible
                HashSet<int> neighborColors = new HashSet<int>();
                foreach (var neighbor in neighbors)
                {
                    if (_nodeColors.ContainsKey(neighbor))
                    {
                        neighborColors.Add(_nodeColors[neighbor]);
                    }
                }

                int color = 0;
                while (neighborColors.Contains(color))
                {
                    color++;
                }

                // Assigne la couleur au nœud
                _nodeColors[node] = color;

                // Met à jour le nombre de couleurs
                _numColors = Math.Max(_numColors, color + 1);
            }
        }

        /// <summary>
        /// Verifie si le graphe est biparti
        /// </summary>
        /// <returns>True si le graphe est biparti</returns>
        private bool IsBipartite()
        {
            // Un graphe est biparti si on peut le colorer avec 2 couleurs
            return _numColors <= 2;
        }

        /// <summary>
        /// Verifie si le graphe est planaire
        /// </summary>
        /// <returns>True si le graphe est planaire</returns>
        private bool IsPlanar()
        {
            // Selon le théorème de Kuratowski, un graphe est planaire si et seulement s'il ne contient pas
            // de sous-graphe qui est une subdivision de K5 ou K3,3
            // Cette vérification est complexe, mais on peut utiliser une approximation basée sur le nombre d'arêtes

            int numNodes = _nodeColors.Count;
            int numEdges = 0;

            // Compte le nombre d'arêtes
            foreach (var pair in _adjacencyList)
            {
                numEdges += pair.Value.Count;
            }

            // Pour un graphe planaire, le nombre d'arêtes est au plus 3n-6 pour n>=3
            return numEdges <= 3 * numNodes - 6;
        }

        /// <summary>
        /// Identifie les groupes indépendants
        /// </summary>
        /// <returns>Dictionnaire des groupes par couleur</returns>
        private Dictionary<int, List<int>> GetIndependentGroups()
        {
            Dictionary<int, List<int>> groups = new Dictionary<int, List<int>>();

            // Regroupe les nœuds par couleur
            foreach (var pair in _nodeColors)
            {
                int node = pair.Key;
                int color = pair.Value;

                if (!groups.ContainsKey(color))
                {
                    groups[color] = new List<int>();
                }

                groups[color].Add(node);
            }

            return groups;
        }

        /// <summary>
        /// Dessine le graphe
        /// </summary>
        /// <param name="outputPath">Le chemin ou sauvegarder l'image</param>
        private void DrawGraph(string outputPath)
        {
            int width = 1200;
            int height = 800;

            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.White);

                // Définit les couleurs
                Color[] colors = new Color[]
                {
                    Color.Red, Color.Blue, Color.Green, Color.Orange,
                    Color.Purple, Color.Cyan, Color.Magenta, Color.Yellow
                };

                // Calcule les positions des nœuds (disposition circulaire)
                Dictionary<int, PointF> nodePositions = new Dictionary<int, PointF>();

                // Sépare les clients et les cuisiniers
                List<int> clientNodes = _adjacencyList.Keys.ToList();
                HashSet<int> chefNodes = new HashSet<int>();
                foreach (var clientChefs in _adjacencyList.Values)
                {
                    foreach (var chef in clientChefs)
                    {
                        chefNodes.Add(chef);
                    }
                }

                // Positionne les clients sur le cercle gauche
                int numClients = clientNodes.Count;
                float clientRadius = height / 3;
                PointF clientCenter = new PointF(width / 3, height / 2);

                for (int i = 0; i < numClients; i++)
                {
                    float angle = (float)(2 * Math.PI * i / numClients);
                    float x = clientCenter.X + clientRadius * (float)Math.Cos(angle);
                    float y = clientCenter.Y + clientRadius * (float)Math.Sin(angle);

                    nodePositions[clientNodes[i]] = new PointF(x, y);
                }

                // Positionne les cuisiniers sur le cercle droit
                int numChefs = chefNodes.Count;
                float chefRadius = height / 3;
                PointF chefCenter = new PointF(2 * width / 3, height / 2);

                int j = 0;
                foreach (var chef in chefNodes)
                {
                    float angle = (float)(2 * Math.PI * j / numChefs);
                    float x = chefCenter.X + chefRadius * (float)Math.Cos(angle);
                    float y = chefCenter.Y + chefRadius * (float)Math.Sin(angle);

                    nodePositions[chef] = new PointF(x, y);
                    j++;
                }

                // Dessine les arêtes
                using (Pen pen = new Pen(Color.Gray, 1))
                {
                    foreach (var pair in _adjacencyList)
                    {
                        int client = pair.Key;
                        foreach (var chef in pair.Value)
                        {
                            g.DrawLine(pen, nodePositions[client], nodePositions[chef]);
                        }
                    }
                }

                // Dessine les nœuds
                int nodeSize = 30;

                // Dessine les clients
                foreach (var client in clientNodes)
                {
                    int colorIndex = _nodeColors.ContainsKey(client) ? _nodeColors[client] % colors.Length : 0;
                    Color nodeColor = colors[colorIndex];

                    using (SolidBrush brush = new SolidBrush(nodeColor))
                    {
                        PointF pos = nodePositions[client];
                        g.FillEllipse(brush, pos.X - nodeSize / 2, pos.Y - nodeSize / 2, nodeSize, nodeSize);
                    }

                    using (Pen pen = new Pen(Color.Black, 2))
                    {
                        PointF pos = nodePositions[client];
                        g.DrawEllipse(pen, pos.X - nodeSize / 2, pos.Y - nodeSize / 2, nodeSize, nodeSize);
                    }

                    // Dessine le nom du client
                    string name = _clientNames.ContainsKey(client) ? _clientNames[client] : "Client " + client;
                    using (Font font = new Font("Arial", 8))
                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        PointF pos = nodePositions[client];
                        SizeF textSize = g.MeasureString(name, font);
                        g.DrawString(name, font, brush, pos.X - textSize.Width / 2, pos.Y + nodeSize / 2 + 5);
                    }
                }

                // Dessine les cuisiniers
                foreach (var chef in chefNodes)
                {
                    int colorIndex = _nodeColors.ContainsKey(chef) ? _nodeColors[chef] % colors.Length : 0;
                    Color nodeColor = colors[colorIndex];

                    using (SolidBrush brush = new SolidBrush(nodeColor))
                    {
                        PointF pos = nodePositions[chef];
                        g.FillRectangle(brush, pos.X - nodeSize / 2, pos.Y - nodeSize / 2, nodeSize, nodeSize);
                    }

                    using (Pen pen = new Pen(Color.Black, 2))
                    {
                        PointF pos = nodePositions[chef];
                        g.DrawRectangle(pen, pos.X - nodeSize / 2, pos.Y - nodeSize / 2, nodeSize, nodeSize);
                    }

                    // Dessine le nom du cuisinier
                    string name = _chefNames.ContainsKey(chef) ? _chefNames[chef] : "Chef " + chef;
                    using (Font font = new Font("Arial", 8))
                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        PointF pos = nodePositions[chef];
                        SizeF textSize = g.MeasureString(name, font);
                        g.DrawString(name, font, brush, pos.X - textSize.Width / 2, pos.Y + nodeSize / 2 + 5);
                    }
                }

                // Dessine la légende
                int legendX = 50;
                int legendY = 50;

                using (Font font = new Font("Arial", 12, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString("Légende:", font, brush, legendX, legendY);
                }

                legendY += 30;

                for (int i = 0; i < _numColors; i++)
                {
                    Color color = colors[i % colors.Length];

                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        g.FillRectangle(brush, legendX, legendY, 20, 20);
                    }

                    using (Pen pen = new Pen(Color.Black, 1))
                    {
                        g.DrawRectangle(pen, legendX, legendY, 20, 20);
                    }

                    using (Font font = new Font("Arial", 10))
                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        g.DrawString("Groupe " + (i + 1), font, brush, legendX + 30, legendY + 5);
                    }

                    legendY += 30;
                }

                // Dessine les informations sur le graphe
                legendY += 20;

                using (Font font = new Font("Arial", 12, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString("Informations:", font, brush, legendX, legendY);
                }

                legendY += 30;

                string[] info = new string[]
                {
                    "Nombre de couleurs: " + _numColors,
                    "Graphe biparti: " + (IsBipartite() ? "Oui" : "Non"),
                    "Graphe planaire: " + (IsPlanar() ? "Oui" : "Non")
                };

                using (Font font = new Font("Arial", 10))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    foreach (var line in info)
                    {
                        g.DrawString(line, font, brush, legendX, legendY);
                        legendY += 20;
                    }
                }

                // Dessine les formes pour la légende des types de nœuds
                legendY += 20;

                using (Font font = new Font("Arial", 12, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString("Types de nœuds:", font, brush, legendX, legendY);
                }

                legendY += 30;

                // Client (cercle)
                using (SolidBrush brush = new SolidBrush(Color.Gray))
                {
                    g.FillEllipse(brush, legendX, legendY, 20, 20);
                }

                using (Pen pen = new Pen(Color.Black, 1))
                {
                    g.DrawEllipse(pen, legendX, legendY, 20, 20);
                }

                using (Font font = new Font("Arial", 10))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString("Client", font, brush, legendX + 30, legendY + 5);
                }

                legendY += 30;

                // Cuisinier (carré)
                using (SolidBrush brush = new SolidBrush(Color.Gray))
                {
                    g.FillRectangle(brush, legendX, legendY, 20, 20);
                }

                using (Pen pen = new Pen(Color.Black, 1))
                {
                    g.DrawRectangle(pen, legendX, legendY, 20, 20);
                }

                using (Font font = new Font("Arial", 10))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString("Cuisinier", font, brush, legendX + 30, legendY + 5);
                }

                // Enregistre l'image
                bitmap.Save(outputPath);
            }
        }

        /// <summary>
        /// Genere un rapport textuel
        /// </summary>
        /// <returns>Le rapport d'analyse</returns>
        private string GenerateReport()
        {
            string report = "=== RAPPORT D'ANALYSE DE GRAPHE ===\n\n";

            // Nombre de couleurs
            report += "Nombre minimal de couleurs nécessaires: " + _numColors + "\n\n";

            // Propriétés du graphe
            report += "Propriétés du graphe:\n";
            report += "- Le graphe est-il biparti? " + (IsBipartite() ? "Oui" : "Non") + "\n";
            report += "  Justification: " + (IsBipartite() ? "Le graphe peut être colorié avec 2 couleurs seulement." : "Le graphe nécessite plus de 2 couleurs pour être colorié.") + "\n\n";

            report += "- Le graphe est-il planaire? " + (IsPlanar() ? "Oui" : "Non") + "\n";
            report += "  Justification: " + (IsPlanar() ? "Le nombre d'arêtes est inférieur ou égal à 3n-6 (où n est le nombre de sommets)." : "Le nombre d'arêtes est supérieur à 3n-6 (où n est le nombre de sommets).") + "\n\n";

            // Groupes indépendants
            report += "Groupes indépendants:\n";

            Dictionary<int, List<int>> groups = GetIndependentGroups();
            foreach (var pair in groups)
            {
                int color = pair.Key;
                List<int> nodes = pair.Value;

                report += "Groupe " + (color + 1) + ":\n";

                // Sépare les clients et les cuisiniers dans ce groupe
                List<int> clients = nodes.Where(n => _adjacencyList.ContainsKey(n)).ToList();
                List<int> chefs = nodes.Where(n => !_adjacencyList.ContainsKey(n)).ToList();

                if (clients.Count > 0)
                {
                    report += "  Clients:\n";
                    foreach (var client in clients)
                    {
                        string name = _clientNames.ContainsKey(client) ? _clientNames[client] : "Client " + client;
                        report += "    - " + name + " (ID: " + client + ")\n";
                    }
                }

                if (chefs.Count > 0)
                {
                    report += "  Cuisiniers:\n";
                    foreach (var chef in chefs)
                    {
                        string name = _chefNames.ContainsKey(chef) ? _chefNames[chef] : "Chef " + chef;
                        report += "    - " + name + " (ID: " + chef + ")\n";
                    }
                }

                report += "\n";
            }

            return report;
        }
    }
}*/




using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LivInParis.Models;
using LivInParis.Services;

namespace LivInParis.Visualization
{
    /// <summary>
    /// Classe qui analyse les relations entre clients et cuisiniers
    /// </summary>
    public class GraphAnalyzer
    {
        /// <summary>Représentation du graphe (client -> liste de cuisiniers)</summary>
        private Dictionary<int, List<int>> _adjacencyList;
        /// <summary>ID client -> nom</summary>
        private Dictionary<int, string> _clientNames;
        /// <summary>ID cuisinier -> nom</summary>
        private Dictionary<int, string> _chefNames;
        /// <summary>ID nœud -> couleur</summary>
        private Dictionary<int, int> _nodeColors;
        /// <summary>Nombre de couleurs utilisées</summary>
        private int _numColors;
        /// <summary>Chemin de l'image générée</summary>
        private string _outputPath;

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="outputPath">Le chemin ou sauvegarder l'image</param>
        public GraphAnalyzer(string outputPath = null)
        {
            _adjacencyList = new Dictionary<int, List<int>>();
            _clientNames = new Dictionary<int, string>();
            _chefNames = new Dictionary<int, string>();
            _nodeColors = new Dictionary<int, int>();
            _numColors = 0;
            _outputPath = outputPath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "graph_analysis.png");
        }

        /// <summary>
        /// Methode principale qui execute toute l'analyse
        /// </summary>
        public void AnalyzeAndVisualize()
        {
            try
            {
                Console.WriteLine("=== ANALYSE DE GRAPHE DES RELATIONS CLIENTS-CUISINIERS ===\n");

                Console.WriteLine("1. Construction du graphe à partir des commandes...");
                BuildGraphFromOrders();
                Console.WriteLine("   Graphe construit avec " + _adjacencyList.Count + " clients et " + CountChefs() + " cuisiniers.\n");

                Console.WriteLine("2. Application de l'algorithme de coloration de Welsh-Powell...");
                ApplyWelshPowellColoring();
                Console.WriteLine("   Coloration terminée. Nombre de couleurs utilisées: " + _numColors + "\n");

                Console.WriteLine("3. Analyse des propriétés du graphe...");
                bool isBipartite = IsBipartite();
                bool isPlanar = IsPlanar();
                Console.WriteLine("   Le graphe est-il biparti? " + (isBipartite ? "Oui" : "Non"));
                Console.WriteLine("   Le graphe est-il planaire? " + (isPlanar ? "Oui" : "Non") + "\n");

                Console.WriteLine("4. Identification des groupes indépendants...");
                Dictionary<int, List<int>> groups = GetIndependentGroups();
                foreach (var pair in groups)
                {
                    int color = pair.Key;
                    List<int> nodes = pair.Value;
                    Console.WriteLine("   Groupe " + (color + 1) + ": " + nodes.Count + " nœuds");
                }
                Console.WriteLine();

                Console.WriteLine("5. Génération de l'image du graphe...");
                DrawGraph(_outputPath);
                Console.WriteLine("   Image générée et sauvegardée à: " + _outputPath + "\n");

                Console.WriteLine("6. Ouverture de l'image...");
                Process.Start(new ProcessStartInfo(_outputPath) { UseShellExecute = true });
                Console.WriteLine("   Image ouverte.\n");

                Console.WriteLine("=== RAPPORT DÉTAILLÉ ===\n");
                Console.WriteLine(GenerateReport());

                Console.WriteLine("\nAnalyse terminée avec succès!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERREUR: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Construit le graphe a partir des données de commandes
        /// </summary>
        private void BuildGraphFromOrders()
        {
            List<Commande> commandes = CommandeService.ObtenirToutesLesCommandes();

            List<Client> clients = ClientService.ObtenirTousLesClients();
            List<Cuisinier> cuisiniers = CuisinierService.ObtenirTousLesCuisiniers();

            foreach (var client in clients)
            {
                _clientNames[client.IdClient] = client.PrenomClient + " " + client.NomClient;
            }

            foreach (var chef in cuisiniers)
            {
                _chefNames[chef.IdCuisinier] = chef.PrenomCuisinier + " " + chef.NomCuisinier;
            }

            foreach (var commande in commandes)
            {
                if (commande.IdCuisinier <= 0)
                    continue;

                int clientId = commande.IdClient;
                int chefId = commande.IdCuisinier;

                if (!_adjacencyList.ContainsKey(clientId))
                {
                    _adjacencyList[clientId] = new List<int>();
                }

                if (!_adjacencyList[clientId].Contains(chefId))
                {
                    _adjacencyList[clientId].Add(chefId);
                }
            }
        }

        /// <summary>
        /// Compte le nombre de cuisiniers uniques
        /// </summary>
        /// <returns>Le nombre de cuisiniers</returns>
        private int CountChefs()
        {
            HashSet<int> chefs = new HashSet<int>();
            foreach (var clientChefs in _adjacencyList.Values)
            {
                foreach (var chef in clientChefs)
                {
                    chefs.Add(chef);
                }
            }
            return chefs.Count;
        }

        /// <summary>
        /// Algorithme de coloration de Welsh-Powell
        /// </summary>
        private void ApplyWelshPowellColoring()
        {
            List<int> allNodes = new List<int>();

            allNodes.AddRange(_adjacencyList.Keys);

            HashSet<int> chefs = new HashSet<int>();
            foreach (var clientChefs in _adjacencyList.Values)
            {
                foreach (var chef in clientChefs)
                {
                    chefs.Add(chef);
                }
            }
            allNodes.AddRange(chefs);

            Dictionary<int, int> nodeDegrees = new Dictionary<int, int>();
            foreach (var node in allNodes)
            {
                int degree = 0;

                if (_adjacencyList.ContainsKey(node))
                {
                    degree = _adjacencyList[node].Count;
                }
                else
                {
                    degree = _adjacencyList.Count(pair => pair.Value.Contains(node));
                }

                nodeDegrees[node] = degree;
            }

            List<int> sortedNodes = allNodes.OrderByDescending(node => nodeDegrees[node]).ToList();

            _nodeColors.Clear();
            _numColors = 0;


            foreach (var node in sortedNodes)
            {
                List<int> neighbors = new List<int>();

                if (_adjacencyList.ContainsKey(node))
                {
                    neighbors.AddRange(_adjacencyList[node]);
                }
                else
                {
                    foreach (var pair in _adjacencyList)
                    {
                        if (pair.Value.Contains(node))
                        {
                            neighbors.Add(pair.Key);
                        }
                    }
                }

                HashSet<int> neighborColors = new HashSet<int>();
                foreach (var neighbor in neighbors)
                {
                    if (_nodeColors.ContainsKey(neighbor))
                    {
                        neighborColors.Add(_nodeColors[neighbor]);
                    }
                }

                int color = 0;
                while (neighborColors.Contains(color))
                {
                    color++;
                }

                _nodeColors[node] = color;

                _numColors = Math.Max(_numColors, color + 1);
            }
        }

        /// <summary>
        /// Verifie si le graphe est biparti
        /// </summary>
        /// <returns>True si le graphe est biparti</returns>
        private bool IsBipartite()
        {
            return _numColors <= 2;
        }

        /// <summary>
        /// Verifie si le graphe est planaire
        /// </summary>
        /// <returns>True si le graphe est planaire</returns>
        private bool IsPlanar()
        {


            int numNodes = _nodeColors.Count;
            int numEdges = 0;

            foreach (var pair in _adjacencyList)
            {
                numEdges += pair.Value.Count;
            }

            return numEdges <= 3 * numNodes - 6;
        }

        /// <summary>
        /// Identifie les groupes indépendants
        /// </summary>
        /// <returns>Dictionnaire des groupes par couleur</returns>
        private Dictionary<int, List<int>> GetIndependentGroups()
        {
            Dictionary<int, List<int>> groups = new Dictionary<int, List<int>>();

            // Regroupe les nœuds par couleur
            foreach (var pair in _nodeColors)
            {
                int node = pair.Key;
                int color = pair.Value;

                if (!groups.ContainsKey(color))
                {
                    groups[color] = new List<int>();
                }

                groups[color].Add(node);
            }

            return groups;
        }

        /// <summary>
        /// Dessine le graphe
        /// </summary>
        /// <param name="outputPath">Le chemin ou sauvegarder l'image</param>
        private void DrawGraph(string outputPath)
        {
            int width = 1200;
            int height = 800;

            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.White);

                Color[] colors = new Color[]
                {
                    Color.Red, Color.Blue, Color.Green, Color.Orange,
                    Color.Purple, Color.Cyan, Color.Magenta, Color.Yellow
                };

                Dictionary<int, PointF> nodePositions = new Dictionary<int, PointF>();

                List<int> clientNodes = _adjacencyList.Keys.ToList();
                HashSet<int> chefNodes = new HashSet<int>();
                foreach (var clientChefs in _adjacencyList.Values)
                {
                    foreach (var chef in clientChefs)
                    {
                        chefNodes.Add(chef);
                    }
                }

                int numClients = clientNodes.Count;
                float clientRadius = height / 3;
                PointF clientCenter = new PointF(width / 3, height / 2);

                for (int i = 0; i < numClients; i++)
                {
                    float angle = (float)(2 * Math.PI * i / numClients);
                    float x = clientCenter.X + clientRadius * (float)Math.Cos(angle);
                    float y = clientCenter.Y + clientRadius * (float)Math.Sin(angle);

                    nodePositions[clientNodes[i]] = new PointF(x, y);
                }

                int numChefs = chefNodes.Count;
                float chefRadius = height / 3;
                PointF chefCenter = new PointF(2 * width / 3, height / 2);

                int j = 0;
                foreach (var chef in chefNodes)
                {
                    float angle = (float)(2 * Math.PI * j / numChefs);
                    float x = chefCenter.X + chefRadius * (float)Math.Cos(angle);
                    float y = chefCenter.Y + chefRadius * (float)Math.Sin(angle);

                    nodePositions[chef] = new PointF(x, y);
                    j++;
                }

                using (Pen pen = new Pen(Color.Gray, 1))
                {
                    foreach (var pair in _adjacencyList)
                    {
                        int client = pair.Key;
                        foreach (var chef in pair.Value)
                        {
                            g.DrawLine(pen, nodePositions[client], nodePositions[chef]);
                        }
                    }
                }

                int nodeSize = 30;

                foreach (var client in clientNodes)
                {
                    int colorIndex = _nodeColors.ContainsKey(client) ? _nodeColors[client] % colors.Length : 0;
                    Color nodeColor = colors[colorIndex];

                    using (SolidBrush brush = new SolidBrush(nodeColor))
                    {
                        PointF pos = nodePositions[client];
                        g.FillEllipse(brush, pos.X - nodeSize / 2, pos.Y - nodeSize / 2, nodeSize, nodeSize);
                    }

                    using (Pen pen = new Pen(Color.Black, 2))
                    {
                        PointF pos = nodePositions[client];
                        g.DrawEllipse(pen, pos.X - nodeSize / 2, pos.Y - nodeSize / 2, nodeSize, nodeSize);
                    }

                    string name = _clientNames.ContainsKey(client) ? _clientNames[client] : "Client " + client;
                    using (Font font = new Font("Arial", 8))
                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        PointF pos = nodePositions[client];
                        SizeF textSize = g.MeasureString(name, font);
                        g.DrawString(name, font, brush, pos.X - textSize.Width / 2, pos.Y + nodeSize / 2 + 5);
                    }
                }

                foreach (var chef in chefNodes)
                {
                    int colorIndex = _nodeColors.ContainsKey(chef) ? _nodeColors[chef] % colors.Length : 0;
                    Color nodeColor = colors[colorIndex];

                    using (SolidBrush brush = new SolidBrush(nodeColor))
                    {
                        PointF pos = nodePositions[chef];
                        g.FillRectangle(brush, pos.X - nodeSize / 2, pos.Y - nodeSize / 2, nodeSize, nodeSize);
                    }

                    using (Pen pen = new Pen(Color.Black, 2))
                    {
                        PointF pos = nodePositions[chef];
                        g.DrawRectangle(pen, pos.X - nodeSize / 2, pos.Y - nodeSize / 2, nodeSize, nodeSize);
                    }

                    string name = _chefNames.ContainsKey(chef) ? _chefNames[chef] : "Chef " + chef;
                    using (Font font = new Font("Arial", 8))
                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        PointF pos = nodePositions[chef];
                        SizeF textSize = g.MeasureString(name, font);
                        g.DrawString(name, font, brush, pos.X - textSize.Width / 2, pos.Y + nodeSize / 2 + 5);
                    }
                }

                int legendX = 50;
                int legendY = 50;

                using (Font font = new Font("Arial", 12, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString("Légende:", font, brush, legendX, legendY);
                }

                legendY += 30;

                for (int i = 0; i < _numColors; i++)
                {
                    Color color = colors[i % colors.Length];

                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        g.FillRectangle(brush, legendX, legendY, 20, 20);
                    }

                    using (Pen pen = new Pen(Color.Black, 1))
                    {
                        g.DrawRectangle(pen, legendX, legendY, 20, 20);
                    }

                    using (Font font = new Font("Arial", 10))
                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        g.DrawString("Groupe " + (i + 1), font, brush, legendX + 30, legendY + 5);
                    }

                    legendY += 30;
                }

                legendY += 20;

                using (Font font = new Font("Arial", 12, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString("Informations:", font, brush, legendX, legendY);
                }

                legendY += 30;

                string[] info = new string[]
                {
                    "Nombre de couleurs: " + _numColors,
                    "Graphe biparti: " + (IsBipartite() ? "Oui" : "Non"),
                    "Graphe planaire: " + (IsPlanar() ? "Oui" : "Non")
                };

                using (Font font = new Font("Arial", 10))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    foreach (var line in info)
                    {
                        g.DrawString(line, font, brush, legendX, legendY);
                        legendY += 20;
                    }
                }

                legendY += 20;

                using (Font font = new Font("Arial", 12, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString("Types de nœuds:", font, brush, legendX, legendY);
                }

                legendY += 30;

                using (SolidBrush brush = new SolidBrush(Color.Gray))
                {
                    g.FillEllipse(brush, legendX, legendY, 20, 20);
                }

                using (Pen pen = new Pen(Color.Black, 1))
                {
                    g.DrawEllipse(pen, legendX, legendY, 20, 20);
                }

                using (Font font = new Font("Arial", 10))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString("Client", font, brush, legendX + 30, legendY + 5);
                }

                legendY += 30;

                using (SolidBrush brush = new SolidBrush(Color.Gray))
                {
                    g.FillRectangle(brush, legendX, legendY, 20, 20);
                }

                using (Pen pen = new Pen(Color.Black, 1))
                {
                    g.DrawRectangle(pen, legendX, legendY, 20, 20);
                }

                using (Font font = new Font("Arial", 10))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.DrawString("Cuisinier", font, brush, legendX + 30, legendY + 5);
                }

                bitmap.Save(outputPath);
            }
        }

        /// <summary>
        /// Genere un rapport textuel
        /// </summary>
        /// <returns>Le rapport d'analyse</returns>
        private string GenerateReport()
        {
            string report = "=== RAPPORT D'ANALYSE DE GRAPHE ===\n\n";

            report += "Nombre minimal de couleurs nécessaires: " + _numColors + "\n\n";

            report += "Propriétés du graphe:\n";
            report += "- Le graphe est-il biparti? " + (IsBipartite() ? "Oui" : "Non") + "\n";
            report += "  Justification: " + (IsBipartite() ? "Le graphe peut être colorié avec 2 couleurs seulement." : "Le graphe nécessite plus de 2 couleurs pour être colorié.") + "\n\n";

            report += "- Le graphe est-il planaire? " + (IsPlanar() ? "Oui" : "Non") + "\n";
            report += "  Justification: " + (IsPlanar() ? "Le nombre d'arêtes est inférieur ou égal à 3n-6 (où n est le nombre de sommets)." : "Le nombre d'arêtes est supérieur à 3n-6 (où n est le nombre de sommets).") + "\n\n";

            report += "Groupes indépendants:\n";

            Dictionary<int, List<int>> groups = GetIndependentGroups();
            foreach (var pair in groups)
            {
                int color = pair.Key;
                List<int> nodes = pair.Value;

                report += "Groupe " + (color + 1) + ":\n";

                List<int> clients = nodes.Where(n => _adjacencyList.ContainsKey(n)).ToList();
                List<int> chefs = nodes.Where(n => !_adjacencyList.ContainsKey(n)).ToList();

                if (clients.Count > 0)
                {
                    report += "  Clients:\n";
                    foreach (var client in clients)
                    {
                        string name = _clientNames.ContainsKey(client) ? _clientNames[client] : "Client " + client;
                        report += "    - " + name + " (ID: " + client + ")\n";
                    }
                }

                if (chefs.Count > 0)
                {
                    report += "  Cuisiniers:\n";
                    foreach (var chef in chefs)
                    {
                        string name = _chefNames.ContainsKey(chef) ? _chefNames[chef] : "Chef " + chef;
                        report += "    - " + name + " (ID: " + chef + ")\n";
                    }
                }

                report += "\n";
            }

            return report;
        }
    }
}