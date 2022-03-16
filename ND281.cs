using System;
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
        public string InstrumentType => "ND281B";
        public string InstrumentID => $"{InstrumentManufacturer} {InstrumentType} @ {DevicePort}";
        public GenericResponse LastResponse { get; private set; } 
        public string LastResponseString => LastResponse.ResponseLine;

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
