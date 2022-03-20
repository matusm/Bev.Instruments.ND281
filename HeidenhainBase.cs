using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace Bev.Instruments.Heidenhain
{
    // Basic functionality to retrieve values from the Heidenhain NDxxx display family
    public abstract class HeidenhainBase
    {
        protected readonly SerialPort comPort;
        protected static int delayTime = 200;     // delay time between send and read, in ms
        
        protected virtual void SerialPortSettings()
        {
            comPort.BaudRate = 9600;
            comPort.Parity = Parity.Even;
            comPort.DataBits = 7;
            comPort.StopBits = StopBits.Two;
            comPort.Handshake = Handshake.RequestToSend;
            comPort.RtsEnable = true;
            comPort.DtrEnable = true;
        }

        protected HeidenhainBase(string portName)
        {
            DevicePort = portName.Trim();
            comPort = new SerialPort(DevicePort);
            SerialPortSettings();
            LastResponse = new GenericResponse(); // invalidate all properties
        }

        public string DevicePort { get; }
        public string InstrumentManufacturer => "Heidenhain";
        public abstract string InstrumentType { get; }
        public string InstrumentID => $"{InstrumentManufacturer} {InstrumentType} @ {DevicePort}";
        public GenericResponse LastResponse { get; private set; } 
        public string LastResponseString => LastResponse.ResponseLine;

        public double GetValue()
        {
            LastResponse = ParseTextLine(Query());
            return LastResponse.Value;
        }

        protected GenericResponse ParseTextLine(string textLine)
        {
            var resp = new GenericResponse(textLine);
            return resp;
        }

        protected static byte CtrlB = 0x02;
        protected string Query()
        {
            OpenPort();
            Thread.Sleep(delayTime);    // TODO really?
            SendSerialBus(GenerateGenericCommand());
            Thread.Sleep(delayTime);
            byte[] buffer = ReadSerialBus();
            return Encoding.ASCII.GetString(buffer, 0, buffer.Length);
        }

        protected byte[] GenerateGenericCommand()
        {
            byte[] command = new byte[1];
            command[0] = CtrlB;
            return command;
        }

        protected void SendSerialBus(byte[] command)
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

        protected byte[] ReadSerialBus()
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

        protected void OpenPort()
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
