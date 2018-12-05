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
        CCTalk ccTalk = new CCTalk();

        public override void close()
        {
            throw new NotImplementedException();
        }

       

        public override void disable()
        {
            throw new NotImplementedException();
            
        }

       

        public override void enable()
        {
            throw new NotImplementedException();
        }

        public override byte[] getCashDesposite(int count)
        {
            throw new NotImplementedException();
        }

        public override bool isConnection()
        {
            throw new NotImplementedException();
        }

        

        public override void open()
        {
            throw new NotImplementedException();
        }
    }
}
