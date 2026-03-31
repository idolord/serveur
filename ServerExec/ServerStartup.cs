using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerExec
{
    class ServerStartup
    {
        public static bool keepalive = true;
        static void Main()
        {
            //demarage du serveur
            serverTCP startTCP = new serverTCP();

            //garder en vie
            while (keepalive)
            {
                if (Console.ReadLine().ToLower() == "exit")
                {
                    keepalive = false;
                }
            }
        }
    }
}
