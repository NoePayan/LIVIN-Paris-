
using System;
using System.Collections.Generic;
using LivInParis.Models;
using LivInParis.Services;
using LivInParis.Data;
using ETAPE2;
using LivInParis.Visualization;
using LivinParis_S2;
using MySql.Data.MySqlClient;
using System.Xml.Serialization;
using System.Xml;
using Newtonsoft.Json;
using System.Diagnostics;

namespace LivInParis
{
    class Program
    {
        private static Client? clientConnecte = null;
        private static Cuisinier? cuisinierConnecte = null;

        private static ConsoleColor couleurTitre = ConsoleColor.Blue;
        private static ConsoleColor couleurSousTitre = ConsoleColor.Yellow;
        private static ConsoleColor couleurInfo = ConsoleColor.Green;
        private static ConsoleColor couleurErreur = ConsoleColor.Red;
        private static ConsoleColor couleurMenu = ConsoleColor.White;
        private static ConsoleColor couleurPrix = ConsoleColor.Magenta;
        /// <summary>
        /// le menu initial qui lance l'appli
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; 
            Console.Title = "Liv'In Paris - Application de livraison de plats cuisinés";

            if (!DatabaseConnection.TestConnection())
            {
                AfficherErreur("Impossible de se connecter à la base de données. Veuillez vérifier vos paramètres de connexion.");
                Console.WriteLine("Appuyez sur une touche pour quitter...");
                Console.ReadKey();
                return;
            }

            bool continuer = true;
            while (continuer)
            {
                if (clientConnecte == null && cuisinierConnecte == null) 
                {
                    AfficherMenuPrincipal();
                    int choix = LireChoix(0, 6); 

                    switch (choix)
                    {
                        case 1:
                            ConnexionClient();
                            break;
                        case 2:
                            ConnexionCuisinier();
                            break;
                        case 3:
                            CreerCompteClient();
                            break;
                        case 4:
                            CreerCompteCuisinier();
                            break;
                        case 5:
                            AfficherInfosApplication();
                            break;
                        case 6: 
                            AfficherListesCompletes();
                            break;
                        case 0:
                            continuer = false;
                            break;
                    }
                }
                else if (clientConnecte != null)
                {
                    AfficherMenuClient();
                    int choix = LireChoix(0, 5);

                    switch (choix)
                    {
                        case 1:
                            ConsulterPlats();
                            break;
                        case 2:
                            PasserCommande();
                            break;
                        case 3:
                            ConsulterCommandes();
                            break;
                        case 4:
                            ModifierProfil();
                            break;
                        case 5:
                            RechercherPlats();
                            break;
                        case 0:
                            clientConnecte = null; 
                            AfficherInfo("Vous avez été déconnecté.");
                            Console.WriteLine("Appuyez sur une touche pour continuer...");
                            Console.ReadKey();
                            break;
                    }
                }
                else if (cuisinierConnecte != null)
                {
                    AfficherMenuCuisinier();
                    int choix = LireChoix(0, 5);

                    switch (choix)
                    {
                        case 1:
                            GererPlats();
                            break;
                        case 2:
                            ConsulterCommandesCuisinier();
                            break;
                        case 3:
                            ModifierProfilCuisinier();
                            break;
                        case 4:
                            AjouterPlat();
                            break;
                        case 5:
                            ModifierPlat();
                            break;
                        case 0:
                            cuisinierConnecte = null; 
                            AfficherInfo("Vous avez été déconnecté.");
                            Console.WriteLine("Appuyez sur une touche pour continuer...");
                            Console.ReadKey();
                            break;
                    }
                }
            }

            AfficherInfo("Merci d'avoir utilisé Liv'In Paris. À bientôt !");
        }

