namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class FunctionalGraphPathAggregate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int* f, long* w, int n, int u, long k)
        {
            long sum = 0;
            int curr = u;
            for (long i = 0; i < k; i++)
            {
                sum += w[curr];
                curr = f[curr];
            }
            return sum;
        }
    }
}
