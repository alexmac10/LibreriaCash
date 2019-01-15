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
        private Log log = Log.GetInstance();
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


        #region Funciones de la interfaz

        public void open()
        {
            try
            {
                COM = ConfigurationManager.AppSettings.Get("COMBillDispenser");
                F53 = ccTalk.openConnection(COM);
                log.registerLogAction("Abriendo conexion con F53");
                checkConfig();
            }
            catch (Exception ex)
            {
                log.registerLogError("Error con puerto (" + ex.Message + @") : Class\DispenserF53\open()", "300");
                throw new Exception(ex.Message);
            }
        }

        public void close()
        {
            F53.Close();
        }

        public bool isConnection()
        {
            if (F53.IsOpen)
            {
                //log.registerLogAction("El dispositivo F53 esta Conectado");
                return true;
            }
            //log.registerLogAction("El dispositivo F53 esta Desconectado");
            return false;
        }

        public void enable()
        {

        }

        public void returnCash(int[] count)
        {
            int sum = count[0] + count[1] + count[2];
            money = true;

            DispenserBill[5] = IsoCodes[count[0]];
            DispenserBill[7] = IsoCodes[count[1]];
            DispenserBill[9] = IsoCodes[count[2]];

            if (sum > 15)
            {
                throw new CashException(new byte[] { 0, 0, 0 });
            }

            createCode(DispenserBill);
            sendMessage(mensaje_final);

            this.searchError(count);
        }

        #endregion

        #region Metodos de la clase

        private void checkConfig()
        {
            try
            {
                createCode(StatusInformation);
                sendMessage(mensaje_final);
                this.validateCassets();
                this.validateConfig();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void createCode(byte[] fun)
        {
            ushort[] table = new ushort[256];
            ushort polynomial = (ushort)0x8408;
            ushort value;
            ushort temp;
            List<byte> list1 = new List<byte>();

            try
            {
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
            }
            catch (Exception ex)
            {
                log.registerLogError("Error en el metodo createCode de la Class DispenserF53", "302");
                throw new Exception("Error crear código :" + ex.Message);
            }

        }

        private void sendMessage(byte[] parameter)
        {
            ccTalk.setMessage(statusRequest);
            ccTalk.getMessage();
            ccTalk.search(ccTalk.resultmessage, releaseRequest);

            if ((ccTalk.status == true) && (ccTalk.resultmessage.Length == 2))
            {
                this.launchActionDevice(parameter);
            }
            else
            {
                setFreeDevice(); //Dispositivo ocupada y liberando
                this.launchActionDevice(parameter);
            }
        }

        private void launchActionDevice(byte[] parameter)
        {
            if (money == true)
            {
                this.setStructureDevice(parameter, 9000);
            }
            else if (config == true)
            {
                this.setStructureDevice(parameter, 5000);
            }
            else
            {
                //otros peticiones que no sean config y returncash
                this.setStructureDevice(parameter, 0);
            }
        }

        private void setStructureDevice(byte[] parameter, int time)
        {
            ccTalk.setMessage(parameter);
            Thread.Sleep(200);
            ccTalk.getMessage();
            Thread.Sleep(time);
            ccTalk.getMessage();
            ccTalk.setMessage(releaseRequest);
            Thread.Sleep(200);
            ccTalk.getMessage();
            ccTalk.setMessage(releaseRequest);
        }

        private void validateCassets()
        {
            byte[] cassets = new byte[] { 0x8C, 0x89, 0x8A };
            ccTalk.search(ccTalk.resultmessage, cassets);

            if (ccTalk.status == false)
            {
                log.registerLogError("Cassets no estan colocados", "303");
                throw new Exception("Cassets no estan colocados");
            }
        }

        private void validateConfig()
        {
            byte[] config = new byte[] { 130, 110, 137, 117, 147, 127 };
            ccTalk.search(ccTalk.resultmessage, config);
            if (ccTalk.status == false)
            {
                config_inicial();
            }
        }

        private void config_inicial()
        {
            config = true;
            createCode(ConfigDefault);
            sendMessage(mensaje_final);
        }

        private void searchError(int[] count)
        {
            byte[] bill_codes = { };
            byte[] error_answer = { 0x8c };
            ccTalk.search(ccTalk.resultmessage, error_answer);

            Error = new List<byte>();
            for (int i = 7, m = 0; i < ccTalk.position; i++, m++)
            {
                Error.Add(ccTalk.resultmessage[i]);
            }


            if (!((Error[0] == 0) && (Error[1] == 0)))
            {
                if ((count[0] <= 2) && (count[1] <= 2) && (count[2] <= 2))
                {
                    bill_codes = new byte[] { ccTalk.resultmessage[46], ccTalk.resultmessage[48], ccTalk.resultmessage[50] };
                }
                else
                {
                    bill_codes = new byte[] { ccTalk.resultmessage[44], ccTalk.resultmessage[46], ccTalk.resultmessage[48] };
                }

                byte[] entrega = getPositions(IsoCodes, bill_codes);
                log.registerLogError("No se entrego todo el efectivo", "303");
                throw new CashException(entrega);
            }
        }

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

        //Funciones del Dispositivo [Codigo Completo] por el momento no son utilizadas      
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

            createCode(StatusDeviceInit);
            sendMessage(mensaje_final);
            byte[] positive_answer = { 0x1C, 0x10, 0x03 };
            ccTalk.search(ccTalk.resultmessage, positive_answer);

            Sensors = new List<byte>();
            for (int l = 43, m = 0; l < ccTalk.position; l++, m++)
            {
                Sensors.Add(ccTalk.resultmessage[l]);
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

        #endregion

    }

}
