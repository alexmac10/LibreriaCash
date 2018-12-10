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
        private bool config;
        private bool stacked;
        //public override event powerUpEventHandler powerUpEvent;
        //public override event connectEventHandler connectEvent;
        //public override event stackEventHandler stackEvent;
        //public override event powerUpCompletedEventHandler powerUpCompleteEvent;
        //public override event escrowEventHandler escrowEvent;

        public AcceptorSCAd()
        {
            billAcceptor = new Acceptor();
            billAcceptor.OnConnected += connectedHandle;

            billAcceptor.OnEscrow += Stacked ;
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
            Console.WriteLine("State: " + billAcceptor.DeviceState);

            if (!billAcceptor.EnableAcceptance)
            {
                billAcceptor.EnableAcceptance = true;


            }
            else
            {
                //Console.WriteLine("State: "+billAcceptor.DeviceState);
            }


        }

        public bool isConnection()
        {
            return config;
        }

        public void open()
        {
            openConnection();





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

        public byte getCashDesposite(int count)
        {
            Console.WriteLine(billAcceptor.DeviceState);
            byte bill = new byte { };
            if (stacked==true)
            {
                if (billAcceptor.DocType == DocumentType.Bill)
                {
                    MPOST.Bill bills = billAcceptor.Bill;
                    //Console.WriteLine(bills.Value);
                    bill= (byte)bills.Value;
                    billAcceptor.EscrowStack();
                }
               
            }
            else
            {
                bill = 0;
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
            //billAcceptor.EscrowStack();
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
        private void Stacked(object sender, EventArgs e)
        {
            //Console.WriteLine("Evento : Stacked");
            stacked= true;
        }

    }
}

