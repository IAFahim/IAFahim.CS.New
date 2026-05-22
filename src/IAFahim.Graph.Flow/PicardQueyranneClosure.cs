namespace IAFahim.Graph.Flow
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PicardQueyranneClosure
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow, bool* inClosure)
        {
            MinimumCutRecover.Run(n, s, head, to, next, cap, flow, inClosure);
        }
    }
}