using System;
namespace IAFahim.DP.General.Bench
{
    using IAFahim.DP.General;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<DPGeneralBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class DPGeneralBench
    {
        [Params(8, 12)]
        public int M;

        private int _n;
        private int* _a;
        private long* _dp;
        private long* _tmp;

        [GlobalSetup]
        public void Setup()
        {
            _n = 16;
            int maskCount = 1 << M;
            _a = (int*)Marshal.AllocHGlobal(_n * M * sizeof(int));
            _dp = (long*)Marshal.AllocHGlobal(maskCount * sizeof(long));
            _tmp = (long*)Marshal.AllocHGlobal(maskCount * sizeof(long));

            Random rng = new Random(42);
            for (int i = 0; i < _n * M; i++)
                _a[i] = rng.Next(1, 100);
        }

        [Benchmark(Baseline = true)]
        public void ProfileDp_Bench()
        {
            ProfileDp.Run(M, _n, _a, _dp, _tmp);
        }

        [Benchmark]
        public void BrokenProfileDp_Bench()
        {
            BrokenProfileDp.Run(M, _n, _a, _dp, _tmp, null);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_a);
            Marshal.FreeHGlobal((nint)_dp);
            Marshal.FreeHGlobal((nint)_tmp);
        }
    }
}