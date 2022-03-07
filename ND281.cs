using System;
using System.Globalization;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace Bev.Instruments.ND281
{
    // Basic functionality to retrieve values from the Heidenhain ND281B display
    // Benutzerhandbuch 6/2000
    public class ND281
    {
        private readonly SerialPort comPort;
        private static int delayTime = 200;     // delay time between send and read, in ms

        public ND281(string portName)
        {
            LastResponse = string.Empty;
            DevicePort = portName.Trim();
            comPort = new SerialPort(DevicePort, 9600, Parity.Even, 7, StopBits.Two);
            comPort.Handshake = Handshake.RequestToSend;
            comPort.RtsEnable = true;
            comPort.DtrEnable = true;
        }

        public string DevicePort { get; }
        public string InstrumentManufacturer => "Heidenhain";
        public string InstrumentType => "ND 281 B";
        public string InstrumentID => $"{InstrumentManufacturer} {InstrumentType} @ {DevicePort}";
        public string LastResponse { get; private set; } // for debuging only, make private when working

        public double GetValue()
        {
            LastResponse = Query();
            return ParseResponse(LastResponse);
        }

        private double ParseResponse(string response)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            if (response.Length < 11) // actually 17?
                return double.NaN;
            double sign = 1;
            string sSign = response.Substring(0, 1);
            if (sSign == "-")
                sign = -1;
            string sValue = response.Substring(1, 10);
            if (double.TryParse(sValue, out double value))
                return sign * value;
            return double.NaN;
        }

        private static byte CtrlB = 0x02;
        private static byte CR = 0x13;
        private static byte LF = 0x10;

        private string Query()
        {
            byte[] command = new byte[1];
            command[0] = CtrlB;
            //command[1] = CR;
            //command[2] = LF;
            OpenPort();
            Thread.Sleep(delayTime);    // TODO really?
            SendSerialBus(command);
            Thread.Sleep(delayTime);
            byte[] buffer = ReadSerialBus();
            string s = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            return s;
        }

        private void SendSerialBus(byte[] command)
        {
            try
            {
                comPort.Write(command, 0, command.Length);
                return;
            }
            catch (Exception)
            {
            }
        }

        private byte[] ReadSerialBus()
        {
            byte[] errBuffer = { 0xFF };
            try
            {
                byte[] buffer = new byte[comPort.BytesToRead];
                comPort.Read(buffer, 0, buffer.Length);
                return buffer;
            }
            catch (Exception e)
            {
                Console.WriteLine($"****ReadSerialBus -> {e.Message}");
                return errBuffer;
            }
        }

        private void OpenPort()
        {
            try
            {
                if (!comPort.IsOpen)
                    comPort.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine($"****OpenPort -> {e.Message}");
            }
        }
    }
}
