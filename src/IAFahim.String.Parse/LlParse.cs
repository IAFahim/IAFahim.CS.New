namespace IAFahim.String.Parse
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class LlParse
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ExpandNonterminal(int* table, int sym, int termCount, byte* input, int len, int pos, ref int top, int* stack)
        {
            int terminal = pos < len ? input[pos] : -1;
            int rule = table[sym * termCount + (terminal + 1)];
            if (rule == -1) return false;
            int rhsLen = rule >> 16;
            int rhsStart = rule & 0xFFFF;
            int* rhs = stackalloc int[rhsLen];
            for (int i = 0; i < rhsLen; i++) rhs[i] = table[rhsStart + i];
            for (int i = rhsLen - 1; i >= 0; i--) stack[top++] = rhs[i];
            return true;
        }

        public static bool Parse(byte* input, int len, int* table, int nontermCount, int termCount)
        {
            int* stack = (int*)Marshal.AllocHGlobal((nint)((long)(len * 2 + 2) * sizeof(int)));
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
                    if (pos >= len || input[pos] != sym) { Marshal.FreeHGlobal((nint)stack); return false; }
                    pos++;
                }
                else
                {
                    if (!ExpandNonterminal(table, sym, termCount, input, len, pos, ref top, stack)) { Marshal.FreeHGlobal((nint)stack); return false; }
                }
            }
            bool llResult = pos == len;
            Marshal.FreeHGlobal((nint)stack);
            return llResult;
        }
    }
}
