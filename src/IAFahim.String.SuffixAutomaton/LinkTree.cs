namespace IAFahim.String.SuffixAutomaton
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class LinkTree
    {
        public static void Traverse(int* linkPtr, int stateCount, int root, delegate*<int, void> callback, int* stack)
        {
            int top = 0;
            stack[top++] = root;
            while (top > 0)
            {
                int v = stack[--top];
                callback(v);
                for (int i = 0; i < stateCount; i++)
                {
                    if (linkPtr[i] == v)
                        stack[top++] = i;
                }
            }
        }
    }
}
