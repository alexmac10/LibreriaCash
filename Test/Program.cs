using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreriaKioscoCash.Class;
using LibreriaKioscoCash.Factory;
using LibreriaKioscoCash.Interfaces;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            FactoryDevice factory = new FactoryDevice();
            IDispenser billDispenser = factory.GetBillDispenser();
            IAcceptor billAcceptor = factory.GetBillAcceptor();

            billAcceptor.stackEvent += stackHandle;
            billAcceptor.powerUpEvent += powerUpHandle;
            billAcceptor.powerUpCompleteEvent += PowerUpCompletedHandle;
            billAcceptor.escrowEvent += escrowHandle;
            billAcceptor.connectEvent += connectedHandle;

            try
            {
                //dispenserBill.open();
                //Console.WriteLine();
                //Console.WriteLine("*****************RETIRO DE EFECTIVO******************");
                //Console.Write("Indique la cantidad de billetes de a $20.00 a retirar: ");
                //string cantidad20 = Console.ReadLine();
                //Console.Write("Indique la cantidad de billetes de a $50.00 a retirar: ");
                //string cantidad50 = Console.ReadLine();
                //Console.Write("Indique la cantidad de billetes de a $100.00 a retirar: ");
                //string cantidad100 = Console.ReadLine();
                //int[] BillCount = { Int32.Parse(cantidad20), Int32.Parse(cantidad50), Int32.Parse(cantidad100) };
                //dispenserBill.returnCash(0, 0,BillCount);
                //Console.WriteLine("*****************************************************");
                //dispenserBill.close();

                billAcceptor.open();
                billAcceptor.enable();

                billAcceptor.getCashDesposite(0);
                
                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        
        static void powerUpHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Manejador de POWER UP");
        }

        static void connectedHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Evento : Configuracion");
        }

        static void stackHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Evento : Stack");
           
        }

        static void PowerUpCompletedHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Evento : POWERUP_COMPLETED");
        }

        static void escrowHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Evento : ESCROW");
        }
    }
}
