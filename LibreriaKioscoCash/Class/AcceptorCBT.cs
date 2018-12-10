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
    public class AcceptorCBT : IAcceptor
    {
        private CCTalk ccTalk = CCTalk.GetInstance();
        private SerialPort ComboT;

        public void close()
        {
            throw new NotImplementedException();
        }

        public void disable()
        {
            throw new NotImplementedException();

        }

        public void enable()
        {
            throw new NotImplementedException();
        }

        public double getCashDesposite()
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            
        }
       


    }
}
