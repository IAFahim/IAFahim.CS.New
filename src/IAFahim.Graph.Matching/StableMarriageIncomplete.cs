namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;

    public static unsafe class StableMarriageIncomplete
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* prefMen, int* prefWomen, int* numPrefMen, int* numPrefWomen, int n, int m, int* matchMen, int* matchWomen)
        {
            for (int i = 0; i < n; i++) matchMen[i] = -1;
            for (int j = 0; j < m; j++) matchWomen[j] = -1;
        }
    }
}