using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Threading;
using MessagerLib;

namespace server
{

    class viewHandler
    {

        delegate void senderDelegate(message m);

        NamedPipeServerStream servpipe;
        string apppath = AppDomain.CurrentDomain.BaseDirectory;
        string exepath;
        string domain;
        List<scClient> clients;
        bool isconnected;


        public viewHandler(string calledDomain)
        {
            domain = calledDomain;
            exepath = apppath + "view/view.exe";
        }

        public void startview(List<scClient> infos)
        {
            clients = infos;
            servpipe = new NamedPipeServerStream(domain, PipeDirection.Out);
            Process.Start(exepath, domain);
            servpipe.WaitForConnection();
            sendViewData(servpipe);
        }

      

        private void sendViewData(NamedPipeServerStream s)
        {
            switch (domain)
            {
                case "client_view":
                    message mes = new message("clients info");
                    scObject capsule = new scObject("capsule");
                    clients = infoscreen.checkinfo();
                    for (int i = 0; i < clients.Count; i++)
                    {
                        capsule.addScClients(clients[i]);
                    }
                    mes.addScObject(capsule);
                    sendMessagePiped(mes);
                    break;
                default:
                    break;
            }

        }

        public void sendViewDataUpdate(scClient cli)
        {
                switch (domain)
                {
                    case "client_view":
                        message mes = new message("clients info update");
                        scObject capsule = new scObject("capsule");
                        capsule.addScClients(cli);
                        mes.addScObject(capsule);
                        sendMessagePiped(mes);
                        break;
                    default:
                        break;
                }
        }

        public void sendMessagePiped(message s)
        {
            try
            {
                if ((servpipe != null) && (servpipe.IsConnected))
                {
                    byte[] messagefullbyte = serverTools.convertObjToByte(s);
                    byte[] rdyToSend = serverTools.wrapMessage(messagefullbyte);
                    servpipe.Write(rdyToSend, 0, rdyToSend.Length);
                    servpipe.Flush();
                    servpipe.WaitForPipeDrain();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
