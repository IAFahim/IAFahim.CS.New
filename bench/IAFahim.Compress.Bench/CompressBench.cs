namespace IAFahim.Compress.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using IAFahim.Compress;
    using CV = IAFahim.Compress.CompressValues;
    using RC = IAFahim.Compress.RestoreCompressed;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<CompressBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class CompressBench
    {
        [Params(1024, 4096)]
        public int N;

        private int* _values;
        private long* _compressed;
        private int* _restored;

        [GlobalSetup]
        public void Setup()
        {
            _values = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _compressed = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            _restored = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
                _values[i] = rng.Next(100000);
        }

        [Benchmark]
        public void CompressValues_Bench()
        {
            CV.Run(_values, _compressed, N);
        }

        [Benchmark]
        public void CompressValuesUnique_Bench()
        {
            int count = CV.RunUnique(_values, _compressed, N);
            RC.Run(_compressed, _restored, count);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_values);
            Marshal.FreeHGlobal((nint)_compressed);
            Marshal.FreeHGlobal((nint)_restored);
        }
    }
}