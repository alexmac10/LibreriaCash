using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using System.IO.Ports;
using System.IO;
using System.Collections;

namespace LibreriaKioscoCash.Class
{
    public class CCTalk
    {
        private Log log = Log.GetInstance();
        private SerialPort device;
        private static Hashtable Devices;

        private string COM;
        public byte[] resultmessage;
        private byte[] parameters;
        private int position;
        private bool status;
        public byte CoinAcceptor, HopperTop, HopperCenter, HopperDown, CoinBox;
        private bool conection;
        private static CCTalk instance = null;

        private CCTalk()
        {
            Devices = new Hashtable();
        }

        public static CCTalk GetInstance()
        {
            if (instance == null)
            {
                instance = new CCTalk();
            }

            return instance;
        }

        private string GetNameDevice()
        {
            var matches = ConfigurationManager.AppSettings.AllKeys.Select(t => new { Key = t, Value = ConfigurationManager.AppSettings[t] }).Where(i => i.Value == this.COM);
            foreach (var i in matches)
            {
                return i.Key.ToString();
            }
            return "";
        }

        public SerialPort openConnection(string COM)
        {
            try
            {
                this.COM = COM;
                if (Devices.ContainsKey(this.COM))
                {
                    device = (SerialPort)Devices[this.COM];
                    if (!device.IsOpen)
                    {
                        device.Open();
                        log.registerLogAction("El puerto " + this.COM + " abre conexión desde CCTAlk.");
                    }
                    log.registerLogAction("El puerto " + this.COM + " ya esta registrado y abre conexión desde CCTAlk.");
                }
                else
                {
                    device = GetNameDevice() == "COMBillDispenser" ? new SerialPort(this.COM, 9600, Parity.Even) : new SerialPort(this.COM, 9600);
                    device.Open();
                    Devices.Add(this.COM, device);
                    log.registerLogAction("El puerto " + this.COM + " abre conexión desde CCTAlk.");
                }

                return device;
            }
            catch (IOException ex)
            {
                throw new Exception(ex.Message + " : metodo openConnection  de la Class CCTalk");
            }

        }

        public void setMessage(byte[] parameters)
        {
            string TX = "TX: ";
            this.parameters = parameters;
            device = (SerialPort)Devices[this.COM];
            device.Write(parameters, 0, parameters.Length);

            for (int i = 0, j = 0; i < parameters.Length; i++, j++)
            {
                TX += parameters[i] + " ";
            }
            //Console.WriteLine(TX);
            //Console.WriteLine("TX: " + ByteArrayToString(parameters));

            Thread.Sleep(50);
        }

        public void getMessage()
        {
            string RX = "RX :";

            byte[] result = new byte[device.BytesToRead];

            device.Read(result, 0, result.Length);

            resultmessage = new byte[result.Length];
            for (int i = 0, j = 0; i < result.Length; i++, j++)
            {
                resultmessage[j] = result[i];
                RX += i + " ";
            }
            CleanEcho();
            //Console.WriteLine("RX: " + ByteArrayToString(resultmessage));
            Thread.Sleep(150);


        }

        private void CleanEcho()
        {
            string RX = "RX :";
            byte[] temp;
            search(resultmessage, this.parameters);
            if (status)
            {
                temp = new byte[this.resultmessage.Length - parameters.Length];
                for (int i = parameters.Length, j = 0; i < resultmessage.Length; i++, j++)
                {
                    temp[j] = resultmessage[i];

                }
                resultmessage = temp;


            }
            foreach (var i in resultmessage)
            {
                RX += i + " ";


            }
            //Console.WriteLine("RX: " + ByteArrayToString(resultmessage));
            //Console.WriteLine(RX);

        }

        private int search(byte[] haystack, byte[] needle)
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

        }

        private bool match(byte[] haystack, byte[] needle, int start)
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

        }

        private string ByteArrayToString(byte[] ba)
        {

            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}" + " ", b);
            return hex.ToString();
        }

        private byte[] setChecksum(byte[] parameter, byte[] data)
        {
            List<byte> list1 = new List<byte>();
            byte sum = 0;
            if (data.Length > 0)
            {
                parameter[1] = (byte)data.Length;
            }
            list1.AddRange(parameter);
            list1.AddRange(data);



            foreach (byte i in list1)
            {
                sum += i;
            }

            sum = (byte)(256 - (sum % 256));


            list1.Add(sum);
            byte[] code = list1.ToArray();

            return code;
        }

        public bool getIdDevice()
        {

            try
            {
                byte[] code = { 0, 0, 1, 253 };
                sendMessage(code);
                if (resultmessage.Length == 0)
                {
                    return conection = false;
                }
                return conection = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }





        }

        public void setDevices()
        {
            try
            {
                getIdDevice();
                foreach (var j in resultmessage)
                {
                    byte[] code = { j, 0, 1, 245 };
                    sendMessage(code);
                    var str = Encoding.Default.GetString(resultmessage);
                    
                    if (str.Contains("Coin Acceptor"))
                    {
                        CoinAcceptor = j;                        
                    }
                    else if (str.Contains("Payoutt"))
                    {
                        HopperTop = j;
                    }
                    else if (str.Contains("Payoutr"))
                    {
                        HopperCenter = j;
                    }
                    else if (str.Contains("Payoutq"))
                    {
                        HopperDown = j;
                    }
                    else if (str.Contains("Dongle"))
                    {
                        CoinBox = j;
                    }
                }
            }
            catch (IOException ex)
            {
                throw new Exception(ex.Message);
            }
            //Console.WriteLine("{0}: {1}: {2}: {3}: {4}:", CoinAcceptor, HopperTop, HopperCenter, HopperDown, CoinBox);
        }

        public void sendMessage(byte[] parameters, byte[] data = null)
        {
            data = (data == null) ? new byte[] { } : data;
            byte[] parameter = this.setChecksum(parameters, data);
            while (true)
            {
                this.setMessage(parameter);
                Thread.Sleep(350);
                this.getMessage();
                if (resultmessage.Length > 0)
                {
                    break;
                }
                else if (conection == false)
                {
                    break;
                }


            }




        }


    }
}
