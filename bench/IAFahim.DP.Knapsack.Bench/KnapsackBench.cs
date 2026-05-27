using System;
namespace IAFahim.DP.Knapsack.Bench
{
    using IAFahim.DP.Knapsack;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<KnapsackBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class KnapsackBench
    {
        [Params(100, 1000, 10000)]
        public int N;

        private long* _w;
        private long* _v;
        private long* _cnt;
        private long* _dp;
        private bool* _bDP;
        private long* _bits;

        [GlobalSetup]
        public void Setup()
        {
            int cap = N * 10;
            _w = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            _v = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            _cnt = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            _dp = (long*)Marshal.AllocHGlobal((cap + 1) * sizeof(long));
            _bDP = (bool*)Marshal.AllocHGlobal((cap + 1) * sizeof(bool));
            _bits = (long*)Marshal.AllocHGlobal(((cap / 64) + 2) * sizeof(long));

            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
            {
                _w[i] = rng.Next(1, 100);
                _v[i] = rng.Next(1, 100);
                _cnt[i] = rng.Next(1, 10);
            }
        }

        [Benchmark(Baseline = true)]
        public void Knapsack01_Bench()
        {
            Knapsack01.Run(N, N * 10, _w, _v, _dp);
        }

        [Benchmark]
        public void KnapsackUnbounded_Bench()
        {
            KnapsackUnbounded.Run(N, N * 10, _w, _v, _dp);
        }

        [Benchmark]
        public void KnapsackBounded_Bench()
        {
            KnapsackBounded.Run(N, N * 10, _w, _v, _cnt, _dp);
        }

        [Benchmark]
        public void SubsetSum_Bench()
        {
            SubsetSum.Run(N, N * 5, _w, _bDP);
        }

        [Benchmark]
        public void BitsetSubsetSum_Bench()
        {
            BitsetSubsetSum.Run(N, N * 5, _w, _bits);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_w);
            Marshal.FreeHGlobal((nint)_v);
            Marshal.FreeHGlobal((nint)_cnt);
            Marshal.FreeHGlobal((nint)_dp);
            Marshal.FreeHGlobal((nint)_bDP);
            Marshal.FreeHGlobal((nint)_bits);
        }
    }
}