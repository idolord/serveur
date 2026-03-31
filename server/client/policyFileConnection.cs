using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    class policyFileConnection
    {
        //variable contenant le socket de la connection PFR
        private Socket cSock;

        //variable contenant la taille maximale des packet
        private int MAX_INC_DATA = 25;

        //definition de la policyfile
        private const string policyFileRequest = "<policy-file-request/>",
            policyFile =
            @"<?xml version='1.0'?>
            <cross-domain-policy>
            <allow-access-from domain=""*"" to-ports""*""/>
            </cross-domain-policy>";

        //array de bytes contenant la policyfile
        private byte[] policyFileSize = Encoding.UTF8.GetBytes(policyFileRequest);

        //methode d'initialisation
        public policyFileConnection(Socket s)
        {
            cSock = s;
            //
            Task.Factory.StartNew(handleConnection);
        }

        private void handleConnection()
        {

            byte[] message = new byte[MAX_INC_DATA];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = cSock.Receive(message, 0, message.Length, SocketFlags.None);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                
                if (bytesRead == 0)
                {
                    break;
                }

                if (compareMessage(message, bytesRead, policyFileRequest))
                {
                    outputConsoleMain.ouToScreen("recieved PFR from client: " + cSock.RemoteEndPoint.ToString(),false);
                    respondToRequest();
                }
            }
        }

        private void respondToRequest()
        {
            outputConsoleMain.ouToScreen("sending PRF handshake to client: " + cSock.RemoteEndPoint.ToString(),false);
            cSock.Send(policyFileSize, 0, policyFileSize.Length, SocketFlags.None);
        }

        //methode de comparaison du message recu avec le message attendu
        public bool compareMessage(byte[] mes, int bRead, string wMes)
        {
            string txt = Encoding.UTF8.GetString(mes, 0, bRead);
            byte[] txtArray = Encoding.UTF8.GetBytes(txt);
            byte[] compareTo = Encoding.UTF8.GetBytes(wMes);

            bool result = false;

            try
            {
                for (int i = 0; i < txt.Length; i++)
                {
                    if (txtArray[i] == compareTo[i])
                        result = true;
                    else
                        return false;
                }
            }
            catch (System.Exception e) { outputConsoleMain.ouToScreen(e.StackTrace,false); }
            return result;
        }
    }
}
