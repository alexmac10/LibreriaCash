using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Interfaces
{
    public abstract class IAcceptor
    {
        public delegate void powerUpEventHandler(object sender, EventArgs e);
        public delegate void connectEventHandler(object sender, EventArgs e);
        public delegate void stackEventHandler(object sender, EventArgs e);
        public delegate void powerUpCompletedEventHandler(object sender, EventArgs e);
        public delegate void escrowEventHandler(object sender, EventArgs e);
        public virtual event powerUpEventHandler powerUpEvent;
        public virtual event connectEventHandler connectEvent;
        public virtual event stackEventHandler stackEvent;
        public virtual event powerUpCompletedEventHandler powerUpCompleteEvent;
        public virtual event escrowEventHandler escrowEvent;

        public abstract void open();
        public abstract void close();
        public abstract bool isConnection();
        public abstract void enable(); //Habilita el recibo de billetes y monedas
        public abstract void disable(); //Deshabilita el recibo de billetes y monedas

        public abstract byte [] getCashDesposite(int count);


    }
}
