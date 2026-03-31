using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MessagerLib;
using MySql.Data.MySqlClient.Memcached;
using System.Collections.Concurrent;

namespace server
{

    delegate void setter(int i, int j);
    delegate List<clientConnection> ClientGetter();

    // Interface interne, non exposée publiquement
    internal interface IServerContext
    {
        void checkClientIpAgainstIpBanTable(clientConnection client);
    }

    class serverTCP : IServerContext
    {
        public bool acceptconnection = false;
        public bool cleaned;

        //variable contenant la banlist
        public List<string[]> banedIPs = new List<string[]>();

        //pointeur vers le gessionaire de socket
        private gessionnaireSocket serverSocketManager = new gessionnaireSocket();

        //variable contenant une liste des clients en connection entrante
        static private ConcurrentBag<clientConnection> clients;

        private readonly Dictionary<string, Action<clientConnection, message>> messageHandlers;

        static public ClientGetter GetClientInfoReq()
        {
            return new ClientGetter(() => clients.ToList());
        }

        private static ConcurrentBag<clientConnection> Getclients()
        {
            return clients;
        }

        //main method
        public serverTCP()
        {
            clients = new ConcurrentBag<clientConnection>();
            outputConsoleMain.consoleHeader("resolving des configs du serveur");
            outputConsoleMain.consoleWSpacer();
            outputConsoleMain.ouToScreen(outputConsoleMain.consoleSpacerTop(), true);
            serverConfigHandler.resolveConfig();

            outputConsoleMain.consoleHeader("communications internes.");
            outputConsoleMain.consoleWSpacer();
            outputConsoleMain.ouToScreen(outputConsoleMain.consoleSpacerTop(), true);

            outputConsoleMain.ouToScreen("tentative de connection a la DB ...", true);

            if (DBHandler.isBDReachable() == true)
            {
                outputConsoleMain.ouToScreen("Récupération de la liste de bans ...", true);

                banedIPs = DBHandler.retreiveBanData();

                outputConsoleMain.consoleHeader("communications externes.");
                outputConsoleMain.consoleWSpacer();
                outputConsoleMain.ouToScreen(outputConsoleMain.consoleSpacerTop(), true);
                serverSocketManager.init(this);
                acceptconnection = true;
                outputConsoleMain.ouToScreen(outputConsoleMain.consoleSpacerBot(), true);
            }
            else
            {
                outputConsoleMain.ouToScreen(outputConsoleMain.consoleSpacerBot(), true);
            }

            messageHandlers = new Dictionary<string, Action<clientConnection, message>>(StringComparer.OrdinalIgnoreCase)
            {
                { "request login", HandleLoginRequest },
                { "request register", HandleRegisterRequest },
                { "request locals", HandleLocalRequest },
                { "create character", HandleCreateCharRequest },
                { "delete character", HandleDeleteCharRequest },
                { "play character", HandlePlayCharRequest }
            };
        }

        //methode de gession des infos des clients
        public void handleClientData(clientConnection cl, message incObject)
        {
            Task.Factory.StartNew(() => { directMessage(cl, incObject); });
        }

        private void directMessage(clientConnection cl, message incObject)
        {
            Debug.WriteLine(incObject.messageText);
            if (messageHandlers.TryGetValue(incObject.messageText, out var handler))
            {
                handler(cl, incObject);
            }
            else
            {
                Debug.WriteLine("message non reconnu");
            }
        }

        private void HandleLoginRequest(clientConnection cl, message incObject)
        {
            try
            {
                // Extraction des identifiants de connexion
                scCred credentials = incObject.GetScObject("cred").GetCred();

                // Vérification des identifiants via DBHandler (sécurisé, paramétré)
                bool isValid = DBHandler.doIdCorrespond(credentials);

                // Préparation de la réponse
                scResLogin response = new scResLogin(isValid);

                if (isValid)
                {
                    // Récupération des personnages de l'utilisateur
                    List<scCharacter> userChars = DBHandler.GetUserInfo(incObject);

                    foreach (scCharacter character in userChars)
                    {
                        if (character.CharId == 0)
                        {
                            cl.clientUID = character.CharUserId;
                        }
                        else
                        {
                            response.GetScObject("response").addScCharacter(character);
                        }
                    }

                    if (cl.clientUID == 0)
                    {
                        // recupération de l'ID utilisateur si non défini
                        cl.clientUID = DBHandler.GetUserIdFromCredentials(credentials);
                    }
                }

                // Envoi de la réponse au client
                SendClientMessage(cl.ClientSocket, response);
            }
            catch (Exception ex)
            {
                outputConsoleMain.LogError(ex, "HandleLoginRequest");
                // Envoi d'une réponse d'échec en cas d'erreur
                SendClientMessage(cl.ClientSocket, new scResLogin(false));
            }
        }

