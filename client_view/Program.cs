using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;

namespace view
{
    class Program
    {

        static void Main(string[] args)
        {
            outputConsoleMain.init();
            if (args[0] != null)
            {
                Console.Title = args[0]; 
            }
            viewHandler VH = new viewHandler(args[0]);
            
            Console.ReadLine();
        }
    }
}
