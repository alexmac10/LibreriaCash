using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreriaKioscoCash.Class;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            DispenserF53 F53 = new DispenserF53();

            F53.open();
            if(F53.IsOpen)
            {
                
                Console.WriteLine();
                Console.WriteLine("*****************RETIRO DE EFECTIVO******************");
                Console.Write("Indique la cantidad de billetes de a $20.00 a retirar: ");
                string cantidad20 = Console.ReadLine();
                Console.Write("Indique la cantidad de billetes de a $50.00 a retirar: ");
                string cantidad50 = Console.ReadLine();
                Console.Write("Indique la cantidad de billetes de a $100.00 a retirar: ");
                string cantidad100 = Console.ReadLine();

                F53.returnCash(0, 0, Int32.Parse(cantidad20), Int32.Parse(cantidad50), Int32.Parse(cantidad100));
                Console.WriteLine("*****************************************************");
                F53.close();
            }

        }
    }
}
