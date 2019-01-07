using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Interfaces
{
    public interface IRecycler
    {
        void openAcceptor();
        void openDispenser();
        void close();
        bool isOpen();
        void enableAcceptance(); //Habilita el recibo de billetes y monedas
        void disable(); //Deshabilita el recibo de billetes y monedas
        double[] getCashDesposite();
        void returnCash(int[] count);
    }
}
