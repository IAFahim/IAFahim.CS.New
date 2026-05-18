namespace IAFahim.Unique.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using IAFahim.Unique;

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

        private int* _valuesInt;
        private int* _uniqueInt;
        private long* _valuesLong;
        private long* _uniqueLong;

        [GlobalSetup]
        public void Setup()
        {
            _valuesInt = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _uniqueInt = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _valuesLong = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            _uniqueLong = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
            {
                _valuesInt[i] = rng.Next(N / 4);
                _valuesLong[i] = rng.Next(N / 4);
            }
        }

        [Benchmark]
        public void UniqueInts_Bench()
        {
            UniqueInts.Run(_valuesInt, N);
        }

        [Benchmark]
        public void UniqueInt64s_Bench()
        {
            UniqueInt64s.Run(_valuesLong, N);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_valuesInt);
            Marshal.FreeHGlobal((nint)_uniqueInt);
            Marshal.FreeHGlobal((nint)_valuesLong);
            Marshal.FreeHGlobal((nint)_uniqueLong);
        }
    }
}