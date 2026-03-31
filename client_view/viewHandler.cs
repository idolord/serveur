using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.Threading;
using MessagerLib;
using System.IO;

namespace view
{
    class viewHandler
    {
        NamedPipeClientStream clientpipe;
        string domain;

        public viewHandler(string s)
        {
            domain = s;
            clientpipe = new NamedPipeClientStream(".", domain, PipeDirection.In);
            startComunication();
        }

        private void startComunication()
        {
            outputConsoleMain.consoleHeader(domain);
            clientpipe.Connect();
            StartListening();
        }

        private void StartListening()
        {
            while (clientpipe.IsConnected)
            {
                try
                {

                    byte[] lenght = new byte[4];
                    clientpipe.Read(lenght, 0, lenght.Length);
                    int messagesize = BitConverter.ToInt32(lenght, 0);
                    if (messagesize > 0)
                    {
                        byte[] messagearray = new byte[messagesize];
                        clientpipe.Read(messagearray, 0, messagearray.Length);
                        try
                        {
                            message mes = (message)serverTools.converByteToObject(messagearray);
                            List<scClient> clients = mes.GetScObject("capsule").GetClients();
                            for (int i = 0; i < clients.Count; i++)
                            {
                                outputConsoleMain.ouToScreen(clients[i].Ip);
                            }
                        }
                        catch
                        {

                            Console.Read();
                        } 
                    }
                }
                catch (Exception)
                {

                    Console.Read();
                    throw;
                }
                Console.Read();
            }
        }


    }
}
