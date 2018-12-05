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
        private Acceptor billAcceptor = new Acceptor();

        public override event powerUpEventHandler powerUpEvent;
        public override event connectEventHandler connectEvent;
        public override event stackEventHandler stackEvent;
        public override event powerUpCompletedEventHandler powerUpCompleteEvent;
        public override event escrowEventHandler escrowEvent;

        public override void close()
        {
            billAcceptor.Close();
        }

        public override void disable()
        {
            billAcceptor.EnableAcceptance = false;
        }

        public override void enable()
        {
            billAcceptor.EnableAcceptance = true;
            billAcceptor.AutoStack = true;
        }

        public override bool isConnection()
        {
            throw new NotImplementedException();
        }

        public override void open()
        {
            openConnection();
            configDefault();

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
                throw new Exception("No se puede conectar al puerto " + COMSCAd);
            }


        }


        public void setEvents()
        {
            billAcceptor.OnPowerUp += new PowerUpEventHandler(powerUpEvent);
            billAcceptor.OnConnected += new ConnectedEventHandler(connectEvent);
            billAcceptor.OnStacked += new StackedEventHandler(stackEvent);
            billAcceptor.OnPowerUpComplete += new PowerUpCompleteEventHandler(powerUpCompleteEvent);
            billAcceptor.OnEscrow += new EscrowEventHandler(escrowEvent);
        }

        private void configDefault()
        {
            MPOST.Bill[] bills = billAcceptor.BillValues;
            Boolean[] enables = billAcceptor.GetBillValueEnables();
            for (int i = 0; i < bills.Length; i++)
            {
                if (bills[i].Value == 1000)
                {
                    enables[i] = false;
                }
            }
            billAcceptor.SetBillValueEnables(ref enables);
        }

        //Despues de validar su funcionamiento con el metod getCashDeposite se elimina esta funcion
        //public double getDepositeBill()
        //{
        //    double bill = 0;
        //    if (billAcceptor.DocType == DocumentType.Bill)
        //    {
        //        MPOST.Bill bills = billAcceptor.Bill;
        //        bill = bills.Value;
        //    }
        //    return bill;
        //}

        public override byte[] getCashDesposite(int count)
        {
            byte[] bill = new byte[1];

            if (billAcceptor.DocType == DocumentType.Bill)
            {
                MPOST.Bill bills = billAcceptor.Bill;
                Console.WriteLine(bills.Value);
                bill[0] = (byte)bills.Value;
            }
            return bill;
        }
    }
}

