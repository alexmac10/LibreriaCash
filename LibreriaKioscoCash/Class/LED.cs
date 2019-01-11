using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Configuration;
using System.Threading;
using FTD2XX_NET;
using System.Text.RegularExpressions;

namespace LibreriaKioscoCash.Class
{
    public class LED
    {

        UInt32 ftdiDeviceCount = 0;
        FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
        FTDI fTDI = new FTDI();
        private string serialnumber = "A70479P9";
        private char[] controlByte;
        private byte pinStates = 0;
        //private byte pin

        public void open()
        {
            ftStatus = fTDI.GetNumberOfDevices(ref ftdiDeviceCount);
            // Allocate storage for device info list
            FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];

            // Populate our device list
            ftStatus = fTDI.GetDeviceList(ftdiDeviceList);
            try
            {
                if (ftStatus == FTDI.FT_STATUS.FT_OK && ftdiDeviceCount != 0)
                {
                    //Console.WriteLine("Number of FTDI devices: " + ftdiDeviceCount.ToString());
                    //Console.WriteLine("");
                    for (UInt32 i = 0; i < ftdiDeviceCount; i++)
                    {
                        if (ftdiDeviceList[i].SerialNumber == serialnumber)
                        {
                            Console.WriteLine("Se Encontro Dispositivo Para LEDs");
                            Console.WriteLine("");
                            Console.WriteLine("Abriendo Conexion ...");
                            Console.WriteLine("");
                            ftStatus = fTDI.OpenBySerialNumber(serialnumber);
                            ftStatus = fTDI.SetBaudRate(9600);
                            ftStatus = fTDI.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
                            if (fTDI.IsOpen)
                            {
                                Console.WriteLine("Dispositivo Conectado");
                                Console.WriteLine("");
                            }



                        }

                    }
                }
                else
                {

                    throw new Exception("Failed to get number of devices (error " + ftStatus.ToString() + ")");

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }



        }
        public void relay(int number, string status)
        {
            uint numWritten = 0;
            //ftStatus = fTDI.GetPinStates(ref pinStates);
            State();
            switch (number)
            {
                case 1:
                    if (status == "ON")
                    {
                        if ((pinStates & 1) != 1)
                        {

                            fTDI.Write(new byte[] { ChangeBinaryValue(3, '1') }, 1, ref numWritten);
                            Console.WriteLine("Encendido Correcto");
                        }


                        else
                        {
                            Console.WriteLine("El LED ya se encuentra encendido");
                        }
                    }
                    else if (status == "OFF")
                    {
                        fTDI.Write(new byte[] { ChangeBinaryValue(3, '0') }, 1, ref numWritten);
                        Console.WriteLine("Apagado Correcto");


                    }
                    else if (status == "State")
                    {
                        if ((pinStates & 1) != 1)
                        {

                            Console.WriteLine("Estado: LED Apagado ");
                        }
                        else
                        {
                            Console.WriteLine("Estado: LED Encendido ");
                        }

                    }
                    break;
                case 2:
                    if (status == "ON")
                    {
                        if ((pinStates & 2) != 2)
                        {
                            fTDI.Write(new byte[] { ChangeBinaryValue(2, '1') }, 1, ref numWritten);
                            Console.WriteLine("Encendido Correcto");
                        }


                        else
                        {
                            Console.WriteLine("El LED ya se encuentra encendido");
                        }
                    }
                    else if (status == "OFF")
                    {

                        fTDI.Write(new byte[] { ChangeBinaryValue(2, '0') }, 1, ref numWritten);
                        Console.WriteLine("Apagado Correcto");


                    }
                    else if (status == "State")
                    {
                        if ((pinStates & 2) != 2)
                        {

                            Console.WriteLine("Estado: LED Apagado ");
                        }
                        else
                        {
                            Console.WriteLine("Estado: LED Encendido  ");
                        }

                    }
                    break;
                case 3:
                    if (status == "ON")
                    {
                        if ((pinStates & 4) != 4)
                        {

                            fTDI.Write(new byte[] { ChangeBinaryValue(1, '1') }, 1, ref numWritten);
                            Console.WriteLine("Encendido Correcto");
                        }


                        else
                        {
                            Console.WriteLine("El LED ya se encuentra encendido");
                        }
                    }
                    else if (status == "OFF")
                    {

                        fTDI.Write(new byte[] { ChangeBinaryValue(1, '0') }, 1, ref numWritten);
                        Console.WriteLine("Apagado Correcto");


                    }
                    else if (status == "State")
                    {
                        if ((pinStates & 4) != 4)
                        {

                            Console.WriteLine("Estado: LED Apagado ");
                        }
                        else
                        {
                            Console.WriteLine("Estado: LED Encendido ");
                        }

                    }
                    break;
                case 4:
                    if (status == "ON")
                    {
                        if ((pinStates & 8) != 8)
                        {
                            fTDI.Write(new byte[] { ChangeBinaryValue(0, '1') }, 1, ref numWritten);
                            Console.WriteLine("Encendido Correcto");
                        }

                        else
                        {
                            Console.WriteLine("El LED ya se encuentra encendido");
                        }
                    }
                    else if (status == "OFF")
                    {
                        fTDI.Write(new byte[] { ChangeBinaryValue(0, '0') }, 1, ref numWritten);
                        Console.WriteLine("Apagado Correcto");

                    }
                    else if (status == "State")
                    {
                        if ((pinStates & 8) != 8)
                        {

                            Console.WriteLine("Estado: LED Apagado ");
                        }
                        else
                        {
                            Console.WriteLine("Estado: LED Encendido ");
                        }

                    }
                    break;
                case 5:
                    if (status == "ON")
                    {
                        fTDI.Write(new byte[] { 15 }, 1, ref numWritten);

                    }
                    else if(status=="OFF")
                    {
                        fTDI.Write(new byte[] { 0 }, 1, ref numWritten);

                    }
                    else if(status=="State")
                    {
                        if(pinStates==15)
                        {
                            Console.WriteLine("Estado: Todos los LEDs Encendidos");
                        }
                        else if(pinStates==0)
                        {
                            Console.WriteLine("Estado: Todos los LEDs Apagados");
                            
                        }
                    }
                    break;

            }
            Console.WriteLine("");


        }
        public void LEDOn(string device)
        {
            switch (device)
            {
                case "Acceptor":


                    break;
                case "Dispenser":

                    break;

            }

        }
        public void LEDOff(string device)
        {
            switch (device)
            {
                case "Acceptor":

                    break;
                case "Dispenser":

                    break;

            }

        }
        private void State()
        {
            ftStatus = fTDI.GetPinStates(ref pinStates);

            string bin = "";
            int dec = pinStates;
            bin = Convert.ToString(dec, 2);
            bin = Regex.Replace(bin, @"\d+", n => n.Value.PadLeft(4, '0'));

            //Console.WriteLine("State: " + pinStates);
            //Console.WriteLine("Binario: " + bin);

            controlByte = bin.ToCharArray();

        }
        private byte ChangeBinaryValue(int position, char value)
        {
            controlByte[position] = value;
            string s = new string(controlByte);
            //Console.WriteLine("ChangeBinary: " + s);
            int convertDecimal = Convert.ToInt32(s, 2);
            //Console.WriteLine((byte)convertDecimal);
            return (byte)convertDecimal;
        }



    }
}
