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

        [Benchmark(Baseline = true)]
        public void DinicMaxFlow()
        {
            int* head = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(N * 4 * sizeof(int));
            int* next = (int*)Marshal.AllocHGlobal(N * 4 * sizeof(int));
            int* cap = (int*)Marshal.AllocHGlobal(N * 4 * sizeof(int));
            for (int i = 0; i < N; i++) head[i] = 0;
            Random rng = new Random(42);
            for (int i = 0; i < N * 2; i++)
                AddWeightedEdge.Run(N, head, to, next, null, i % N, (i + 1) % N, 10);
            long flow = DinicMaxFlow.Run(N, 0, N - 1, head, to, next, cap);
            Marshal.FreeHGlobal((nint)head);
            Marshal.FreeHGlobal((nint)to);
            Marshal.FreeHGlobal((nint)next);
            Marshal.FreeHGlobal((nint)cap);
        }
    }
}
