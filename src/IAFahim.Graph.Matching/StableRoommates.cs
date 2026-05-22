namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;

    public static unsafe class StableRoommates
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* pref, int n, int* match)
        {
            for (int i = 0; i < n; i++) match[i] = -1;
            return false; // False if no stable matching exists
        }
    }
}