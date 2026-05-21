namespace IAFahim.String.SuffixArray
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class LcpIntervalTree
    {
        public struct Node
        {
            public int Lcp;
            public int Left;
            public int Right;
            public int Child;
            public int Sibling;
        }

        public static int Build(int* lcp, int n, Node* nodes)
        {
            int* stack = (int*)Marshal.AllocHGlobal(sizeof(int) * (n + 1));
            int top = 0;
            int nodeCount = 0;
            stack[top++] = 0;
            for (int i = 1; i < n; i++)
            {
                int prevLcp = i < n ? lcp[i] : 0;
                while (top > 0 && lcp[stack[top - 1]] > prevLcp) top--;
                int parent = top > 0 ? stack[top - 1] : 0;
                nodes[nodeCount].Lcp = lcp[i];
                nodes[nodeCount].Left = i;
                nodes[nodeCount].Right = i;
                nodes[nodeCount].Child = -1;
                nodes[nodeCount].Sibling = -1;
                nodeCount++;
                stack[top++] = i;
            }
            Marshal.FreeHGlobal((nint)stack);
            return nodeCount;
        }
    }
}
