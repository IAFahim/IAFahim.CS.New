namespace IAFahim.Graph.Tree.Bench
{
    using IAFahim.Graph.Tree;
    using IAFahim.Graph;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<TreeBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class TreeBench
    {
        [Params(1000, 5000)]
        public int N;

        private int* _head;
        private int* _to;
        private int* _next;
        private int* _depth;
        private int* _parent;

        [GlobalSetup]
        public void Setup()
        {
            int m = (N - 1) * 2;
            _head = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _to = (int*)Marshal.AllocHGlobal(m * sizeof(int));
            _next = (int*)Marshal.AllocHGlobal(m * sizeof(int));
            _depth = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _parent = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            for (int i = 0; i < N; i++) { _head[i] = 0; _depth[i] = -1; _parent[i] = -1; }
            for (int i = 0; i < N - 1; i++)
                AddEdge.Run(N, _head, _to, _next, i, i + 1);
        }

        [Benchmark(Baseline = true)]
        public void TreeDepth()
        {
            for (int i = 0; i < N; i++) _depth[i] = -1;
            TreeDepth.Run(N, 0, _head, _to, _next, _depth);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_head);
            Marshal.FreeHGlobal((nint)_to);
            Marshal.FreeHGlobal((nint)_next);
            Marshal.FreeHGlobal((nint)_depth);
            Marshal.FreeHGlobal((nint)_parent);
        }
    }
}
