using LibreriaKioscoCash.Class;
using LibreriaKioscoCash.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Factory
{
   public class FactoryDevice
    {
        public IDispenser GetBillDispenser()
        {
            IDispenser device = null;
            string name = ConfigurationManager.AppSettings.Get("BillDispenser");
            switch (name)
            {
                case "F53":
                    device = new DispenserF53();
                    break;
            }
            return device;

        }
        public IAcceptor GetBillAcceptor()
        {
            IAcceptor device = null;
            string name = ConfigurationManager.AppSettings.Get("BillAcceptor");
            switch (name)
            {
                case "SCAdvance":
                    device = new AcceptorSCAd();
                    break;
            }
            return device;
        }
        public IDispenser GetCoinDispenser()
        {
            IDispenser device = null;
            string name = ConfigurationManager.AppSettings.Get("CoinDispenser");
            switch (name)
            {
                case "ComboT":
                    device = new DispenserCBT();
                    break;
            }
            return device;

        }
        public IAcceptor GetCoinAcceptor()
        {
            IAcceptor device = null;
            string name = ConfigurationManager.AppSettings.Get("CoinAcceptor");
            switch (name)
            {
                case "ComboT":
                    device = new AcceptorCBT();
                    break;
            }
            return device;
        }

    }
}
