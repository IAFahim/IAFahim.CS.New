namespace IAFahim.Optimization.Treewidth
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class CutAndCount
    {
        public static int Run(int n, bool* adj, int* bag, int bagSize, long* dp)
        {
            int conn = 1;
            int cut = 0;
            for (int i = 0; i < bagSize; i++)
            {
                for (int j = i + 1; j < bagSize; j++)
                {
                    if (adj[bag[i] * n + bag[j]])
                    {
                        conn++;
                    }
                    else
                    {
                        cut++;
                    }
                }
            }
            return cut == 0 ? conn : 0;
        }
    }
}
