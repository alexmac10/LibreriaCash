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

            if (!billAcceptor.EnableAcceptance)
            {
                billAcceptor.EnableAcceptance = true;


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

        public double getCashDesposite()
        {
            //Console.WriteLine(billAcceptor.DeviceState);
            double bill=0 ;
            if (billAcceptor.DeviceState==State.Escrow)
            {
                if (billAcceptor.DocType == DocumentType.Bill)
                {
                    MPOST.Bill bills = billAcceptor.Bill;
                    //Console.WriteLine(bills.Value);
                    bill= bills.Value;
                    billAcceptor.EscrowStack();
                    
                    
                }
               
            }
            else
            {
                bill = 0;
            }
            Thread.Sleep(400);
            return bill;
        }




        private void connectedHandle(object sender, EventArgs e)
        {
            //Console.WriteLine(billAcceptor.DeviceState);

            if (billAcceptor.DeviceState == State.Idling)
            {
                
                configDefault();
                config = true;

                
            }
            if (billAcceptor.DeviceState == State.Escrow)
            {
                billAcceptor.EscrowStack();
                configDefault();
                config = true;



            }


        }




    }
}