        private void HandleRegisterRequest(clientConnection cl, message incObject)
        {
            bool tempvalide = DBHandler.isidfree(incObject);
            scResRegister mes = new scResRegister(tempvalide);
            if (tempvalide)
            {
                DBHandler.register(incObject);
            }
            SendClientMessage(cl.ClientSocket, mes);
        }

        private void HandleLocalRequest(clientConnection cl, message incObject)
        {
            List<scMenuPanel> scMenuPanels = DBHandler.retreiveMenuInfo(incObject.GetScObject("Langue").GetString("Local"));
            scMesResLocal mes = new scMesResLocal(scMenuPanels);
            SendClientMessage(cl.ClientSocket, mes);
        }

        private void HandleCreateCharRequest(clientConnection cl, message incObject)
        {
            try
            {
                // Extraction du personnage à créer depuis le message
                scCharacter newChar = incObject.GetScObject("char").GetCharacter().FirstOrDefault();
                if (newChar == null)
                {
                    outputConsoleMain.ouToScreen("Aucun personnage à créer n'a été trouvé dans la requête.", true);
                    SendClientMessage(cl.ClientSocket, new scResRelList(new List<scCharacter>()));
                    return;
                }

                // Tentative de création du personnage
                bool creationSuccess = DBHandler.tryCreateChar(newChar, cl.clientUID);

                // Récupération et filtrage des personnages de l'utilisateur
                List<scCharacter> filteredChars = GetUserPlayableCharacters(cl.clientUID);

                // Construction et envoi de la réponse
                scResRelList response = new scResRelList(filteredChars);
                SendClientMessage(cl.ClientSocket, response);

                if (!creationSuccess)
                    outputConsoleMain.ouToScreen("La création du personnage a échoué (nom déjà utilisé ?).", true);
            }
            catch (Exception ex)
            {
                outputConsoleMain.LogError(ex, "HandleCreateCharRequest");
                SendClientMessage(cl.ClientSocket, new scResRelList(new List<scCharacter>()));
            }
        }

        /// <summary>
        /// Récupère et filtre la liste des personnages jouables pour un utilisateur donné.
        /// </summary>
        private List<scCharacter> GetUserPlayableCharacters(int userId)
        {
            return DBHandler.GetUserInfoUid(userId)
                .Where(c => c.CharId != 0)
                .ToList();
        }

        private void HandleDeleteCharRequest(clientConnection cl, message incObject)
        {
            try
            {
                // Extraction du personnage à supprimer depuis le message
                scCharacter charToDelete = incObject.GetScObject("char").GetCharacter().FirstOrDefault();
                if (charToDelete == null)
                {
                    outputConsoleMain.ouToScreen("Aucun personnage à supprimer n'a été trouvé dans la requête.", true);
                    SendClientMessage(cl.ClientSocket, new scResRelList(new List<scCharacter>()));
                    return;
                }

                // Suppression du personnage
                bool deleteSuccess = DBHandler.tryDeleteChar(charToDelete, cl.clientUID);

                // Récupération de la liste des personnages restants de l'utilisateur
                List<scCharacter> userChars = DBHandler.GetUserInfoUid(cl.clientUID)
                    .Where(c => c.CharId != 0)
                    .ToList();

                // Construction de la réponse
                scResRelList response = new scResRelList(userChars);

                // Envoi de la réponse au client
                SendClientMessage(cl.ClientSocket, response);

                // Log optionnel
                if (!deleteSuccess)
                    outputConsoleMain.ouToScreen("La suppression du personnage a échoué.", true);
            }
            catch (Exception ex)
            {
                outputConsoleMain.LogError(ex, "HandleDeleteCharRequest");
                // Envoi d'une liste vide en cas d'erreur pour éviter le blocage côté client
                SendClientMessage(cl.ClientSocket, new scResRelList(new List<scCharacter>()));
            }
        }

