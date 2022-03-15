using System;
using System.Globalization;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace Bev.Instruments.ND281
{
    // Basic functionality to retrieve values from the Heidenhain ND281B display
    // Benutzerhandbuch 6/2000
    // works ND280 too!
    public class ND281
    {
        private readonly SerialPort comPort;
        private static int delayTime = 200;     // delay time between send and read, in ms

        public ND281(string portName)
        {
            DevicePort = portName.Trim();
            comPort = new SerialPort(DevicePort, 9600, Parity.Even, 7, StopBits.Two);
            comPort.Handshake = Handshake.RequestToSend;
            comPort.RtsEnable = true;
            comPort.DtrEnable = true;
            LastResponse = new GenericResponse(); // invalidate all properties
        }

        public string DevicePort { get; }
        public string InstrumentManufacturer => "Heidenhain";
        public string InstrumentType => "ND281/ND280";
        public string InstrumentID => $"{InstrumentManufacturer} {InstrumentType} @ {DevicePort}";
        public GenericResponse LastResponse { get; private set; }
        public string LastResponseString => LastResponse.ResponseLine; // for debuging only, make private when working

        public double GetValue()
        {
            LastResponse = ParseTextLine(Query());
            return LastResponse.Value;
        }

        private GenericResponse ParseTextLine(string textLine)
        {
            var resp = new GenericResponse(textLine);
            return resp;
        }

        private static byte CtrlB = 0x02;
        private static byte CR = 0x13;
        private static byte LF = 0x10;
        private static byte ESC = 0x1B;

        private string Query()
        {
            OpenPort();
            Thread.Sleep(delayTime);    // TODO really?
            SendSerialBus(GenerateGenericCommand());
            Thread.Sleep(delayTime);
            byte[] buffer = ReadSerialBus();
            return Encoding.ASCII.GetString(buffer, 0, buffer.Length);
        }

        private byte[] GenerateGenericCommand()
        {
            byte[] command = new byte[1];
            command[0] = CtrlB;
            return command;
        }

        private byte[] GenerateRemoteACommand()
        {
            byte[] command = new byte[7];
            command[0] = ESC;
            command[1] = 0x41;
            command[2] = 0x30;
            command[3] = 0x30;
            command[4] = 0x30;
            command[5] = 0x30;
            command[6] = CR;
            return command;
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
