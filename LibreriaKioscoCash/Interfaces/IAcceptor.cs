using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Interfaces
{
    interface IAcceptor
    {
        void setConfig();//Establece la configuración de monedas a reciclar y aceptar, tambien los billetes que aceptara (denominacion).
        bool validate();
        void enable(); //Habilita el recibo de billetes y monedas
        void disable(); //Deshabilita el recibo de billetes y monedas
    }
}
