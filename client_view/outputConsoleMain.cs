using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace view
{
    static class outputConsoleMain
    {

        public static void init()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.BufferHeight = 1024;
            Console.BufferWidth = 120;
            Console.SetWindowSize(120, 50);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
        }


        public static void colorizeheader(string s)
        {
            int spacer = (int)Math.Floor((double)54 - (s.Length / 2));
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            sb.Append("╟");
            for (int i = 0; i < spacer; i++)
            {
                sb.Append("─");
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(sb.ToString());
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            sb.Append("◄  " + s + "  ►");
            Console.Write("◄  " + s + "  ►");
            Console.ForegroundColor = ConsoleColor.Cyan;
            for (int i = 0; i < spacer; i++)
            {
                sb2.Append("─");
            }
            int temp = sb.Length + sb2.Length;
            if (temp <= 118)
            {
                for (int i = temp; i <= 118; i++)
                {
                    sb2.Append("─");
                }
            }
            sb2.Append("╢");
            Console.Write(sb2.ToString());
            Console.ForegroundColor = ConsoleColor.White;
        }


        public static void colorizeline(string t)
        {
            string temp = t;
            if (temp.Length < 120)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(temp);

                for (int i = 0; i < 120; i++)
                {
                    if (i > temp.Length)
                    {
                        sb.Append(" ");
                    }
                }
                sb.AppendLine();
                temp = sb.ToString();
                sb.Clear();
            }

            for (int i = 0; i < temp.Length; i++)
            {
                if (i < Math.Floor((float)(temp.Length) / 3))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(temp[i]);
                }
                else if (i < (Math.Floor((float)(temp.Length) / 3)) * 2)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(temp[i]);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(temp[i]);
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static string center(string t)
        {
            string temp;
            if (t.Length < 120)
            {
                int spaceToAdd = (int)Math.Floor((double)(120 - t.Length) / 2);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < spaceToAdd; i++)
                {
                    sb.Append(" ");
                }
                sb.Append(t);
                temp = sb.ToString();
                sb.Clear();
                return temp;
            }
            else
            {
                return t;
            }
        }

        public static void ouToScreen(string t)
        {

            colorizeline(t);

        }

        public static void consoleHeader(string s)
        {
            colorizeline("╔══════════════════════════════════════════════════════════════════════════════════════════════════════════════════════╗");
            colorizeheader(s);
            colorizeline("╚══════════════════════════════════════════════════════════════════════════════════════════════════════════════════════╝");
        }

        public static void consoleWSpacer()
        {
            colorizeline(" ");
        }
    }
}