        #region Méthodes d'affichage avec couleurs
        /// <summary>
        /// formatage destitres
        ///</summary>
        /// <param name="titre">le titre a afficher</param>
        static void AfficherTitre(string titre)
        {
            Console.ForegroundColor = couleurTitre;
            Console.WriteLine(titre);
            Console.ResetColor();
        }
        /// <summary>
        /// formatage des sous titres
        ///</summary>
        /// <param name="titre">le sous titre a afficher</param>
        static void AfficherSousTitre(string sousTitre)
        {
            Console.ForegroundColor = couleurSousTitre;
            Console.WriteLine(sousTitre);
            Console.ResetColor();
        }
        /// <summary>
        /// formatage des informations
        ///</summary>
        /// <param name="titre">l'information a afficher</param>
        static void AfficherInfo(string info)
        {
            Console.ForegroundColor = couleurInfo;
            Console.WriteLine(info);
            Console.ResetColor();
        }
        /// <summary>
        /// formatage des erreurs
        ///</summary>
        /// <param name="titre">l'erreur a afficher</param>
        static void AfficherErreur(string erreur)
        {
            Console.ForegroundColor = couleurErreur;
            Console.WriteLine(erreur);
            Console.ResetColor();
        }
        /// <summary>
        /// formatage des prix
        ///</summary>
        /// <param name="titre">le prix a afficher</param>
        static void AfficherPrix(decimal prix)
        {
            Console.ForegroundColor = couleurPrix;
            Console.Write(prix.ToString("$"));
            Console.ResetColor();
        }
        /// <summary>
        /// creation de cadres autout d'un titre
        ///</summary>
        /// <param name="titre">le titre a encadrer</param>
        static void AfficherCadre(string titre)
        {
            int largeur = Math.Max(titre.Length + 8, 40);
            string ligne = new string('═', largeur);

            Console.ForegroundColor = couleurTitre;
            Console.WriteLine("╔"+ligne+"╗");
            Console.WriteLine("║"+titre.PadLeft((largeur + titre.Length) / 2).PadRight(largeur)+"║");
            Console.WriteLine("╚"+ligne+"╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        #endregion

        #region Menus
        /// <summary>
        /// le menu ou géneral de l'application
        /// </summary>
        static void AfficherMenuPrincipal()
        {
            Console.Clear();
            AfficherCadre("LIV'IN PARIS - Livraison de plats cuisinés maison");

            Console.ForegroundColor = couleurMenu;
            Console.WriteLine("1. Connexion client");
            Console.WriteLine("2. Connexion cuisinier");
            Console.WriteLine("3. Créer un compte client");
            Console.WriteLine("4. Créer un compte cuisinier");
            Console.WriteLine("5. À propos de l'application");
            Console.WriteLine("6. Afficher les listes (clients, cuisiniers, plats, commandes)"); 
            Console.WriteLine("0. Quitter");
            Console.ResetColor();
            Console.WriteLine();
            Console.Write("Votre choix : ");
        }
        /// <summary>
        /// le menu de l'interface client
        /// </summary>
        static void AfficherMenuClient()
        {
            Console.Clear();
            AfficherCadre("LIV'IN PARIS - Livraison de plats cuisinés maison");

            AfficherInfo("Bienvenue, "+clientConnecte?.PrenomClient+" " +clientConnecte?.NomClient);
            Console.WriteLine();

            Console.ForegroundColor = couleurMenu;
            Console.WriteLine("1. Consulter les plats disponibles");
            Console.WriteLine("2. Passer une commande");
            Console.WriteLine("3. Consulter mes commandes");
            Console.WriteLine("4. Modifier mon profil");
            Console.WriteLine("5. Rechercher des plats");
            Console.WriteLine("0. Déconnexion");
            Console.ResetColor();
            Console.WriteLine();
            Console.Write("Votre choix : ");
        }
        /// <summary>
        /// le menu de l'interfce cuisinier
        /// </summary>
        static void AfficherMenuCuisinier()
        {
            Console.Clear();
            AfficherCadre("LIV'IN PARIS - Livraison de plats cuisinés maison");

            AfficherInfo("Bienvenue, "+cuisinierConnecte?.PrenomCuisinier+" "+cuisinierConnecte?.NomCuisinier+" - Spécialité: "+cuisinierConnecte?.SpecialiteCuisinier);
            Console.WriteLine();

            Console.ForegroundColor = couleurMenu;
            Console.WriteLine("1. Gérer mes plats");
            Console.WriteLine("2. Consulter les commandes");
            Console.WriteLine("3. Modifier mon profil");
            Console.WriteLine("4. Ajouter un nouveau plat");
            Console.WriteLine("5. Modifier un plat existant");
            Console.WriteLine("0. Déconnexion");
            Console.ResetColor();
            Console.WriteLine();
            Console.Write("Votre choix : ");
        }

        #endregion

        #region Fonctions utilitaires
        /// <summary>
        /// methode de lecture d'un choix nottament pour un switchcase
        /// </summary>
        /// <param name="min">le choix minimum</param>
        /// <param name="max">le choix maximum</param>
        /// <returns></returns>
        static int LireChoix(int min, int max)
        {
            int choix;
            while (!int.TryParse(Console.ReadLine(), out choix) || choix < min || choix > max)
            {
                Console.ForegroundColor = couleurErreur;
                Console.Write("Choix invalide. Veuillez entrer un nombre entre "+min+" et "+max+" : ");
                Console.ResetColor();
            }
            return choix;
        }
        /// <summary>
        /// lecture d'un decimal pour les prix
        /// </summary>
        /// <param name="message">l'entree (le decimal)</param>
        /// <returns>la valeur du decimal donné</returns>
        static decimal LireDecimal(string message)
        {
            decimal valeur;
            Console.Write(message);
            while (!decimal.TryParse(Console.ReadLine(), out valeur) || valeur < 0)
            {
                Console.ForegroundColor = couleurErreur;
                Console.Write("Valeur invalide. Veuillez entrer un nombre positif : ");
                Console.ResetColor();
            }
            return valeur;
        }
        /// <summary>
        /// lecture poussée d'un entier
        /// </summary>
        /// <param name="message">l'entier</param>
        /// <param name="min">la valeur minimae de l'entier</param>
        /// <returns>la valeur de l'entier</returns>
        static int LireEntier(string message, int min = 0)
        {
            int valeur;
            Console.Write(message);
            while (!int.TryParse(Console.ReadLine(), out valeur) || valeur < min)
            {
                Console.ForegroundColor = couleurErreur;
                Console.Write("Valeur invalide. Veuillez entrer un nombre supérieur ou égal à "+min+" : ");
                Console.ResetColor();
            }
            return valeur;
        }
        /// <summary>
        /// lire une date
        /// </summary>
        /// <param name="message">le message envoye</param>
        /// <returns>la date</returns>
        static DateTime LireDate(string message)
        {
            DateTime date;
            Console.Write(message);
            while (!DateTime.TryParse(Console.ReadLine(), out date))
            {
                Console.ForegroundColor = couleurErreur;
                Console.Write("Date invalide. Veuillez entrer une date valide (JJ/MM/AAAA) : ");
                Console.ResetColor();
            }
            return date;
        }
        /// <summary>
        /// lit une chaine de caracteres
        /// </summary>
        /// <param name="message">la chaine</param>
        /// <param name="obligatoire">cree une obligation d'avoir des caracters</param>
        /// <returns>la chaine </returns>
        static string LireChaine(string message, bool obligatoire = true)
        {
            string? valeur;
            do
            {
                Console.Write(message);
                valeur = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(valeur) && obligatoire)
                {
                    Console.ForegroundColor = couleurErreur;
                    Console.WriteLine("Cette information est obligatoire.");
                    Console.ResetColor();
                }
            } while (string.IsNullOrWhiteSpace(valeur) && obligatoire);

            return valeur ?? string.Empty;
        }

        #endregion

        #region Fonctionnalités communes
        /// <summary>
        /// affiche les informations de l'application, permet l'exportation en JSON ou XML, affiche le Graphe relationnel
        /// </summary>
        static void AfficherInfosApplication()
        {
            Console.Clear();
            AfficherCadre("À PROPOS DE L'APPLICATION");
            Console.WriteLine("Extraction de la base de données vers XML ou JSON");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Choisissez le format de sortie :");
            Console.WriteLine("1. XML");
            Console.WriteLine("2. JSON");
            Console.Write("Votre choix (1 ou 2) : ");

            string choix = Console.ReadLine();

            if (choix == "1")
            {
                Console.WriteLine("\nExportation en XML...");
                ExtraireSQLVersXML();
            }
            else if (choix == "2")
            {
                Console.WriteLine("\nExportation en JSON...");
                ExtraireSQLVersJSON();
            }
            else
            {
                Console.WriteLine("\nChoix invalide. Veuillez entrer 1 pour XML ou 2 pour JSON.");
            }

            Console.WriteLine("\nAppuyez sur une touche pour quitter...");
            Console.ReadKey();
            AfficherSousTitre("Liv'In Paris est une application de mise en relation entre des cuisiniers amateurs");
            AfficherSousTitre("et des clients souhaitant se faire livrer des plats cuisinés maison.");
            Console.WriteLine();
            AfficherInfo("Version: 1.0");
            AfficherInfo("Date: Avril 2023");
            GraphAnalyzer analyzer = new GraphAnalyzer();
            analyzer.AnalyzeAndVisualize();
            Console.WriteLine();
            Console.WriteLine("Voulez vous les détails de la partie graphique du projet ? O");
            if (Console.ReadLine() == "O")
            {
                LivinParisMetro.PresGraphe();
            }

            Console.WriteLine("Appuyez sur une touche pour revenir au menu principal...");
            Console.ReadKey();
        }
        /// <summary>
        /// affiche les listes de clients, plats, commandes
        /// </summary>
        static void AfficherListesCompletes()
        {
            bool retourMenu = false;

            while (!retourMenu)
            {
                Console.Clear();
                AfficherCadre("LISTES COMPLÈTES");

                Console.ForegroundColor = couleurMenu;
                Console.WriteLine("1. Liste de tous les clients");
                Console.WriteLine("2. Liste de tous les cuisiniers");
                Console.WriteLine("3. Liste de tous les plats");
                Console.WriteLine("4. Liste de toutes les commandes");
                Console.WriteLine("0. Retour au menu principal");
                Console.ResetColor();
                Console.WriteLine();
                Console.Write("Votre choix : ");

                int choix = LireChoix(0, 4);

                switch (choix)
                {
                    case 1:
                        AfficherTousLesClients();
                        break;
                    case 2:
                        AfficherTousLesCuisiniers();
                        break;
                    case 3:
                        AfficherTousLesPlats();
                        break;
                    case 4:
                        AfficherToutesLesCommandes();
                        break;
                    case 0:
                        retourMenu = true;
                        break;
                }
            }
        }
        /// <summary>
        /// affiche la liste des clients
        /// </summary>
        static void AfficherTousLesClients()
        {
            Console.Clear();
            AfficherCadre("LISTE DE TOUS LES CLIENTS");

            List<Client> clients = ClientService.ObtenirTousLesClients();

            if (clients.Count == 0)
            {
                AfficherInfo("Aucun client enregistré.");
            }
            else
            {
                AfficherSousTitre($"Nombre total de clients: {clients.Count}");
                Console.WriteLine();

                foreach (var client in clients)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("ID: " + client.IdClient + " | Nom: " + client.PrenomClient + " " + client.NomClient);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Email: " + client.EmailClient + " | Téléphone: " + client.TelephoneClient);
                    Console.WriteLine("Adresse: " + client.AdresseClient);

                    List<Commande> commandes = CommandeService.ObtenirCommandesParClient(client.IdClient);
                    if (commandes.Count > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("Commandes (" + commandes.Count + "):");
                        foreach (var commande in commandes)
                        {
                            Console.WriteLine("  - " + commande.NumeroCommande + " (" + commande.DateCommande.ToString("dd/MM/yyyy") + ") - " + commande.PrixTotal.ToString("C") + " - " + commande.StatutCommande);
                        }
                    }
                    Console.WriteLine(new string('-', 50));
                    Console.ResetColor();

                }
            }

            Console.WriteLine();
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
        /// <summary>
        /// affiche la liste des cuisiniers
        /// </summary>
        static void AfficherTousLesCuisiniers()
        {
            Console.Clear();
            AfficherCadre("LISTE DE TOUS LES CUISINIERS");

            List<Cuisinier> cuisiniers = CuisinierService.ObtenirTousLesCuisiniers();

            if (cuisiniers.Count == 0)
            {
                AfficherInfo("Aucun cuisinier enregistré.");
            }
            else
            {
                AfficherSousTitre($"Nombre total de cuisiniers: {cuisiniers.Count}");
                Console.WriteLine();

                foreach (var cuisinier in cuisiniers)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("ID: " + cuisinier.IdCuisinier + " | Nom: " + cuisinier.PrenomCuisinier + " " + cuisinier.NomCuisinier);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Spécialité: " + cuisinier.SpecialiteCuisinier);
                    Console.WriteLine("Email: " + cuisinier.EmailCuisinier + " | Téléphone: " + cuisinier.TelephoneCuisinier);
                    Console.WriteLine("Adresse: " + cuisinier.AdresseCuisinier);

                    List<Plat> plats = PlatService.ObtenirPlatsParCuisinier(cuisinier.IdCuisinier);
                    if (plats.Count > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("Plats proposés (" + plats.Count + "):");
                        foreach (var plat in plats)
                        {
                            Console.WriteLine("  - " + plat.NomPlat + " - " + plat.PrixPlat.ToString("C") + " - " + plat.CategoriePlat);
                        }
                    }
                    Console.WriteLine(new string('-', 50));
                }

            }

            Console.WriteLine();
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
        /// <summary>
        /// affiche la liste des plats
        /// </summary>
        static void AfficherTousLesPlats()
        {
            Console.Clear();
            AfficherCadre("LISTE DE TOUS LES PLATS");

            List<Plat> plats = PlatService.ObtenirTousLesPlats();

            if (plats.Count == 0)
            {
                AfficherInfo("Aucun plat enregistré.");
            }
            else
            {
                AfficherSousTitre($"Nombre total de plats: {plats.Count}");
                Console.WriteLine();


                var platsParCategorie = plats.GroupBy(delegate (Plat p) { return p.CategoriePlat; });


                foreach (var groupe in platsParCategorie)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("=== " + groupe.Key.ToUpper() + " ===");
                    Console.ResetColor();

                    foreach (var plat in groupe)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("ID: " + plat.IdPlat + " | " + plat.NomPlat);
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Prix: " + plat.PrixPlat.ToString("C"));
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("Description: " + plat.DescriptionPlat);

                        Cuisinier? cuisinier = CuisinierService.ObtenirCuisinierParId(plat.IdCuisinier);
                        if (cuisinier != null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine("Préparé par: " + cuisinier.PrenomCuisinier + " " + cuisinier.NomCuisinier);
                        }

                        Console.WriteLine(new string('-', 40));
                        Console.ResetColor();
                    }
                }

            }

