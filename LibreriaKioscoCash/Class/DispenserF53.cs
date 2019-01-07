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
using System.Collections;
using LibreriaKioscoCash.Exceptions;

namespace LibreriaKioscoCash.Class
{

    public class DispenserF53 : IDispenser
    {
        private SerialPort F53;
        private CommunicationProtocol ccTalk = CommunicationProtocol.GetInstance();
        private string COM;

        //RS232C(Comunicación)
        private byte[] startoftext = new byte[] { 0x10, 0x02, 0x00 };
        private byte[] endoftext = new byte[] { 0x10, 0x03 };
        private byte[] mensaje_final = new byte[] { };
        private byte[] statusRequest = new byte[] { 0x10, 0x05 };
        private byte[] releaseRequest = new byte[] { 0x10, 0x06 };

        //Variables
        private bool money = false;
        private bool config = false;
        private List<byte> Sensors, Error, positions;
        //private StreamWriter Log = File.AppendText("LogF53.txt");

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


        //Funciones de la interfaz

        public void close()
        {
            F53.Close();
        }

        public bool isOpen()
        {
            try
            {


                if (!F53.IsOpen)
                {
                    throw new Exception("Dispositivo No Conectado");
                }


            }
            catch (IOException ex)
            {
                throw new Exception(ex.Message);

            }
            return F53.IsOpen;



        }

