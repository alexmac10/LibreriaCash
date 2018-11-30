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
            //if(dispenserBill.isConnection()==true)
            //{
            //    dispenserBill.CheckConfig();
            //    if(dispenserBill.IsConfig)
            //    {
            //        Console.WriteLine();
            //        Console.WriteLine("*****************RETIRO DE EFECTIVO******************");
            //        Console.Write("Indique la cantidad de billetes de a $20.00 a retirar: ");
            //        string cantidad20 = Console.ReadLine();
            //        Console.Write("Indique la cantidad de billetes de a $50.00 a retirar: ");
            //        string cantidad50 = Console.ReadLine();
            //        Console.Write("Indique la cantidad de billetes de a $100.00 a retirar: ");
            //        string cantidad100 = Console.ReadLine();

            //        dispenserBill.returnCash(0, 0, Int32.Parse(cantidad20), Int32.Parse(cantidad50), Int32.Parse(cantidad100));
            //        Console.WriteLine("*****************************************************");
            //        Console.WriteLine("Conexion:{0}",dispenserBill.isConnection());

            //        dispenserBill.close();
            //    }
            //    else
            //    {
            //        Console.WriteLine("Excepcion: Error al configurar");
            //        dispenserBill.close();
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Excepcion: El dispositivo no se encuentra alimentado");
            //    dispenserBill.close();
            //}

        }
    }
}
