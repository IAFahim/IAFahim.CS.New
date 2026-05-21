namespace IAFahim.String.Parse
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class LrParse
    {
        public struct Action
        {
            public int Type;
            public int Target;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Parse(byte* input, int len, Action* table, int* gotos, int stateCount, int symbolCount)
        {
            int* stack = stackalloc int[len + 1];
            int top = 0;
            stack[top++] = 0;
            int pos = 0;
            while (pos <= len)
            {
                int state = stack[top - 1];
                int terminal = pos < len ? input[pos] : -1;
                Action action = table[state * symbolCount + (terminal + 1)];
                if (action.Type == 0)
                {
                    stack[top++] = action.Target;
                    pos++;
                }
                else if (action.Type == 1)
                {
                    if (action.Target == -1) return true;
                    int lhs = action.Target >> 16;
                    int rhsLen = action.Target & 0xFFFF;
                    top -= rhsLen;
                    int newState = stack[top - 1];
                    stack[top++] = gotos[newState * symbolCount + lhs];
                }
                else return false;
            }
            return false;
        }
    }
}
