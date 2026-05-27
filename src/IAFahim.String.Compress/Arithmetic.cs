namespace IAFahim.String.Compress
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Arithmetic
    {
        private const int AlphabetSize = 256;
        private const int MaxByteValue = 255;
        private const int BaseTwo = 2;

        public static void Encode(byte* input, int len, long* output, int* outLen, long precision)
        {
            long lo = 0;
            long hi = precision;
            int pos = 0;
            int K = 0;
            while ((1L << K) < precision)
            {
                K++;
            }
            for (int i = 0; i < len; i++)
            {
                long range = hi - lo;
                long freq = range / AlphabetSize;
                lo += freq * input[i];
                hi = lo + freq;
                while (true)
                {
                    if (hi < precision / BaseTwo)
                    {
                        output[pos++] = 0;
                        lo = lo * BaseTwo;
                        hi = hi * BaseTwo;
                    }
                    else if (lo >= precision / BaseTwo)
                    {
                        output[pos++] = 1;
                        lo = (lo - precision / BaseTwo) * BaseTwo;
                        hi = (hi - precision / BaseTwo) * BaseTwo;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            for (int i = K - 1; i >= 0; i--)
            {
                output[pos++] = (lo >> i) & 1;
            }
            *outLen = pos;
        }

        public static int Decode(long* input, int len, byte* output, int* outLen, long precision)
        {
            long lo = 0;
            long hi = precision;
            int pos = 0;
            int decodedCount = 0;
            int K = 0;
            while ((1L << K) < precision)
            {
                K++;
            }
            long val = 0;
            for (int i = 0; i < K && pos < len; i++)
            {
                val = val * BaseTwo + input[pos++];
            }
            while (pos < len)
            {
                long range = hi - lo;
                long freq = range / AlphabetSize;
                if (freq == 0)
                {
                    break;
                }
                int c = (int)((val - lo) / freq);
                if (c < 0)
                {
                    c = 0;
                }
                if (c > MaxByteValue)
                {
                    c = MaxByteValue;
                }
                output[decodedCount++] = (byte)c;
                lo += freq * c;
                hi = lo + freq;
                while (true)
                {
                    if (hi < precision / BaseTwo)
                    {
                        lo = lo * BaseTwo;
                        hi = hi * BaseTwo;
                        long nextBit = 0;
                        if (pos < len)
                        {
                            nextBit = input[pos++];
                        }
                        val = val * BaseTwo + nextBit;
                    }
                    else if (lo >= precision / BaseTwo)
                    {
                        lo = (lo - precision / BaseTwo) * BaseTwo;
                        hi = (hi - precision / BaseTwo) * BaseTwo;
                        long nextBit = 0;
                        if (pos < len)
                        {
                            nextBit = input[pos++];
                        }
                        val = (val - precision / BaseTwo) * BaseTwo + nextBit;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            *outLen = decodedCount;
            return decodedCount;
        }
    }
}
