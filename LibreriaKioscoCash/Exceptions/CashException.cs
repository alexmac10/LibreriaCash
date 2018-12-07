using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Exceptions
{
    public class CashException : Exception
    {
        private int[] countCash;

        public CashException(int[] count)
        {
            countCash = count;
        }

        public int[] getInformationCashNotDeliveredException()
        {
            return countCash;
        }
    }
}
