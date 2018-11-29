using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Interfaces
{
    interface IDispenser
    {
        void returnCash(int denominationCash, int countMoney, int cantidad_20, int cantidad_50, int cantidad_100);
    }
}
