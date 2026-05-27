using System;
namespace IAFahim.DP.Optimization.Bench
{
    using IAFahim.DP.Optimization;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<DPOptimizationBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class DPOptimizationBench
    {
        [Params(64, 256, 1024)]
        public int N;

        private long* _dp;
        private long* _a;
        private long* _opt;
        private long* _seg;

        [GlobalSetup]
        public void Setup()
        {
            _dp = (long*)Marshal.AllocHGlobal(N * N * sizeof(long));
            _a = (long*)Marshal.AllocHGlobal((N + 1) * sizeof(long));
            _opt = (long*)Marshal.AllocHGlobal(N * N * sizeof(long));
            _seg = (long*)Marshal.AllocHGlobal(N * 4 * 2 * sizeof(long));

            Random rng = new Random(42);
            for (int i = 0; i <= N; i++)
                _a[i] = rng.Next(1, 100);
            for (int i = 0; i < N * 4 * 2; i++)
                _seg[i] = 0;
        }

        [Benchmark(Baseline = true)]
        public void KnuthOptimization_Bench()
        {
            KnuthOptimization.Run(N, _dp, _a, _opt);
        }

        [Benchmark]
        public void LiChaoAddLine_Bench()
        {
            for (int i = 0; i < N * 4 * 2; i++) _seg[i] = 0;
            for (int i = 0; i < N; i++)
                LiChaoAddLine.Run(_seg, i + 1, i * 7, 1, 0, N, 0, N - 1);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_dp);
            Marshal.FreeHGlobal((nint)_a);
            Marshal.FreeHGlobal((nint)_opt);
            Marshal.FreeHGlobal((nint)_seg);
        }
    }
}