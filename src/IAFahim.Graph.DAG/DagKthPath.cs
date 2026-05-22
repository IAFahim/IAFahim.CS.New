namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class DagKthPath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* head, int* next, int* to, long* pathCount, int u, long k, int* pathOut)
        {
            int len = 0;
            while (true)
            {
                pathOut[len++] = u;
                if (k <= 1) break;
                k--;
                int nextNode = -1;
                for (int e = head[u]; e != -1; e = next[e])
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