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
        private readonly SerialPort comPort;    // this must not be static!
        private static byte CtrlB = 0x02;       //
        private static int delayTime = 50;      // delay time between send and read, in ms
        private bool debugFlag = false;

        public ND281(string portName)
        {
            LastResponse = string.Empty;
            DevicePort = portName.Trim();
            if (DevicePort == "debug")
            {
                debugFlag = true;
            }
            else
            {
                comPort = new SerialPort(DevicePort, 9600, Parity.Even, 7, StopBits.Two);
                comPort.RtsEnable = true;   // ?
                comPort.DtrEnable = true;	// ?
            }
        }

        public string DevicePort { get; }
        public string InstrumentManufacturer => "Heidenhain";
        public string InstrumentType => "ND 281 B";
        public string InstrumentID => $"{InstrumentManufacturer} {InstrumentType} @ {DevicePort}";
        public string LastResponse { get; private set; } // for debuging only, make private when working

        public double GetValue()
        {
            if (debugFlag) return 31.1415926;
            LastResponse = Query();
            return ParseResponse(LastResponse);
        }

        private double ParseResponse(string lastResponse)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            if (lastResponse.Length < 11) // actually 17?
                return double.NaN;
            double sign = 1;
            string sSign = lastResponse.Substring(0, 1);
            if (sSign == "-")
                sign = -1;
            string sValue = lastResponse.Substring(1, 10);
            if(double.TryParse(sValue, out double value)) 
                return sign*value;
            return double.NaN;
        }

        private string Query()
        {
            byte[] command = new byte[1];
            command[0] = CtrlB;
            OpenPort();
            SendSerialBus(command);
            Thread.Sleep(delayTime);
            byte[] buffer = ReadSerialBus();
            string s = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            return s;
        }

        private void OpenPort()
        {
            try
            {
                if (!comPort.IsOpen)
                    comPort.Open();
            }
            catch (Exception)
            { }
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
            catch (Exception)
            {
                return errBuffer;
            }
        }

    }
}
