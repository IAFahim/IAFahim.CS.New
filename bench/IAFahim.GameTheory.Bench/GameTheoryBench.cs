using System;
namespace IAFahim.GameTheory.Bench
{
    using IAFahim.GameTheory;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<GameTheoryBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class GameTheoryBench
    {
        [Params(256, 1024)]
        public int N;

        private int* _to;
        private int* _grundy;
        private int* _indeg;
        private int* _queue;
        private long* _piles;

        [GlobalSetup]
        public void Setup()
        {
            _to = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _grundy = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _indeg = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _queue = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _piles = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) _to[i] = i + 1 < N ? i + 1 : -1;
            for (int i = 0; i < N; i++) _indeg[i] = i == 0 ? 0 : 1;
            for (int i = 0; i < N; i++) _piles[i] = rng.Next() & 0xFF;
        }

        [Benchmark]
        public void GrundyDAG()
        {
            GrundyDAG.Run(N, _to, _grundy, _indeg, _queue);
        }

        [Benchmark(Baseline = true)]
        public void NimSum()
        {
            NimSum.Run(N, _piles);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_to);
            Marshal.FreeHGlobal((nint)_grundy);
            Marshal.FreeHGlobal((nint)_indeg);
            Marshal.FreeHGlobal((nint)_queue);
            Marshal.FreeHGlobal((nint)_piles);
        }
    }
}