namespace IAFahim.String.Compress
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Arithmetic
    {
        public static void Encode(byte* input, int len, long* output, int* outLen, long precision)
        {
            long lo = 0, hi = precision;
            int pos = 0;
            for (int i = 0; i < len; i++)
            {
                long range = hi - lo;
                long freq = (hi - lo) / 256;
                lo += freq * input[i];
                hi = lo + freq;
                while (true)
                {
                    if (hi < precision / 2)
                    {
                        output[pos++] = lo;
                        lo = 0; hi = precision;
                    }
                    else if (lo >= precision / 2)
                    {
                        output[pos++] = lo;
                        lo -= precision / 2; hi -= precision / 2;
                    }
                    else break;
                }
            }
            output[pos++] = lo;
            *outLen = pos;
        }

        public static int Decode(long* input, int len, byte* output, int* outLen, long precision)
        {
            long lo = 0, hi = precision;
            int pos = 0;
            long val = input[pos++];
            while (pos < len)
            {
                long range = hi - lo;
                long freq = range / 256;
                int c = (int)((val - lo) / freq);
                if (c >= 256) break;
                output[*outLen++] = (byte)c;
                lo += freq * c;
                hi = lo + freq;
            }
            return *outLen;
        }
    }
}
