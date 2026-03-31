using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Messager;

namespace ServerExec
{
    class serverTCP
    {

        private static int
            clientPort = 3000, policyFilePort = 2999, MAX_INC_DATA = 1024;

        private static Socket
            policyFileListenSocket, clientListenSocket;

        public serverTCP()
        {
            try
            {
                //listen for policy File Request
                policyFileListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                policyFileListenSocket.Bind(new IPEndPoint(IPAddress.Any, policyFilePort));
                policyFileListenSocket.Listen(int.MaxValue);
                policyFileListenSocket.BeginAccept(new AsyncCallback(acceptPFR), null);
                //old sync call
                //ThreadPool.QueueUserWorkItem(new WaitCallback(listenForPFR));

                //listen for clients
                clientListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientListenSocket.Bind(new IPEndPoint(IPAddress.Any, clientPort));
                clientListenSocket.Listen(int.MaxValue);
                clientListenSocket.BeginAccept(new AsyncCallback(acceptClient), null);
                //old sync call
                //ThreadPool.QueueUserWorkItem(new WaitCallback(listenForClients));

                output.ouToScreen("Listening for PFR on :" + policyFilePort + "...");
                output.ouToScreen("Listening for client on :" + clientPort + "...");
            }
            catch { }
        }

        private void acceptPFR(IAsyncResult AR)
        {
            while (ServerStartup.keepalive)
            {
                Socket pfRequest = policyFileListenSocket.EndAccept(AR);
                policyFileListenSocket.BeginAccept(new AsyncCallback(acceptPFR), pfRequest);
                policyFileConnection newPfRequest = new policyFileConnection(pfRequest);
            }
        }

        private void acceptClient(IAsyncResult AR)
        {
            Socket cSocket = clientListenSocket.EndAccept(AR);
            clientListenSocket.BeginAccept(new AsyncCallback(acceptClient), cSocket);
            clientConnection newClientRequest = new clientConnection(cSocket, this);
        }

        public void handleClientData(message incObject)
        {
            output.ouToScreen("le client a envoyé un message");
        }

        public void SendClientMessage(Socket cSock,message message)
        {
            try
            {
                byte[] messageObject = serverTools.convertObjToByte(message);
                byte[] readyToSend = serverTools.wrapMessage(messageObject);
                cSock.Send(readyToSend);
            }
            catch {}
        }
    }
}
