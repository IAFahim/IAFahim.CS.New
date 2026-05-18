namespace IAFahim.Unique.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<UniqueBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class UniqueBench
    {
        [Params(256, 1024)]
        public int N;

        private int* _values;
        private int* _unique;

        [GlobalSetup]
        public void Setup()
        {
            _values = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _unique = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
                _values[i] = rng.Next(N / 4);
        }

        [Benchmark]
        public void UniqueInts()
        {
            UniqueInts.Run(_values, _unique, N);
        }

        [Benchmark]
        public void UniqueInt64s()
        {
            UniqueInt64s.Run(_values, _unique, N);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_values);
            Marshal.FreeHGlobal((nint)_unique);
        }
    }
}