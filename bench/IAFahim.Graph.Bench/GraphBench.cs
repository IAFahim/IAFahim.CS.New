namespace IAFahim.Graph.Bench
{
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
        private long* _dist;
        private int _m;

        [GlobalSetup]
        public void Setup()
        {
            _m = N * 2;
            _head = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _to = (int*)Marshal.AllocHGlobal(_m * sizeof(int));
            _next = (int*)Marshal.AllocHGlobal(_m * sizeof(int));
            _dist = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            for (int i = 0; i < N; i++) _head[i] = 0;
            Random rng = new Random(42);
            for (int i = 0; i < N - 1; i++)
                AddEdge.Run(N, _head, _to, _next, i, i + 1);
            for (int i = 0; i < N; i++)
                _dist[i] = -1;
        }

        [Benchmark(Baseline = true)]
        public void Bfs()
        {
            for (int i = 0; i < N; i++) _dist[i] = -1;
            Bfs.Run(0, N, _head, _to, _next, _dist);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_head);
            Marshal.FreeHGlobal((nint)_to);
            Marshal.FreeHGlobal((nint)_next);
            Marshal.FreeHGlobal((nint)_dist);
        }
    }
}
