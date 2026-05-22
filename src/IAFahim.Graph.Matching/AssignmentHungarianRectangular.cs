namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;

    public static unsafe class AssignmentHungarianRectangular
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* cost, int n, int m, int* matchLeft, int* matchRight)
        {
            // Hungarian for N x M
            for (int i = 0; i < n; i++) matchLeft[i] = -1;
            for (int j = 0; j < m; j++) matchRight[j] = -1;
        }
    }
}