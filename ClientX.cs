using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LivinParis_S2
{
    public class ClientX
    {
        private int id;
        private string nom;
        private string prenom;
        private string adresse;
        private int? stationMetroProche;
        private string telephone;
        private string typeClient;
        private string entreprise;
        private string referent;
        private string email;
        private string motDePasse;

        // Constructeur par défaut nécessaire pour la sérialisation
        public ClientX()
        {
        }

        // Constructeur complet
        public ClientX(int id, string nom, string prenom, string adresse, int? stationMetroProche,
                     string telephone, string typeClient, string entreprise, string referent,
                     string email, string motDePasse)
        {
            this.id = id;
            this.nom = nom;
            this.prenom = prenom;
            this.adresse = adresse;
            this.stationMetroProche = stationMetroProche;
            this.telephone = telephone;
            this.typeClient = typeClient;
            this.entreprise = entreprise;
            this.referent = referent;
            this.email = email;
            this.motDePasse = motDePasse;
        }

        // Propriétés pour la sérialisation XML et JSON
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        [XmlElement("nom")]
        [JsonProperty("nom")]
        public string Nom
        {
            get { return nom; }
            set { nom = value; }
        }

        [XmlElement("prenom")]
        [JsonProperty("prenom")]
        public string Prenom
        {
            get { return prenom; }
            set { prenom = value; }
        }

        [XmlElement("adresse")]
        [JsonProperty("adresse")]
        public string Adresse
        {
            get { return adresse; }
            set { adresse = value; }
        }

        [XmlElement("stationMetro")]
        [JsonProperty("stationMetro")]
        public int? StationMetroProche
        {
            get { return stationMetroProche; }
            set { stationMetroProche = value; }
        }

        [XmlElement("telephone")]
        [JsonProperty("telephone")]
        public string Telephone
        {
            get { return telephone; }
            set { telephone = value; }
        }

        [XmlElement("type")]
        [JsonProperty("type")]
        public string TypeClient
        {
            get { return typeClient; }
            set { typeClient = value; }
        }

        [XmlElement("entreprise")]
        [JsonProperty("entreprise")]
        public string Entreprise
        {
            get { return entreprise; }
            set { entreprise = value; }
        }

        [XmlElement("referent")]
        [JsonProperty("referent")]
        public string Referent
        {
            get { return referent; }
            set { referent = value; }
        }

        [XmlElement("email")]
        [JsonProperty("email")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [XmlElement("motDePasse")]
        [JsonProperty("motDePasse")]
        public string MotDePasse
        {
            get { return motDePasse; }
            set { motDePasse = value; }
        }

        public override string ToString()
        {
            return $"Client {id}: {nom} {prenom}, Email: {email}";
        }
    }
}
