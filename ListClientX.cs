using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LivinParis_S2
{
    [XmlRoot("clients")]
    public class ClientListX
    {
        private List<ClientX> clients;

        public ClientListX()
        {
            clients = new List<ClientX>();
        }

        [XmlElement("client")]
        [JsonProperty("clients")]
        public List<ClientX> Clients
        {
            get { return clients; }
            set { clients = value; }
        }

        public void Ajouter(ClientX client)
        {
            clients.Add(client);
        }

        public int NombreClients()
        {
            return clients.Count;
        }

        public override string ToString()
        {
            string resultat = "Liste des clients : \n";
            foreach (ClientX client in clients)
            {
                resultat += "\t" + client.ToString();
                resultat += "\n";
            }
            return resultat;
        }
    }
}
