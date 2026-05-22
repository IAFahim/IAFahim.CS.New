namespace IAFahim.Graph.Connectivity
{
    using System.Runtime.CompilerServices;

    public static unsafe class DynamicReachabilityApprox
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* head, int* next, int* to, int u, int v, int maxDepth)
        {
            if (u == v) return true;
            if (maxDepth <= 0) return false;
            
            for (int e = head[u]; e != -1; e = next[e])
            {
                if (Run(head, next, to, to[e], v, maxDepth - 1))
                    return true;
            }
            return false;
        }
    }
}
