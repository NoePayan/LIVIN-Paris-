using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace ETAPE2
{
    /// <summary>
    /// Cette classe permet de créer une représentation visuelle d'un chemin dans le réseau de métro.
    /// Elle affiche les stations, les connexions entre elles, et met en évidence le chemin le plus court.
    /// </summary>
    public class RepresentationChemin
    {
        /// <summary>
        /// Le graphe contenant toutes les stations et les liens du réseau de métro
        /// </summary>
        private Graphe graphe;
        /// <summary>
        /// La liste des noms des stations du chemin
        /// </summary>
        private List<string> listeStations;
        /// <summary>
        /// La liste des lignes de métro utilisées 
        /// </summary>
        private List<string> listeLignes;
        /// <summary>
        /// Dictionnaire qui associe chaque ligne de métro aux couleurs
        /// </summary>
        private Dictionary<string, Color> couleursLignes;
        /// <summary>
        /// Largeur de l'image en pixels
        /// </summary>
        private int largeur;
        /// <summary>
        /// Hauteur de l'image en pixels
        /// </summary>
        private int hauteur;
        /// <summary>
        /// Valeurs min et max des latitudes et longitudes pour cadrer la carte
        /// </summary>
        private float latMin, latMax, longMin, longMax;
        /// <summary>
        /// Marge en pixels autour de la carte
        /// </summary>
        private float marge = 50;
        /// <summary>
        /// Rayon des stations
        /// </summary>
        private float rayonNoeud = 20;
        /// <summary>
        /// Police utilisée pour les noms des stations
        /// </summary>
        private Font policeStation;
        /// <summary>
        /// Police utilisée pour la légende
        /// </summary>
        private Font policeLégende;
        /// <summary>
        /// Police utilisée pour les temps de trajet
        /// </summary>
        private Font policeTemps;
        /// <summary>
        /// Constructeur de la classe RepresentationChemin
        /// </summary>
        /// <param name="graphe">Le graphe contenant toutes les stations et liens</param>
        /// <param name="listeStations">La liste des stations qui composent le chemin</param>
        /// <param name="listeLignes">La liste des lignes utilisées entre chaque station</param>
        /// <param name="largeur">Largeur de l'image en pixels (1200 par défaut)</param>
        /// <param name="hauteur">Hauteur de l'image en pixels (800 par défaut)</param>
        public RepresentationChemin(Graphe graphe, List<string> listeStations, List<string> listeLignes, int largeur = 1200, int hauteur = 800)
        {
            this.graphe = graphe;
            this.listeStations = listeStations;
            this.listeLignes = listeLignes;
            this.largeur = largeur;
            this.hauteur = hauteur;
            this.policeStation = new Font("Arial", 9, FontStyle.Bold);
            this.policeLégende = new Font("Arial", 10, FontStyle.Bold);
            this.policeTemps = new Font("Arial", 9, FontStyle.Regular);
            InitialiserCouleurs();
            CalculerLimites();
        }

        /// <summary>
        /// Initialise les couleurs pour chaque ligne de métro
        /// </summary>
        private void InitialiserCouleurs()
        {
            couleursLignes = new Dictionary<string, Color>
            {
                { "1", Color.FromArgb(255, 255, 168, 0) },
                { "2", Color.FromArgb(255, 0, 0, 255) },
                { "3", Color.FromArgb(255, 152, 251, 152) },
                { "30", Color.FromArgb(255, 137, 207, 240) },
                { "4", Color.FromArgb(255, 128, 0, 128) },
                { "5", Color.FromArgb(255, 255, 165, 0) },
                { "6", Color.FromArgb(255, 0, 128, 0) },
                { "7", Color.FromArgb(255, 255, 192, 203) },
                { "70", Color.FromArgb(255, 173, 216, 230) },
                { "8", Color.FromArgb(255, 255, 0, 0) },
                { "9", Color.FromArgb(255, 255, 215, 0) },
                { "10", Color.FromArgb(255, 210, 180, 140) },
                { "11", Color.FromArgb(255, 128, 0, 0) },
                { "12", Color.FromArgb(255, 0, 128, 128) },
                { "13", Color.FromArgb(255, 0, 255, 0) },
                { "14", Color.FromArgb(255, 75, 0, 130) }
            };

            foreach (var ligne in listeLignes.Distinct())
            {
                if (!couleursLignes.ContainsKey(ligne))
                {
                    Random rnd = new Random(ligne.GetHashCode());
                    couleursLignes[ligne] = Color.FromArgb(255, rnd.Next(256), rnd.Next(256), rnd.Next(256));
                }
            }
        }

        /// <summary>
        /// Calcule les limites géographiques du chemin pour adapter l'affichage
        /// Ça permet de zoomer sur la partie qui nous intéresse
        /// </summary>
        private void CalculerLimites()
        {

            var noeudsTrajet = graphe.Noeuds.Where(n => listeStations.Contains(n.Nom)).ToList();

            if (noeudsTrajet.Count == 0)
            {
                throw new Exception("Aucun noeud trouvé dans le chemin");
            }


            latMin = noeudsTrajet.Min(n => n.latitude);
            latMax = noeudsTrajet.Max(n => n.latitude);
            longMin = noeudsTrajet.Min(n => n.longitude);
            longMax = noeudsTrajet.Max(n => n.longitude);

            float margeLatitude = (latMax - latMin) * 0.1f;
            float margeLongitude = (longMax - longMin) * 0.1f;

            latMin -= margeLatitude;
            latMax += margeLatitude;
            longMin -= margeLongitude;
            longMax += margeLongitude;
        }

        /// <summary>
        /// Convertit les coordonnées GPS en coordonnées pixels pour l'affichage
        /// C'est comme une projection cartographique simplifiée
        /// </summary>
        /// <param name="lat">Latitude de la station</param>
        /// <param name="lon">Longitude de la station</param>
        /// <returns>Les coordonnées en pixels sur l'image</returns>
        private PointF ConvertirCoordonnéesEnPixels(float lat, float lon)
        {
            float x = ((lon - longMin) / (longMax - longMin)) * (largeur - 2 * marge) + marge;
            /// On inverse l'axe Y car la latitude augmente vers le nord mais les coordonnées Y augmentent vers le bas
            float y = (1 - ((lat - latMin) / (latMax - latMin))) * (hauteur - 2 * marge) + marge;
            return new PointF(x, y);
        }

        /// <summary>
        /// Dessine une flèche pour indiquer le sens de circulation
        /// Utile pour les lignes à sens unique
        /// </summary>
        /// <param name="g">Le contexte graphique</param>
        /// <param name="stylo">Le stylo utilisé pour dessiner</param>
        /// <param name="début">Point de départ de la flèche</param>
        /// <param name="fin">Point d'arrivée de la flèche</param>
        /// <param name="tailleFlèche">Taille de la pointe de la flèche</param>
        private void DessinerFlèche(Graphics g, Pen stylo, PointF début, PointF fin, float tailleFlèche)
        {
            float dx = fin.X - début.X;
            float dy = fin.Y - début.Y;

            float longueur = (float)Math.Sqrt(dx * dx + dy * dy);
            dx /= longueur;
            dy /= longueur;

            PointF pointFlèche1 = new PointF(
                fin.X - tailleFlèche * (dx + dy / 2),
                fin.Y - tailleFlèche * (dy - dx / 2)
            );

            PointF pointFlèche2 = new PointF(
                fin.X - tailleFlèche * (dx - dy / 2),
                fin.Y - tailleFlèche * (dy + dx / 2)
            );

            g.DrawLine(stylo, fin, pointFlèche1);
            g.DrawLine(stylo, fin, pointFlèche2);
        }

        /// <summary>
        /// Vérifie si un lien est à sens unique
        /// </summary>
        /// <param name="départ">Noeud de départ</param>
        /// <param name="arrivée">Noeud d'arrivée</param>
        /// <returns>Vrai si le lien est à sens unique, faux sinon</returns>
        private bool EstLienSensUnique(Noeud<int> départ, Noeud<int> arrivée)
        {
            return !graphe.Liens.Any(l => l.Depart == arrivée && l.Arrivee == départ);
        }

        /// <summary>
        /// Récupère le temps de trajet entre deux stations
        /// </summary>
        /// <param name="départ">Station de départ</param>
        /// <param name="arrivée">Station d'arrivée</param>
        /// <returns>Le temps en minutes ou 0 si le lien n'existe pas</returns>
        private int ObtenirTempsTrajet(Noeud<int> départ, Noeud<int> arrivée)
        {
            /// Cherche un lien entre les deux stations (dans un sens ou dans l'autre)
            var lien = graphe.Liens.FirstOrDefault(l =>
                (l.Depart == départ && l.Arrivee == arrivée) ||
                (l.Depart == arrivée && l.Arrivee == départ));

            /// Retourne le temps du lien ou 0 si le lien n'existe pas
            return lien?.Temps ?? 0;
        }
        /// <summary>
        /// Dessine le chemin complet sur une image et l'enregistre
        /// </summary>
        /// <param name="cheminFichier">Chemin où enregistrer l'image</param>
        public void DessinerChemin(string cheminFichier)
        {
            using (Bitmap image = new Bitmap(largeur, hauteur))
            using (Graphics g = Graphics.FromImage(image))
            {
                /// Configuration pour une meilleure qualité d'image
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                g.Clear(Color.White);

                var noeudsTrajet = new List<Noeud<int>>();
                foreach (var nomStation in listeStations)
                {
                    var noeud = graphe.Noeuds.FirstOrDefault(n => n.Nom == nomStation);
                    if (noeud != null)
                    {
                        noeudsTrajet.Add(noeud);
                    }
                }

                // Dessin des liens entre les stations
                for (int i = 0; i < noeudsTrajet.Count - 1; i++)
                {
                    var noeudDépart = noeudsTrajet[i];
                    var noeudArrivée = noeudsTrajet[i + 1];
                    var ligne = i < listeLignes.Count ? listeLignes[i] : "Inconnue";

                    PointF pointDépart = ConvertirCoordonnéesEnPixels(noeudDépart.latitude, noeudDépart.longitude);
                    PointF pointArrivée = ConvertirCoordonnéesEnPixels(noeudArrivée.latitude, noeudArrivée.longitude);

                    float dx = pointArrivée.X - pointDépart.X;
                    float dy = pointArrivée.Y - pointDépart.Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (distance > 0)
                    {
                        PointF départAjusté = new PointF(
                            pointDépart.X + (dx / distance) * rayonNoeud,
                            pointDépart.Y + (dy / distance) * rayonNoeud
                        );

                        PointF arrivéeAjustée = new PointF(
                            pointArrivée.X - (dx / distance) * rayonNoeud,
                            pointArrivée.Y - (dy / distance) * rayonNoeud
                        );

                        Color couleurLigne = couleursLignes.ContainsKey(ligne) ?
                            couleursLignes[ligne] : Color.Gray;

                        using (Pen styloLigne = new Pen(couleurLigne, 3))
                        {
                            g.DrawLine(styloLigne, départAjusté, arrivéeAjustée);

                            if (EstLienSensUnique(noeudDépart, noeudArrivée))
                            {
                                DessinerFlèche(g, styloLigne, départAjusté, arrivéeAjustée, 12);
                            }

                            int temps = ObtenirTempsTrajet(noeudDépart, noeudArrivée);
                            string texteTemps = $"{temps} min";

                            PointF pointMilieu = new PointF(
                                (départAjusté.X + arrivéeAjustée.X) / 2,
                                (départAjusté.Y + arrivéeAjustée.Y) / 2
                            );

                            float décalageX = dy * 0.1f;
                            float décalageY = -dx * 0.1f;

                            PointF positionTemps = new PointF(
                                pointMilieu.X + décalageX,
                                pointMilieu.Y + décalageY
                            );

                            SizeF tailleTexte = g.MeasureString(texteTemps, policeTemps);
                            RectangleF rectTexte = new RectangleF(
                                positionTemps.X - 2,
                                positionTemps.Y - 2,
                                tailleTexte.Width + 4,
                                tailleTexte.Height + 4
                            );

                            using (Brush pinceau = new SolidBrush(Color.FromArgb(240, 255, 255, 255)))
                            {
                                g.FillRectangle(pinceau, rectTexte);
                            }

                            using (Brush pinceau = new SolidBrush(Color.Black))
                            {
                                g.DrawString(texteTemps, policeTemps, pinceau, positionTemps);
                            }
                        }
                    }
                }

                /// Dessin des noeuds (stations)
                for (int i = 0; i < noeudsTrajet.Count; i++)
                {
                    var noeud = noeudsTrajet[i];
                    PointF centre = ConvertirCoordonnéesEnPixels(noeud.latitude, noeud.longitude);

                    Color couleurNoeud;
                    if (i == 0)
                    {
                        couleurNoeud = Color.Red;
                    }
                    else if (i == noeudsTrajet.Count - 1)
                    {
                        couleurNoeud = Color.Green;
                    }
                    else
                    {
                        couleurNoeud = Color.White;
                    }

                    using (Brush pinceauNoeud = new SolidBrush(couleurNoeud))
                    using (Pen styloNoeud = new Pen(Color.Black, 2))
                    {
                        g.FillEllipse(pinceauNoeud, centre.X - rayonNoeud, centre.Y - rayonNoeud,
                                     rayonNoeud * 2, rayonNoeud * 2);
                        g.DrawEllipse(styloNoeud, centre.X - rayonNoeud, centre.Y - rayonNoeud,
                                     rayonNoeud * 2, rayonNoeud * 2);
                    }

                    string nomStation = noeud.Nom;
                    if (nomStation.Length > 15)
                    {
                        nomStation = nomStation.Substring(0, 12) + "...";
                    }

                    SizeF tailleTexte = g.MeasureString(nomStation, policeStation);
                    PointF positionTexte = new PointF(
                        centre.X - tailleTexte.Width / 2,
                        centre.Y - tailleTexte.Height / 2
                    );

                    Color couleurTexte = (couleurNoeud == Color.Red) ? Color.White : Color.Black;

                    using (Brush pinceauTexte = new SolidBrush(couleurTexte))
                    {
                        g.DrawString(nomStation, policeStation, pinceauTexte, positionTexte);
                    }
                }

                for (int i = 1; i < noeudsTrajet.Count - 1; i++)
                {
                    var noeudPrecedent = noeudsTrajet[i - 1];
                    var noeudCourant = noeudsTrajet[i];
                    var noeudSuivant = noeudsTrajet[i + 1];

                    string lignePrecedente = (i - 1) < listeLignes.Count ? listeLignes[i - 1] : "Inconnue";
                    string ligneSuivante = i < listeLignes.Count ? listeLignes[i] : "Inconnue";

                    PointF centre = ConvertirCoordonnéesEnPixels(noeudCourant.latitude, noeudCourant.longitude);

                    AfficherTempsChangement(g, noeudCourant, centre, lignePrecedente, ligneSuivante);
                }

                DessinerLégende(g);

                image.Save(cheminFichier);
                Console.WriteLine($"Visualisation du chemin enregistrée dans {cheminFichier}");
            }
        }

        /// <summary>
        /// Dessine la légende sur l'image
        /// Explique les couleurs et les symboles utilisés
        /// </summary>
        /// <param name="g">Le contexte graphique</param>
        private void DessinerLégende(Graphics g)
        {
            int posX = largeur - 200;
            int posY = 50;
            int hauteurLigne = 25;
            int longueurLigne = 40;
            int décalageTexte = 50;

            using (Brush pinceauTitre = new SolidBrush(Color.Black))
            {
                g.DrawString("Légende", policeLégende, pinceauTitre, posX, posY - 30);
            }

            int nombreLignes = listeLignes.Distinct().Count() + 3; /// +3 pour départ, arrivée et titre
            int hauteurLégende = nombreLignes * hauteurLigne + 20;

            using (Brush pinceauLégende = new SolidBrush(Color.FromArgb(240, 240, 240)))
            using (Pen styloLégende = new Pen(Color.Black, 1))
            {
                g.FillRectangle(pinceauLégende, posX - 10, posY - 10, 170, hauteurLégende);
                g.DrawRectangle(styloLégende, posX - 10, posY - 10, 170, hauteurLégende);
            }

            int index = 0;

            using (Brush pinceauDépart = new SolidBrush(Color.Red))
            using (Pen styloNoeud = new Pen(Color.Black, 1))
            {
                g.FillEllipse(pinceauDépart, posX, posY + index * hauteurLigne, 15, 15);
                g.DrawEllipse(styloNoeud, posX, posY + index * hauteurLigne, 15, 15);

                using (Brush pinceauTexte = new SolidBrush(Color.Black))
                {
                    g.DrawString("Station de départ", policeLégende, pinceauTexte,
                                posX + décalageTexte, posY + index * hauteurLigne);
                }
            }
            index++;

            using (Brush pinceauArrivée = new SolidBrush(Color.Green))
            using (Pen styloNoeud = new Pen(Color.Black, 1))
            {
                g.FillEllipse(pinceauArrivée, posX, posY + index * hauteurLigne, 15, 15);
                g.DrawEllipse(styloNoeud, posX, posY + index * hauteurLigne, 15, 15);

                using (Brush pinceauTexte = new SolidBrush(Color.Black))
                {
                    g.DrawString("Station d'arrivée", policeLégende, pinceauTexte,
                                posX + décalageTexte, posY + index * hauteurLigne);
                }
            }
            index++;

            using (Brush pinceauInter = new SolidBrush(Color.White))
            using (Pen styloNoeud = new Pen(Color.Black, 1))
            {
                g.FillEllipse(pinceauInter, posX, posY + index * hauteurLigne, 15, 15);
                g.DrawEllipse(styloNoeud, posX, posY + index * hauteurLigne, 15, 15);

                using (Brush pinceauTexte = new SolidBrush(Color.Black))
                {
                    g.DrawString("Station intermédiaire", policeLégende, pinceauTexte,
                                posX + décalageTexte, posY + index * hauteurLigne);
                }
            }
            index++;

            foreach (var ligne in listeLignes.Distinct().OrderBy(l => l))
            {
                int y = posY + index * hauteurLigne;

                Color couleurLigne = couleursLignes.ContainsKey(ligne) ? couleursLignes[ligne] : Color.Gray;
                using (Pen styloLigne = new Pen(couleurLigne, 3))
                {
                    g.DrawLine(styloLigne, posX, y + hauteurLigne / 2, posX + longueurLigne, y + hauteurLigne / 2);
                }

                using (Brush pinceauTexte = new SolidBrush(Color.Black))
                {
                    g.DrawString("Ligne " + ligne, policeLégende, pinceauTexte, posX + décalageTexte, y + hauteurLigne / 2 - 8);
                }

                index++;
            }
        }

        /// <summary>
        /// Affiche le temps de changement de ligne pour une station
        /// </summary>
        /// <param name="g">Le contexte graphique</param>
        /// <param name="noeud">La station où le changement a lieu</param>
        /// <param name="centre">Les coordonnées du centre de la station</param>
        /// <param name="ligneAvant">La ligne avant le changement</param>
        /// <param name="ligneApres">La ligne après le changement</param>
        private void AfficherTempsChangement(Graphics g, Noeud<int> noeud, PointF centre, string ligneAvant, string ligneApres)
        {
            if (ligneAvant != ligneApres)
            {
                double tempsChangement = noeud.Pondération;

                if (tempsChangement <= 0)
                    return;

                string texteChangement = $"Changement: {tempsChangement} min";

                PointF positionTexte = new PointF(
                    centre.X,
                    centre.Y - rayonNoeud - 20
                );

                SizeF tailleTexte = g.MeasureString(texteChangement, policeTemps);

                RectangleF rectFond = new RectangleF(
                    positionTexte.X - tailleTexte.Width / 2 - 4,
                    positionTexte.Y - 2,
                    tailleTexte.Width + 8,
                    tailleTexte.Height + 4
                );

                using (Brush pinceauFond = new SolidBrush(Color.FromArgb(255, 255, 255, 200)))
                using (Pen styleBordure = new Pen(Color.FromArgb(255, 255, 165, 0), 1))
                {
                    g.FillRectangle(pinceauFond, rectFond);
                    g.DrawRectangle(styleBordure, rectFond.X, rectFond.Y, rectFond.Width, rectFond.Height);
                }

                using (Brush pinceauTexte = new SolidBrush(Color.FromArgb(255, 255, 69, 0)))
                {
                    g.DrawString(texteChangement, policeTemps, pinceauTexte,
                        positionTexte.X - tailleTexte.Width / 2,
                        positionTexte.Y);
                }
            }
        }
    }
}