namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;

    public static unsafe class AssignmentAuctionAlgorithm
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* cost, int n, int* match, int* prices)
        {
            for (int i = 0; i < n; i++)
            {
                match[i] = -1;
                prices[i] = 0;
            }
        }
    }
}