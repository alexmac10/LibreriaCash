using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Interfaces
{
    interface IAcceptor
    {
        void enable(); //Habilita el recibo de billetes y monedas
        void disable(); //Deshabilita el recibo de billetes y monedas
        void open();
        void close();
        bool isConnection();
    }
}
