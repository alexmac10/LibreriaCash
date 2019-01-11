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
        private Log log = Log.GetInstance();

        public IDispenser GetBillDispenser()
        {
            IDispenser device = null;
            string name = ConfigurationManager.AppSettings.Get("BillDispenser");
            switch (name)
            {
                case "F53":
                    device = new DispenserF53();
                    log.registerLogAction("Se genera instancia de BillDispenser");
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
                    log.registerLogAction("Se genera instancia de BillAcceptor");
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
                    device = new RecyclerComboT();
                    log.registerLogAction("Se genera instancia de CoinDispenser");
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
                    device = new RecyclerComboT();
                    log.registerLogAction("Se genera instancia de CoinAcceptor");
                    break;
            }
            return device;
        }

    }
}
