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
                        testCoinDispenser();
                        break;
                    case 4:
                        testCoinAcceptor();
                        break;
                    case 5:
                        allDevices();
                        break;
                    case 6:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Debe seleccionar una opcion del menu ");
                        break;
                }

                //Console.Clear();
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

            ///<remarks>
            ///Variables para pruebas
            ///</remarks>                        
            bool continuar = true;

            try
            {
                ///<remarks>
                ///Abriendo comunicacion con dispositivo
                ///</remarks>                
                billDispenser.open();

                while (continuar)
                {
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
                    /// El primer parametro debe tener un valor a cero (0)
                    /// el tercer parametro es un arreglo tipo int y debera definir la cantidad 
                    /// a entregar y el orden del arreglo para los billetes que son : 
                    ///                         [20,50,100] 
                    ///</remarks>                
                    billDispenser.returnCash(new int[0], billCount);


                    Console.WriteLine("¿ Deseas realizar otra operación  (Y/N) ?");
                    string respuesta = Console.ReadLine();
                    if (respuesta == "n" || respuesta == "N")
                    {
                        continuar = false;

                        ///<remarks>
                        ///Cierra la conexion con el dispositivo
                        ///</remarks>
                        billDispenser.close();
                    }

                }
                               
            }
            catch (CashException ex)
            {
                ///<remarks>
                ///Se genera la excepción cuando no entrego todo el efectivo el dispensador
                ///de billetes. El cual regresa un arreglo byte indicando la cantidad de 
                ///billetes que se entregaron. El arreglo esta ordenado de la siguiente 
                ///manera :  [20,50,100]
                ///</remarks>   
                ///
                Console.WriteLine("Error: Solo se pudo entregar la siguiente cantidad");
                string[] error = { "$20.00", "$50.00", "$100.00" };
                byte[] Count = ex.getInformationCashNotDeliveredException();
                for (byte i = 0; i < Count.Length; i++)
                {
                    Console.WriteLine("{0}: {1}",error[i],Count[i]);
                }


            }
            catch (Exception ex)
            {
                ///<remarks>
                ///Se recibe la excepción con el error de conexión
                ///</remarks>
                Console.WriteLine(ex.Message);

            }
        }

        /// <summary>
        /// Pruebas con dispositivo para recibir billetes
        /// </summary>
        static void testBillAcceptor()
        {
            Console.Clear();

            /// <summary>
            /// Obtiene la instancia para el dispositivo bill Acceptor
            /// </summary>      
            IAcceptor billAcceptor = factory.GetBillAcceptor();

            ///<remarks>
            ///Variables para pruebas
            ///</remarks>
            int count = 0;
            double depositado = 0;
            bool continuar = true;

            try
            {
                ///<remarks>
                ///Abriendo comunicacion con dispositivo
                ///</remarks>
                billAcceptor.open();


                while (continuar)
                {

                    ///<remarks>
                    ///Solicitando efectivo a depositar en el dispositvo bill Acceptor
                    ///</remarks>
                    Console.WriteLine("************ DEPOSITAR BILLETES ************");
                    Console.Write("Indique el efectivo a depositar: ");
                    double total = Int32.Parse(Console.ReadLine());
                    Console.WriteLine("");
                    Console.WriteLine("Espere ...");
                    Console.WriteLine("");

                    while (depositado < total)
                    {

                        ///<remarks>
                        ///Validar antes si el dispositivo ya esta conectado
                        ///antes de activarse.
                        ///</remarks>
                        if (!billAcceptor.isConnection())
                        {
                            continue;
                        }

                        ///<remarks>
                        ///Habilita el dispositivo para revcibir efectivo
                        /// </remarks>     
                        billAcceptor.enable();

                        if (count == 0)
                        {
                            count = 1;
                            Console.WriteLine("Inserte Efectivo...");
                        }


                        ///<remarks>
                        ///Función que solicita la denomincion del billete recibido. 
                        ///Regresa un double 
                        ///</remarks>
                        double[] recibido = billAcceptor.getCashDesposite();

                        switch (recibido[0])
                        {
                            case 20:
                                depositado += recibido[0];
                                Console.WriteLine("Se recibio el un billete de ${0}.00 y el acomulado es de ${1}.00 ", recibido[0], depositado);
                                break;
                            case 50:
                                depositado += recibido[0];
                                Console.WriteLine("Se recibio el un billete de ${0}.00 y el acomulado es de ${1}.00 ", recibido[0], depositado);
                                break;
                            case 100:
                                depositado += recibido[0];
                                Console.WriteLine("Se recibio el un billete de ${0}.00 y el acomulado es de ${1}.00 ", recibido[0], depositado);
                                break;
                            case 200:
                                depositado += recibido[0];
                                Console.WriteLine("Se recibio el un billete de ${0}.00 y el acomulado es de ${1}.00 ", recibido[0], depositado);
                                break;
                            case 500:
                                depositado += recibido[0];
                                Console.WriteLine("Se recibio el un billete de ${0}.00 y el acomulado es de ${1}.00 ", recibido[0], depositado);
                                break;
                            default:
                                break;
                        }

                    }

                    Console.WriteLine("");
                    Console.WriteLine("Se recibio: ${0}.00",depositado);
                    Console.WriteLine("");
                    Console.WriteLine("Transacción Terminada ....");
                    Console.WriteLine("");
                    
                    ///<remarks>
                    ///Deshabilita el dispositivo para ya no recibir el efectivo. 
                    ///Esto metodo no cierra el puerto de comunicación
                    ///</remarks>
                    billAcceptor.disable();

                    Console.WriteLine("¿ Deseas realizar otra operación (Y/N) ?");
                    string respuesta = Console.ReadLine();

                    if (respuesta == "n" || respuesta == "N")
                    {
                        continuar = false;

                        ///<remarks>
                        ///Cierra la conexion con el dispositivo
                        ///</remarks>
                        billAcceptor.close();
                    }
                }                
            }
            catch (Exception ex)
            {
                ///<remarks>
                ///Muestra un mensaje en caso de error de conexion
                ///</remarks>
                Console.WriteLine(ex.Message);
            }
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

            ///<remarks>
            ///Variables para pruebas
            ///</remarks>                        
            bool continuar = true;

            try
            {

                ///<remarks>
                ///Abriendo comunicacion con dispositivo
                ///</remarks>
                coinDispenser.open();

                while (continuar)
                {

                    Console.WriteLine("");
                    Console.WriteLine("************ RETIRAR MONEDAS ************");
                    Console.Write("Indique la cantidad de monedas de a $1.00 a retirar: ");
                    int cantidad1 = Int32.Parse(Console.ReadLine());
                    Console.Write("Indique la cantidad de monedas de a $5.00 a retirar: ");
                    int cantidad5 = Int32.Parse(Console.ReadLine());
                    Console.Write("Indique la cantidad de monedas de a $10.00 a retirar: ");
                    int cantidad10 = Int32.Parse(Console.ReadLine());
                    Console.WriteLine("");
                    Console.WriteLine("Espere ...");

                    ///<remarks>
                    ///Se genera un arreglo con la cantidad de monedas que se van a entregar. 
                    ///El arreglo esta conformado de la siguiente manera: [1,5,10]
                    ///</remarks>
                    int[] coinCount = { cantidad1, cantidad5, cantidad10 };


                    ///<remarks>
                    ///Se le indica al dispositivo coinDispenser que entregue las monedas.
                    ///</remarks>
                    coinDispenser.returnCash(coinCount, new int[0]);

                    Console.WriteLine("¿ Deseas realizar otra operación  (Y/N)?");
                    string respuesta = Console.ReadLine();

                    if (respuesta == "n" || respuesta == "N")
                    {
                        continuar = false;

                        ///<remarks>
                        ///Cierra la conexion con el dispositivo
                        ///</remarks>
                        coinDispenser.close();
                    }

                } 
                
            }
            catch (Exception ex)
            {
                ///<remarks>
                ///Muestra la excepcion en caso de error de conexión.
                ///</remarks>                
                Console.WriteLine(ex.Message);
            }

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

            ///<remarks>
            ///Variables para pruebas
            ///</remarks>                        
            int count = 0;
            double depositado = 0;
            bool continuar = true;
            double count_actual = 0;

            try
            {

                ///<remarks>
                ///Abriendo comunicacion con dispositivo
                ///</remarks>
                coinAcceptor.open();

                while (continuar)
                {
                    Console.WriteLine("************ DEPOSITAR MONEDAS ************");
                    Console.Write("Indique la cantidas a depositar: $ ");
                    double total = Int32.Parse(Console.ReadLine());
                    Console.WriteLine("");
                    Console.WriteLine("Espere ...");

                    while (depositado < total)
                    {
                        ///<remarks>
                        ///Habilitando el dipostivo coinAcceptor para recibir las monedas.
                        ///</remarks>
                        coinAcceptor.enable();
                        if (count == 0)
                        {
                            count = 1;
                            Console.WriteLine("");
                            Console.WriteLine("Inserte Efectivo...");
                        }

                        ///<remarks>
                        ///Recibe un arreglo  con el siguiente orden :
                        ///                 [denominacion,contador]
                        ///</remarks>
                        double[] recibido = coinAcceptor.getCashDesposite();


                        if (count_actual != recibido[1])
                        {
                            switch (recibido[0])
                            {
                                case 10:
                                    depositado += recibido[0];
                                    count_actual = recibido[1];
                                    Console.WriteLine("Se recibio moneda de ${0}.00 y el acomulado es de ${1}.00 ", recibido[0], depositado);
                                    break;
                                case 5:
                                    depositado += recibido[0];
                                    count_actual = recibido[1];
                                    Console.WriteLine("Se recibio moneda de ${0}.00 y el acomulado es de ${1}.00 ", recibido[0], depositado);
                                    break;
                                case 2:
                                    depositado += recibido[0];
                                    count_actual = recibido[1];
                                    Console.WriteLine("Se recibio moneda de ${0}.00 y el acomulado es de ${1}.00 ", recibido[0], depositado);
                                    break;
                                case 1:
                                    depositado += recibido[0];
                                    count_actual = recibido[1];
                                    Console.WriteLine("Se recibio moneda de ${0}.00 y el acomulado es de ${1}.00 ", recibido[0], depositado);
                                    break;

                                default:
                                    break;
                            }
                        }


                    }


                    Console.WriteLine("");
                    Console.WriteLine("Recibido: ${0}.00", depositado);
                    Console.WriteLine("");
                    Console.WriteLine("Transacción Finalizada...");
                    Console.WriteLine("");

                    ///<remarks>
                    ///Deshabilita el dipostivo para no aceptar mas monedas
                    ///</remarks>
                    coinAcceptor.disable();

                    //Resetea variables locales 
                    depositado = 0;
                    count = 0;
                    count_actual = 0;

                    Console.WriteLine("¿ Deseas realizar otra operación  (Y/N) ?");
                    string respuesta = Console.ReadLine();

                    if (respuesta == "n" || respuesta == "N")
                    {
                        continuar = false;

                        ///<remarks>
                        ///Cierra la conexion con el dispositivo
                        ///</remarks>
                        coinAcceptor.close();
                    }                    
                }

               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        static void allDevices()
        {
            IDispenser billDispenser = factory.GetBillDispenser();
            IAcceptor billAcceptor = factory.GetBillAcceptor();
            IAcceptor coinAcceptor = factory.GetCoinAcceptor();
            IDispenser coinDispenser = factory.GetCoinDispenser();

            billDispenser.open();
            billAcceptor.open();
            coinAcceptor.open();
            coinDispenser.open();


        }
    }
}
