namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class DagLexicographicKthPath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* head, int* next, int* to, long* pathCount, int u, long k, int* pathOut)
        {
            // Assuming adjacency lists are sorted lexicographically
            int len = 0;
            while (true)
            {
                pathOut[len++] = u;
                if (k <= 1) break;
                k--;
                int nextNode = -1;
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (k <= pathCount[v])
                    {
                        nextNode = v;
                        break;
                    }
                    k -= pathCount[v];
                }
                if (nextNode == -1) break;
                u = nextNode;
            }
            return len;
        }
    }
}