using LibreriaKioscoCash.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPOST;
using System.Management;
using System.IO;
using System.Configuration;
using System.Threading;

namespace LibreriaKioscoCash
{
    public class AcceptorSCAd : IAcceptor
    {
        private Acceptor billAcceptor;
        bool config;
        //public override event powerUpEventHandler powerUpEvent;
        //public override event connectEventHandler connectEvent;
        //public override event stackEventHandler stackEvent;
        //public override event powerUpCompletedEventHandler powerUpCompleteEvent;
        //public override event escrowEventHandler escrowEvent;

        public AcceptorSCAd()
        {
            billAcceptor = new Acceptor();
            billAcceptor.OnConnected += connectedHandle;
        }
        public void close()
        {
            billAcceptor.Close();
        }

        public void disable()
        {
            billAcceptor.EnableAcceptance = false;
        }

        public void enable()
        {

            billAcceptor.EnableAcceptance = true;
            billAcceptor.AutoStack = true;


           
        }

        public bool isConnection()
        {
            return config;
        }

        public void open()
        {
            //setEvents();
            openConnection();
            //Console.WriteLine(billAcceptor.DeviceState);
            //connectedHandle(new object(), new EventArgs());
            //connectEvent(new object(),new EventArgs());

            //enable();
            //Console.WriteLine(billAcceptor.DeviceState);




        }



        private void openConnection()
        {
            string COMSCAd = ConfigurationManager.AppSettings.Get("COMBillAcceptor");
            try
            {
                billAcceptor.Open(COMSCAd, PowerUp.A);
            }
            catch (IOException ex)
            {
                throw new Exception("Unable to open the bill acceptor on com port <" + COMSCAd + "> " + ex.Message + "Open Bill Acceptor Error");
            }


        }


        private void configDefault()
        {
            //Console.WriteLine("Configurando desde configdefault");
            MPOST.Bill[] bills = billAcceptor.BillValues;
            Boolean[] enables = billAcceptor.GetBillValueEnables();
            for (int i = 0; i < bills.Length; i++)
            {
                if (bills[i].Value == 1000)
                {
                    enables[i] = false;
                }
                //Console.WriteLine("{0} :: {1}", bills[i].Value, enables[i]);
            }
            billAcceptor.SetBillValueEnables(ref enables);
        }

        public byte[] getCashDesposite(int count)
        {
            byte[] bill = new byte[1];

            if (billAcceptor.DocType == DocumentType.Bill)
            {
                MPOST.Bill bills = billAcceptor.Bill;
                //Console.WriteLine(bills.Value);
                bill[0] = (byte)bills.Value;
            }
            return bill;
        }


        private void powerUpHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Manejador de POWER UP");
        }

        private void connectedHandle(object sender, EventArgs e)
        {
            //Console.WriteLine("Evento : Configuracion");
            //Console.WriteLine(billAcceptor.DeviceState);
            if (billAcceptor.DeviceState == State.Idling)
            {
                configDefault();
                config = true;
                
            }
            //Console.WriteLine(billAcceptor.DeviceState);

        }

        private void PowerUpCompletedHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Evento : POWERUP_COMPLETED");
        }

        private void escrowHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Evento : ESCROW");
        }
    }
}

