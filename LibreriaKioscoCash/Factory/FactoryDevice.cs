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
        public IDispenser GetDeviceDispenserBill()
        {
            IDispenser device = null;
            string name = ConfigurationManager.AppSettings.Get("DispenserBill");
            switch (name)
            {
                case "F53":
                    device = new DispenserF53();
                    break;
            }
            return device;

        }

    }
}
