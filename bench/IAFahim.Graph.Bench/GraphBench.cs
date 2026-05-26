namespace IAFahim.Graph.Bench
{
    using System;
    using IAFahim.Graph;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<GraphBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class GraphBench
    {
        [Params(1000, 5000)]
        public int N;

        private int* _head;
        private int* _to;
        private int* _next;
        private int* _dist;
        private int* _parent;
        private int _m;

        [GlobalSetup]
        public void Setup()
        {
            _m = N * 2;
            _head = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _to = (int*)Marshal.AllocHGlobal(_m * sizeof(int));
            _next = (int*)Marshal.AllocHGlobal(_m * sizeof(int));
            _dist = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _parent = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            for (int i = 0; i < N; i++) _head[i] = 0;
            int edgeId = 1;
            int edgeCount = _m;
            for (int i = 0; i < N - 1; i++)
            {
                AddEdge.Run(_head, _to, _next, &edgeId, i, i + 1, &edgeCount);
            }
        }

        [Benchmark(Baseline = true)]
        public void Bfs()
        {
            IAFahim.Graph.Bfs.Run(N, 0, _head, _to, _next, _dist, _parent);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_head);
            Marshal.FreeHGlobal((nint)_to);
            Marshal.FreeHGlobal((nint)_next);
            Marshal.FreeHGlobal((nint)_dist);
            Marshal.FreeHGlobal((nint)_parent);
        }
    }
}
