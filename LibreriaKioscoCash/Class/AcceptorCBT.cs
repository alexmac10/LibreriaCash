using LibreriaKioscoCash.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Class
{
    public class AcceptorCBT : IAcceptor
    {
        CCTalk cctalk = new CCTalk();
        public void close()
        {
            throw new NotImplementedException();
        }

        public void disable()
        {
            throw new NotImplementedException();
        }

        public void enable()
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
    }
}
