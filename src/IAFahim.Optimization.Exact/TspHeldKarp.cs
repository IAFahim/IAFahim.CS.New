namespace IAFahim.Optimization.Exact
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TspHeldKarp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, long* w, long inf, long* dp)
        {
            return HamiltonianCycle.Run(n, w, inf, dp);
        }
    }
}
