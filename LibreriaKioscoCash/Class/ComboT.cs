using LibreriaKioscoCash.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Class
{
    class ComboT : IRecycler
    {

        private CommunicationProtocol ccTalk= CommunicationProtocol.GetInstance();
        private SerialPort Recycler;
        private List<byte> Sensors;
        private byte count_actual = 0;
        //Funciones de la interfaz

        public void openAcceptor()
        {
            try
            {
                string COM = ConfigurationManager.AppSettings.Get("COMRecycler");
                Recycler = ccTalk.openConnection(COM);
                

                if (isOpen())
                {
                    ccTalk.setDevices();
                    clearCounterMoney();
                    setInibitCoins();
                    setConfigDefaultHoppers();
                    count_actual = 0;
                }
                else
                {
                    throw new Exception("Error: Dispositivo Desconectado");
                }                                                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public void openDispenser()
        {
       
     try
            {
                string COM = ConfigurationManager.AppSettings.Get("COMRecycler");
                Recycler = ccTalk.openConnection(COM);


                if (isOpen())
                {
                    ccTalk.getIdDevice();

                }
                else
                {
                    throw new Exception("Error: Dispositivo Desconectado");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void close()
        {
            if (Recycler.IsOpen)
            {
                Recycler.Close();
            }
        }

        public bool isOpen()
        {
            return Recycler.IsOpen;

        }

        public void enableAcceptance()
        {
            byte[] data = { 4 };
            byte[] parameter = { this.ccTalk.CoinAcceptor, 0, 1, 229 };
            this.ccTalk.sendMessage(parameter, data);
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
            //Console.WriteLine("Retirando Efectivo ...");
            //Console.WriteLine("");
            foreach(var j in count)
            {
                if (count[0] > 0)
                {
                    this.enableContainerCoin(this.ccTalk.HopperDown);
                    byte[] serie = this.getNumberSerie(this.ccTalk.HopperDown);
                    serie[3] = (byte)count[0];
                    byte[] code = { this.ccTalk.HopperDown, 0, 1, 167 };
                    this.ccTalk.sendMessage(code, serie);
                    count[0] = 0;
                }
                else if (count[1] > 0)
                {

                    this.enableContainerCoin(this.ccTalk.HopperCenter);
                    byte[] serie = this.getNumberSerie(this.ccTalk.HopperCenter);
                    serie[3] = (byte)count[1];
                    byte[] code = { this.ccTalk.HopperCenter, 0, 1, 167 };
                    this.ccTalk.sendMessage(code, serie);
                    count[1] = 0;
                }
                else if (count[2] > 0)
                {

                    this.enableContainerCoin(this.ccTalk.HopperTop);
                    byte[] serie = this.getNumberSerie(this.ccTalk.HopperTop);
                    serie[3] = (byte)count[2];
                    byte[] code = { this.ccTalk.HopperTop, 0, 1, 167 };
                    this.ccTalk.sendMessage(code, serie);
                    count[2] = 0;
                }
            }

            
            
           
        }


        //Metodos de la clase

        // Encargado de obtener los numero de serie del dispositvo        
        private void enableContainerCoin(byte device)
        {
            byte[] code = { device, 0, 1, 164 };
            byte[] data = {165};
            this.ccTalk.sendMessage(code, data);

        }

        private byte[] getNumberSerie(byte device)
        {
            byte[] serie = new byte[4];
            byte[] code = { device, 0, 1, 242 };

            this.ccTalk.sendMessage(code);

            for (int i = 4, j = 0; i < this.ccTalk.resultmessage.Length - 1; i++, j++)
            {
                serie[j] = this.ccTalk.resultmessage[i];
            }
            return serie;
        }

        private void checkStatusSensors()
        {
            Console.WriteLine("Checando Estatus de los Contenedores....");
            byte[] device = { this.ccTalk.HopperTop, this.ccTalk.HopperCenter, this.ccTalk.HopperDown };
            string[] name_device = { "Conetenedor Superior", "Contenedor Central", "Contenedor Inferior" };
            Sensors = new List<byte>();
            for (byte i = 0; i < device.Length; i++)
            {
                byte[] code = { device[i], 0, 1, 236 };
                this.ccTalk.sendMessage(code);
                Sensors.Add(ccTalk.resultmessage[4]);

            }

            if ((Sensors[0] == 3) || (Sensors[1] == 3) || (Sensors[2] == 3))
            {
                throw new Exception("Error: No hay cambio en monedas en ninguno de los contenedores");
            }
        }

        private void setConfigDefaultHoppers()
        {
            byte[] parameter = { this.ccTalk.CoinAcceptor, 0, 1, 210 };

            this.ccTalk.sendMessage(parameter, new byte[] { 4, 2 });
            this.ccTalk.sendMessage(parameter, new byte[] { 5, 0 });
            this.ccTalk.sendMessage(parameter, new byte[] { 6, 3 });
            this.ccTalk.sendMessage(parameter, new byte[] { 7, 5 });
            this.ccTalk.sendMessage(parameter, new byte[] { 8, 5 });

        }

        private void setInibitCoins()
        {
            byte[] data = { 248, 255 };
            byte[] parameter = { this.ccTalk.CoinAcceptor, 0, 1, 231 };
            this.ccTalk.sendMessage(parameter, data);

        }

        private void clearCounterMoney()
        {
            byte[] parameter = { this.ccTalk.CoinAcceptor, 0, 1, 1 };
            this.ccTalk.sendMessage(parameter);
        }

        private void emptyMoneyBox()
        {
            //si data = 1 se vacia por atras
            //si data = 2 se vacia por enfrente
            byte[] parameter = { this.ccTalk.CoinBox, 0, 1, 70 };
            byte[] data = { 1 };
            this.ccTalk.sendMessage(parameter, data);

        }

        
    }
}
