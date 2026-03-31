using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using Messager;

namespace ServerExec
{
    class clientConnection
    {
        private serverTCP srv;
        public Socket cSock;

        public clientConnection(Socket s, serverTCP sv)
        {
            srv = sv;
            cSock = s;
            ThreadPool.QueueUserWorkItem(new WaitCallback(handleConnection));
        }
        

        private void handleConnection(object state)
        {
            //affichage des nouvelles connections
            output.ouToScreen("un client c'est connecté depuis l'IP: " + cSock.RemoteEndPoint.ToString());

            //TODO: supprimer le message de test
            srv.SendClientMessage(cSock,new message("testmessage"));

            try
            {
                while (cSock.Connected)
                {
                    byte[] sizeInfo = new byte[4];

                    int byteRead = 0,
                        currentRead = 0;

                    currentRead = byteRead = cSock.Receive(sizeInfo);

                    while (byteRead < sizeInfo.Length && currentRead > 0)
                    {
                        currentRead =
                            cSock.Receive
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
                        cSock.Receive
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
                            cSock.Receive
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
                            srv.handleClientData(incObject);
                        }
                    }
                    catch {}

                }
            }
            catch{}

            output.ouToScreen("Un client c'est déconnécté depuis l'IP: " + cSock.RemoteEndPoint.ToString());
            cSock.Close();
        }
    }
}