        public void open()
        {
            try
            {

                openConnection();
                if (F53.IsOpen)
                {
                    CheckConfig();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public void returnCash(int[] count)
        {

            byte[] bill_codes = { };
            money = true;
            //Console.WriteLine(" ");
            //Console.WriteLine("Retirando Efectivo...");

            DispenserBill[5] = IsoCodes[count[0]];
            DispenserBill[7] = IsoCodes[count[1]];
            DispenserBill[9] = IsoCodes[count[2]];


            createCode(DispenserBill);
            sendMessage(mensaje_final);
            byte[] error_answer = { 0x8c };
            ccTalk.search(ccTalk.resultmessage, error_answer);
            Error = new List<byte>();
            for (int l = 7, m = 0; l < ccTalk.position; l++, m++)
            {
                Error.Add(ccTalk.resultmessage[l]);
                //Console.WriteLine(ccTalk.resultmessage[l].ToString("X"));
            }
            if ((Error[0] == 0) && (Error[1] == 0))
            {
                Console.WriteLine("Se Entregaron los Billetes De Manera Correcta");
                Console.WriteLine(" ");



                //Console.WriteLine("Error Code: " + Error[0].ToString("X") + " " + Error[1].ToString("X"));
                //Console.WriteLine("Error Adress: " + Error[2].ToString("X") + " " + Error[3].ToString("X"));
                //Console.WriteLine("Error Register: " + Error[6].ToString("X") + " " + Error[7].ToString("X") + " " + Error[8].ToString("X"));
                //Console.WriteLine("Sensor Register: " + Error[9].ToString("X") + " " + Error[10].ToString("X") + " " + Error[11].ToString("X") + " " + Error[12].ToString("X") + " " + Error[13].ToString("X") + " " + Error[14].ToString("X"));



            }
            else
            {


                //Console.WriteLine("No se pudo entregar el dinero");
                //Console.WriteLine("Error Code: " + Error[0].ToString("X") + " " + Error[1].ToString("X"));
                //Console.WriteLine("Error Adress: " + Error[2].ToString("X") + " " + Error[3].ToString("X"));
                //Console.WriteLine("Error Register: " + Error[6].ToString("X") + " " + Error[7].ToString("X") + " " + Error[8].ToString("X"));
                //Console.WriteLine("Sensor Register: " + Error[9].ToString("X") + " " + Error[10].ToString("X") + " " + Error[11].ToString("X") + " " + Error[12].ToString("X") + " " + Error[13].ToString("X") + " " + Error[14].ToString("X"));

                if ((count[0] <= 2) && (count[1] <= 2) && (count[2] <= 2))
                {
                    bill_codes = new byte[] { ccTalk.resultmessage[46], ccTalk.resultmessage[48], ccTalk.resultmessage[50] };

                }
                else
                {
                    bill_codes = new byte[] { ccTalk.resultmessage[44], ccTalk.resultmessage[46], ccTalk.resultmessage[48] };

                }

                byte[] entrega = getPositions(IsoCodes, bill_codes);

                throw new CashException(entrega);





            }





        }

        //Metodos de la clase

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
            ccTalk.setMessage(mensaje_final);
            ccTalk.getMessage();
            ccTalk.setMessage(releaseRequest);
            ccTalk.setMessage(statusRequest);
            ccTalk.getMessage();
            ccTalk.setMessage(releaseRequest);
            ccTalk.getMessage();
            ccTalk.setMessage(releaseRequest);
            ccTalk.setMessage(statusRequest);
            ccTalk.getMessage();
            ccTalk.setMessage(statusRequest);
            ccTalk.getMessage();
        }

        private void sendMessage(byte[] parameter)
        {

            //byte result = 0;

            ccTalk.setMessage(statusRequest);
            ccTalk.getMessage();
            ccTalk.search(ccTalk.resultmessage, releaseRequest);
            //Console.WriteLine(ccTalk.resultmessage.Length);
            if ((ccTalk.status == true) && (ccTalk.resultmessage.Length == 2))
            {
                //Console.WriteLine("Se encuentra libre");

                if (money == true)
                {

                    ccTalk.setMessage(parameter);
                    Thread.Sleep(200);
                    ccTalk.getMessage();
                    Thread.Sleep(9000);
                    ccTalk.getMessage();
                    ccTalk.setMessage(releaseRequest);
                    Thread.Sleep(200);
                    ccTalk.getMessage();
                    ccTalk.setMessage(releaseRequest);
                    Console.WriteLine("");

                }
                else if (config == true)
                {
                    ccTalk.setMessage(parameter);
                    Thread.Sleep(200);
                    ccTalk.getMessage();
                    Thread.Sleep(5000);
                    ccTalk.getMessage();
                    ccTalk.setMessage(releaseRequest);
                    Thread.Sleep(200);
                    ccTalk.getMessage();
                    ccTalk.setMessage(releaseRequest);
                    Console.WriteLine("Configuración correcta");
                }
                else
                {

                    ccTalk.setMessage(parameter);
                    Thread.Sleep(200);
                    ccTalk.getMessage();
                    ccTalk.getMessage();
                    ccTalk.setMessage(releaseRequest);
                    Thread.Sleep(200);
                    ccTalk.getMessage();
                    ccTalk.setMessage(releaseRequest);
                }
            }
            else
            {
                Console.WriteLine("Dispositivo Ocupado");
                Console.WriteLine("Liberando...");
                setFreeDevice();
                if (money == true)
                {
                    ccTalk.setMessage(parameter);
                    ccTalk.getMessage();
                    Thread.Sleep(9000);
                    ccTalk.getMessage();
                    ccTalk.setMessage(releaseRequest);
                    Thread.Sleep(200);
                    ccTalk.getMessage();
                    ccTalk.setMessage(releaseRequest);
                    Console.WriteLine("");

                }
                else if (config == true)
                {
                    ccTalk.setMessage(parameter);
                    Thread.Sleep(200);
                    ccTalk.getMessage();
                    Thread.Sleep(5000);
                    ccTalk.getMessage();
                    ccTalk.setMessage(releaseRequest);
                    Thread.Sleep(200);
                    ccTalk.getMessage();
                    ccTalk.setMessage(releaseRequest);
                    Console.WriteLine("Configuración correcta");
                }
                else
                {

                    ccTalk.setMessage(parameter);
                    Thread.Sleep(200);
                    ccTalk.getMessage();
                    ccTalk.getMessage();
                    ccTalk.setMessage(releaseRequest);
                    Thread.Sleep(200);
                    ccTalk.getMessage();
                    ccTalk.setMessage(releaseRequest);
                }

            }



        }

        private void openConnection()
        {
            COM = ConfigurationManager.AppSettings.Get("COMBillDispenser");
            F53 = ccTalk.openConnection(COM);
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
            //Console.WriteLine("Checando Estatus de los Sensores....");
            Console.WriteLine("Checando Estatus de los Sensores....");

            createCode(StatusDeviceInit);
            sendMessage(mensaje_final);
            byte[] positive_answer = { 0x1C, 0x10, 0x03 };
            ccTalk.search(ccTalk.resultmessage, positive_answer);

            //Sensores que contiene el F53


            Sensors = new List<byte>();
            for (int l = 43, m = 0; l < ccTalk.position; l++, m++)
            {
                Sensors.Add(ccTalk.resultmessage[l]);
                //Console.WriteLine(ccTalk.resultmessage[l]);
            }

            for (int s = 0; s < Sensors.Count - 1; s++)
            {
                if ((Sensors[s] <= 8) && (Sensors[s] > 0))
                {
                    Console.WriteLine("Sensor {0}: Normal", Sensors_name[s]);
                    Console.WriteLine("Sensor " + Sensors_name[s] + ":" + " Normal");
                }
                else if ((Sensors[s] > 8) && (Sensors[s] <= 12))
                {
                    Console.WriteLine("Sensor {0}: Maintenance necessary (Cleaning）", Sensors_name[s]);
                    Console.WriteLine("Sensor " + Sensors_name[s] + ":" + " Maintenance necessary (Cleaning)");
                }
                else if (Sensors[s] > 12)
                {
                    Console.WriteLine("Sensor {0}: Error (Requires replacement", Sensors_name[s]);
                    Console.WriteLine("Sensor " + Sensors_name[s] + ":" + " Error (Requires replacement");
                }
            }
        }

        private void CheckConfig()
        {
            //Console.WriteLine("Checando Estatus del Dispositivo....");
            createCode(StatusInformation);
            sendMessage(mensaje_final);
            byte[] cassets = new byte[] { 0x8C, 0x89, 0x8A };
            ccTalk.search(ccTalk.resultmessage, cassets);
            try
            {
                if (ccTalk.status == true)
                {
                    Console.WriteLine("Estatus: OK");
                    Console.WriteLine("");
                    //Console.WriteLine("Los Cassets se encuentran colocados");
                    byte[] config = new byte[] { 130, 110, 137, 117, 147, 127 };
                    ccTalk.search(ccTalk.resultmessage, config);
                    //Console.WriteLine("Checando Estatus de Configuración....");
                    if (ccTalk.status == false)
                    {
                        Console.WriteLine("Configurando Dispositivo....");
                        Config_inicial();


                    }
                    else
                    {
                        Console.WriteLine("El dispositivo ya se encuentra configurado");
                        Console.WriteLine("");

                    }

                }
                else
                {
                    throw new Exception("Error: Colocar Cassets");

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        //Funciones complementarias de programación

        /* private int search(byte[] haystack, byte[] needle)
         {
             for (int i = 0; i <= haystack.Length - needle.Length; i++)
             {

                 if (match(haystack, needle, i))
                 {
                     position = i;
                     //Console.WriteLine("Status:{0}\nPosición:{1}", status, position);
                     return i;
                 }

             }


             return -1;

         }*/

        /*private bool match(byte[] haystack, byte[] needle, int start)
        {
            if (needle.Length + start > haystack.Length)
            {


                return false;
            }
            else
            {
                for (int i = 0; i < needle.Length; i++)
                {
                    if (needle[i] != haystack[i + start])
                    {
                        status = false;
                        return false;

                    }
                }
                status = true;
                return true;
            }

        }*/

        private byte[] getPositions(byte[] where, byte[] code)
        {
            positions = new List<byte>();

            for (byte i = 0; i < code.Length; i++)
            {
                for (byte j = 0; j < where.Length; j++)
                {

                    if (code[i] == where[j])
                    {

                        positions.Add(j);

                    }
                }
            }

            return positions.ToArray();
        }



    }

}
