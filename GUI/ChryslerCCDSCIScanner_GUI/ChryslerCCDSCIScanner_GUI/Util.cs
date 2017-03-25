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
            string ret = str.Trim().Replace(" ", string.Empty).Replace(",", string.Empty).
                                    Replace(";", string.Empty).Replace("$", string.Empty).
                                    Replace("0x", string.Empty); // remove spaces, commas, semi-colons

            try
            {
                return Enumerable.Range(0, ret.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(ret.Substring(x, 2), 16)).ToArray();
            }
            catch
            {
                return new byte[] { 0x00 }; // return an array with a single zero byte as an error
            }
        }

        /// <summary>Looks for the next occurrence of a sequence in a byte array</summary>
        /// <param name="array">Array that will be scanned</param>
        /// <param name="start">Index in the array at which scanning will begin</param>
        /// <param name="sequence">Sequence the array will be scanned for</param>
        /// <returns>
        ///   The index of the next occurrence of the sequence of -1 if not found
        /// </returns>
        public static int SearchBytes(byte[] array, int start, byte[] sequence)
        {
            int end = array.Length - sequence.Length; // past here no match is possible
            byte firstByte = sequence[0]; // cached to tell compiler there's no aliasing

            while (start < end)
            {
                // scan for first byte only. compiler-friendly.
                if (array[start] == firstByte)
                {
                    // scan for rest of sequence
                    for (int offset = 1; offset < sequence.Length; ++offset)
                    {
                        if (array[start + offset] != sequence[offset])
                        {
                            break; // mismatch? continue scanning with next byte
                        }
                        else if (offset == sequence.Length - 1)
                        {
                            return start; // all bytes matched!
                        }
                    }
                }
                ++start;
            }

            // end of array reached without match
            return -1;
        }
    }
}
