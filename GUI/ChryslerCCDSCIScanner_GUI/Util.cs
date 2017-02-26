using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChryslerCCDSCIScanner_GUI
{
    static class Util
    {
        public static bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        public static byte[] GetBytes(string str)
        {
            string ret = str.Trim().Replace(" ", string.Empty).Replace(",", string.Empty).Replace(";", string.Empty); // remove spaces, commas, semi-colons
            return Enumerable.Range(0, ret.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(ret.Substring(x, 2), 16)).ToArray();
            //return str.Split().Select(s => byte.Parse(s, NumberStyles.HexNumber)).ToArray();
        }
    }

}
