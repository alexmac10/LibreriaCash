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
    public class AcceptorCBT : IAcceptor
    {
        private CCTalk ccTalk = CCTalk.GetInstance();
        private SerialPort ComboT;
        private string COM;
        private byte count_actual = 0;


        //Funciones de la interfaz

        public void open()
        {
            try
            {
                COM = ConfigurationManager.AppSettings.Get("COMComboT");
                ComboT = ccTalk.openConnection(COM);
                if (isConnection())
                {
                    Console.WriteLine("Configurando ...");
                    Console.WriteLine("");
                    ccTalk.setDevices();
                    clearCounterMoney();
                    setInibitCoins();
                    setConfigDefaultHoppers();
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
            
        }

        public bool isConnection()
        {
            try
            {

                if (!ccTalk.getIdDevice())
                {
                    return false;
                    throw new Exception("Error: Dispositivo No Conectado");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void enable()
        {
            byte[] data = { 4 };
            byte[] parameter = { this.ccTalk.CoinAcceptor, 0, 1, 229 };
            this.ccTalk.sendMessage(parameter, data);
        }

        public void disable()
        {
            clearCounterMoney();
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

        //Metodos de la clase

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
