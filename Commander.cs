﻿using System;
using System.Text;

namespace Bev.Instruments.Heidenhain
{
    public static class Commander
    {
        private static byte CtrlB = 0x02;
        private static byte CR = 0x13;
        private static byte LF = 0x10;
        private static byte ESC = 0x1B;

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
