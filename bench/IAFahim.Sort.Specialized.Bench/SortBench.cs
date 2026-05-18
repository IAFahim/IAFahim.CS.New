namespace IAFahim.Sort.Specialized.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using IAFahim.Sort.Specialized;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<SortBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class SortBench
    {
        [Params(64, 256, 1024)]
        public int N;

        private int* _sourceInt;
        private int* _workInt;
        private long* _sourceLong;
        private long* _workLong;

        [GlobalSetup]
        public void Setup()
        {
            _sourceInt = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _workInt = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _sourceLong = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            _workLong = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
            {
                _sourceInt[i] = rng.Next();
                _sourceLong[i] = rng.Next();
            }
        }

        [IterationSetup]
        public void CopySource()
        {
            Buffer.MemoryCopy(_sourceInt, _workInt, N * sizeof(int), N * sizeof(int));
            Buffer.MemoryCopy(_sourceLong, _workLong, N * sizeof(long), N * sizeof(long));
        }

        [Benchmark(Baseline = true)]
        public void SpanSort()
        {
            new Span<int>(_workInt, N).Sort();
        }

        [Benchmark]
        public void SortInts_Bench()
        {
            SortInts.Run(_workInt, N);
        }

        [Benchmark]
        public void SortInt64s_Bench()
        {
            SortInt64s.Run(_workLong, N);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_sourceInt);
            Marshal.FreeHGlobal((nint)_workInt);
            Marshal.FreeHGlobal((nint)_sourceLong);
            Marshal.FreeHGlobal((nint)_workLong);
        }
    }
}