using MessagerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    class clientConnection
    {
        //variable contenant une instance du serveur
        private serverTCP ServeurTCPInstance;

        internal int clientUID;

        //variable contenant le socket du client
        public Socket ClientSocket;

        //variable de ban
        public bool isBaned = false;
        public string banReason;

        //initialisation
        public clientConnection(Socket s, serverTCP sv)
        {
            ServeurTCPInstance = sv;
            ClientSocket = s;
            //on démare la gession du client
            Task.Factory.StartNew(handleConnection);
        }

        //gession des connections
        private void handleConnection()
        {
            if (!isBaned)
            {
                try
                {
                    ReceiveLoop();
                }
                catch
                {
                    CloseConnection("closed trying connection.");
                }
                CloseConnection("closed Gracefully.");
            }
            else
            {
                SendBanNotice();
                CloseConnection("connection rejeté pour cause de ban.");
            }
        }

        private void ReceiveLoop()
        {
            while (ClientSocket.Connected)
            {
                byte[] sizeInfo = new byte[4];

                int byteRead = 0,
                    currentRead = 0;

                currentRead = byteRead = ClientSocket.Receive(sizeInfo);

                while (byteRead < sizeInfo.Length && currentRead > 0)
                {
                    currentRead =
                        ClientSocket.Receive
                        (
                            sizeInfo, //cadre du message, taille du message entrant
                            byteRead, //offset du curseur dans le message
                            sizeInfo.Length - byteRead, // nombre maximum de bytes a lire
                            SocketFlags.None //pas de flag pour le socket
                        );
                    byteRead += currentRead;
                }
                // recupération de la taille du message
                int messageSize = BitConverter.ToInt32(sizeInfo, 0);

                //creation d'un array avec la taille correspondante a celle du message
                byte[] incMessage = new byte[messageSize];

                //on commence a recevoir le message
                byteRead = 0; //on reset les bytes lue pour avoir une bonne lecture des bytes lue

                currentRead =
                    byteRead =
                    ClientSocket.Receive
                    (
                        incMessage, //message entrant
                        byteRead,
                        incMessage.Length - byteRead,
                        SocketFlags.None
                    );
                //verification de la reception du message dans son integralité
                while (byteRead < messageSize && currentRead > 0)
                {
                    currentRead =
                        ClientSocket.Receive
                        (
                            incMessage,
                            byteRead,
                            incMessage.Length - byteRead,
                            SocketFlags.None
                        );
                    byteRead += currentRead;
                }
                //toutes le donnée sont recue on continue
                try
                {
                    //tentative de déserialisation du message
                    message incObject = (message)serverTools.converByteToObject(incMessage);
                    if (incObject != null)
                    {
                        //send data to handler
                        ServeurTCPInstance.handleClientData(this, incObject);
                    }
                    else
                    {
                        outputConsoleMain.ouToScreen("message format not recognised", false);
                    }
                }
                catch
                {
                    CloseConnection("closed trying sending packet.");
                    break;
                }
            }
        }

        private void CloseConnection(string reason)
        {
            outputConsoleMain.ouToScreenNormal(reason);
            if (ClientSocket != null && ClientSocket.Connected)
                ClientSocket.Close();
        }

        private void SendBanNotice()
        {
            message ban_notice = new message("Ban_Notice");
            scObject ban = new scObject("ban");
            ban.addScString("Ban_Reason", banReason);
            ban_notice.addScObject(ban);
            ServeurTCPInstance.SendClientMessage(ClientSocket, ban_notice);
            outputConsoleMain.ouToScreen(ClientSocket.RemoteEndPoint.ToString() + " connection rejeté pour cause de ban.", false);
            outputConsoleMain.ouToScreen("raison du ban: " + banReason, false);
        }
    }
}
