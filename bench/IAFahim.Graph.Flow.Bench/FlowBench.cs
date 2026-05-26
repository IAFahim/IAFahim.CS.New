namespace IAFahim.Graph.Flow.Bench
{
    using System;
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
        private int* _cost;
        private int* _flow;
        private int _m;

        [GlobalSetup]
        public void Setup()
        {
            _m = N * 4;
            _head = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _to = (int*)Marshal.AllocHGlobal(_m * sizeof(int));
            _next = (int*)Marshal.AllocHGlobal(_m * sizeof(int));
            _cap = (int*)Marshal.AllocHGlobal(_m * sizeof(int));
            _cost = (int*)Marshal.AllocHGlobal(_m * sizeof(int));
            _flow = (int*)Marshal.AllocHGlobal(_m * sizeof(int));

            for (int i = 0; i < N; i++) _head[i] = 0;
            int edgeId = 1;
            for (int i = 0; i < N * 2; i++)
            {
                MinCostFlowAddEdge.Run(_head, _to, _next, _cost, _cap, &edgeId, i % N, (i + 1) % N, 10, 1);
            }
        }

        [IterationSetup]
        public void ResetFlow()
        {
            for (int i = 0; i < _m; i++) _flow[i] = 0;
        }

        [Benchmark(Baseline = true)]
        public void RunDinicMaxFlow()
        {
            long flow = DinicMaxFlow.Run(N, 0, N - 1, _head, _to, _next, _cap, _flow);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_head);
            Marshal.FreeHGlobal((nint)_to);
            Marshal.FreeHGlobal((nint)_next);
            Marshal.FreeHGlobal((nint)_cap);
            Marshal.FreeHGlobal((nint)_cost);
            Marshal.FreeHGlobal((nint)_flow);
        }
    }
}