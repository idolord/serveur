using MessagerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class gessionnaireSocket
    {
        private static Socket clientListenSocket;

        private serverTCP srv;
        private bool isListening = false;

        public void init(serverTCP s)
        {
            outputConsoleMain.ouToScreen("Initialisation des socket d'écoute...",true);
            srv = s;
            startListening();
        }

        private void startListening()
        {
            try
            {
                //listen for clients
                clientListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientListenSocket.Bind(new IPEndPoint(IPAddress.Any, serverConf.clientPort));
                clientListenSocket.Listen(int.MaxValue);
                isListening = true;
                clientListenSocket.BeginAccept(acceptClient, null);
                outputConsoleMain.ouToScreen("Début de l'écoute client sur le port : " + serverConf.clientPort + "...",true);
                outputConsoleMain.ouToScreen("server running ...", true);
            }
            catch (System.Exception e)
            {
                outputConsoleMain.LogError(e, "startListening");
                isListening = false;
            }
        }

        private void acceptClient(IAsyncResult AR)
        {
            try
            {
                if (srv.acceptconnection)
                {
                    if (!isListening) return;
                    Socket clientSocket = clientListenSocket.EndAccept(AR);
                    clientConnection newClientRequest = new clientConnection(clientSocket, srv);
                    clientListenSocket.BeginAccept(acceptClient, clientListenSocket);
                    if (isListening)
                    {
                        srv.checkClientIpAgainstIpBanTable(newClientRequest);
                        inspectPage.addClientInfoWindow(new scClient(newClientRequest.ClientSocket.RemoteEndPoint.ToString()));

                    }
                }
            }
            catch (Exception e)
            {
                outputConsoleMain.LogError(e, "acceptClient");
            }
        }

        public void StopListening()
        {
            isListening = false;
            if (clientListenSocket != null)
            {
                clientListenSocket.Close();
                clientListenSocket = null;
            }
        }
    }
}
