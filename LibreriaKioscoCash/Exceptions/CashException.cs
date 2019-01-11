using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Exceptions
{
    public class CashException : Exception
    {
        private byte[] countCash;

        public CashException(byte[] count)
        {

            countCash = count;
        }

        public byte[] getInformationCashNotDeliveredException()
        {            
            return countCash;
        }
    }
}
