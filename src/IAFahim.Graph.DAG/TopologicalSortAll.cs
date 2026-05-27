namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class TopologicalSortAll
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* head, int* next, int* to, int* indegree, int n, int* currentOrder, int count, int* totalFound)
        {
            if (count == n)
            {
                (*totalFound)++;
                return *totalFound;
            }

            for (int i = 0; i < n; i++)
            {
                if (indegree[i] == 0)
                {
                    currentOrder[count] = i;
                    indegree[i] = -1;

                    for (int e = head[i]; e != 0; e = next[e])
                    {
                        indegree[to[e]]--;
                    }

                    Run(head, next, to, indegree, n, currentOrder, count + 1, totalFound);

                    indegree[i] = 0;
                    for (int e = head[i]; e != 0; e = next[e])
                    {
                        indegree[to[e]]++;
                    }
                }
            }
            return *totalFound;
        }
    }
}