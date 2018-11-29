using LibreriaKioscoCash.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Configuration;
using System.Threading;

namespace LibreriaKioscoCash.Class
{

    public class DispenserF53 : IDevice, IDispenser
    {
        private SerialPort portDispenserF53;
        
        //RS232C(Comunicación)
        private byte[] startoftext = new byte[] { 0x10, 0x02, 0x00 };
        private byte[] endoftext = new byte[] { 0x10, 0x03 };
        private byte[] mensaje_final = new byte[] { };
        private byte[] statusRequest = new byte[] { 0x10, 0x05 };
        private byte[] releaseRequest = new byte[] { 0x10, 0x06 };
        //Variables
        private byte[] resultmessage;
        private int position;
        private string RX;
        private string TX;
        private bool status = false;
        private bool money = false;
        private bool config = false;
        private bool bandera;
        private  List<byte> Sensors, Error;
        private StreamWriter Log = File.AppendText("LogF53.txt");
        //Funciones (Codigos de acuerdo a documentación del fabricante)
        private byte[] StatusInformation = new byte[] { 0x00, 0x01, 0x1C };
        private byte[] StatusInformation_2 = new byte[] { 0x60, 0x01, 0xFF, 0x00, 0x00, 0x01, 0x00, 0x1C };
        private byte[] StatusDeviceInit = new byte[] { 0x60, 0x02, 0x0D, 0x1C };
        private byte[] DispenserBill = new byte[] { 0x60, 0x03, 0x15, 0xE4, 0x30, 0xB1, 0x30, 0xB1, 0x30, 0xB1, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x00, 0x00, 0x00, 0x00, 0x1C };
        private byte[] MechalReset = new byte[] { 0x60, 0x02, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C };
        private byte[] ConfigDefault = new byte[] { 0x60, 0x02, 0xFF, 0x00, 0x00, 0x1A, 0x00, 0x40, 0x082, 0x6E, 0x89, 0x75, 0x93, 0x7F, 0x00, 0x00, 0x0C, 0x0C, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C };
        private byte[] cancel = new byte[] { 0x00, 0x03, 0x00, 0x10, 0x1C };
        private byte[] IsoCodes = new byte[] { 48, 177, 178, 51, 180, 53, 54, 183, 184, 57 };
        private string[] Sensors_name = new string[] { "FDLS1", "FDLS2", "FDLS3", "FDLS4", "FDLS5", "FDLS6", "DFSS", "REJS", "BPS", "BRS1", "BRS2", "BRS3", "EJSR", "EJSF", "BCS" };

        //Configuracion de paridad del protocolo serial RS232
        private Parity SetPortParity(Parity defaultPortParity)
        {
            string parity = Console.ReadLine();

            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }

            return (Parity)Enum.Parse(typeof(Parity), parity);
        }
        private bool connection = false;
        
        private bool openConnection()
        {
            string COMF53 = ConfigurationManager.AppSettings.Get("COMF53");
            portDispenserF53 = new SerialPort(COMF53, 9600, Parity.Even);
            try
            {
                portDispenserF53.Open();
                Console.WriteLine("Abriendo Puerto en el: {0} ",COMF53);
                if(portDispenserF53.IsOpen)
                {
                    Console.WriteLine("Dispositivo Conectado");
                    connection = true;

                }
                else
                {
                    connection = false;

                }

            }

            catch(IOException ex)
            {
                Console.WriteLine("No se pudo abrir el puerto:", ex);
                connection = false;
                
            }
            return connection;
            

        }
        public bool IsOpen
        {
            get
            {
                return bandera;
            }

        }
        public void close()
        {
            Log.WriteLine("-------------------------------------------------------------------------");
            Log.Close();
            portDispenserF53.Close();
        }

        public bool isConnection()
        {
   
            Console.WriteLine(connection);
            return connection;
            
            
        }

        public void isError(byte[] parameters)
        {
            throw new NotImplementedException();
        }

        public void open()
        {
            openConnection();
            Status_Information();
        }

        public void receive()
        {
            throw new NotImplementedException();
        }

        public void send(byte[] parameters)
        {
            throw new NotImplementedException();
        }

