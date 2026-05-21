namespace IAFahim.String.SuffixAutomaton
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class OccurrencePositions
    {
        public static int Find(int* linkPtr, int* lenPtr, int stateCount, int root, int targetLen, int* outPos)
        {
            int* stack = stackalloc int[stateCount];
            int top = 0;
            int count = 0;
            stack[top++] = root;
            while (top > 0)
            {
                int v = stack[--top];
                if (lenPtr[v] == targetLen)
                {
                    outPos[count++] = lenPtr[v];
                }
                for (int i = 0; i < stateCount; i++)
                {
                    if (linkPtr[i] == v)
                        stack[top++] = i;
                }
            }
            return count;
        }
    }
}
