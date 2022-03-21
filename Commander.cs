using System;
using System.Text;

namespace Bev.Instruments.Heidenhain
{
    public static class Commander
    {
        private static char CtrlB = (char)0x02; //STX
        private static char CtrlS = (char)0x13; //DC3
        private static char CtrlQ = (char)0x11; //DC1
        private static char CtrlE = (char)0x05; //ENQ
        private static char CtrlF = (char)0x06; //ACK
        private static char CtrlU = (char)0x15; //NAK
        private static char CR = (char)0x0D;
        private static char LF = (char)0x0A;
        private static char ESC = (char)0x1B;

        public static byte[] ACommand(int cmd) => StringToBytes(BuildCommandString('A', cmd));
        public static byte[] TCommand(int cmd) => StringToBytes(BuildCommandString('T', cmd));
        public static byte[] SCommand(int cmd) => StringToBytes(BuildCommandString('S', cmd));
        public static byte[] FCommand(int cmd) => StringToBytes(BuildCommandString('F', cmd));

        private static string BuildCommandString(char key, int cmd) => $"{ESC}{key}{IntTo4String(cmd)}{CR}"; // or escape sequence?

        private static string IntTo4String(int n)
        {
            if (n < 0) n = 0;
            if (n > 9999) n = 0;
            return n.ToString("D4");
        }

        private static byte[] StringToBytes(string str)
        {
            Encoding enc = Encoding.ASCII;
            byte[] bytes = enc.GetBytes(str);
            return bytes;
        }

        public static string HexBytesToString(byte[] bytes)
        {
            string hexString = BitConverter.ToString(bytes);
            return hexString.Replace('-', ' '); // or leave it as is?
        }
    }
}
