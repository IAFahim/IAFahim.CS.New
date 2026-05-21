namespace IAFahim.String.Compress
{
using System.Runtime.InteropServices;
    using System;
    
    using System.Runtime.CompilerServices;

    public static unsafe class Huffman
    {
        public struct Code
        {
            public int Length;
            public long Bits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Build(int* freq, int sigma, Code* codes)
        {
            var pq = new System.Collections.Generic.List<(int freq, int symbol, int left, int right)>();
            for (int c = 0; c < sigma; c++)
            {
                if (freq[c] > 0)
                    pq.Add((freq[c], c, -1, -1));
            }
            pq.Sort((a, b) => a.freq.CompareTo(b.freq));
            while (pq.Count > 1)
            {
                var left = pq[0]; pq.RemoveAt(0);
                var right = pq[0]; pq.RemoveAt(0);
                pq.Add((left.freq + right.freq, -1, left.symbol, right.symbol));
                pq.Sort((a, b) => a.freq.CompareTo(b.freq));
            }
        }

        public static void Encode(byte* input, int len, int* output, int* outLen, Code* codes)
        {
            long buffer = 0;
            int bits = 0;
            int pos = 0;
            for (int i = 0; i < len; i++)
            {
                var code = codes[input[i]];
                buffer = (buffer << code.Length) | code.Bits;
                bits += code.Length;
                while (bits >= 32)
                {
                    output[pos++] = (int)(buffer >> (bits - 32));
                    bits -= 32;
                }
            }
            if (bits > 0)
                output[pos++] = (int)(buffer << (32 - bits));
            *outLen = pos;
        }

        public static void Decode(int* input, int inLen, Code* codes, byte* output, int* outLen)
        {
            long buffer = 0;
            int bits = 0;
            int ipos = 0;
            int opos = 0;
            while (ipos < inLen && opos < *outLen)
            {
                while (bits < 32 && ipos < inLen)
                {
                    buffer = (buffer << 32) | ((uint)input[ipos++]);
                    bits += 32;
                }
            }
        }
    }
}
