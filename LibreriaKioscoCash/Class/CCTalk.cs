using System;
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
    public class CCTalk
    {
        private SerialPort device;
        private static Hashtable Devices;

        public  byte[] resultmessage;

        private static CCTalk instance = null;

        private CCTalk()
        {
            Devices = new Hashtable();
        }

        public static CCTalk GetInstance()
        {
            if (instance == null)
            {

                instance = new CCTalk();
            }

            return instance;
        }
        public SerialPort openConnection(string COM)

        {
            try
            {
                if(Devices.ContainsKey(COM))
                {
                    //Console.WriteLine("Puerto Abierto:{0} ", COM);
                    return (SerialPort)Devices[COM];

                }
                else
                {
                    device = new SerialPort(COM, 9600, Parity.Even);
                    device.Open();
                    //Console.WriteLine("Abriendo puerto:{0}",COM);
                    Devices.Add(COM, device);
                    return device;
                }
                
            }
            catch (IOException ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public void setMessage(string COM,byte[] parameters)
        {
            string TX = "TX: ";
            device = (SerialPort) Devices[COM];
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
            byte finalByte = 0;
            byte[] result = new byte[device.BytesToRead];

            device.Read(result, 0, result.Length);
            resultmessage = new byte[result.Length];
            for (int i = 0, j = 0; i < result.Length; i++, j++)
            {
                resultmessage[j] = result[i];
                RX += result[i] + " ";
                finalByte = result[i];
            }
            //Console.WriteLine(RX);
            Console.WriteLine("RX: " + ByteArrayToString(resultmessage));
            Thread.Sleep(150);

        }
        private int searchElement(byte[] code)
        {
            ArrayList positions = this.getPositions(code);

            if (code.Length != positions.Count)
            {
                throw new Exception("Error en el dispositivo");
            }

            if (!validateConsecutivePosition(positions))
            {
                Console.WriteLine("Se genera el error");
                throw new Exception("Error en el dispositivo");
            }

            return (int)positions[0];
        }

        private ArrayList getPositions(byte[] code)
        {
            ArrayList positions = new ArrayList();

            for (int i = 0; i < code.Length; i++)
            {
                for (int j = 0; j < resultmessage.Length; j++)
                {
                    if (code[i] == resultmessage[j])
                    {
                        positions.Add(j);
                    }
                }
            }

            return positions;
        }

        private bool validateConsecutivePosition(ArrayList positions)
        {
            int? anterior = null;

            if (positions.Count == 0)
            {
                return false;
            }

            foreach (int actual in positions)
            {

                if (anterior == null)
                {
                    anterior = actual;
                    continue;
                }

                if ((anterior + 1) == actual)
                {
                    anterior = actual;
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
        private string ByteArrayToString(byte[] ba)
        {

            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}" + " ", b);
            return hex.ToString();
        }

        //    //Envia mensaje al dispositivo
        //    public void setMessage(List<byte> parameter, List<byte> data)
        //    {
        //        parameter = this.setChecksum(parameter, data);
        //        this.sendMessage(parameter);
        //        Thread.Sleep(500);
        //        this.getMessage(parameter);
        //    }

        //    //Regresa el Checksum
        //    private List<byte> setChecksum(List<byte> parameter, List<byte> data)
        //    {
        //        byte sum = 0;
        //        if (data.Count > 0)
        //        {
        //            parameter[1] = (byte)data.Count;
        //        }

        //        parameter.AddRange(data);

        //        foreach (byte i in parameter)
        //        {
        //            sum += i;
        //        }

        //        sum = (byte)(256 - (sum % 256));

        //        parameter.Add(sum);

        //        return parameter;
        //    }

        //    //Envia el mensaje al dispositivo
        //    private void sendMessage(List<byte> parameters)
        //    {
        //        string TX = "TX : ";
        //        byte[] arrayWrite = new byte[parameters.Count];

        //        for (int i = 0; i < parameters.Count; i++)
        //        {
        //            arrayWrite[i] = parameters[i];
        //            TX += parameters[i] + " ";
        //        }

        //        portHopper.Write(arrayWrite, 0, arrayWrite.Length);
        //        //Console.WriteLine(TX);
        //    }

        //    //Regresa la respuesta del dispositvo
        //    private void getMessage(List<byte> parameters)
        //    {
        //        string RX = "RX : ";
        //        int length = 0;
        //        byte[] result = new byte[portHopper.BytesToRead];
        //        portHopper.Read(result, 0, result.Length);
        //        length = (this.device == "COMBOT") ? result.Length : (result.Length - parameters.Count);
        //        resultMessage = new byte[length];
        //        length = (this.device == "COMBOT") ? 0 : parameters.Count;

        //        for (int i = length, j = 0; i < result.Length; i++, j++)
        //        {
        //            resultMessage[j] = result[i];
        //            RX += result[i] + " ";
        //        }
        //        //Console.WriteLine(RX);            
        //    }

        //    /*
        //     * Encargado de definir por default la denominacion para todos los 
        //     * contenedores de monedas
        //     */
        //    protected void setDefaultConfigConteinersCoins()
        //    {
        //        this.setValueConteinerCoins(new byte[] { 4, 2 });//monedas de 1
        //        this.setValueConteinerCoins(new byte[] { 5, 0 });//monedas de 2
        //        this.setValueConteinerCoins(new byte[] { 6, 3 });//monedas de 5
        //        this.setValueConteinerCoins(new byte[] { 7, 5 });//monedas de 10
        //    }

        //    /*
        //    * Encardado de modificar la denominacion que tendra el contenedor de monedas
        //    */
        //    public void setValueConteinerCoins(byte[] data)
        //    {
        //        this.setMessage(new List<byte>() { 26, 0, 1, 210 }, new
        //        List<byte>(data));
        //    }

        //    //Estableciendo la configuracion inicial para el hooper
        //    public virtual void setConfig()
        //    {
        //        if (this.device == "COMBOT")
        //        {
        //            this.resetAccepter();
        //            this.setDefaultConfigConteinersCoins();
        //            this.setConfigDefault();
        //        }
        //        else
        //        {
        //            this.resetAccepter();
        //            this.setConfigDefault();
        //        }
        //    }

        //    /*
        //    * Encargado de recetear el Acceptador para limpiar los datos 
        //    * de la ultima transacción
        //    */
        //    private void resetAccepter()
        //    {
        //        if (this.device == "COMBOT")
        //        {
        //            this.setMessage(new List<byte>() { 26, 0, 1, 1 }, new List<byte>());
        //        }
        //        else
        //        {
        //            this.setMessage(new List<byte>() { 2, 0, 1, 1 }, new List<byte>());
        //        }
        //    }

        //    /*
        //     * Encargado de configurar por default las monedas que se van aceptar con las denominaciones:
        //     * $1,$2,$5,$10 
        //     */
        //    private void setConfigDefault()
        //    {
        //        //Binario para aceptar las monedas
        //        //[00000000] = 0 no acepta monedas
        //        //[00000001] = 1 acepta $0.10 (coin 1)
        //        //[00000010] = 2 acepta $0.20 (coin 2)
        //        //[00000100] = 6 acepta $0.50 (coin 3)
        //        //[00001000] = 8 acepta $1 (coin 4)
        //        //[00010000] = 16 acepta $2 (coin 5)
        //        //[00100000] = 32 acepta $5 (coin 6)
        //        //[01000000] = 64 acepta $10 (coin 7)
        //        //[10000000] = 128 acepta $20 (coin 8)

        //        //Para agregar solo las $10,$5,$2 $1 mandar 120 
        //        //[01111000] = 120 
        //        // El data queda [120,255] el 255 siempres se va a poner
        //        if (this.device == "COMBOT")
        //        {
        //            this.setMessage(new List<byte>() { 26, 0, 1, 231 }, new
        //        List<byte>() { 120, 255 });
        //        }
        //        else
        //        {
        //            this.setMessage(new List<byte>() { 2, 0, 1, 231 }, new
        //       List<byte>() { 124, 255 });
        //        }

        //    }

        //    /*
        //     * Obtiene los id que tienen los dispositivos 
        //     */
        //    public void getIdDevice()
        //    {
        //        this.setMessage(new List<byte>() { 0, 0, 1, 253 }, new
        //        List<byte>());
        //    }

        //    public void emptyMoneyBox()
        //    {
        //        //si data = 1 se vacia por atras
        //        //si data = 2 se vacia por enfrente
        //        this.setMessage(new List<byte>() { 12, 0, 1, 70 }, new
        //            List<byte>() { 1 });
        //    }
        //    private void emptyContainerCoin(byte device, byte count = 255)
        //    {
        //        this.enableContainerCoin(device);
        //        Thread.Sleep(500);
        //        byte[] serie = this.getNumberSerie(device);
        //        serie[3] = count; //Define la cantidad que debe vaciar
        //        this.setMessage(new List<byte>() { device, 0, 1, 167 }, new
        //        List<byte>(serie));
        //        Thread.Sleep(500);
        //    }

        //    /*
        //     * Encargado de habilitar el contenedor de monedas.
        //     */
        //    private void enableContainerCoin(byte device)
        //    {
        //        this.setMessage(new List<byte>() { device, 0, 1, 164 }, new
        //        List<byte>() { 165 });
        //    }

        //    /*
        //    * Encargado de obtener los numero de serie del dispositvo
        //    */
        //    private byte[] getNumberSerie(byte device)
        //    {
        //        byte[] serie = new byte[4];
        //        this.setMessage(new List<byte>() { device, 0, 1, 242 }, new
        //        List<byte>());

        //        for (int i = 4, j = 0; i < this.resultMessage.Length - 1; i++, j++)
        //        {
        //            serie[j] = this.resultMessage[i];
        //        }
        //        return serie;
        //    }
    }
}
