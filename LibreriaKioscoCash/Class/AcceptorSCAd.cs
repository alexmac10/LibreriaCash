﻿using LibreriaKioscoCash.Interfaces;
using LibreriaKioscoCash.Class;
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
using System.Collections;

namespace LibreriaKioscoCash
{
    public class AcceptorSCAd : IAcceptor
    {
        private Acceptor billAcceptor;
        private Log log = Log.GetInstance();

        public AcceptorSCAd()
        {
            billAcceptor = new Acceptor();
            billAcceptor.OnConnected += connectedHandle;
        }

        #region Funciones de la interfaz

        public void open()
        {
            string COMSCAd = ConfigurationManager.AppSettings.Get("COMBillAcceptor");
            try
            {
                billAcceptor.Open(COMSCAd, PowerUp.A);
                log.registerLogAction("Abriendo conexion con SCAd : onConnnect " + billAcceptor.Connected);
            }
            catch (IOException ex)
            {
                log.registerLogError("Error con puerto (" + ex.Message + @") : Class\AcceptorSCAd\open()", "300");
                throw new Exception(ex.Message);
            }
        }

        public void close()
        {
            try
            {
                billAcceptor.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("AcceptorSCAd error : " + ex.Message);
            }
        }

        public bool isConnection()
        {
            log.registerLogAction("onConnnect :  " + billAcceptor.Connected);
            if (billAcceptor.Connected)
            {
                //log.registerLogAction("El dispositivo SCAd esta Conectado");
                return true;
            }

            //log.registerLogAction("El dispositivo SCAd esta Desconectado");
            return false;
        }

        public void enable()
        {
            try
            {
                if (!billAcceptor.EnableAcceptance)
                {
                    billAcceptor.EnableAcceptance = true;
                    log.registerLogAction("Habilitado el dispositivo SCAd");
                }
            }
            catch (Exception ex)
            {
                log.registerLogError("No es posible habilitar el dispositivo SCAd (" + ex.Message + ") : metodo enable de la Class AcceptorSCAd", "301");
                throw new Exception("No es posible habilitar el dispositivo SCAd");
            }
        }

        public void disable()
        {
            billAcceptor.EnableAcceptance = false;
            log.registerLogAction("Deshabilitado el dispositivo SCAd");
        }

        public double[] getCashDesposite()
        {
            double[] bill = new double[1];
            if (billAcceptor.DeviceState == State.Escrow)
            {
                if (billAcceptor.DocType == DocumentType.Bill)
                {
                    MPOST.Bill bills = billAcceptor.Bill;
                    bill[0] = bills.Value;
                    billAcceptor.EscrowStack();
                    log.registerLogAction("Recibe $" + bill[0] + " el dispositivo SCAd");
                }
            }
            else
            {
                bill[0] = 0;
            }

            Thread.Sleep(400);
            return bill;
        }

        #endregion

        #region Metodos de la clase

        private void connectedHandle(object sender, EventArgs e)
        {
            log.registerLogAction("Evento : DeviceState " + billAcceptor.DeviceState);

            if (billAcceptor.DeviceState == State.Idling)
            {
                configDefault();
            }
            if (billAcceptor.DeviceState == State.Escrow)
            {
                billAcceptor.EscrowStack();
                configDefault();
            }
            log.registerLogAction("Esta evento de que conectado para SCAd");
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
            log.registerLogAction("Estableciendo configuración para SCAd");
        }

        #endregion




    }
}

