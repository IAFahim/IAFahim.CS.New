namespace IAFahim.String.SuffixTree
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SuffixTreeUkkonen
    {
        public struct Node { public int Link; public int Start; public int Len; public int FirstEdge; }
        public struct Edge { public int To; public int Char; public int Next; public int Min; public int Max; }

        public static void Build(int* s, int len, Node* nodes, Edge* edges, ref int nodeCount, ref int edgeCount, ref int last)
        {
            nodeCount = 1; edgeCount = 0; last = 0;
            nodes[0].Start = -1; nodes[0].Len = 0; nodes[0].Link = 0; nodes[0].FirstEdge = -1;
            int activeNode = 0, activeEdge = -1, activeLen = 0;
            int i = 0;
            while (i < len)
            {
                int c = s[i];
                int added = 0;
                while (added <= 1 && i + added < len)
                {
                    c = s[i + added];
                    int nextEdge = nodes[activeNode].FirstEdge;
                    bool found = false;
                    while (nextEdge != -1)
                    {
                        if (edges[nextEdge].Char == c) { found = true; break; }
                        nextEdge = edges[nextEdge].Next;
                    }
                    if (!found) break;
                    added++;
                }
                if (added > 0)
                {
                    i += added;
                    continue;
                }
                int newNode = nodeCount++;
                nodes[newNode].Start = i;
                nodes[newNode].Len = len - i;
                nodes[newNode].FirstEdge = -1;
                nodes[newNode].Link = 0;

                int newEdge = edgeCount++;
                edges[newEdge].To = newNode;
                edges[newEdge].Char = c;
                edges[newEdge].Min = i;
                edges[newEdge].Max = len;
                edges[newEdge].Next = nodes[activeNode].FirstEdge;
                nodes[activeNode].FirstEdge = newEdge;
                i++;
            }
        }
    }
}
