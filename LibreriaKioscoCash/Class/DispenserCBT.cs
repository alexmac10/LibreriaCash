using LibreriaKioscoCash.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Class
{
    class DispenserCBT : IDispenser
    {

        private CCTalk ccTalk= CCTalk.GetInstance();
        private SerialPort ComboT;

        public void close()
        {
            throw new NotImplementedException();
        }

        public bool isConnection()
        {
            throw new NotImplementedException();
        }

        public void open()
        {
            try
            {
                string COM = ConfigurationManager.AppSettings.Get("COMComboT");
                ComboT = ccTalk.openConnection(COM);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void returnCash(int denominationCash, int countMoney, int[] BillCount)
        {
            throw new NotImplementedException();
        }
    }
}
