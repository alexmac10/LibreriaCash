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
            IDispenser dispenserBill = factory.GetDeviceDispenserBill();
            try
            {
                dispenserBill.open();
                if (dispenserBill.isConnection())
                {
                    Console.WriteLine("Esta conectado");
                }
                else
                {
                    Console.WriteLine("No esta conectado");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
    }
}
