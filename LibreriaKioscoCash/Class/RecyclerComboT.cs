using LibreriaKioscoCash.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Class
{


    class RecyclerComboT : IAcceptor, IDispenser
    {
        private Log log = Log.GetInstance();
        private CommunicationProtocol ccTalk = CommunicationProtocol.GetInstance();
        private SerialPort Recycler;
        private string COM;
        private List<byte> Sensors;
        private byte count_actual = 0;
        private byte CoinAcceptor, HopperTop, HopperCenter, HopperDown, CoinBox;
        private bool conection;

        //Funciones de la interfaz
        public void open()
        {
            try
            {
                COM = ConfigurationManager.AppSettings.Get("COMComboT");
                Recycler = ccTalk.openConnection(COM);
                log.registerLogAction("Abriendo conexion con ComboT");                

                if (Recycler.IsOpen)
                {                    
                    setDevices();
                    clearCounterMoney();
                    setInibitCoins();
                    setConfigDefaultHoppers();
                    count_actual = 0;
                }

            }
            catch (Exception ex)
            {
                log.registerLogError("Error con puerto (" + ex.Message + @") : Class\RecyclerComboT\open()", "300");
                throw new Exception(ex.Message);
            }
        }

        public void close()
        {
            Recycler.Close();
            ccTalk.close(this.COM);
        }

        public bool isConnection()
        {
            return Recycler.IsOpen;
        }

        public void enable()
        {
            byte[] data = { 4 };
            byte[] parameter = { CoinAcceptor, 0, 1, 229 };
            sendMessage(parameter, data);
        }

        public void disable()
        {
            setInibitCoins();
            setConfigDefaultHoppers();
            count_actual = 0;
        }

        public double[] getCashDesposite()
        {
            double[] money = new double[2];
            if (count_actual != this.ccTalk.resultmessage[4])
            {
                switch (this.ccTalk.resultmessage[5])
                {
                    case 8:
                        money[0] = 10;
                        count_actual = this.ccTalk.resultmessage[4];
                        break;

                    case 7:
                        money[0] = 10;
                        count_actual = this.ccTalk.resultmessage[4];

                        break;
                    case 6:
                        money[0] = 5;
                        count_actual = this.ccTalk.resultmessage[4];


                        break;
                    case 5:
                        money[0] = 2;
                        count_actual = this.ccTalk.resultmessage[4];
                        emptyMoneyBox();


                        break;
                    case 4:
                        money[0] = 1;
                        count_actual = this.ccTalk.resultmessage[4];

                        break;
                }


                money[1] = count_actual;
            }
            return money;
        }

        public void returnCash(int[] count)
        {
            foreach (var j in count)
            {
                if (count[0] > 0)
                {
                    this.enableContainerCoin(HopperDown);
                    byte[] serie = this.getNumberSerie(HopperDown);
                    serie[3] = (byte)count[0];
                    byte[] code = { HopperDown, 0, 1, 167 };
                    sendMessage(code, serie);
                    count[0] = 0;
                }
                else if (count[1] > 0)
                {
                    this.enableContainerCoin(HopperCenter);
                    byte[] serie = this.getNumberSerie(HopperCenter);
                    serie[3] = (byte)count[1];
                    byte[] code = { HopperCenter, 0, 1, 167 };
                    sendMessage(code, serie);
                    count[1] = 0;
                }
                else if (count[2] > 0)
                {

                    this.enableContainerCoin(HopperTop);
                    byte[] serie = this.getNumberSerie(HopperTop);
                    serie[3] = (byte)count[2];
                    byte[] code = { HopperTop, 0, 1, 167 };
                    sendMessage(code, serie);
                    count[2] = 0;
                }
            }
        }


        //Metodos de la clase

        private void enableContainerCoin(byte device)
        {
            byte[] code = { device, 0, 1, 164 };
            byte[] data = { 165 };
            sendMessage(code, data);
        }

        private byte[] getNumberSerie(byte device)
        {
            byte[] serie = new byte[4];
            byte[] code = { device, 0, 1, 242 };

            sendMessage(code);

            for (int i = 4, j = 0; i < this.ccTalk.resultmessage.Length - 1; i++, j++)
            {
                serie[j] = this.ccTalk.resultmessage[i];
            }
            return serie;
        }

        private void checkStatusSensors()
        {
            Console.WriteLine("Checando Estatus de los Contenedores....");
            byte[] device = { HopperTop, HopperCenter, HopperDown };
            string[] name_device = { "Conetenedor Superior", "Contenedor Central", "Contenedor Inferior" };
            Sensors = new List<byte>();
            for (byte i = 0; i < device.Length; i++)
            {
                byte[] code = { device[i], 0, 1, 236 };
                sendMessage(code);
                Sensors.Add(ccTalk.resultmessage[4]);
            }

            if ((Sensors[0] == 3) || (Sensors[1] == 3) || (Sensors[2] == 3))
            {
                throw new Exception("Error: No hay cambio en monedas en ninguno de los contenedores");
            }
        }

        private void setConfigDefaultHoppers()
        {
            byte[] parameter = { CoinAcceptor, 0, 1, 210 };

            sendMessage(parameter, new byte[] { 4, 2 });
            sendMessage(parameter, new byte[] { 5, 0 });
            sendMessage(parameter, new byte[] { 6, 3 });
            sendMessage(parameter, new byte[] { 7, 5 });
            sendMessage(parameter, new byte[] { 8, 5 });

        }

        private void setInibitCoins()
        {
            byte[] data = { 248, 255 };
            byte[] parameter = { CoinAcceptor, 0, 1, 231 };
            sendMessage(parameter, data);
        }

        private void clearCounterMoney()
        {
            byte[] parameter = { CoinAcceptor, 0, 1, 1 };
            sendMessage(parameter);
        }

        private void emptyMoneyBox()
        {
            //si data = 1 se vacia por atras
            //si data = 2 se vacia por enfrente
            byte[] parameter = { CoinBox, 0, 1, 70 };
            byte[] data = { 1 };
            sendMessage(parameter, data);

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
                if (ccTalk.resultmessage.Length == 0)
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
                foreach (var j in ccTalk.resultmessage)
                {
                    byte[] code = { j, 0, 1, 245 };
                    sendMessage(code);
                    var str = Encoding.Default.GetString(ccTalk.resultmessage);

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
                ccTalk.setMessage(parameter);
                Thread.Sleep(350);
                ccTalk.getMessage();
                if (ccTalk.resultmessage.Length > 0)
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
