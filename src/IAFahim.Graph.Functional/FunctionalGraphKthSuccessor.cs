namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class FunctionalGraphKthSuccessor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* f, int n, int u, long k)
        {
            int curr = u;
            for (long i = 0; i < k; i++)
            {
                curr = f[curr];
            }
            return curr;
        }
    }
}
