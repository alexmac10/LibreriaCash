﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using System.IO.Ports;
using System.IO;
using System.Collections;

namespace LibreriaKioscoCash.Class
{
    class CommunicationProtocol
    {
        private Log log = Log.GetInstance();
        private SerialPort device;
        private static Hashtable Devices;

        private string COM;
        public byte[] resultmessage;
        private byte[] parameters;
        public int position;
        public bool status;        

        private static CommunicationProtocol instance = null;

        private CommunicationProtocol()
        {
            Devices = new Hashtable();
        }

        public static CommunicationProtocol GetInstance()
        {
            if (instance == null)
            {

                instance = new CommunicationProtocol();
            }

            return instance;
        }

        private string GetNameDevice()
        {
            var matches = ConfigurationManager.AppSettings.AllKeys.Select(t => new { Key = t, Value = ConfigurationManager.AppSettings[t] }).Where(i => i.Value == this.COM);
            foreach (var i in matches)
            {
                return i.Key.ToString();
            }
            return "";
        }

        public SerialPort openConnection(string COM)
        {
            try
            {
                this.COM = COM;
                if (Devices.ContainsKey(this.COM))
                {
                    device = (SerialPort)Devices[this.COM];
                    if (!device.IsOpen)
                    {
                        device.Open();
                        log.registerLogAction("El puerto " + this.COM + " abre conexión desde CCTAlk.");
                    }
                    log.registerLogAction("El puerto " + this.COM + " ya esta registrado y abre conexión desde CCTAlk.");
                }
                else
                {
                    device = GetNameDevice() == "COMBillDispenser" ? new SerialPort(this.COM, 9600, Parity.Even) : new SerialPort(this.COM, 9600);
                    device.Open();
                    Devices.Add(this.COM, device);
                    log.registerLogAction("El puerto " + this.COM + " abre conexión desde CCTAlk.");
                }

                return device;
            }
            catch (IOException ex)
            {
                throw new Exception(ex.Message + " : metodo openConnection  de la Class CCTalk");
            }

        }
        
        public void setMessage(byte[] parameters)
        {
            string TX = "TX: ";
            this.parameters = parameters;
            device = (SerialPort)Devices[this.COM];
            device.Write(parameters, 0, parameters.Length);

            for (int i = 0, j = 0; i < parameters.Length; i++, j++)
            {
                TX += parameters[i] + " ";
            }
            //Console.WriteLine(TX);
            //Console.WriteLine("TX: " + ByteArrayToString(parameters));

            Thread.Sleep(50);
        }

        public void getMessage()
        {
            string RX = "RX :";

            byte[] result = new byte[device.BytesToRead];

            device.Read(result, 0, result.Length);

            resultmessage = new byte[result.Length];
            for (int i = 0, j = 0; i < result.Length; i++, j++)
            {
                resultmessage[j] = result[i];
                RX += i + " ";
            }
            CleanEcho();
            //Console.WriteLine("RX: " + ByteArrayToString(resultmessage));
            Thread.Sleep(150);
            

        }

        private void CleanEcho()
        {
            string RX = "RX :";
            byte[] temp;
            search(resultmessage, this.parameters);
            if (status)
            {
                temp = new byte[this.resultmessage.Length - parameters.Length];
                for (int i = parameters.Length, j = 0; i < resultmessage.Length; i++, j++)
                {
                    temp[j] = resultmessage[i];

                }
                resultmessage = temp;


            }
            foreach (var i in resultmessage)
            {
                RX += i + " ";


            }
            //Console.WriteLine("RX: " + ByteArrayToString(resultmessage));
            //Console.WriteLine(RX);

        }

        public int search(byte[] haystack, byte[] needle)
        {

            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {

                if (match(haystack, needle, i))
                {
                    position = i;
                    //Console.WriteLine("Status:{0}\nPosición:{1}", status, position);
                    return i;
                }

            }


            return -1;

        }

        public bool match(byte[] haystack, byte[] needle, int start)
        {
            if (needle.Length + start > haystack.Length)
            {


                return false;
            }
            else
            {
                for (int i = 0; i < needle.Length; i++)
                {
                    if (needle[i] != haystack[i + start])
                    {
                        status = false;
                        return false;

                    }
                }
                status = true;
                return true;
            }

        }

        private string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}" + " ", b);
            return hex.ToString();
        }        
    }
}