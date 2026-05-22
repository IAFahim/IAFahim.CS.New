namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class DagPathCoverRestore
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* match, int n, int* nextInPath)
        {
            int numPaths = 0;
            for (int i = 0; i < n; i++)
            {
                nextInPath[i] = match[i];
                if (match[i] == -1) numPaths++;
            }
            return numPaths;
        }
    }
}