namespace IAFahim.Math.Transform.Bench
{
    using IAFahim.Math.Transform;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<TransformBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class TransformBench
    {
        [Params(256, 1024, 4096)]
        public int N;

        private long* _f;
        private long* _g;

        [GlobalSetup]
        public void Setup()
        {
            _f = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            _g = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) { _f[i] = rng.Next(100); _g[i] = rng.Next(100); }
        }

        [Benchmark(Baseline = true)]
        public void WalshHadamardXor()
        {
            long* tmp = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            for (int i = 0; i < N; i++) tmp[i] = _f[i];
            WalshHadamardXor.Run(tmp, N, 1);
            Marshal.FreeHGlobal((nint)tmp);
        }

        [Benchmark]
        public void SubsetZeta()
        {
            long* tmp = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            for (int i = 0; i < N; i++) tmp[i] = _f[i];
            SubsetZeta.Run(tmp, N);
            Marshal.FreeHGlobal((nint)tmp);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_f);
            Marshal.FreeHGlobal((nint)_g);
        }
    }
}
