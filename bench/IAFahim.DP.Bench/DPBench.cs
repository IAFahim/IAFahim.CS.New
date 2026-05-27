using System;
namespace IAFahim.DP.Bench
{
    using IAFahim.DP;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<DPBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class DPBench
    {
        [Params(16, 20)]
        public int N;

        private long* _weight;
        private long* _value;
        private long* _dp;
        private long* _dpWork;
        private long* _cost;
        private int* _bestSet;

        [GlobalSetup]
        public void Setup()
        {
            _weight = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            _value = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            _dp = (long*)Marshal.AllocHGlobal(N * N * sizeof(long));
            _dpWork = (long*)Marshal.AllocHGlobal(N * N * sizeof(long));
            _cost = (long*)Marshal.AllocHGlobal(N * N * sizeof(long));
            _bestSet = (int*)Marshal.AllocHGlobal(N * sizeof(int));

            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
            {
                _weight[i] = rng.Next(1, 100);
                _value[i] = rng.Next(1, 100);
            }
            for (int i = 0; i < N * N; i++)
                _cost[i] = rng.Next(1, 100);
        }

        [Benchmark(Baseline = true)]
        public void Baseline()
        {
            Knapsack01.RunSpaceOptimized(N, N * 50, _weight, _value, _dp);
        }

        [Benchmark]
        public void SosDp_Bench()
        {
            for (int i = 0; i < (1 << N); i++) _dp[i] = i;
            SosDp.Run(N, _dp);
        }

        [Benchmark]
        public void IntervalDp_Bench()
        {
            IntervalDp.Run(N, _dp, _cost);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_weight);
            Marshal.FreeHGlobal((nint)_value);
            Marshal.FreeHGlobal((nint)_dp);
            Marshal.FreeHGlobal((nint)_dpWork);
            Marshal.FreeHGlobal((nint)_cost);
            Marshal.FreeHGlobal((nint)_bestSet);
        }
    }
}