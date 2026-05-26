namespace IAFahim.String.Bench
{
    using System;
    using IAFahim.String;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<StringBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class StringBench
    {
        [Params(100, 1000, 10000)]
        public int N;

        private byte* _s;

        [GlobalSetup]
        public void Setup()
        {
            _s = (byte*)Marshal.AllocHGlobal(N * sizeof(byte));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) _s[i] = (byte)('a' + rng.Next(26));
        }

        [Benchmark(Baseline = true)]
        public void ZAlgorithm()
        {
            int* z = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            IAFahim.String.ZAlgorithm.Run(_s, N, z);
            Marshal.FreeHGlobal((nint)z);
        }

        [Benchmark]
        public void ManacherOdd()
        {
            int* d = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            IAFahim.String.ManacherOdd.Run(_s, N, d);
            Marshal.FreeHGlobal((nint)d);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_s);
        }
    }
}
