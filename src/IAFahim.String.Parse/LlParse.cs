namespace IAFahim.String.Parse
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LlParse
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Parse(byte* input, int len, int* table, int nontermCount, int termCount)
        {
            int* stack = stackalloc int[len * 2 + 2];
            int top = 0;
            stack[top++] = -1;
            stack[top++] = 0;
            int pos = 0;
            while (top > 0)
            {
                int sym = stack[--top];
                if (sym == -1) continue;
                if (sym < 256)
                {
                    if (pos >= len || input[pos] != sym) return false;
                    pos++;
                }
                else
                {
                    int terminal = pos < len ? input[pos] : -1;
                    int rule = table[sym * termCount + (terminal + 1)];
                    if (rule == -1) return false;
                    int rhsLen = rule >> 16;
                    int rhsStart = rule & 0xFFFF;
                    int* rhs = stackalloc int[rhsLen];
                    for (int i = 0; i < rhsLen; i++)
                        rhs[i] = table[rhsStart + i];
                    for (int i = rhsLen - 1; i >= 0; i--)
                        stack[top++] = rhs[i];
                }
            }
            return pos == len;
        }
    }
}
