using MessagerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    class executable
    {
        private static Thread MainConsoleThread;
        private static bool keepalive = true;
        public static serverTCP startTCP;

        static void Main(string[] args)
        {
            MainConsoleThread = new Thread(new ThreadStart(MainConsole));
            MainConsoleThread.Start();
        }

        private static void MainConsole()
        {
            Console.Title = "main_view";
            outputConsoleMain.init();
            outputConsoleMain.consoleHeader("lancement du serveur");
            outputConsoleMain.consoleWSpacer();
            startTCP = new serverTCP();
            Console.CursorTop = 47;

            while (!startTCP.cleaned)
            {
                outputConsoleMain.resetConsole();
                Console.CursorTop = 47;
                outputConsoleMain.ouToScreenNormal("");
                while (keepalive)
                {
                    outputConsoleMain.ouToScreenNormal("enter 'help' for help or 'exit' or 'x' to close");
                    string commanOperator = Console.ReadLine();
                    if (!ProcessMainCommand(commanOperator))
                        break;
                }
                startTCP.closeAllConections();
            }
        }

        /// <summary>
        /// Traite les commandes principales de la console.
        /// Retourne false si la boucle principale doit s'arrêter.
        /// </summary>
        private static bool ProcessMainCommand(string command)
        {
            switch (command)
            {
                case "exit":
                case "x":
                    keepalive = false;
                    return false;
                case "info":
                    inspectPage.startClientInfoWindow();
                    outputConsoleMain.resetConsole();
                    Console.CursorTop = 47;
                    outputConsoleMain.ouToScreenNormal("");
                    outputConsoleMain.ouToScreenNormal("client info page opened");
                    break;
                case "clr":
                    outputConsoleMain.resetConsole();
                    Console.CursorTop = 48;
                    outputConsoleMain.ouToScreenNormal("");
                    break;
                case "help":
                    ShowHelp();
                    break;
                default:
                    ProcessComplexCommand(command);
                    break;
            }
            return true;
        }

        /// <summary>
        /// Affiche l'aide de la console.
        /// </summary>
        private static void ShowHelp()
        {
            outputConsoleMain.resetConsole();
            Console.CursorTop = 44;
            outputConsoleMain.ouToScreenNormal("");
            outputConsoleMain.ouToScreenNormal("'info' pour voir la page des informations sur les clients connectés.");
            outputConsoleMain.ouToScreenNormal("'ban' ou 'delban' pour dé/banir.");
            outputConsoleMain.ouToScreenNormal("'clr' pour rafraichir la console.");
        }

        /// <summary>
        /// Traite les commandes complexes (ban, delban, etc.).
        /// </summary>
        private static void ProcessComplexCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            string[] args = input.Split(' ');
            string command = args[0].ToLowerInvariant();

            switch (command)
            {
                case "ban":
                    HandleBanCommand(args);
                    break;
                case "delban":
                    HandleDelBanCommand(args);
                    break;
                default:
                    outputConsoleMain.resetConsole();
                    Console.CursorTop = 46;
                    Console.ForegroundColor = ConsoleColor.Red;
                    outputConsoleMain.ouToScreenNormal("commande non reconnue : " + command);
                    Console.ForegroundColor = ConsoleColor.White;
                    outputConsoleMain.ouToScreenNormal("'help' pour obtenir de l'aide");
                    break;
            }
        }

        private static void HandleBanCommand(string[] args)
        {
            if (args.Length > 2)
            {
                string ip = args[1];
                string reason = string.Join(" ", args.Skip(2)) + ".";
                DBHandler.createBanIp(new string[] { ip, reason });
                outputConsoleMain.resetConsole();
                Console.CursorTop = 47;
                outputConsoleMain.ouToScreen(ip + " Ban effectif pour raison: " + reason, false);
                startTCP.banedIPs = DBHandler.retreiveBanData();
            }
            else
            {
                outputConsoleMain.resetConsole();
                Console.CursorTop = 46;
                Console.ForegroundColor = ConsoleColor.Red;
                outputConsoleMain.ouToScreenNormal("la syntaxe de la commande n'a pas été reconnue.");
                Console.ForegroundColor = ConsoleColor.White;
                outputConsoleMain.ouToScreenNormal("syntaxe attendue : ban IP raison(en plusieurs mots)");
            }
        }

        private static void HandleDelBanCommand(string[] args)
        {
            if (args.Length != 2)
            {
                outputConsoleMain.resetConsole();
                Console.CursorTop = 46;
                Console.ForegroundColor = ConsoleColor.Red;
                outputConsoleMain.ouToScreenNormal("la syntaxe de la commande n'a pas été reconnue.");
                Console.ForegroundColor = ConsoleColor.White;
                outputConsoleMain.ouToScreenNormal("syntaxe attendue : delban IP(format *.*.*.*)");
            }
            else
            {
                string ip = args[1];
                outputConsoleMain.resetConsole();
                DBHandler.DelBan(ip);
                Console.CursorTop = 47;
                outputConsoleMain.resetConsole();
                outputConsoleMain.ouToScreen("Client : " + ip + " débannis.", false);
                startTCP.banedIPs = DBHandler.retreiveBanData();
            }
        }
    }
}
