

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace ETAPE2
{
    /// <summary>
    /// Cette classe permet de créer une représentation visuelle du graphe complet du réseau de métro
    /// </summary>
    public class RepresentationGraphique
    {
        /// <summary>
        /// Le graphe contenant toutes les stations et les liens du réseau de métro
        /// </summary>
        private Graphe graphe;

        /// <summary>
        /// Dictionnaire qui associe chaque ligne de métro à une couleur spécifique
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
        /// Rayon des cercles représentant les stations
        /// </summary>
        private float rayonNoeud = 15;

        /// <summary>
        /// Police utilisée pour les noms des stations
        /// </summary>
        private Font policeStation;

        /// <summary>
        /// Police utilisée pour la légende
        /// </summary>
        private Font policeLégende;

        /// <summary>
        /// Constructeur de la classe RepresentationGraphe
        /// </summary>
        /// <param name="graphe">Le graphe contenant toutes les stations et liens</param>
        /// <param name="largeur">Largeur de l'image en pixels (6000 par défaut)</param>
        /// <param name="hauteur">Hauteur de l'image en pixels (4000 par défaut)</param>
        public RepresentationGraphique(Graphe graphe, int largeur = 4500, int hauteur = 3000)
        {
            this.graphe = graphe;
            this.largeur = largeur;
            this.hauteur = hauteur;
            this.policeStation = new Font("Arial", 8, FontStyle.Bold);
            this.policeLégende = new Font("Arial", 10, FontStyle.Bold);
            InitialiserCouleurs();
            CalculerLimites();
        }

        /// <summary>
        /// Initialise les couleurs pour chaque ligne de métro
        /// </summary>
        private void InitialiserCouleurs()
        {
            /// Création du dictionnaire avec les couleurs des lignes de métro
            couleursLignes = new Dictionary<string, Color>
        {
            { "1", Color.FromArgb(255, 255, 168, 0) },    /// Jaune pour la ligne 1
            { "2", Color.FromArgb(255, 0, 0, 255) },      /// Bleu pour la ligne 2
            { "3", Color.FromArgb(255, 152, 251, 152) },  /// Vert clair pour la ligne 3
            { "30", Color.FromArgb(255, 137, 207, 240) }, /// Bleu clair pour la ligne 3bis
            { "4", Color.FromArgb(255, 128, 0, 128) },    /// Violet pour la ligne 4
            { "5", Color.FromArgb(255, 255, 165, 0) },    /// Orange pour la ligne 5
            { "6", Color.FromArgb(255, 0, 128, 0) },      /// Vert pour la ligne 6
            { "7", Color.FromArgb(255, 255, 192, 203) },  /// Rose pour la ligne 7
            { "70", Color.FromArgb(255, 173, 216, 230) }, /// Bleu clair pour la ligne 7bis
            { "8", Color.FromArgb(255, 255, 0, 0) },      /// Rouge pour la ligne 8
            { "9", Color.FromArgb(255, 255, 215, 0) },    /// Or pour la ligne 9
            { "10", Color.FromArgb(255, 210, 180, 140) }, /// Beige pour la ligne 10
            { "11", Color.FromArgb(255, 128, 0, 0) },     /// Marron pour la ligne 11
            { "12", Color.FromArgb(255, 0, 128, 128) },   /// Turquoise pour la ligne 12
            { "13", Color.FromArgb(255, 0, 255, 0) },     /// Vert fluo pour la ligne 13
            { "14", Color.FromArgb(255, 75, 0, 130) }     /// Indigo pour la ligne 14
        };

            /// Pour les lignes qui ne sont pas dans notre liste prédéfinie, on génère une couleur aléatoire
            var toutesLesLignes = graphe.Liens.Select(l => l.Ligne).Distinct();
            foreach (var ligne in toutesLesLignes)
            {
                if (!couleursLignes.ContainsKey(ligne))
                {
                    Random rnd = new Random(ligne.GetHashCode());
                    couleursLignes[ligne] = Color.FromArgb(255, rnd.Next(256), rnd.Next(256), rnd.Next(256));
                }
            }
        }

        /// <summary>
        /// Calcule les limites géographiques du graphe pour adapter l'affichage
        /// </summary>
        private void CalculerLimites()
        {
            /// Trouve les valeurs min et max de latitude et longitude pour cadrer la carte
            latMin = graphe.Noeuds.Min(n => n.latitude);
            latMax = graphe.Noeuds.Max(n => n.latitude);
            longMin = graphe.Noeuds.Min(n => n.longitude);
            longMax = graphe.Noeuds.Max(n => n.longitude);
        }

        /// <summary>
        /// Convertit les coordonnées GPS en coordonnées pixels pour l'affichage
        /// </summary>
        /// <param name="lat">Latitude de la station</param>
        /// <param name="lon">Longitude de la station</param>
        /// <returns>Les coordonnées en pixels sur l'image</returns>
        private PointF ConvertirCoordonnéesEnPixels(float lat, float lon)
        {
            /// Conversion des coordonnées GPS en pixels
            float x = ((lon - longMin) / (longMax - longMin)) * (largeur - 2 * marge) + marge;
            /// On inverse l'axe Y car la latitude augmente vers le nord mais les coordonnées Y augmentent vers le bas
            float y = (1 - ((lat - latMin) / (latMax - latMin))) * (hauteur - 2 * marge) + marge;
            return new PointF(x, y);
        }

        /// <summary>
        /// Vérifie si un lien est à sens unique
        /// </summary>
        /// <param name="départ">Noeud de départ</param>
        /// <param name="arrivée">Noeud d'arrivée</param>
        /// <returns>Vrai si le lien est à sens unique, faux sinon</returns>
        private bool EstLienSensUnique(Noeud<int> départ, Noeud<int> arrivée)
        {
            /// Vérifie s'il existe un lien de 'arrivée' vers 'départ'
            /// Si ce lien n'existe pas, alors le lien est à sens unique
            return !graphe.Liens.Any(l => l.Depart == arrivée && l.Arrivee == départ);
        }

        /// <summary>
        /// Dessine une flèche pour indiquer le sens de circulation
        /// </summary>
        /// <param name="g">Le contexte graphique</param>
        /// <param name="stylo">Le stylo utilisé pour dessiner</param>
        /// <param name="début">Point de départ de la flèche</param>
        /// <param name="fin">Point d'arrivée de la flèche</param>
        /// <param name="tailleFlèche">Taille de la pointe de la flèche</param>
        private void DessinerFlèche(Graphics g, Pen stylo, PointF début, PointF fin, float tailleFlèche)
        {
            /// Calcul du vecteur de direction
            float dx = fin.X - début.X;
            float dy = fin.Y - début.Y;

            /// Normalisation du vecteur de direction
            float longueur = (float)Math.Sqrt(dx * dx + dy * dy);
            dx /= longueur;
            dy /= longueur;

            /// Calcul des points de la pointe de flèche
            PointF pointFlèche1 = new PointF(
                fin.X - tailleFlèche * (dx + dy / 2),
                fin.Y - tailleFlèche * (dy - dx / 2)
            );

            PointF pointFlèche2 = new PointF(
                fin.X - tailleFlèche * (dx - dy / 2),
                fin.Y - tailleFlèche * (dy + dx / 2)
            );

            /// Dessin de la pointe de flèche
            g.DrawLine(stylo, fin, pointFlèche1);
            g.DrawLine(stylo, fin, pointFlèche2);
        }

        /// <summary>
        /// Dessine le graphe complet sur une image et l'enregistre
        /// </summary>
        /// <param name="cheminFichier">Chemin où enregistrer l'image</param>
        public void DessinerGraphe(string cheminFichier)
        {
            using (Bitmap image = new Bitmap(largeur, hauteur))
            using (Graphics g = Graphics.FromImage(image))
            {
                /// Configuration pour une meilleure qualité d'image
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                /// Remplissage du fond en blanc
                g.Clear(Color.White);

                /// Dessin des liens d'abord (pour qu'ils apparaissent derrière les noeuds)
                foreach (var lien in graphe.Liens)
                {
                    PointF pointDépart = ConvertirCoordonnéesEnPixels(lien.Depart.latitude, lien.Depart.longitude);
                    PointF pointArrivée = ConvertirCoordonnéesEnPixels(lien.Arrivee.latitude, lien.Arrivee.longitude);

                    /// Ajustement des points de départ et d'arrivée pour qu'ils soient sur le bord des cercles
                    float dx = pointArrivée.X - pointDépart.X;
                    float dy = pointArrivée.Y - pointDépart.Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (distance > 0)
                    {
                        /// Ajustement des points de départ et d'arrivée pour qu'ils soient sur le bord des cercles
                        PointF départAjusté = new PointF(
                            pointDépart.X + (dx / distance) * rayonNoeud,
                            pointDépart.Y + (dy / distance) * rayonNoeud
                        );

                        PointF arrivéeAjustée = new PointF(
                            pointArrivée.X - (dx / distance) * rayonNoeud,
                            pointArrivée.Y - (dy / distance) * rayonNoeud
                        );

                        /// Récupération de la couleur pour cette ligne
                        Color couleurLigne = couleursLignes.ContainsKey(lien.Ligne) ?
                            couleursLignes[lien.Ligne] : Color.Gray;

                        using (Pen styloLigne = new Pen(couleurLigne, 2))
                        {
                            g.DrawLine(styloLigne, départAjusté, arrivéeAjustée);

                            /// Si c'est un lien à sens unique, on dessine une flèche
                            if (EstLienSensUnique(lien.Depart, lien.Arrivee))
                            {
                                DessinerFlèche(g, styloLigne, départAjusté, arrivéeAjustée, 10);
                            }
                        }
                    }
                }

                /// Dessin des noeuds
                foreach (var noeud in graphe.Noeuds)
                {
                    PointF centre = ConvertirCoordonnéesEnPixels(noeud.latitude, noeud.longitude);

                    /// Dessin du cercle de la station
                    using (Brush pinceauNoeud = new SolidBrush(Color.White))
                    using (Pen styloNoeud = new Pen(Color.Black, 2))
                    {
                        g.FillEllipse(pinceauNoeud, centre.X - rayonNoeud, centre.Y - rayonNoeud,
                                        rayonNoeud * 2, rayonNoeud * 2);
                        g.DrawEllipse(styloNoeud, centre.X - rayonNoeud, centre.Y - rayonNoeud,
                                        rayonNoeud * 2, rayonNoeud * 2);
                    }

                    /// Dessin du nom de la station
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

                    using (Brush pinceauTexte = new SolidBrush(Color.Black))
                    {
                        g.DrawString(nomStation, policeStation, pinceauTexte, positionTexte);
                    }
                }

                /// Dessin de la légende
                DessinerLégende(g);

                /// Enregistrement de l'image
                image.Save(cheminFichier);
                Console.WriteLine($"Visualisation du graphe enregistrée dans {cheminFichier}");
            }
        }

        /// <summary>
        /// Dessine la légende sur l'image
        /// </summary>
        /// <param name="g">Le contexte graphique</param>
        private void DessinerLégende(Graphics g)
        {
            int posX = largeur - 200;
            int posY = 50;
            int hauteurLigne = 25;
            int longueurLigne = 40;
            int décalageTexte = 50;

            /// Dessin du titre de la légende
            using (Brush pinceauTitre = new SolidBrush(Color.Black))
            {
                g.DrawString("Lignes de Métro", policeLégende, pinceauTitre, posX, posY - 30);
            }

            /// Dessin du fond de la légende
            int hauteurLégende = couleursLignes.Count * hauteurLigne + 20;
            using (Brush pinceauLégende = new SolidBrush(Color.FromArgb(240, 240, 240)))
            using (Pen styloLégende = new Pen(Color.Black, 1))
            {
                g.FillRectangle(pinceauLégende, posX - 10, posY - 10, 170, hauteurLégende);
                g.DrawRectangle(styloLégende, posX - 10, posY - 10, 170, hauteurLégende);
            }

            /// Dessin de chaque ligne dans la légende
            int index = 0;
            foreach (var ligne in couleursLignes.OrderBy(l => l.Key))
            {
                int y = posY + index * hauteurLigne;

                /// Dessin de l'échantillon de ligne
                using (Pen styloLigne = new Pen(ligne.Value, 2))
                {
                    g.DrawLine(styloLigne, posX, y + hauteurLigne / 2, posX + longueurLigne, y + hauteurLigne / 2);
                }

                /// Dessin du libellé de la ligne
                using (Brush pinceauTexte = new SolidBrush(Color.Black))
                {
                    g.DrawString("Ligne " + ligne.Key, policeLégende, pinceauTexte, posX + décalageTexte, y + hauteurLigne / 2 - 8);
                }

                index++;
            }
        }
    }
}