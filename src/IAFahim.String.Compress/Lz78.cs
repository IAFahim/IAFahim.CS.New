namespace IAFahim.String.Compress
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Lz78
    {
        public struct Token
        {
            public int Phrase;
            public byte Literal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Encode(byte* input, int len, Token* output)
        {
            int outCount = 0;
            int i = 0;
            while (i < len)
            {
                int bestPhrase = 0;
                int bestLen = 1;
                for (int j = 0; j < outCount; j++)
                {
                    int pLen = 0;
                    int phraseStart = 0;
                    int phraseIdx = j;
                    while (phraseIdx > 0)
                    {
                        pLen++;
                        phraseIdx = output[phraseIdx].Phrase;
                    }
                    if (i + pLen >= len) continue;
                    bool match = true;
                    for (int k = 0; k < pLen; k++)
                    {
                        if (input[i + k] != GetByte(output, j, k)) { match = false; break; }
                    }
                    if (match && pLen + 1 > bestLen)
                    {
                        bestLen = pLen + 1;
                        bestPhrase = j + 1;
                    }
                }
                output[outCount].Phrase = bestPhrase;
                output[outCount].Literal = input[i + bestLen - 1];
                outCount++;
                i += bestLen;
            }
            return outCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Decode(Token* input, int count, byte* output)
        {
            int pos = 0;
            for (int i = 0; i < count; i++)
            {
                int phraseStart = pos;
                int phraseLen = 0;
                int idx = input[i].Phrase - 1;
                if (idx >= 0)
                {
                    int* chain = stackalloc int[count];
                    int chainLen = 0;
                    int cur = idx;
                    while (cur >= 0)
                    {
                        chain[chainLen++] = input[cur].Literal;
                        cur = input[cur].Phrase - 1;
                    }
                    for (int j = chainLen - 1; j >= 0; j--)
                        output[pos++] = (byte)chain[j];
                }
                output[pos++] = input[i].Literal;
            }
            return pos;
        }

        private static byte GetByte(Token* tokens, int idx, int offset)
        {
            int* chain = stackalloc int[256];
            int chainLen = 0;
            int cur = idx;
            while (cur >= 0 && chainLen < 256)
            {
                chain[chainLen++] = tokens[cur].Literal;
                cur = tokens[cur].Phrase - 1;
            }
            int target = chainLen - 1 - offset;
            return (byte)(target >= 0 ? chain[target] : 0);
        }
    }
}
