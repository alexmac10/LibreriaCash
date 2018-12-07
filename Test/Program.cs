using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            IAcceptor coinAcceptor = factory.GetCoinAcceptor();
            IDispenser coinDispenser = factory.GetCoinDispenser();
            

            try
            {
                int count = 0;
                //coinAcceptor.open();
                //coinDispenser.open();
                billAcceptor.open();
                billDispenser.open();
                while (count < 10)
                {
                    if (!billAcceptor.isConnection())
                    {
                        continue;
                    }
                    billAcceptor.enable();
                    Console.WriteLine();
                    Console.WriteLine("*****************RETIRO DE EFECTIVO******************");
                    Console.Write("Indique la cantidad de billetes de a $20.00 a retirar: ");
                    string cantidad20 = Console.ReadLine();
                    Console.Write("Indique la cantidad de billetes de a $50.00 a retirar: ");
                    string cantidad50 = Console.ReadLine();
                    Console.Write("Indique la cantidad de billetes de a $100.00 a retirar: ");
                    string cantidad100 = Console.ReadLine();
                    int[] BillCount = { Int32.Parse(cantidad20), Int32.Parse(cantidad50), Int32.Parse(cantidad100) };
                    foreach(var b in billDispenser.returnCash(0, 0, BillCount))
                    {

                        Console.WriteLine(b);
                    }
                   
                    Console.WriteLine("*****************************************************");
                    
                   
                    if (count == 9)
                    {
                        billAcceptor.disable();
                        billDispenser.close();
                    }
                    //Console.WriteLine(count);
                    Thread.Sleep(2000);
                    count++;
                }
                





            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }


    

    }
}
