using Microsoft.VisualStudio.TestTools.UnitTesting;
using LivInParis.Models;
using LivInParis.Services;
using LivInParis.Visualization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;

namespace LivInParis.Tests
{
    [TestClass]
    public class LivInParisTests
    {
        #region GraphAnalyzer Tests

        private GraphAnalyzer _analyzer;

        [TestInitialize]
        public void Initialize()
        {
            // Utiliser un chemin temporaire pour les tests
            string tempPath = Path.Combine(Path.GetTempPath(), "graph_test.png");
            _analyzer = new GraphAnalyzer(tempPath);
        }

        [TestMethod]
        public void GraphAnalyzer_AnalyzeAndVisualize_ShouldNotThrowException()
        {
            try
            {
                _analyzer.AnalyzeAndVisualize();
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"L'analyse du graphe a échoué avec l'exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void GraphAnalyzer_BuildGraphFromOrders_ShouldCreateAdjacencyList()
        {
            // Utiliser la réflexion pour accéder à la méthode privée
            MethodInfo buildGraphMethod = typeof(GraphAnalyzer).GetMethod("BuildGraphFromOrders",
                BindingFlags.NonPublic | BindingFlags.Instance);

            buildGraphMethod.Invoke(_analyzer, null);

            // Accéder au champ _adjacencyList via réflexion
            FieldInfo adjacencyListField = typeof(GraphAnalyzer).GetField("_adjacencyList",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var adjacencyList = adjacencyListField.GetValue(_analyzer) as Dictionary<int, List<int>>;

            Assert.IsNotNull(adjacencyList);
        }

        [TestMethod]
        public void GraphAnalyzer_ApplyWelshPowellColoring_ShouldAssignColors()
        {
            // Créer un graphe simulé via réflexion
            FieldInfo adjacencyListField = typeof(GraphAnalyzer).GetField("_adjacencyList",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var adjacencyList = new Dictionary<int, List<int>>();

            // Simuler un graphe simple
            adjacencyList.Add(1, new List<int> { 101, 102 });
            adjacencyList.Add(2, new List<int> { 101, 103 });
            adjacencyList.Add(3, new List<int> { 102, 103 });

            adjacencyListField.SetValue(_analyzer, adjacencyList);

            // Accéder à la méthode privée
            MethodInfo coloringMethod = typeof(GraphAnalyzer).GetMethod("ApplyWelshPowellColoring",
                BindingFlags.NonPublic | BindingFlags.Instance);

            coloringMethod.Invoke(_analyzer, null);

            // Vérifier que les couleurs ont été assignées
            FieldInfo nodeColorsField = typeof(GraphAnalyzer).GetField("_nodeColors",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var nodeColors = nodeColorsField.GetValue(_analyzer) as Dictionary<int, int>;

            Assert.IsNotNull(nodeColors);
            Assert.IsTrue(nodeColors.Count > 0, "Des couleurs devraient être assignées aux nœuds");

            // Vérifier que le nombre de couleurs est correct
            FieldInfo numColorsField = typeof(GraphAnalyzer).GetField("_numColors",
                BindingFlags.NonPublic | BindingFlags.Instance);
            int numColors = (int)numColorsField.GetValue(_analyzer);

            // Pour ce graphe biparti, il devrait y avoir 2 couleurs
            Assert.AreEqual(2, numColors, "Ce graphe devrait être coloré avec 2 couleurs");
        }

        [TestMethod]
        public void GraphAnalyzer_IsBipartite_ShouldReturnTrueForBipartiteGraph()
        {
            // Simuler la coloration d'un graphe biparti
            FieldInfo numColorsField = typeof(GraphAnalyzer).GetField("_numColors",
                BindingFlags.NonPublic | BindingFlags.Instance);
            numColorsField.SetValue(_analyzer, 2); // 2 couleurs = biparti

            // Accéder à la méthode privée
            MethodInfo isBipartiteMethod = typeof(GraphAnalyzer).GetMethod("IsBipartite",
                BindingFlags.NonPublic | BindingFlags.Instance);

            bool isBipartite = (bool)isBipartiteMethod.Invoke(_analyzer, null);

            Assert.IsTrue(isBipartite, "Le graphe devrait être biparti");
        }

        [TestMethod]
        public void GraphAnalyzer_IsBipartite_ShouldReturnFalseForNonBipartiteGraph()
        {
            // Simuler la coloration d'un graphe non biparti
            FieldInfo numColorsField = typeof(GraphAnalyzer).GetField("_numColors",
                BindingFlags.NonPublic | BindingFlags.Instance);
            numColorsField.SetValue(_analyzer, 3); // Plus de 2 couleurs = non biparti

            // Accéder à la méthode privée
            MethodInfo isBipartiteMethod = typeof(GraphAnalyzer).GetMethod("IsBipartite",
                BindingFlags.NonPublic | BindingFlags.Instance);

            bool isBipartite = (bool)isBipartiteMethod.Invoke(_analyzer, null);

            Assert.IsFalse(isBipartite, "Le graphe ne devrait pas être biparti");
        }

        [TestMethod]
        public void GraphAnalyzer_IsPlanar_ShouldReturnTrueForPlanarGraph()
        {
            // Créer un graphe planaire simulé via réflexion
            FieldInfo adjacencyListField = typeof(GraphAnalyzer).GetField("_adjacencyList",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var adjacencyList = new Dictionary<int, List<int>>();

            // Graphe planaire simple avec peu d'arêtes
            adjacencyList.Add(1, new List<int> { 101 });
            adjacencyList.Add(2, new List<int> { 102 });
            adjacencyList.Add(3, new List<int> { 103 });

            adjacencyListField.SetValue(_analyzer, adjacencyList);

            // Simuler les nœuds colorés
            FieldInfo nodeColorsField = typeof(GraphAnalyzer).GetField("_nodeColors",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var nodeColors = new Dictionary<int, int>
            {
                { 1, 0 }, { 2, 0 }, { 3, 0 },
                { 101, 1 }, { 102, 1 }, { 103, 1 }
            };
            nodeColorsField.SetValue(_analyzer, nodeColors);

            // Accéder à la méthode privée
            MethodInfo isPlanarMethod = typeof(GraphAnalyzer).GetMethod("IsPlanar",
                BindingFlags.NonPublic | BindingFlags.Instance);

            bool isPlanar = (bool)isPlanarMethod.Invoke(_analyzer, null);

            Assert.IsTrue(isPlanar, "Le graphe devrait être planaire");
        }

        #endregion

        #region Plat Tests

        [TestMethod]
        public void Plat_Constructor_Default_SetsDefaultValues()
        {
            Plat plat = new Plat();

            Assert.AreEqual(string.Empty, plat.NomPlat);
            Assert.AreEqual(string.Empty, plat.DescriptionPlat);
            Assert.AreEqual(0, plat.PrixPlat);
            Assert.AreEqual(string.Empty, plat.CategoriePlat);
            Assert.AreEqual(0, plat.IdCuisinier);
            Assert.AreEqual(DateTime.Now.Date, plat.DateFabrication.Date);
            Assert.AreEqual(DateTime.Now.AddDays(3).Date, plat.DatePeremption.Date);
        }

        [TestMethod]
        public void Plat_Constructor_WithParameters_SetsCorrectValues()
        {
            string nom = "Couscous";
            string description = "Plat traditionnel";
            decimal prix = 15.99m;
            string categorie = "Plat principal";
            int idCuisinier = 1;
            string ingredients = "Semoule, légumes";
            string type = "Chaud";
            string regime = "Végétarien";
            string nationalite = "Marocain";
            int nombre = 5;

            Plat plat = new Plat(nom, description, prix, categorie, idCuisinier,
                ingredients, type, regime, nationalite, nombre);

            Assert.AreEqual(nom, plat.NomPlat);
            Assert.AreEqual(description, plat.DescriptionPlat);
            Assert.AreEqual(prix, plat.PrixPlat);
            Assert.AreEqual(categorie, plat.CategoriePlat);
            Assert.AreEqual(idCuisinier, plat.IdCuisinier);
            Assert.AreEqual(ingredients, plat.IngredientPlat);
            Assert.AreEqual(type, plat.TypePlat);
            Assert.AreEqual(regime, plat.RegimePlat);
            Assert.AreEqual(nationalite, plat.NationalitePlat);
            Assert.AreEqual(nombre, plat.NombrePlat);
        }

       

        #endregion

        #region Client Tests

        [TestMethod]
        public void Client_Constructor_Default_SetsDefaultValues()
        {
            Client client = new Client();

            Assert.AreEqual(0, client.IdClient);
            Assert.AreEqual(string.Empty, client.NomClient);
            Assert.AreEqual(string.Empty, client.PrenomClient);
            Assert.AreEqual(string.Empty, client.EmailClient);
            Assert.AreEqual(string.Empty, client.MotDePasseClient);
            Assert.AreEqual(string.Empty, client.TelephoneClient);
            Assert.AreEqual(string.Empty, client.AdresseClient);
            Assert.AreEqual(0, client.StationMetroProche);
        }

        [TestMethod]
        public void Client_Constructor_WithParameters_SetsCorrectValues()
        {
            string nom = "Dupont";
            string prenom = "Jean";
            string email = "jean.dupont@example.com";
            string motDePasse = "password123";
            string telephone = "0123456789";
            string adresse = "123 Rue de Paris";
            int stationMetro = 5;

            Client client = new Client(nom, prenom, email, motDePasse, telephone, adresse, stationMetro);

            Assert.AreEqual(nom, client.NomClient);
            Assert.AreEqual(prenom, client.PrenomClient);
            Assert.AreEqual(email, client.EmailClient);
            Assert.AreEqual(motDePasse, client.MotDePasseClient);
            Assert.AreEqual(telephone, client.TelephoneClient);
            Assert.AreEqual(adresse, client.AdresseClient);
            Assert.AreEqual(stationMetro, client.StationMetroProche);
        }

        [TestMethod]
        public void Client_ToString_ReturnsCorrectFormat()
        {
            Client client = new Client
            {
                PrenomClient = "Jean",
                NomClient = "Dupont",
                EmailClient = "jean.dupont@example.com"
            };

            string result = client.ToString();

            Assert.AreEqual("Jean Dupont, jean.dupont@example.com", result);
        }

        #endregion

        #region Cuisinier Tests

        [TestMethod]
        public void Cuisinier_Constructor_Default_SetsDefaultValues()
        {
            Cuisinier cuisinier = new Cuisinier();

            Assert.AreEqual(0, cuisinier.IdCuisinier);
            Assert.AreEqual(string.Empty, cuisinier.NomCuisinier);
            Assert.AreEqual(string.Empty, cuisinier.PrenomCuisinier);
            Assert.AreEqual(string.Empty, cuisinier.EmailCuisinier);
            Assert.AreEqual(string.Empty, cuisinier.MotDePasseCuisinier);
            Assert.AreEqual(string.Empty, cuisinier.TelephoneCuisinier);
            Assert.AreEqual(string.Empty, cuisinier.SpecialiteCuisinier);
            Assert.AreEqual(string.Empty, cuisinier.AdresseCuisinier);
            Assert.AreEqual(0, cuisinier.StationMetroProche);
        }

        [TestMethod]
        public void Cuisinier_Constructor_WithParameters_SetsCorrectValues()
        {
            string nom = "Martin";
            string prenom = "Pierre";
            string email = "pierre.martin@example.com";
            string motDePasse = "chefpass123";
            string telephone = "0123456789";
            string specialite = "Cuisine française";
            string adresse = "456 Avenue de la République";
            int stationMetro = 8;

            Cuisinier cuisinier = new Cuisinier(nom, prenom, email, motDePasse, telephone, specialite, adresse, stationMetro);

            Assert.AreEqual(nom, cuisinier.NomCuisinier);
            Assert.AreEqual(prenom, cuisinier.PrenomCuisinier);
            Assert.AreEqual(email, cuisinier.EmailCuisinier);
            Assert.AreEqual(motDePasse, cuisinier.MotDePasseCuisinier);
            Assert.AreEqual(telephone, cuisinier.TelephoneCuisinier);
            Assert.AreEqual(specialite, cuisinier.SpecialiteCuisinier);
            Assert.AreEqual(adresse, cuisinier.AdresseCuisinier);
            Assert.AreEqual(stationMetro, cuisinier.StationMetroProche);
        }

        [TestMethod]
        public void Cuisinier_ToString_ReturnsCorrectFormat()
        {
            Cuisinier cuisinier = new Cuisinier
            {
                PrenomCuisinier = "Pierre",
                NomCuisinier = "Martin",
                SpecialiteCuisinier = "Cuisine française"
            };

            string result = cuisinier.ToString();

            Assert.AreEqual("Pierre Martin - Spécialité: Cuisine française", result);
        }

        #endregion

        #region Livraison Tests

        [TestMethod]
        public void Livraison_Constructor_Default_SetsDefaultValues()
        {
            Livraison livraison = new Livraison();

            Assert.AreEqual(0, livraison.IdLivraison);
            Assert.AreEqual(DateTime.Now.AddHours(1).Date, livraison.DateLivraison.Date);
            Assert.AreEqual(string.Empty, livraison.AdresseLivraison);
            Assert.AreEqual("En préparation", livraison.StatutLivraison);
            Assert.AreEqual(string.Empty, livraison.CommentaireLivraison);
        }

        [TestMethod]
        public void Livraison_Constructor_WithParameters_SetsCorrectValues()
        {
            string adresse = "123 Rue de Paris";
            DateTime dateLivraison = DateTime.Now.AddHours(2);

            Livraison livraison = new Livraison(adresse, dateLivraison);

            Assert.AreEqual(adresse, livraison.AdresseLivraison);
            Assert.AreEqual(dateLivraison, livraison.DateLivraison);
            Assert.AreEqual("En préparation", livraison.StatutLivraison);
        }

        [TestMethod]
        public void Livraison_ToString_ReturnsCorrectFormat()
        {
            DateTime dateLivraison = new DateTime(2023, 5, 1, 14, 30, 0);
            string adresse = "123 Rue de Paris";
            string statut = "En cours de livraison";

            Livraison livraison = new Livraison
            {
                DateLivraison = dateLivraison,
                AdresseLivraison = adresse,
                StatutLivraison = statut
            };

            string result = livraison.ToString();

            Assert.AreEqual($"Livraison prévue le 01/05/2023 à 14:30 - En cours de livraison - 123 Rue de Paris", result);
        }

        #endregion

        #region Commande Tests

        [TestMethod]
        public void Commande_Constructor_Default_SetsDefaultValues()
        {
            Commande commande = new Commande();

            Assert.AreEqual(0, commande.IdCommande);
            Assert.AreEqual(DateTime.Now.Date, commande.DateCommande.Date);
            Assert.AreEqual("En attente", commande.StatutCommande);
            Assert.AreEqual(0, commande.IdCuisinier);
            Assert.IsNotNull(commande.LignesCommande);
            Assert.AreEqual(0, commande.LignesCommande.Count);
        }

        [TestMethod]
        public void Commande_CalculerPrixTotal_CalculatesCorrectTotal()
        {
            Commande commande = new Commande();
            commande.LignesCommande.Add(new LigneCommande { IdPlat = 1, Quantite = 2, PrixUnitaire = 10.5m });
            commande.LignesCommande.Add(new LigneCommande { IdPlat = 2, Quantite = 1, PrixUnitaire = 15.75m });

            commande.CalculerPrixTotal();

            Assert.AreEqual(10.5m * 2 + 15.75m, commande.PrixTotal);
            Assert.AreEqual(commande.PrixTotal, commande.PrixCommande);
        }

        [TestMethod]
        public void Commande_AjouterLigneCommande_AddsLineAndUpdatesTotal()
        {
            Commande commande = new Commande();
            LigneCommande ligne = new LigneCommande { IdPlat = 1, Quantite = 2, PrixUnitaire = 10.5m };

            commande.AjouterLigneCommande(ligne);

            Assert.AreEqual(1, commande.LignesCommande.Count);
            Assert.AreEqual(ligne, commande.LignesCommande[0]);
            Assert.AreEqual(10.5m * 2, commande.PrixTotal);
        }

       

        #endregion

        #region Service Tests

        // Tests pour vérifier la structure des services

        [TestMethod]
        public void PlatService_MethodsExist()
        {
            Type type = typeof(PlatService);

            Assert.IsNotNull(type.GetMethod("AjouterPlat"));
            Assert.IsNotNull(type.GetMethod("MettreAJourPlat"));
            Assert.IsNotNull(type.GetMethod("SupprimerPlat"));
            Assert.IsNotNull(type.GetMethod("ObtenirPlatParId"));
            Assert.IsNotNull(type.GetMethod("ObtenirTousLesPlats"));
            Assert.IsNotNull(type.GetMethod("ObtenirPlatsParCuisinier"));
            Assert.IsNotNull(type.GetMethod("RechercherPlats"));
        }

        [TestMethod]
        public void ClientService_MethodsExist()
        {
            Type type = typeof(ClientService);

            Assert.IsNotNull(type.GetMethod("AjouterClient"));
            Assert.IsNotNull(type.GetMethod("MettreAJourClient"));
            Assert.IsNotNull(type.GetMethod("SupprimerClient"));
            Assert.IsNotNull(type.GetMethod("ObtenirClientParId"));
            Assert.IsNotNull(type.GetMethod("ObtenirClientParEmail"));
            Assert.IsNotNull(type.GetMethod("AuthentifierClient"));
            Assert.IsNotNull(type.GetMethod("ObtenirTousLesClients"));
            Assert.IsNotNull(type.GetMethod("EmailExiste"));
        }

        [TestMethod]
        public void CuisinierService_MethodsExist()
        {
            Type type = typeof(CuisinierService);

            Assert.IsNotNull(type.GetMethod("AjouterCuisinier"));
            Assert.IsNotNull(type.GetMethod("MettreAJourCuisinier"));
            Assert.IsNotNull(type.GetMethod("SupprimerCuisinier"));
            Assert.IsNotNull(type.GetMethod("ObtenirCuisinierParId"));
            Assert.IsNotNull(type.GetMethod("ObtenirCuisinierParEmail"));
            Assert.IsNotNull(type.GetMethod("AuthentifierCuisinier"));
            Assert.IsNotNull(type.GetMethod("ObtenirTousLesCuisiniers"));
            Assert.IsNotNull(type.GetMethod("EmailExiste"));
        }

        [TestMethod]
        public void LivraisonService_MethodsExist()
        {
            Type type = typeof(LivraisonService);

            Assert.IsNotNull(type.GetMethod("AjouterLivraison"));
            Assert.IsNotNull(type.GetMethod("MettreAJourStatutLivraison"));
            Assert.IsNotNull(type.GetMethod("ObtenirLivraisonParId"));
        }

        [TestMethod]
        public void CommandeService_MethodsExist()
        {
            Type type = typeof(CommandeService);

            Assert.IsNotNull(type.GetMethod("AjouterCommande"));
            Assert.IsNotNull(type.GetMethod("MettreAJourStatutCommande"));
            Assert.IsNotNull(type.GetMethod("ObtenirCommandeParId"));
            Assert.IsNotNull(type.GetMethod("ObtenirToutesLesCommandes"));
            Assert.IsNotNull(type.GetMethod("ObtenirCommandesParClient"));
            Assert.IsNotNull(type.GetMethod("ObtenirCommandesParCuisinier"));
        }

        #endregion
    }
}