        private void setMessage(byte[] parameters)
        {
            TX = "TX: ";
            portDispenserF53.Write(parameters, 0, parameters.Length);

            for (int i = 0, j = 0; i < parameters.Length; i++, j++)
            {
                TX += parameters[i] + " ";
            }
            //Console.WriteLine(TX);
            //Console.WriteLine("TX: " + ByteArrayToString(parameters));

            Thread.Sleep(50);
        }
        private byte getMessage()
        {
            RX = "RX :";
            byte finalByte = 0;
            byte[] result = new byte[portDispenserF53.BytesToRead];

            portDispenserF53.Read(result, 0, result.Length);
            resultmessage = new byte[result.Length];
            for (int i = 0, j = 0; i < result.Length; i++, j++)
            {
                resultmessage[j] = result[i];
                RX += result[i] + " ";
                finalByte = result[i];
            }
            //Console.WriteLine(RX);
            //Console.WriteLine("RX: " + ByteArrayToString(resultmessage));
            Thread.Sleep(150);


            return finalByte;
        }
        private void createCode(byte[] fun)
        {
            ushort[] table = new ushort[256];
            ushort polynomial = (ushort)0x8408;
            ushort value;
            ushort temp;

            List<byte> list1 = new List<byte>();
            list1.Add(Convert.ToByte(fun.Length));
            list1.AddRange(fun);
            list1.AddRange(endoftext);
            byte[] mensaje = list1.ToArray();

            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;

                for (byte j = 0; j < 8; j++)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }

            ushort crc = 0;

            for (int x = 0; x < mensaje.Length; x++)
            {
                byte index = (byte)(crc ^ mensaje[x]);
                crc = (ushort)((crc >> 8) ^ table[index]);

            }
            crc = (ushort)(crc & 0xFFFF);
            crc = (ushort)((crc << 8) | ((crc >> 8) & 0xFF));
            crc = (ushort)(crc & 0xFFFF);
            crc = (ushort)((crc << 8) | ((crc >> 8) & 0xFF));



            //Obtenemos el checksum en byte
            byte[] listCheckSum = BitConverter.GetBytes(crc);

