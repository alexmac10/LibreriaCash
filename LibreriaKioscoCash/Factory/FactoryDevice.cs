using LibreriaKioscoCash.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreriaKioscoCash.Class;
using System.Configuration;

namespace LibreriaKioscoCash.Factory
{
    public class FactoryDevice
    {
        public IDispenser GetDeviceDispenserBill()
        {
            IDispenser device = null;
            string name = ConfigurationManager.AppSettings.Get("DispenserBill");
            switch(name)
            {
                case "F53":
                    device =  new DispenserF53();
                    break;
            }
            return device;

        }
    }
}
