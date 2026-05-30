namespace IAFahim.Graph.Eertree
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Eertree
    {
        public struct Node { public int Len; public int Link; public int FirstNext; }
        public struct Next { public int Char; public int To; public int Next; }

        public static void Build(int* s, int len, Node* nodes, Next* next, ref int nodeCount, ref int nextCount, ref int last, ref int cur)
        {
            nodeCount = 2; nextCount = 0; last = 1;
            nodes[0].Len = -1; nodes[0].Link = 0; nodes[0].FirstNext = -1;
            nodes[1].Len = 0; nodes[1].Link = 0; nodes[1].FirstNext = -1;
            cur = 1;
            for (int i = 0; i < len; i++)
            {
                int c = s[i];
                while (true)
                {
                    int curLen = nodes[cur].Len;
                    if (i - curLen - 1 >= 0 && s[i - curLen - 1] == c) break;
                    cur = nodes[cur].Link;
                }
                int exist = FindTransition(cur, c, nodes, next);
                if (exist != -1) { cur = exist; continue; }

                int newNode = nodeCount++;
                nodes[newNode].Len = nodes[cur].Len + 2;
                nodes[newNode].FirstNext = -1;

                NextEdge(newNode, c, newNode, nodes, next, ref nextCount);

                if (nodes[newNode].Len == 1) { nodes[newNode].Link = 1; }
                else
                {
                    int temp = nodes[cur].Link;
                    while (true)
                    {
                        int tempLen = nodes[temp].Len;
                        if (i - tempLen - 1 >= 0 && s[i - tempLen - 1] == c) break;
                        temp = nodes[temp].Link;
                    }
                    nodes[newNode].Link = FindTransition(temp, c, nodes, next);
                }
                cur = newNode;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindTransition(int node, int c, Node* nodes, Next* next)
        {
            for (int e = nodes[node].FirstNext; e != -1; e = next[e].Next)
                if (next[e].Char == c) return next[e].To;
            return -1;
        }

        private static void NextEdge(int to, int c, int newNode, Node* nodes, Next* next, ref int nextCount)
        {
            next[nextCount] = new Next { Char = c, To = newNode, Next = nodes[to].FirstNext };
            nodes[to].FirstNext = nextCount++;
        }
    }
}
