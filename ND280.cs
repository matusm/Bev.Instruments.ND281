
namespace Bev.Instruments.Heidenhain
{
    public class ND280 : HeidenhainBase
    {
        public ND280(string portName) : base(portName) { }
        public override string InstrumentType => "ND280";


        private static byte CtrlB = 0x02;
        private static byte CR = 0x13;
        private static byte LF = 0x10;
        private static byte ESC = 0x1B;

        private byte[] GenerateRemoteCommandA()
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

    }
}
