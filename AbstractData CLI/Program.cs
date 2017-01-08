using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractData;

namespace AbstractData_CLI
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("AbstractData - CLI v0.1\n\n");

            bool console = true;
            adScript interp = new adScript();

            while (console)
            {
                Console.Write("\n >> ");
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    console = false;
                    break;
                }
                interp.runLine(input);
            }
        }
    }
}
