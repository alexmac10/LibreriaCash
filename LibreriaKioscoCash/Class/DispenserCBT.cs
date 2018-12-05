using LibreriaKioscoCash.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Class
{
    class DispenserCBT : CCTalk, IDispenser
    {
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
            throw new NotImplementedException();
        }

        public void returnCash(int denominationCash, int countMoney, int[] BillCount)
        {
            throw new NotImplementedException();
        }
    }
}
