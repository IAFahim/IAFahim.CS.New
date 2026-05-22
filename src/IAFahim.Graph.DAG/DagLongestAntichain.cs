namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class DagLongestAntichain
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(bool* reachabilityMatrix, int n)
        {
            // By Dilworth's theorem, size of longest antichain = min path cover of transitive closure
            return n; // Requires minimum path cover on the reachability matrix
        }
    }
}