namespace IAFahim.String.Compress
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LzFactorization
    {
        public struct Factor
        {
            public int Position;
            public int Length;
            public byte Literal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Factorize(byte* input, int len, Factor* output)
        {
            int count = 0;
            int i = 0;
            while (i < len)
            {
                int bestPos = -1;
                int bestLen = 0;
                for (int j = 0; j < i; j++)
                {
                    int l = 0;
                    while (i + l < len && j + l < i && input[j + l] == input[i + l])
                        l++;
                    if (l > bestLen)
                    {
                        bestLen = l;
                        bestPos = j;
                    }
                }
                if (bestLen > 0)
                {
                    output[count].Position = bestPos;
                    output[count].Length = bestLen;
                    output[count].Literal = 0;
                    count++;
                    i += bestLen;
                }
                else
                {
                    output[count].Position = -1;
                    output[count].Length = 1;
                    output[count].Literal = input[i];
                    count++;
                    i++;
                }
            }
            return count;
        }
    }
}
