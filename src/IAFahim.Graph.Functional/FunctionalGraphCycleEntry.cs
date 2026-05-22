namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class FunctionalGraphCycleEntry
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* f, int n, int u)
        {
            int slow = f[u];
            int fast = f[f[u]];
            while (slow != fast)
            {
                slow = f[slow];
                fast = f[f[fast]];
            }
            slow = u;
            while (slow != fast)
            {
                slow = f[slow];
                fast = f[fast];
            }
            return slow;
        }
    }
}
