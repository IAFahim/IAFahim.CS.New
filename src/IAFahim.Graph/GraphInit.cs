namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GraphInit
    {
        public static void Run(int n, int* head, int* parent, int* depth, int* size)
        {
            for (int i = 0; i < n; i++)
            {
                head[i] = 0;
                parent[i] = -1;
                depth[i] = 0;
                size[i] = 1;
            }
        }
    }
}