using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Interfaces
{
    interface IDevice
    {
        void open(); //Abre la conexion con el dispositvo
        bool isConnection(); //Regresa el valor de la conexion actual  
        void close();
        void send(byte[] parameters);
        void receive();
        void isError(byte[] parameters);


    }
}
