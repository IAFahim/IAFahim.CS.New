namespace IAFahim.Math.Polynomial.Bench
{
    using IAFahim.Math.Polynomial;
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<OnlineNttBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class OnlineNttBench
    {
        [Params(64, 256, 1024)]
        public int N;

        private long* _a;
        private long* _res;
        private long* _work;

        [GlobalSetup]
        public void Setup()
        {
            _a = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            _res = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            _work = (long*)Marshal.AllocHGlobal(16 * N * sizeof(long));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) _a[i] = rng.Next(100);
        }

        [Benchmark]
        public void OnlineNtt()
        {
            OnlineNttConvolution.Run(_a, N, _res, N, 998244353, 3, _work);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_a);
            Marshal.FreeHGlobal((nint)_res);
            Marshal.FreeHGlobal((nint)_work);
        }
    }
}
