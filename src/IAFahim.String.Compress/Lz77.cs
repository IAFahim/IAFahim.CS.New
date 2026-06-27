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
            const int HashBits = 16;
            const int HashMask = (1 << HashBits) - 1;
            const int MinMatch = 2;
            const int MaxMatch = 255;
            const int MaxChain = 256;

            int outCount = 0;
            if (len == 0) return 0;

            int hashSize = 1 << HashBits;
            int* head = (int*)Marshal.AllocHGlobal(sizeof(int) * hashSize);
            int* prev = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            try
            {
                for (int h = 0; h < hashSize; h++) head[h] = -1;
                for (int p = 0; p < len; p++) prev[p] = -1;

                int i = 0;
                while (i < len)
                {
                    int bestLen = 0;
                    int bestDist = 0;
                    if (i + MinMatch <= len)
                    {
                        int h = ((input[i] << 10) ^ (input[i + 1] << 5) ^ input[i + 2]) & HashMask;
                        int cur = head[h];
                        int windowStart = i - windowSize;
                        if (windowStart < 0) windowStart = 0;
                        int chain = MaxChain;
                        while (cur >= windowStart && cur >= 0 && chain > 0)
                        {
                            int l = MatchLen(input, len, cur, i, MaxMatch);
                            if (l > bestLen) { bestLen = l; bestDist = i - cur; }
                            cur = prev[cur];
                            chain--;
                        }
                    }

                    if (bestLen >= MinMatch)
                    {
                        output[outCount++] = new Token { Offset = bestDist, Length = bestLen, Literal = 0 };
                        int end = i + bestLen;
                        for (int k = i; k < end; k++) Insert(input, len, k, head, prev, HashMask);
                        i = end;
                    }
                    else
                    {
                        output[outCount++] = new Token { Offset = 0, Length = 0, Literal = input[i] };
                        Insert(input, len, i, head, prev, HashMask);
                        i++;
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((System.IntPtr)head);
                Marshal.FreeHGlobal((System.IntPtr)prev);
            }
            return outCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int MatchLen(byte* input, int len, int a, int b, int maxLen)
        {
            int l = 0;
            int limit = len - b;
            if (maxLen > limit) maxLen = limit;
            while (l < maxLen && input[a + l] == input[b + l]) l++;
            return l;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Insert(byte* input, int len, int p, int* head, int* prev, int hashMask)
        {
            if (p + 2 >= len) return;
            int h = ((input[p] << 10) ^ (input[p + 1] << 5) ^ input[p + 2]) & hashMask;
            prev[p] = head[h];
            head[h] = p;
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
