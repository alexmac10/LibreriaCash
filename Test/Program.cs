using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibreriaKioscoCash.Class;
using LibreriaKioscoCash.Exceptions;
using LibreriaKioscoCash.Factory;
using LibreriaKioscoCash.Interfaces;

namespace Test
{
    class Program
    {
        /// <summary>
        /// Objeto Factory que genera las instancias de los dispositivos 
        /// </summary>
        /// <remarks>
        /// Este objeto se encarga de regresar la instancia del dispositivo
        /// solicitado que puedes ser de los siguientes tipos:
        ///     IDispenser o IAcceptor
        /// </remarks>
        static FactoryDevice factory = new FactoryDevice();

        static void Main(string[] args)
        {
            int test = 0;
            bool continuar = true;


            while (continuar)
            {
                Console.Clear();
                Console.WriteLine("***************** MENU PRINCIPAL ******************");
                Console.WriteLine("Indique el tipo de prueba que desea realizar: ");
                Console.WriteLine("     1) Prueba con dispositivo Bill Dispenser ");
                Console.WriteLine("     2) Prueba con dispositivo Bill Acceptor ");
                Console.WriteLine("     3) Prueba con dispositivo Coin Dispenser ");
                Console.WriteLine("     4) Prueba con dispositivo Coin Acceptor ");
                Console.WriteLine("     5) Prueba con todos los dispositivos ");
                Console.WriteLine("     6) Salir ");
                Console.WriteLine();
                Console.Write("     Indique la opción: ");
                try
                {
                    test = Int32.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    test = 0;
                }

                switch (test)
                {
                    case 1:
                        testBillDispenser();
                        break;
                    case 2:
                        testBillAcceptor();
                        break;
                    case 3:

                        break;
                    case 4:

                        break;
                    case 5:

                        break;
                    case 6:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Debe seleccionar una opcion del menu ");
                        break;
                }

                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("¿Quieres ingresar con otra opción del menu (y/n)?");
                string respuesta = Console.ReadLine();

                if (respuesta == "n" || respuesta == "N")
                {
                    continuar = false;
                }

            }

        }

        /// <summary>
        /// Pruebas con dispositivo para entregar billetes
        /// </summary>
        static void testBillDispenser()
        {
            Console.Clear();
            /// <summary>
            /// Obtiene la instancia para el dispositivo bill dispenser
            /// </summary>            
            IDispenser billDispenser = factory.GetBillDispenser();

            try
            {
                ///<remarks>
                ///Abriendo comunicacion con dispositivo
                ///</remarks>                
                billDispenser.open();

                //Solicitando efectivo a retirar del dispositvo bill dispenser
                Console.WriteLine("************ RETIRO DE BILLETES ************");
                Console.Write("Indique la cantidad de billetes de a $20.00 a retirar: ");
                int cantidad20 = Int32.Parse(Console.ReadLine());
                Console.Write("Indique la cantidad de billetes de a $50.00 a retirar: ");
                int cantidad50 = Int32.Parse(Console.ReadLine());
                Console.Write("Indique la cantidad de billetes de a $100.00 a retirar: ");
                int cantidad100 = Int32.Parse(Console.ReadLine());

                //Genera arreglo para mandar a dipositivo bill dispenser
                int[] billCount = { cantidad20, cantidad50, cantidad100 };


                ///<summary>
                ///Se le indica al dispositivo que regrese el efectivo
                ///</summary>
                ///<remarks>
                /// Los dos primeros parametros siempre deben que tener un valor a cero (0)
                /// el tercer parametro es un arreglo tipo int y debera definir la cantidad 
                /// a entregar y el orden del arreglo para los billetes que son : 
                ///                         [20,50,100] 
                ///</remarks>                
                billDispenser.returnCash(0, 0, billCount);
            }
            catch (CashException ex)
            {
                ///<remarks>
                ///Se genera la excepción cuando no entrego todo el efectivo el dispensador
                ///de billetes. El cual regresa un arreglo byte indicando la cantidad de 
                ///billetes que se entregaron. El arreglo esta ordenado de la siguiente 
                ///manera :  [20,50,100]
                ///</remarks>                
                foreach (byte i in ex.getInformationCashNotDeliveredException())
                {
                    Console.WriteLine(i);
                }
            }
            catch (Exception ex)
            {
                ///<remarks>
                ///Se recibe la excepción con el error de conexión
                ///</remarks>
                Console.WriteLine(ex.Message);
            }

            ///<remarks>
            ///Cierra la conexion con el dispositivo
            ///</remarks>
            billDispenser.close();

        }

