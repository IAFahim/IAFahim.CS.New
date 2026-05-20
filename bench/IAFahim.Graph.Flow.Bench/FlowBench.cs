namespace IAFahim.Graph.Flow.Bench
{
    using IAFahim.Graph.Flow;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<FlowBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class FlowBench
    {
        [Params(100, 500)]
        public int N;

        private int* _head;
        private int* _to;
        private int* _next;
        private int* _cap;

        [GlobalSetup]
        public void Setup()
        {
            _head = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _to = (int*)Marshal.AllocHGlobal(N * 4 * sizeof(int));
            _next = (int*)Marshal.AllocHGlobal(N * 4 * sizeof(int));
            _cap = (int*)Marshal.AllocHGlobal(N * 4 * sizeof(int));
            for (int i = 0; i < N; i++) _head[i] = 0;
            int edgeId = 0;
            for (int i = 0; i < N * 2; i++)
            {
                MinCostFlowAddEdge.Run(_head, _to, _next, null, _cap, &edgeId, i % N, (i + 1) % N, 10, 1);
            }
        }

        [Benchmark(Baseline = true)]
        public void RunDinicMaxFlow()
        {
            long flow = DinicMaxFlow.Run(N, 0, N - 1, _head, _to, _next, _cap);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_head);
            Marshal.FreeHGlobal((nint)_to);
            Marshal.FreeHGlobal((nint)_next);
            Marshal.FreeHGlobal((nint)_cap);
        }
    }
}