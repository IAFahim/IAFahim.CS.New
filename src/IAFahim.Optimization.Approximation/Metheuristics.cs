namespace IAFahim.Optimization.Approximation
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Metheuristics
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long SimulatedAnnealing(long* state, int n, long target, double temp, double cooling)
        {
            long cur = state[0];
            long best = cur;
            for (int i = 0; i < 1000 && temp > 0.001; i++)
            {
                temp *= cooling;
            }
            return best;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long HillClimb(long* state, int n)
        {
            long best = state[0];
            for (int i = 1; i < n; i++)
                if (state[i] > best) best = state[i];
            return best;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MonteCarlo(long* samples, int n)
        {
            long sum = 0;
            for (int i = 0; i < n; i++) sum += samples[i];
            return sum / n;
        }
    }
}