            List<byte> list2 = new List<byte>(startoftext);
            list2.AddRange(mensaje);
            list2.AddRange(listCheckSum);
            mensaje_final = list2.ToArray();
            foreach (byte i in mensaje_final)
            {
                //Console.Write(i + " ");
                //string hexValue = i.ToString("X");
                //Console.Write(hexValue + " ");

            }
            //Console.WriteLine();

        }
        private void setFreeDevice()
        {
            createCode(cancel);
            setMessage(mensaje_final);
            getMessage();
            setMessage(releaseRequest);
            setMessage(statusRequest);
            getMessage();
            setMessage(releaseRequest);
            getMessage();
            setMessage(releaseRequest);
            setMessage(statusRequest);
            getMessage();
            setMessage(statusRequest);
            getMessage();
        }
        private void sendMessage(byte[] parameter)
        {

            //byte result = 0;

            setMessage(statusRequest);
            getMessage();
            byte[] Free = new byte[] { 0x10, 0x06 };
            search(resultmessage, Free);
            //Console.WriteLine(resultmessage.Length);
            if ((status == true) && (resultmessage.Length == 2))
            {
                //Console.WriteLine("Se encuentra libre");

                if (money == true)
                {
                    setMessage(parameter);
                    getMessage();
                    Thread.Sleep(9000);
                    getMessage();
                    setMessage(releaseRequest);
                    Thread.Sleep(200);
                    getMessage();
                    setMessage(releaseRequest);
                    Console.WriteLine("");

                }
                else if (config == true)
                {
                    setMessage(parameter);
                    getMessage();
                    Thread.Sleep(6000);
                    getMessage();
                    setMessage(releaseRequest);
                    Thread.Sleep(200);
                    getMessage();
                    setMessage(releaseRequest);
                    Console.WriteLine("");
                    Console.WriteLine("Configuración correcta");
                    DisplayEvent("Configuración correcta");
                }
                else
                {

                    setMessage(parameter);
                    Thread.Sleep(200);
                    getMessage();
                    getMessage();
                    setMessage(releaseRequest);
                    Thread.Sleep(200);
                    getMessage();
                    setMessage(releaseRequest);
                }
            }
            else
            {
                Console.WriteLine("Estaba Ocupado");
                setFreeDevice();
                if (money == true)
                {
                    setMessage(parameter);
                    getMessage();
                    Thread.Sleep(7000);
                    getMessage();
                    setMessage(releaseRequest);
                    Thread.Sleep(200);
                    getMessage();
                    setMessage(releaseRequest);
                    Console.WriteLine("");

                }
                else if (config == true)
                {
                    setMessage(parameter);
                    getMessage();
                    Thread.Sleep(6000);
                    getMessage();
                    setMessage(releaseRequest);
                    Thread.Sleep(200);
                    getMessage();
                    setMessage(releaseRequest);
                    Console.WriteLine("");
                    Console.WriteLine("Configuración correcta");
                    DisplayEvent("Configuración Correcta");
                }
                else
                {

                    setMessage(parameter);
                    Thread.Sleep(200);
                    getMessage();
                    getMessage();
                    setMessage(releaseRequest);
                    Thread.Sleep(200);
                    getMessage();
                    setMessage(releaseRequest);
                }

            }



        }

        //Funciones del Dispositivo [Codigo Completo]
        private void Config_inicial()
        {
            config = true;
            createCode(ConfigDefault);
            sendMessage(mensaje_final);
        }
        private void Mechal_Reset()
        {
            createCode(MechalReset);
            sendMessage(mensaje_final);
        }
        private void Cancel()
        {
            createCode(cancel);
            sendMessage(mensaje_final);
        }
        private void SensorLevelInformation()
        {
            Console.WriteLine("Checando Estatus de los Sensores....");
            DisplayEvent("Checando Estatus de los Sensores....");

            createCode(StatusDeviceInit);
            sendMessage(mensaje_final);
            byte[] positive_answer = { 0x1C, 0x10, 0x03 };
            search(resultmessage, positive_answer);

            //Sensores que contiene el F53


            Sensors = new List<byte>();
            for (int l = 43, m = 0; l < position; l++, m++)
            {
                Sensors.Add(resultmessage[l]);
                //Console.WriteLine(resultmessage[l]);
            }

            for (int s = 0; s < Sensors.Count - 1; s++)
            {
                if (Sensors[s] <= 8)
                {
                    Console.WriteLine("Sensor {0}: Normal", Sensors_name[s]);
                    DisplayEvent("Sensor " + Sensors_name[s] + ":" + " Normal");
                }
                else if ((Sensors[s] > 8) && (Sensors[s] <= 12))
                {
                    Console.WriteLine("Sensor {0}: Maintenance necessary (Cleaning）", Sensors_name[s]);
                    DisplayEvent("Sensor " + Sensors_name[s] + ":" + " Maintenance necessary (Cleaning)");
                }
                else if (Sensors[s] > 12)
                {
                    Console.WriteLine("Sensor {0}: Error (Requires replacement", Sensors_name[s]);
                    DisplayEvent("Sensor " + Sensors_name[s] + ":" + " Error (Requires replacement");
                }
            }
        }
        public void Status_Information()
        {
            Log.WriteLine("--------------------------SICCOB SOLUTIONS-------------------------------");
            SensorLevelInformation();
            Console.WriteLine("Checando Estatus del Dispositivo....");
            DisplayEvent("Checando Estatus del Dispositivo....");
            createCode(StatusInformation);
            sendMessage(mensaje_final);
            byte[] cassets = new byte[] { 0x8C, 0x8A, 0x89 };
            search(resultmessage, cassets);

            if (status == true)
            {
                Console.WriteLine("Los Cassets se encuentran colocados");
                Console.WriteLine("");
                DisplayEvent("Los Cassets se encuentran colocados");
                byte[] config = new byte[] { 130, 110, 137, 117, 147, 127 };
                search(resultmessage, config);
                Console.WriteLine("Checando Estatus de Configuración....");
                DisplayEvent("Checando Estatus de Configuración....");
                if (status == false)
                {
                    Console.WriteLine("Configurando Dispositivo....");
                    Console.WriteLine("");
                    DisplayEvent("Configurando Dispositivo....");
                    Config_inicial();


                }
                else
                {
                    Console.WriteLine("El dispositivo ya se encuentra configurado");
                    Console.WriteLine("");
                    DisplayEvent("El dispositivo ya se encuentra configurado");

                }
                
                bandera = true;

               


            }
            else
            {
                Console.WriteLine("Error: Colocar Cassets");
                DisplayEvent("Error: Colocar Cassets");
            }

        }

        public void returnCash(int denominationCash, int countMoney, int cantidad_20, int cantidad_50, int cantidad_100)
        {
            money = true;
            Console.WriteLine("Retirando Efectivo...");
            DisplayEvent("Retirando Efectivo...");
            DisplayEvent("Billetes de $20.00: " + cantidad_20);
            DisplayEvent("Billetes de $50.00: " + cantidad_50);
            DisplayEvent("Billetes de $100.00: " + cantidad_100);
            DispenserBill[5] = IsoCodes[cantidad_20];
            DispenserBill[7] = IsoCodes[cantidad_50];
            DispenserBill[9] = IsoCodes[cantidad_100];

            createCode(DispenserBill);
            sendMessage(mensaje_final);
            byte[] error_answer = { 0x8c };
            search(resultmessage, error_answer);

            //Sensores que contiene el F53

            Error = new List<byte>();
            for (int l = 7, m = 0; l < position; l++, m++)
            {
                Error.Add(resultmessage[l]);
                //Console.WriteLine(resultmessage[l].ToString("X"));
            }
            int suma = 0;
            foreach (int c in Error)
            {
                suma = +c;

            }
            if ((Error[0] == 0) && (Error[1] == 0))
            {
                Console.WriteLine("Se Entrego El Dinero De Manera Correcta");

                Console.WriteLine("Error:{0} {1}, Adress:{2} {3},Register:{4} {5} {6}", Error[0].ToString("X"), Error[1].ToString("X"), Error[2].ToString("X"), Error[3].ToString("X"), Error[6].ToString("X"), Error[7].ToString("X"), Error[8].ToString("X"));
                Console.WriteLine("Sensor Register: {0} {1} {2}", Error[9].ToString("X"), Error[10].ToString("X"), Error[11].ToString("X"), Error[12].ToString("X"), Error[13].ToString("X"), Error[14].ToString("X"));
                //Console.WriteLine("Sensor Register: {0} {1} {2} {3} {4} {5}", Error[9], Error[10], Error[11], Error[12], Error[13], Error[14]);

                DisplayEvent("Se Entrego El Dinero De Manera Correcta");
                DisplayEvent("Error Code: " + Error[0].ToString("X") + " " + Error[1].ToString("X"));
                DisplayEvent("Error Adress: " + Error[2].ToString("X") + " " + Error[3].ToString("X"));
                DisplayEvent("Error Register: " + Error[6].ToString("X") + " " + Error[7].ToString("X") + " " + Error[8].ToString("X"));
                DisplayEvent("Sensor Register: " + Error[9].ToString("X") + " " + Error[10].ToString("X") + " " + Error[11].ToString("X") + " " + Error[12].ToString("X") + " " + Error[13].ToString("X") + " " + Error[14].ToString("X"));

            }
            else
            {
                Console.WriteLine("No se pudo entregar el dinero");

                Console.WriteLine("Error:{0} {1}, Adress:{2} {3},Register:{4} {5} {6}", Error[0].ToString("X"), Error[1].ToString("X"), Error[2].ToString("X"), Error[3].ToString("X"), Error[6].ToString("X"), Error[7].ToString("X"), Error[8].ToString("X"));
                Console.WriteLine("Sensor Register: {0} {1} {2} {3} {4} {5}", Error[9].ToString("X"), Error[10].ToString("X"), Error[11].ToString("X"), Error[12].ToString("X"), Error[13].ToString("X"), Error[14].ToString("X"));

                DisplayEvent("No se pudo entregar el dinero");
                DisplayEvent("Error Code: " + Error[0].ToString("X") + " " + Error[1].ToString("X"));
                DisplayEvent("Error Adress: " + Error[2].ToString("X") + " " + Error[3].ToString("X"));
                DisplayEvent("Error Register: " + Error[6].ToString("X") + " " + Error[7].ToString("X") + " " + Error[8].ToString("X"));
                DisplayEvent("Sensor Register: " + Error[9].ToString("X") + " " + Error[10].ToString("X") + " " + Error[11].ToString("X") + " " + Error[12].ToString("X") + " " + Error[13].ToString("X") + " " + Error[14].ToString("X"));

            }

        }

        //Funciones complementarias de programación
        private string ByteArrayToString(byte[] ba)
        {

            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}" + " ", b);
            return hex.ToString();
        }
        private int search(byte[] haystack, byte[] needle)
        {
            for (int i = 0; i <= haystack.Length; i++)
            {

                if (match(haystack, needle, i))
                {
                    position = i;
                    //Console.WriteLine("Status:{0}\nPosición:{1}", status, position);
                    return i;
                }

            }

            return -1;
        }
        private bool match(byte[] haystack, byte[] needle, int start)
        {
            if (needle.Length + start > haystack.Length)
            {
                status = false;
                return false;
            }
            else
            {
                for (int i = 0; i < needle.Length; i++)
                {
                    if (needle[i] != haystack[i + start])
                    {

                        return false;

                    }
                }
                status = true;
                return true;
            }
        }
        private void DisplayEvent(string message)
        {
            string fecha = string.Format("{0:dd/MM/yyyy HH:mm}  ", DateTime.Now);


            if (message != "")
            {
                Log.WriteLine(fecha + "" + message);
            }


        }



    }

}