        private void HandlePlayCharRequest(clientConnection cl, message incObject)
        {
            outputConsoleMain.ouToScreen("demande de connection d'un client", true);
            scCharacter ch = incObject.GetScObject("char").GetCharacter()[0];
            scLocation loc = DBHandler.tryPlayChar(ch, cl.clientUID);
            scMap map = DBHandler.GetMapInfo(loc);
            Debug.WriteLine("map size : " + map.mapSize + " map type : " + map.mapType);
            foreach (scTile item in map.getTiles())
            {
                Debug.WriteLine("tile type : " + item.tiletype + " tile x : " + item.tilex + " tile y : " + item.tiley);
            }
            scResPlayChar mes = new scResPlayChar(map, loc);
            SendClientMessage(cl.ClientSocket, mes);
        }

        //methode pour l'envoie des infos au clients
        public void SendClientMessage(Socket cSock, message message)
        {
            try
            {
                byte[] messageObject = serverTools.convertObjToByte(message);
                byte[] readyToSend = serverTools.wrapMessage(messageObject);
                cSock.Send(readyToSend);
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "sendClientMessage");
            }
        }

        //verification de l'ip client par rapport a la table de ban
        public void checkClientIpAgainstIpBanTable(clientConnection client)
        {
            //verification de l'ip
            checkAginstIpBanlist(client);
            scMesHandcheck mes;
            if (client.isBaned)
            {
                //creation du message
                mes = new scMesHandcheck(true, client.banReason);
            }
            else
            {
                mes = new scMesHandcheck(false, client.banReason);
                clients.Add(client);
            }
            SendClientMessage(client.ClientSocket, mes);
        }

        //methode retournant "vrais" si le client est ban
        private void checkAginstIpBanlist(clientConnection client)
        {
            foreach (string[] bans in banedIPs)
            {
                string Ip = splitIp(client.ClientSocket.RemoteEndPoint.ToString());
                if (splitIp(bans[0]) == Ip)
                {
                    client.isBaned = true;
                    client.banReason = bans[1];
                }
            }
        }

        //methode de commande console pour ban un client
        public void banclient(clientConnection client, string reason)
        {
            DBHandler.createBanIp(new string[2] { client.ClientSocket.RemoteEndPoint.ToString(), reason });
        }

        //coupe le "string" de l'ip (d'un coté l'ip et de l'autre le port) et retourne l'ip (utile pour verifier les ban)
        private string splitIp(string s)
        {
            string[] temp = s.Split(':');
            string toReturn = temp[0];
            return toReturn;
        }

        internal void closeAllConections()
        {
            acceptconnection = false;
            foreach (var client in clients)
            {
                if (client.ClientSocket != null && client.ClientSocket.Connected)
                {
                    client.ClientSocket.Close();
                }
            }
            cleaned = true;
        }
    }

    static class infoscreen
    {
        public static List<scClient> clients { get; private set; }

        static private List<scClient> infoRequest()
        {
            ClientGetter getter = serverTCP.GetClientInfoReq();
            List<scClient> temp = new List<scClient>();
            List<clientConnection> tempco = getter();
            for (int i = 0; i < tempco.Count; i++)
            {
                temp.Add(new scClient(tempco[i].ClientSocket.RemoteEndPoint.ToString()));
            }
            return temp;
        }

        static public List<scClient> checkinfo()
        {
            var infoenswer = infoRequest();
            return infoenswer;
        }
    }

    //class contener des configs du serveur
    static class serverConf
    {
        //variables de port
        public static int clientPort { get; private set; }
        public static int policyFilePort { get; private set; }

        //délégué pour avoir les ports
        static public setter getPortDelegate()
        {
            return new setter(setPort);
        }

        //methode pour regler les ports
        static void setPort(int i, int j)
        {
            outputConsoleMain.ouToScreen("récupération des ports.", true);
            policyFilePort = i;
            clientPort = j;
        }
    }
}
