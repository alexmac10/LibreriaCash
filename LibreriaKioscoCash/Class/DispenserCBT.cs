﻿using LibreriaKioscoCash.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibreriaKioscoCash.Class
{
    class DispenserCBT : IDispenser
    {

        private CCTalk ccTalk= CCTalk.GetInstance();
        private SerialPort ComboT;
        private List<byte> Sensors;

        //Funciones de la interfaz

        public void close()
        {

  
        }

        public bool isConnection()
        {
            return ComboT.IsOpen;

        }

        public void open()
        {
            try
            {
                string COM = ConfigurationManager.AppSettings.Get("COMComboT");
                ComboT = ccTalk.openConnection(COM);
                
                if (ComboT.IsOpen)
                {

                    ccTalk.getIdDevice();
                }
                else
                {
                    throw new Exception("Error: Dispositivo Desconectado");
                }
                                                            
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public void returnCash(int[] count)
        {
            //Console.WriteLine("Retirando Efectivo ...");
            //Console.WriteLine("");
            foreach(var j in count)
            {
                if (count[0] > 0)
                {
                    this.enableContainerCoin(this.ccTalk.HopperDown);
                    byte[] serie = this.getNumberSerie(this.ccTalk.HopperDown);
                    serie[3] = (byte)count[0];
                    byte[] code = { this.ccTalk.HopperDown, 0, 1, 167 };
                    this.ccTalk.sendMessage(code, serie);
                    count[0] = 0;
                }
                else if (count[1] > 0)
                {

                    this.enableContainerCoin(this.ccTalk.HopperCenter);
                    byte[] serie = this.getNumberSerie(this.ccTalk.HopperCenter);
                    serie[3] = (byte)count[1];
                    byte[] code = { this.ccTalk.HopperCenter, 0, 1, 167 };
                    this.ccTalk.sendMessage(code, serie);
                    count[1] = 0;
                }
                else if (count[2] > 0)
                {

                    this.enableContainerCoin(this.ccTalk.HopperTop);
                    byte[] serie = this.getNumberSerie(this.ccTalk.HopperTop);
                    serie[3] = (byte)count[2];
                    byte[] code = { this.ccTalk.HopperTop, 0, 1, 167 };
                    this.ccTalk.sendMessage(code, serie);
                    count[2] = 0;
                }
            }

            
            
           
        }

        private void enableContainerCoin(byte device)
        {
            byte[] code = { device, 0, 1, 164 };
            byte[] data = {165};
            this.ccTalk.sendMessage(code, data);

        }

        //Metodos de la clase
        

        // Encargado de obtener los numero de serie del dispositvo        
        private byte[] getNumberSerie(byte device)
        {
            byte[] serie = new byte[4];
            byte[] code = { device, 0, 1, 242 };

            this.ccTalk.sendMessage(code);

            for (int i = 4, j = 0; i < this.ccTalk.resultmessage.Length - 1; i++, j++)
            {
                serie[j] = this.ccTalk.resultmessage[i];
            }
            return serie;
        }

        private void checkStatusSensors()
        {
            Console.WriteLine("Checando Estatus de los Contenedores....");
            byte[] device = { this.ccTalk.HopperTop, this.ccTalk.HopperCenter, this.ccTalk.HopperDown };
            string[] name_device = { "Conetenedor Superior", "Contenedor Central", "Contenedor Inferior" };
            Sensors = new List<byte>();
            for (byte i = 0; i < device.Length; i++)
            {
                byte[] code = { device[i], 0, 1, 236 };
                this.ccTalk.sendMessage(code);
                Sensors.Add(ccTalk.resultmessage[4]);

            }

            if ((Sensors[0] == 3) || (Sensors[1] == 3) || (Sensors[2] == 3))
            {
                throw new Exception("Error: No hay cambio en monedas en ninguno de los contenedores");
            }
        }

        public void enable()
        {
            try
            {
                string COM = ConfigurationManager.AppSettings.Get("COMComboT");
                ComboT = ccTalk.openConnection(COM);
                //checkStatusSensors();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