            Console.WriteLine();
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
        /// <summary>
        /// affiche la liste des commandes
        /// </summary>
        static void AfficherToutesLesCommandes()
        {
            Console.Clear();
            AfficherCadre("LISTE DE TOUTES LES COMMANDES");

            List<Commande> commandes = CommandeService.ObtenirToutesLesCommandes();

            if (commandes.Count == 0)
            {
                AfficherInfo("Aucune commande enregistrée.");
            }
            else
            {
                AfficherSousTitre($"Nombre total de commandes: {commandes.Count}");
                Console.WriteLine();

                commandes.Sort((a, b) => b.DateCommande.CompareTo(a.DateCommande));

                foreach (var commande in commandes)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Commande #{commande.NumeroCommande} - {commande.DateCommande:dd/MM/yyyy HH:mm}");

                    Console.Write("Statut: ");
                    switch (commande.StatutCommande.ToLower())
                    {
                        case "en attente":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case "en préparation":
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case "prêt pour livraison":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case "en cours de livraison":
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        case "livré":
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case "annulé":
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                    }
                    Console.WriteLine(commande.StatutCommande);
                    Console.ResetColor();

                    Client? client = ClientService.ObtenirClientParId(commande.IdClient);
                    if (client != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("Client: " + client.PrenomClient + " " + client.NomClient);
                    }

                    if (commande.IdCuisinier > 0)
                    {
                        Cuisinier? cuisinier = CuisinierService.ObtenirCuisinierParId(commande.IdCuisinier);
                        if (cuisinier != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine("Cuisinier: " + cuisinier.PrenomCuisinier + " " + cuisinier.NomCuisinier);
                        }
                    }


                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Plats commandés:");
                    foreach (var ligne in commande.LignesCommande)
                    {
                        Plat? plat = PlatService.ObtenirPlatParId(ligne.IdPlat);
                        if (plat != null)
                        {
                            Console.WriteLine("  - " + ligne.Quantite + " x " + plat.NomPlat + " (" + ligne.PrixUnitaire.ToString("$") + ") = " + ligne.PrixTotal.ToString("$"));
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Prix total: "+commande.PrixTotal+"$");

                    Console.WriteLine(new string('=', 50));
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        #endregion

        #region Fonctionnalités Client
        /// <summary>
        /// peremet la connexion d'un client via son mail et mot de passe
        /// </summary>
        static void ConnexionClient()
        {
            Console.Clear();
            AfficherCadre("CONNEXION CLIENT");

            string email = LireChaine("Email : ");
            string motDePasse = LireChaine("Mot de passe : ");

            Client? client = ClientService.AuthentifierClient(email, motDePasse);
            if (client != null)
            {
                clientConnecte = client;
                AfficherInfo("Bienvenue, "+client.PrenomClient+" "+ client.NomClient);
            }
            else
            {
                AfficherErreur("Email ou mot de passe incorrect.");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
        /// <summary>
        /// cree un compte client dans la base de donnee a partir d'un mail existant
        /// </summary>
        static void CreerCompteClient()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║      CRÉATION DE COMPTE CLIENT      ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();

            string nom = LireChaine("Nom : ");
            string prenom = LireChaine("Prénom : ");
            string email = LireChaine("Email : ");

            if (ClientService.EmailExiste(email))
            {
                Console.WriteLine("Cet email est déjà utilisé. Veuillez en choisir un autre.");
                Console.WriteLine("Appuyez sur une touche pour revenir au menu principal...");
                Console.ReadKey();
                return;
            }

            string motDePasse = LireChaine("Mot de passe : ");
            string telephone = LireChaine("Téléphone : ");
            string adresse = LireChaine("Adresse : ");
            int stationMetro = LivinParisMetro.RecupererStation();

            Client client = new Client(nom, prenom, email, motDePasse, telephone, adresse, stationMetro);
            int idClient = ClientService.AjouterClient(client);

            if (idClient > 0)
            {
                Console.WriteLine("Compte créé avec succès !");
                client.IdClient = idClient;
                clientConnecte = client;
            }
            else
            {
                Console.WriteLine("Erreur lors de la création du compte.");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
        /// <summary>
        /// permet de voir les plats disponibles
        /// </summary>
        static void ConsulterPlats()
        {
            Console.Clear();
            AfficherCadre("PLATS DISPONIBLES");

            List<Plat> plats = PlatService.ObtenirTousLesPlats();
            if (plats.Count == 0)
            {
                AfficherInfo("Aucun plat disponible pour le moment.");
            }
            else
            {
                AfficherSousTitre("Liste des plats disponibles :");
                Console.WriteLine(new string('─', 50));

                var platsParCategorie = plats.GroupBy(p => p.CategoriePlat);

                foreach (var groupe in platsParCategorie)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("=== " + groupe.Key.ToUpper() + " ===");
                    Console.ResetColor();

                    foreach (var plat in groupe)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(plat.IdPlat + ". " + plat.NomPlat + " - ");
                        Console.ForegroundColor = couleurPrix;
                        Console.Write(plat.PrixPlat.ToString("C"));
                        Console.ResetColor();
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }


                Console.WriteLine();
                Console.Write("Entrez l'ID d'un plat pour voir les détails (0 pour revenir au menu) : ");
                int idPlat = LireChoix(0, int.MaxValue);

                if (idPlat > 0)
                {
                    Plat? platDetail = plats.Find(p => p.IdPlat == idPlat);
                    if (platDetail != null)
                    {
                        Console.Clear();
                        AfficherCadre("DÉTAILS DU PLAT");

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Nom: " + platDetail.NomPlat);

                        Console.ForegroundColor = couleurPrix;
                        Console.WriteLine("Prix: " + platDetail.PrixPlat.ToString("C"));
                        Console.ResetColor();

                        Console.WriteLine("Description: " + platDetail.DescriptionPlat);
                        Console.WriteLine("Catégorie: " + platDetail.CategoriePlat);
                        Console.WriteLine("Type: " + platDetail.TypePlat);
                        Console.WriteLine("Régime: " + platDetail.RegimePlat);
                        Console.WriteLine("Nationalité: " + platDetail.NationalitePlat);
                        Console.WriteLine("Ingrédients: " + platDetail.IngredientPlat);

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Fabriqué le: " + platDetail.DateFabrication.ToString("dd/MM/yyyy"));
                        Console.WriteLine("À consommer avant le: " + platDetail.DatePeremption.ToString("dd/MM/yyyy"));
                        Console.ResetColor();

                        Console.WriteLine("Quantité disponible: " + platDetail.NombrePlat);

                        Cuisinier? cuisinier = CuisinierService.ObtenirCuisinierParId(platDetail.IdCuisinier);
                        if (cuisinier != null)
                        {
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Préparé par: " + cuisinier.PrenomCuisinier + " " + cuisinier.NomCuisinier);
                            Console.WriteLine("Spécialité: " + cuisinier.SpecialiteCuisinier);
                            Console.ResetColor();
                        }
                    }

                    else
                    {
                        AfficherErreur("Plat non trouvé.");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }
        /// <summary>
        /// permet au client de passer une commande d'un ou plusieurs plats. la commande sera reliée au cuisinier du premier plat
        /// </summary>
        static void PasserCommande()
        {
            if (clientConnecte == null) return;

            Console.Clear();
            AfficherCadre("PASSER UNE COMMANDE");

            List<Plat> plats = PlatService.ObtenirTousLesPlats();
            if (plats.Count == 0)
            {
                AfficherInfo("Aucun plat disponible pour le moment.");
                Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
                Console.ReadKey();
                return;
            }

            AfficherSousTitre("Liste des plats disponibles :");
            Console.WriteLine(new string('─', 50));

            var platsParCategorie = plats.GroupBy(p => p.CategoriePlat);

            foreach (var groupe in platsParCategorie)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"=== {groupe.Key.ToUpper()} ===");
                Console.ResetColor();

                foreach (var plat in groupe)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(plat.IdPlat + ". " + plat.NomPlat + " - ");
                    Console.ForegroundColor = couleurPrix;
                    Console.Write(plat.PrixPlat.ToString("C"));
                    Console.ResetColor();
                    Console.WriteLine();
                }
                Console.WriteLine();

            }

            List<LigneCommande> lignesCommande = new List<LigneCommande>();
            bool continuerAjout = true;
            int idCuisinierCommande = 0; 

            while (continuerAjout)
            {
                Console.WriteLine();
                Console.Write("Entrez l'ID du plat à ajouter (0 pour terminer) : ");
                int idPlat = LireChoix(0, int.MaxValue);

                if (idPlat == 0)
                {
                    continuerAjout = false;
                }
                else
                {
                    Plat? platChoisi = plats.Find(p => p.IdPlat == idPlat);
                    if (platChoisi != null)
                    {
                        int quantite = LireEntier("Quantité : ", 1);
                        lignesCommande.Add(new LigneCommande(idPlat, quantite, platChoisi.PrixPlat));
                        AfficherInfo(platChoisi.NomPlat+" ajouté à la commande.");

                        if (idCuisinierCommande == 0)
                        {
                            idCuisinierCommande = platChoisi.IdCuisinier;
                        }
                        else if (idCuisinierCommande != platChoisi.IdCuisinier)
                        {
                            AfficherErreur("ATTENTION: Ce plat est proposé par un cuisinier différent des plats précédents.");
                            Console.WriteLine("Votre commande sera associée au cuisinier du premier plat sélectionné.");
                        }
                    }
                    else
                    {
                        AfficherErreur("Plat non trouvé.");
                    }
                }
            }

            if (lignesCommande.Count == 0)
            {
                AfficherInfo("Commande annulée - aucun plat sélectionné.");
                Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
                Console.ReadKey();
                return;
            }

            Commande commande = new Commande(clientConnecte.IdClient, lignesCommande);
            commande.IdCuisinier = idCuisinierCommande; 

            Console.Clear();
            AfficherCadre("RÉCAPITULATIF COMMANDE");

            AfficherSousTitre("Plats commandés :");
            Console.WriteLine(new string('─', 40));

            foreach (var ligne in lignesCommande)
            {
                Plat? plat = plats.Find(p => p.IdPlat == ligne.IdPlat);
                if (plat != null)
                {
                    Console.Write(ligne.Quantite + " x " + plat.NomPlat + " - ");
                    Console.ForegroundColor = couleurPrix;
                    Console.Write(ligne.PrixUnitaire.ToString("$") + " = " + ligne.PrixTotal.ToString("$"));
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }

            Cuisinier? cuisinier = CuisinierService.ObtenirCuisinierParId(idCuisinierCommande);
            if (cuisinier != null)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Cuisinier: " + cuisinier.PrenomCuisinier + " " + cuisinier.NomCuisinier);
                Console.ResetColor();
            }


            Console.WriteLine();
            Console.Write("Total : ");
            Console.ForegroundColor = couleurPrix;
            Console.WriteLine(commande.PrixTotal+"$");
            Console.ResetColor();
            Console.WriteLine();

            Console.Write("Confirmer la commande ? (O/N) : ");
            string confirmation = Console.ReadLine() ?? "";

            if (confirmation.ToUpper() == "O")
            {
                string adresseLivraison = LireChaine("Adresse de livraison [" + clientConnecte.AdresseClient + "] : ", false);
                if (string.IsNullOrWhiteSpace(adresseLivraison))
                {
                    adresseLivraison = clientConnecte.AdresseClient;
                }

                Livraison livraison = new Livraison(adresseLivraison, DateTime.Now.AddHours(1));
                int idLivraison = LivraisonService.AjouterLivraison(livraison);
                commande.IdLivraison = idLivraison;

                Historique historique = new Historique("Création commande", "Commande créée par le client " + clientConnecte.IdClient);
                int idHistorique = HistoriqueService.AjouterHistorique(historique);
                commande.IdHistorique = idHistorique;

                int idCommande = CommandeService.AjouterCommande(commande);

                if (idCommande > 0)
                {
                    AfficherInfo("Commande enregistrée avec succès !");
                    Console.WriteLine("Numéro de commande : " + commande.NumeroCommande);
                    Console.WriteLine("Livraison prévue le : " + livraison.DateLivraison.ToString("dd/MM/yyyy 'à' HH:mm"));
                }
                else
                {
                    AfficherErreur("Erreur lors de l'enregistrement de la commande.");
                }
            }

            else
            {
                AfficherInfo("Commande annulée.");
            }

            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }

        static void ConsulterCommandes()
        {
            if (clientConnecte == null) return;

            Console.Clear();
            AfficherCadre("MES COMMANDES");

            List<Commande> commandes = CommandeService.ObtenirCommandesParClient(clientConnecte.IdClient);
            if (commandes.Count == 0)
            {
                AfficherInfo("Vous n'avez pas encore passé de commande.");
            }
            else
            {
                AfficherSousTitre("Liste de vos commandes :");
                Console.WriteLine(new string('─', 50));

                // Trier les commandes par date (plus récentes d'abord)
                commandes.Sort((a, b) => b.DateCommande.CompareTo(a.DateCommande));

                foreach (var commande in commandes)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(commande.IdCommande + ". " + commande.NumeroCommande + " - " + commande.DateCommande.ToString("dd/MM/yyyy HH:mm") + " - ");
                    Console.ForegroundColor = couleurPrix;
                    Console.Write(commande.PrixTotal+"$");
                    Console.ResetColor();
                    Console.Write(" - ");

                    // Colorer le statut
                    switch (commande.StatutCommande.ToLower())
                    {
                        case "en attente":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case "en préparation":
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case "prêt pour livraison":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case "en cours de livraison":
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        case "livré":
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case "annulé":
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                    }
                    Console.WriteLine(commande.StatutCommande);
                    Console.ResetColor();
                }

                Console.WriteLine();
                Console.Write("Entrez l'ID d'une commande pour voir les détails (0 pour revenir au menu) : ");
                int idCommande = LireChoix(0, int.MaxValue);

                if (idCommande > 0)
                {
                    Commande? commandeDetail = commandes.Find(c => c.IdCommande == idCommande);
                    if (commandeDetail != null)
                    {
                        Console.Clear();
                        AfficherCadre("DÉTAILS DE LA COMMANDE");

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Commande #" + commandeDetail.NumeroCommande);
                        Console.WriteLine("Date: " + commandeDetail.DateCommande.ToString("dd/MM/yyyy HH:mm"));


                        Console.Write("Statut: ");
                        switch (commandeDetail.StatutCommande.ToLower())
                        {
                            case "en attente":
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case "en préparation":
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            case "prêt pour livraison":
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                            case "en cours de livraison":
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                break;
                            case "livré":
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case "annulé":
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.Gray;
                                break;
                        }
                        Console.WriteLine(commandeDetail.StatutCommande);
                        Console.ResetColor();

                        Console.WriteLine();
                        AfficherSousTitre("Détails de la commande:");
                        Console.WriteLine(new string('─', 40));

                        foreach (var ligne in commandeDetail.LignesCommande)
                        {
                            Plat? plat = PlatService.ObtenirPlatParId(ligne.IdPlat);
                            if (plat != null)
                            {
                                Console.Write("- " + ligne.Quantite + " x " + plat.NomPlat + " (");
                                Console.ForegroundColor = couleurPrix;
                                Console.Write(ligne.PrixUnitaire.ToString("$"));
                                Console.ResetColor();
                                Console.Write(") = ");
                                Console.ForegroundColor = couleurPrix;
                                Console.WriteLine(ligne.PrixTotal.ToString("$"));
                                Console.ResetColor();
                            }
                        }

                        Console.WriteLine();
                        Console.Write("Prix total: ");
                        Console.ForegroundColor = couleurPrix;
                        Console.WriteLine(commandeDetail.PrixTotal.ToString("$"));
                        Console.ResetColor();


                        if (commandeDetail.IdLivraison.HasValue)
                        {
                            Livraison? livraison = LivraisonService.ObtenirLivraisonParId(commandeDetail.IdLivraison.Value);
                            if (livraison != null)
                            {
                                Console.WriteLine();
                                AfficherSousTitre("Informations de livraison :");
                                Console.WriteLine(new string('─', 40));
                                Console.WriteLine("Date de livraison : " + livraison.DateLivraison.ToString("dd/MM/yyyy 'à' HH:mm"));
                                Console.WriteLine("Adresse : " + livraison.AdresseLivraison);
                                Console.WriteLine("Statut : " + livraison.StatutLivraison);
                                if (!string.IsNullOrEmpty(livraison.CommentaireLivraison))
                                {
                                    Console.WriteLine("Commentaire : " + livraison.CommentaireLivraison);
                                }
                            }

                        }
                    }
                    else
                    {
                        AfficherErreur("Commande non trouvée.");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }
        /// <summary>
        /// la modification d'un profil de client
        /// </summary>
        static void ModifierProfil()
        {
            if (clientConnecte == null) return;

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║       MODIFIER MON PROFIL           ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine("Laissez un champ vide pour conserver la valeur actuelle.");
            Console.WriteLine();

            string nom = LireChaine("Nom [" + clientConnecte.NomClient + "] : ", false);
            if (!string.IsNullOrWhiteSpace(nom)) clientConnecte.NomClient = nom;

            string prenom = LireChaine("Prénom [" + clientConnecte.PrenomClient + "] : ", false);
            if (!string.IsNullOrWhiteSpace(prenom)) clientConnecte.PrenomClient = prenom;

            string telephone = LireChaine("Téléphone [" + clientConnecte.TelephoneClient + "] : ", false);
            if (!string.IsNullOrWhiteSpace(telephone)) clientConnecte.TelephoneClient = telephone;

            string adresse = LireChaine("Adresse [" + clientConnecte.AdresseClient + "] : ", false);
            if (!string.IsNullOrWhiteSpace(adresse)) clientConnecte.AdresseClient = adresse;

            string stationMetroStr = LireChaine("Numéro de la station de métro la plus proche [" + clientConnecte.StationMetroProche + "] : ", false);
            if (!string.IsNullOrWhiteSpace(stationMetroStr) && int.TryParse(stationMetroStr, out int stationMetro))
            {
                clientConnecte.StationMetroProche = stationMetro;
            }


            string motDePasse = LireChaine("Nouveau mot de passe (laissez vide pour ne pas changer) : ", false);
            if (!string.IsNullOrWhiteSpace(motDePasse)) clientConnecte.MotDePasseClient = motDePasse;

            if (ClientService.MettreAJourClient(clientConnecte))
            {
                Console.WriteLine("Profil mis à jour avec succès !");
            }
            else
            {
                Console.WriteLine("Erreur lors de la mise à jour du profil.");
            }

            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }
        static void RechercherPlats()
        {
            Console.Clear();
            AfficherCadre("RECHERCHER DES PLATS");

            string terme = LireChaine("Entrez un terme de recherche (nom, catégorie, ingrédient, etc.) : ");
            List<Plat> plats = PlatService.RechercherPlats(terme);

            if (plats.Count == 0)
            {
                AfficherInfo("Aucun plat trouvé pour cette recherche.");
            }
            else
            {
                AfficherSousTitre(plats.Count + " plat(s) trouvé(s) :");
                Console.WriteLine(new string('─', 50));

                foreach (var plat in plats)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(plat.IdPlat + ". " + plat.NomPlat + " - ");
                    Console.ForegroundColor = couleurPrix;
                    Console.Write(plat.PrixPlat.ToString("C"));
                    Console.ResetColor();
                    Console.WriteLine(" - " + plat.CategoriePlat);
                }


                Console.WriteLine();
                Console.Write("Entrez l'ID d'un plat pour voir les détails (0 pour revenir au menu) : ");
                int idPlat = LireChoix(0, int.MaxValue);

                if (idPlat > 0)
                {
                    Plat? platDetail = plats.Find(p => p.IdPlat == idPlat);
                    if (platDetail != null)
                    {
                        Console.Clear();
                        AfficherCadre("DÉTAILS DU PLAT");

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Nom: " + platDetail.NomPlat);

                        Console.ForegroundColor = couleurPrix;
                        Console.WriteLine("Prix: " + platDetail.PrixPlat.ToString("C"));
                        Console.ResetColor();

                        Console.WriteLine("Description: " + platDetail.DescriptionPlat);
                        Console.WriteLine("Catégorie: " + platDetail.CategoriePlat);
                        Console.WriteLine("Type: " + platDetail.TypePlat);
                        Console.WriteLine("Régime: " + platDetail.RegimePlat);
                        Console.WriteLine("Nationalité: " + platDetail.NationalitePlat);
                        Console.WriteLine("Ingrédients: " + platDetail.IngredientPlat);

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Fabriqué le: " + platDetail.DateFabrication.ToString("dd/MM/yyyy"));
                        Console.WriteLine("À consommer avant le: " + platDetail.DatePeremption.ToString("dd/MM/yyyy"));
                        Console.ResetColor();

                        Console.WriteLine("Quantité disponible: " + platDetail.NombrePlat);

                        Cuisinier? cuisinier = CuisinierService.ObtenirCuisinierParId(platDetail.IdCuisinier);
                        if (cuisinier != null)
                        {
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Préparé par: " + cuisinier.PrenomCuisinier + " " + cuisinier.NomCuisinier);
                            Console.WriteLine("Spécialité: " + cuisinier.SpecialiteCuisinier);
                            Console.ResetColor();
                        }
                    }

                    else
                    {
                        AfficherErreur("Plat non trouvé.");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }


        #endregion

        #region Fonctionnalités Cuisinier

        static void ConnexionCuisinier()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║         CONNEXION CUISINIER         ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();

            string email = LireChaine("Email : ");
            string motDePasse = LireChaine("Mot de passe : ");

            Cuisinier? cuisinier = CuisinierService.AuthentifierCuisinier(email, motDePasse);
            if (cuisinier != null)
            {
                cuisinierConnecte = cuisinier;
                Console.WriteLine("Bienvenue, "+cuisinier.PrenomCuisinier+" "+cuisinier.NomCuisinier);
            }
            else
            {
                Console.WriteLine("Email ou mot de passe incorrect.");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
        /// <summary>
        /// creation d'un compte cuisinier
        /// </summary>
        static void CreerCompteCuisinier()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║    CRÉATION DE COMPTE CUISINIER     ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();

            string nom = LireChaine("Nom : ");
            string prenom = LireChaine("Prénom : ");
            string email = LireChaine("Email : ");

            if (CuisinierService.EmailExiste(email))
            {
                Console.WriteLine("Cet email est déjà utilisé. Veuillez en choisir un autre.");
                Console.WriteLine("Appuyez sur une touche pour revenir au menu principal...");
                Console.ReadKey();
                return;
            }

            string motDePasse = LireChaine("Mot de passe : ");
            string telephone = LireChaine("Téléphone : ");
            string specialite = LireChaine("Spécialité culinaire : ");
            string adresse = LireChaine("Adresse : ");
            int stationMetro = LivinParisMetro.RecupererStation();

            Cuisinier cuisinier = new Cuisinier(nom, prenom, email, motDePasse, telephone, specialite, adresse, stationMetro);
            int idCuisinier = CuisinierService.AjouterCuisinier(cuisinier);

            if (idCuisinier > 0)
            {
                Console.WriteLine("Compte créé avec succès !");
                cuisinier.IdCuisinier = idCuisinier;
                cuisinierConnecte = cuisinier;
            }
            else
            {
                Console.WriteLine("Erreur lors de la création du compte.");
            }

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
        static void GererPlats()
        {
            if (cuisinierConnecte == null) return;

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║       GÉRER MES PLATS               ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();

            List<Plat> plats = PlatService.ObtenirPlatsParCuisinier(cuisinierConnecte.IdCuisinier);
            if (plats.Count == 0)
            {
                Console.WriteLine("Vous n'avez pas encore ajouté de plats.");
            }
            else
            {
                Console.WriteLine("Liste de vos plats :");
                Console.WriteLine("------------------");
                foreach (var plat in plats)
                {
                    Console.WriteLine(plat.IdPlat + ". " + plat.NomPlat + " - " + plat.PrixPlat.ToString("C") + " - " + plat.CategoriePlat);
                }

                Console.WriteLine();
                Console.WriteLine("1. Ajouter un nouveau plat");
                Console.WriteLine("2. Modifier un plat existant");
                Console.WriteLine("3. Supprimer un plat");
                Console.WriteLine("0. Revenir au menu");
                Console.WriteLine();
                Console.Write("Votre choix : ");
                int choix = LireChoix(0, 3);

                switch (choix)
                {
                    case 1:
                        AjouterPlat();
                        break;
                    case 2:
                        ModifierPlat();
                        break;
                    case 3:
                        SupprimerPlat();
                        break;
                }
            }

            if (plats.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine("Voulez-vous ajouter un plat maintenant ? (O/N) : ");
                string reponse = Console.ReadLine() ?? "";
                if (reponse.ToUpper() == "O")
                {
                    AjouterPlat();
                    return;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }
        /// <summary>
        /// ajout d'un plat pour un cuisinier
        /// </summary>
        static void AjouterPlat()
        {
            if (cuisinierConnecte == null) return;

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║       AJOUTER UN PLAT               ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();

            string nom = LireChaine("Nom du plat : ");
            string description = LireChaine("Description : ");
            decimal prix = LireDecimal("Prix (€) : ");
            string categorie = LireChaine("Catégorie (entrée, plat, dessert, etc.) : ");
            string ingredients = LireChaine("Ingrédients (séparés par des virgules) : ");
            string type = LireChaine("Type de cuisine (française, italienne, etc.) : ");
            string regime = LireChaine("Régime (végétarien, sans gluten, etc.) : ", false);
            string nationalite = LireChaine("Nationalité du plat : ", false);
            int nombre = LireEntier("Nombre de portions disponibles : ", 1);

            Plat plat = new Plat(nom, description, prix, categorie, cuisinierConnecte.IdCuisinier,
                                 ingredients, type, regime, nationalite, nombre);

            int idPlat = PlatService.AjouterPlat(plat);
            if (idPlat > 0)
            {
                Console.WriteLine("Plat ajouté avec succès !");
            }
            else
            {
                Console.WriteLine("Erreur lors de l'ajout du plat.");
            }

            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }
        /// <summary>
        /// modification d'un plat pour un cuisinier
        /// </summary>
        static void ModifierPlat()
        {
            if (cuisinierConnecte == null) return;

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║       MODIFIER UN PLAT              ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();

            List<Plat> plats = PlatService.ObtenirPlatsParCuisinier(cuisinierConnecte.IdCuisinier);
            if (plats.Count == 0)
            {
                Console.WriteLine("Vous n'avez pas encore ajouté de plats.");
                Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Liste de vos plats :");
            Console.WriteLine("------------------");
            foreach (var plat in plats)
            {
                Console.WriteLine(plat.IdPlat + ". " + plat.NomPlat + " - " + plat.PrixPlat.ToString("$") + " - " + plat.CategoriePlat);
            }

            Console.WriteLine();
            Console.Write("Entrez l'ID du plat à modifier (0 pour annuler) : ");
            int idPlat = LireChoix(0, int.MaxValue);

            if (idPlat == 0) return;

            Plat? platAModifier = plats.Find(p => p.IdPlat == idPlat);
            if (platAModifier == null)
            {
                Console.WriteLine("Plat non trouvé.");
                Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║       MODIFIER UN PLAT              ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("Modification du plat : "+platAModifier.NomPlat);
            Console.WriteLine();
            Console.WriteLine("Laissez un champ vide pour conserver la valeur actuelle.");
            Console.WriteLine();

            string nom = LireChaine("Nom [" + platAModifier.NomPlat + "] : ", false);
            if (!string.IsNullOrWhiteSpace(nom)) platAModifier.NomPlat = nom;

            string description = LireChaine("Description [" + platAModifier.DescriptionPlat + "] : ", false);
            if (!string.IsNullOrWhiteSpace(description)) platAModifier.DescriptionPlat = description;

            string prixStr = LireChaine("Prix [" + platAModifier.PrixPlat.ToString("C") + "] : ", false);
            if (!string.IsNullOrWhiteSpace(prixStr) && decimal.TryParse(prixStr, out decimal prix))
            {
                platAModifier.PrixPlat = prix;
            }

            string categorie = LireChaine("Catégorie [" + platAModifier.CategoriePlat + "] : ", false);
            if (!string.IsNullOrWhiteSpace(categorie)) platAModifier.CategoriePlat = categorie;

            string ingredients = LireChaine("Ingrédients [" + platAModifier.IngredientPlat + "] : ", false);
            if (!string.IsNullOrWhiteSpace(ingredients)) platAModifier.IngredientPlat = ingredients;

            string type = LireChaine("Type de cuisine [" + platAModifier.TypePlat + "] : ", false);
            if (!string.IsNullOrWhiteSpace(type)) platAModifier.TypePlat = type;

            string regime = LireChaine("Régime [" + platAModifier.RegimePlat + "] : ", false);
            if (!string.IsNullOrWhiteSpace(regime)) platAModifier.RegimePlat = regime;

            string nationalite = LireChaine("Nationalité [" + platAModifier.NationalitePlat + "] : ", false);
            if (!string.IsNullOrWhiteSpace(nationalite)) platAModifier.NationalitePlat = nationalite;

            string nombreStr = LireChaine("Nombre de portions disponibles [" + platAModifier.NombrePlat + "] : ", false);

            if (!string.IsNullOrWhiteSpace(nombreStr) && int.TryParse(nombreStr, out int nombre))
            {
                platAModifier.NombrePlat = nombre;
            }

            if (PlatService.MettreAJourPlat(platAModifier))
            {
                Console.WriteLine("Plat mis à jour avec succès !");
            }
            else
            {
                Console.WriteLine("Erreur lors de la mise à jour du plat.");
            }

            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }
        /// <summary>
        /// suppression d'un plat pour un cuisinier
        /// </summary>
        static void SupprimerPlat()
        {
            if (cuisinierConnecte == null) return;

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║       SUPPRIMER UN PLAT             ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();

            List<Plat> plats = PlatService.ObtenirPlatsParCuisinier(cuisinierConnecte.IdCuisinier);
            if (plats.Count == 0)
            {
                Console.WriteLine("Vous n'avez pas encore ajouté de plats.");
                Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Liste de vos plats :");
            Console.WriteLine("------------------");
            foreach (var plat in plats)
            {
                Console.WriteLine(plat.IdPlat+plat.NomPlat+" - "+plat.PrixPlat+" - "+plat.CategoriePlat);
            }

            Console.WriteLine();
            Console.Write("Entrez l'ID du plat à supprimer (0 pour annuler) : ");
            int idPlat = LireChoix(0, int.MaxValue);

            if (idPlat == 0) return;

            Plat? platASupprimer = plats.Find(p => p.IdPlat == idPlat);
            if (platASupprimer == null)
            {
                Console.WriteLine("Plat non trouvé.");
                Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Êtes-vous sûr de vouloir supprimer le plat "+platASupprimer.NomPlat+" ? (O/N) : ");
            string confirmation = Console.ReadLine() ?? "";

            if (confirmation.ToUpper() == "O")
            {
                if (PlatService.SupprimerPlat(idPlat))
                {
                    Console.WriteLine("Plat supprimé avec succès !");
                }
                else
                {
                    Console.WriteLine("Erreur lors de la suppression du plat.");
                }
            }
            else
            {
                Console.WriteLine("Suppression annulée.");
            }

            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }
        /// <summary>
        /// le cuisinker peu consulter ses commandes
        /// </summary>
        static void ConsulterCommandesCuisinier()
        {
            if (cuisinierConnecte == null) return;

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║       MES COMMANDES                 ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();

            List<Commande> commandes = CommandeService.ObtenirCommandesParCuisinier(cuisinierConnecte.IdCuisinier);
            if (commandes.Count == 0)
            {
                Console.WriteLine("Vous n'avez pas encore reçu de commandes.");
            }
            else
            {
                Console.WriteLine("Liste des commandes :");
                Console.WriteLine("-------------------");
                foreach (var commande in commandes)
                {
                    Console.WriteLine(commande.IdCommande + ". " + commande.NumeroCommande + " - " + commande.DateCommande.ToString("dd/MM/yyyy HH:mm") + " - " + commande.PrixTotal.ToString("C") + " - " + commande.StatutCommande);
                }

                Console.WriteLine();
                Console.Write("Entrez l'ID d'une commande pour voir les détails (0 pour revenir au menu) : ");
                int idCommande = LireChoix(0, int.MaxValue);

                if (idCommande > 0)
                {
                    Commande? commandeDetail = commandes.Find(c => c.IdCommande == idCommande);
                    if (commandeDetail != null)
                    {
                        Console.Clear();
                        Console.WriteLine("╔══════════════════════════════════════╗");
                        Console.WriteLine("║       DÉTAILS DE LA COMMANDE        ║");
                        Console.WriteLine("╚══════════════════════════════════════╝");
                        Console.WriteLine();
                        Console.WriteLine(commandeDetail.AfficherDetails());

                        Client? client = ClientService.ObtenirClientParId(commandeDetail.IdClient);
                        if (client != null)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Informations du client :");
                            Console.WriteLine("----------------------");
                            Console.WriteLine("Nom : " + client.PrenomClient + " " + client.NomClient);
                            Console.WriteLine("Téléphone : " + client.TelephoneClient);
                            Console.WriteLine("Email : " + client.EmailClient);

                            int Id2 = client.StationMetroProche;
                            Console.WriteLine("Souhaitez vous obtenir un trajet via le métro parisien ? O/N");
                            if (Console.ReadLine() == "O")
                            {
                                LivinParisMetro.CreerChemin(cuisinierConnecte.StationMetroProche, Id2);

                            }
                        }

                        if (commandeDetail.IdLivraison.HasValue)
                        {
                            Livraison? livraison = LivraisonService.ObtenirLivraisonParId(commandeDetail.IdLivraison.Value);
                            if (livraison != null)
                            {
                                Console.WriteLine();
                                Console.WriteLine("Informations de livraison :");
                                Console.WriteLine("--------------------------");
                                Console.WriteLine("Date de livraison : " + livraison.DateLivraison.ToString("dd/MM/yyyy à HH:mm"));
                                Console.WriteLine("Adresse : " + livraison.AdresseLivraison);
                                Console.WriteLine("Statut : " + livraison.StatutLivraison);

                                if (!string.IsNullOrEmpty(livraison.CommentaireLivraison))
                                {
                                    Console.WriteLine($"Commentaire : {livraison.CommentaireLivraison}");
                                }
                            }
                        }

                        Console.WriteLine();
                        Console.WriteLine("Mettre à jour le statut de la commande :");
                        Console.WriteLine("1. En préparation");
                        Console.WriteLine("2. Prêt pour livraison");
                        Console.WriteLine("3. En cours de livraison");
                        Console.WriteLine("4. Livré");
                        Console.WriteLine("5. Annulé");
                        Console.WriteLine("0. Ne pas modifier");
                        Console.WriteLine();
                        Console.Write("Votre choix : ");
                        int choixStatut = LireChoix(0, 5);

                        if (choixStatut > 0)
                        {
                            string nouveauStatut;
                            switch (choixStatut)
                            {
                                case 1:
                                    nouveauStatut = "En préparation";
                                    break;
                                case 2:
                                    nouveauStatut = "Prêt pour livraison";
                                    break;
                                case 3:
                                    nouveauStatut = "En cours de livraison";
                                    break;
                                case 4:
                                    nouveauStatut = "Livré";
                                    break;
                                case 5:
                                    nouveauStatut = "Annulé";
                                    break;
                                default:
                                    nouveauStatut = commandeDetail.StatutCommande;
                                    break;
                            }

                            if (nouveauStatut == "Livré")
                            {
                                LivraisonService.MettreAJourStatutLivraison(commandeDetail.IdLivraison.Value, nouveauStatut);
                            }

                            if (CommandeService.MettreAJourStatutCommande(commandeDetail.IdCommande, nouveauStatut))
                            {
                                Console.WriteLine("Statut mis à jour : "+nouveauStatut);
                            }
                            else
                            {
                                Console.WriteLine("Erreur lors de la mise à jour du statut.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Commande non trouvée.");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }

        static void ModifierProfilCuisinier()
        {
            if (cuisinierConnecte == null) return;

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║       MODIFIER MON PROFIL           ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine("Laissez un champ vide pour conserver la valeur actuelle.");
            Console.WriteLine();

            string nom = LireChaine("Nom [" + cuisinierConnecte.NomCuisinier + "] : ", false);
            if (!string.IsNullOrWhiteSpace(nom)) cuisinierConnecte.NomCuisinier = nom;

            string prenom = LireChaine("Prénom [" + cuisinierConnecte.PrenomCuisinier + "] : ", false);
            if (!string.IsNullOrWhiteSpace(prenom)) cuisinierConnecte.PrenomCuisinier = prenom;

            string telephone = LireChaine("Téléphone [" + cuisinierConnecte.TelephoneCuisinier + "] : ", false);
            if (!string.IsNullOrWhiteSpace(telephone)) cuisinierConnecte.TelephoneCuisinier = telephone;

            string specialite = LireChaine("Spécialité [" + cuisinierConnecte.SpecialiteCuisinier + "] : ", false);
            if (!string.IsNullOrWhiteSpace(specialite)) cuisinierConnecte.SpecialiteCuisinier = specialite;

            string adresse = LireChaine("Adresse [" + cuisinierConnecte.AdresseCuisinier + "] : ", false);
            if (!string.IsNullOrWhiteSpace(adresse)) cuisinierConnecte.AdresseCuisinier = adresse;

            string stationMetroStr = LireChaine("Numéro de la station de métro la plus proche [" + cuisinierConnecte.StationMetroProche + "] : ", false);

            if (!string.IsNullOrWhiteSpace(stationMetroStr) && int.TryParse(stationMetroStr, out int stationMetro))
            {
                cuisinierConnecte.StationMetroProche = stationMetro;
            }

            string motDePasse = LireChaine("Nouveau mot de passe (laissez vide pour ne pas changer) : ", false);
            if (!string.IsNullOrWhiteSpace(motDePasse)) cuisinierConnecte.MotDePasseCuisinier = motDePasse;

            if (CuisinierService.MettreAJourCuisinier(cuisinierConnecte))
            {
                Console.WriteLine("Profil mis à jour avec succès !");
            }
            else
            {
                Console.WriteLine("Erreur lors de la mise à jour du profil.");
            }

            Console.WriteLine("Appuyez sur une touche pour revenir au menu...");
            Console.ReadKey();
        }
        #endregion



        /// <summary>
        /// Sérialise une liste de clients en JSON
        /// </summary>
        /// <param name="clients">La liste de clients à sérialiser</param>
        /// <param name="nomFichier">Le nom du fichier de sortie</param>
        static void SerialiserClientsEnJSON(ClientListX clients, string nomFichier)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(clients, settings);

            File.WriteAllText(nomFichier, json);

            Console.WriteLine("Fichier JSON créé: " + Path.GetFullPath(nomFichier));

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Path.GetFullPath(nomFichier),
                    UseShellExecute = true
                });
                Console.WriteLine("Fichier JSON ouvert automatiquement.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Impossible d'ouvrir le fichier automatiquement: " + ex.Message);
            }
        }

        /// <summary>
        /// Extrait les données de la base de données et les sérialise en JSON
        /// </summary>
        static void ExtraireSQLVersJSON()
        {
            try
            {
                ClientListX clients = ChargerClientsDepuisMySQL();

                Console.WriteLine("Nombre de clients extraits: " + clients.NombreClients());

                SerialiserClientsEnJSON(clients, "clients.json");

                Console.WriteLine("Sérialisation dans fichier clients.json terminée");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        /// <summary>
        /// Extrait les données de la base de données et les sérialise en XML
        /// </summary>
        static void ExtraireSQLVersXML()
        {
            try
            {
                ClientListX clients = ChargerClientsDepuisMySQL();

                Console.WriteLine("Nombre de clients extraits: " + clients.NombreClients());

                SerialiserClientsEnXML(clients, "clients.xml");

                Console.WriteLine("Sérialisation dans fichier clients.xml terminée");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        /// <summary>
        /// Charge les clients depuis la base de données MySQL
        /// </summary>
        /// <returns>Une liste de clients</returns>
        static ClientListX ChargerClientsDepuisMySQL()
        {
            ClientListX clientList = new ClientListX();

            // Utiliser la connexion de DatabaseConnection au lieu d'une chaîne codée en dur
            using (MySqlConnection connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                Console.WriteLine("Connexion à la base de données réussie");

                string query = "SELECT * FROM client";
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ClientX client = new ClientX(
                            reader.GetInt32("id_Client"),
                            reader.GetString("nom_Client"),
                            reader.IsDBNull(reader.GetOrdinal("prenom_Client")) ? null : reader.GetString("prenom_Client"),
                            reader.IsDBNull(reader.GetOrdinal("adresse_Client")) ? null : reader.GetString("adresse_Client"),
                            reader.IsDBNull(reader.GetOrdinal("station_metro_proche")) ? (int?)null : reader.GetInt32("station_metro_proche"),
                            reader.IsDBNull(reader.GetOrdinal("telephone_Client")) ? null : reader.GetString("telephone_Client"),
                            reader.IsDBNull(reader.GetOrdinal("type_Client")) ? null : reader.GetString("type_Client"),
                            reader.IsDBNull(reader.GetOrdinal("entreprise_Client")) ? null : reader.GetString("entreprise_Client"),
                            reader.IsDBNull(reader.GetOrdinal("referent_Client")) ? null : reader.GetString("referent_Client"),
                            reader.GetString("email_Client"),
                            reader.GetString("mot_de_passe_Client")
                        );

                        clientList.Ajouter(client);
                        Console.WriteLine("Client extrait: " + client);
                    }
                }
            }

            return clientList;
        }

        /// <summary>
        /// Sérialise une liste de clients en XML
        /// </summary>
        /// <param name="clients">La liste de clients à sérialiser</param>
        /// <param name="nomFichier">Le nom du fichier de sortie</param>
        static void SerialiserClientsEnXML(ClientListX clients, string nomFichier)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ClientListX));

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };

            using (XmlWriter writer = XmlWriter.Create(nomFichier, settings))
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration xmldecl = doc.CreateXmlDeclaration("1.0", "UTF-8", "no");

                xs.Serialize(writer, clients);
            }

            Console.WriteLine("Fichier XML créé: " + Path.GetFullPath(nomFichier));

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Path.GetFullPath(nomFichier),
                    UseShellExecute = true
                });
                Console.WriteLine("Fichier XML ouvert automatiquement.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Impossible d'ouvrir le fichier automatiquement: " + ex.Message);
            }
        }

        /// <summary>
        /// Lit un fichier XML
        /// </summary>
        /// <param name="nomFichier">Le nom du fichier à lire</param>
        static void Lecture(string nomFichier)
        {
            XmlDocument docXml = new XmlDocument();
            docXml.Load(nomFichier);

            XmlElement racine = docXml.DocumentElement;

            Console.WriteLine("racine : " + racine.Name);

            Console.WriteLine();
            foreach (XmlNode e in racine)
            {
                Console.WriteLine("balise : " + e.Name);

                if (e.ChildNodes != null)
                {
                    foreach (XmlAttribute a in e.Attributes)
                    {
                        Console.Write("  attribute :");
                        Console.Write(" name = " + a.Name);
                        Console.WriteLine(", value : " + a.Value);
                    }

                    foreach (XmlNode e2 in e)
                    {
                        if (e2.NodeType == XmlNodeType.Text)
                        {
                            Console.WriteLine("  InnerText : " + e2.InnerText);
                        }
                        else
                        {
                            Console.WriteLine("  type : " + e2.NodeType + ", nom : " + e2.Name);
                        }
                    }
                }
            }
        }

    }
}
