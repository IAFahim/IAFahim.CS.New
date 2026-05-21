namespace IAFahim.String.Compress
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Lz77
    {
        public struct Token
        {
            public int Offset;
            public int Length;
            public byte Literal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Encode(byte* input, int len, Token* output, int windowSize)
        {
            int outCount = 0;
            int i = 0;
            while (i < len)
            {
                int bestLen = 0, bestPos = 0;
                int start = Math.Max(0, i - windowSize);
                for (int j = start; j < i; j++)
                {
                    int l = 0;
                    while (i + l < len && input[j + l] == input[i + l])
                    {
                        l++;
                        if (l > 255) break;
                    }
                    if (l > bestLen)
                    {
                        bestLen = l;
                        bestPos = i - j;
                    }
                }
                if (bestLen >= 2)
                {
                    output[outCount++] = new Token { Offset = bestPos, Length = bestLen, Literal = 0 };
                    i += bestLen;
                }
                else
                {
                    output[outCount++] = new Token { Offset = 0, Length = 0, Literal = input[i] };
                    i++;
                }
            }
            return outCount;
        }

        public static int Decode(Token* input, int count, byte* output)
        {
            int pos = 0;
            for (int i = 0; i < count; i++)
            {
                if (input[i].Length > 0)
                {
                    for (int j = 0; j < input[i].Length; j++)
                        output[pos + j] = output[pos - input[i].Offset + j];
                    pos += input[i].Length;
                }
                else
                    output[pos++] = input[i].Literal;
            }
            return pos;
        }
    }
}