        /// <summary>
        /// Pruebas con dispositivo para recibir billetes
        /// </summary>
        static void testBillAcceptor()
        {
            Console.Clear();
            int count = 0;
            /// <summary>
            /// Obtiene la instancia para el dispositivo bill Acceptor
            /// </summary>      
            IAcceptor billAcceptor = factory.GetBillAcceptor();

            //Solicitando efectivo a depositar en el dispositvo bill Acceptor
            Console.WriteLine("************ DEPOSITAR BILLETES ************");
            Console.Write("Indique el efectivo a depositar: ");
            int total = Int32.Parse(Console.ReadLine());

            billAcceptor.open();

            Console.WriteLine("Espere ...");
            int depositado = 0;
            while (depositado <= total)
            {

                ///<remarks>
                ///Validar antes si el dispositivo ya esta conectado antes de activarse
                ///</remarks>
                if (!billAcceptor.isConnection())
                {
                    continue;
                }
                else
                {
                    count += 1;
                }

                if (count == 1) {
                    Console.WriteLine("Inserte Efectivo...");
                }

                ///<remarks>
                ///Habilita el dispositivo para revcibir efectivo
                /// </remarks>                
                billAcceptor.enable();

                ///<remarks>
                ///Función que solicita la denomincion del billete recibido. Regresa un 
                ///byte por lo que se tiene que cambiar a int 
                ///</remarks>
                int recibido = Convert.ToInt32(billAcceptor.getCashDesposite(0));


                switch (recibido)
                {
                    case 20:
                        depositado += recibido;
                        Console.WriteLine("Se recibio el un billete de ${} y el acomulado es de ${1} ", recibido, depositado);
                        break;
                    case 50:
                        depositado += recibido;
                        Console.WriteLine("Se recibio el un billete de ${} y el acomulado es de ${1} ", recibido, depositado);
                        break;
                    case 100:
                        depositado += recibido;
                        Console.WriteLine("Se recibio el un billete de ${} y el acomulado es de ${1} ", recibido, depositado);
                        break;
                    case 200:
                        depositado += recibido;
                        Console.WriteLine("Se recibio el un billete de ${} y el acomulado es de ${1} ", recibido, depositado);
                        break;
                    case 500:
                        depositado += recibido;
                        Console.WriteLine("Se recibio el un billete de ${} y el acomulado es de ${1} ", recibido, depositado);
                        break;
                }

            }

            ///<remarks>
            ///Deshabilita el dispositivo para ya no recibir el efectivo. Esto metodo no 
            ///cierra el puerto de comunicación
            ///</remarks>
            billAcceptor.disable();

            ///<remarks>
            ///Cierra la conexion con el dispositivo
            ///</remarks>
            billAcceptor.close();

        }

        /// <summary>
        /// Pruebas con dispositivo para entregar monedas
        /// </summary>
        static void testCoinDispenser()
        {
            Console.Clear();
            ///<summary>
            /// Obtiene la instancia para el dispositivo Coin Dispenser
            ///</summary>
            IDispenser coinDispenser = factory.GetCoinDispenser();

        }

        /// <summary>
        /// Pruebas con dispositivo para recibir monedas
        /// </summary>
        static void testCoinAcceptor()
        {
            Console.Clear();
            ///<summary>
            /// Obtiene la instancia para el dispositivo Coin Acceptor
            ///</summary>
            IAcceptor coinAcceptor = factory.GetCoinAcceptor();

            //Solicitando efectivo a depositar en el dispositvo Coin Acceptor
            Console.WriteLine("************ DEPOSITAR MONEDAS ************");
            Console.Write("Indique la cantidad de monedas de a $1.00 a retirar: ");
            int cantidad20 = Int32.Parse(Console.ReadLine());
            Console.Write("Indique la cantidad de monedas de a $5.00 a retirar: ");
            int cantidad50 = Int32.Parse(Console.ReadLine());
            Console.Write("Indique la cantidad de monedas de a $10.00 a retirar: ");
            int cantidad100 = Int32.Parse(Console.ReadLine());
        }




    }
}
