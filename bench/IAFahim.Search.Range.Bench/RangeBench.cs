namespace IAFahim.Search.Range.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using RangeSum = IAFahim.Search.Range.RangeSum;
    using RangeAdd = IAFahim.Search.Range.RangeAdd;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<RangeBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class RangeBench
    {
        [Params(1024, 4096)]
        public int N;

        private int* _data;
        private int* _prefix;
        private int* _diff;

        [GlobalSetup]
        public void Setup()
        {
            _data = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _prefix = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _diff = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
                _data[i] = rng.Next(100);
        }

        [Benchmark]
        public void BuildPrefix()
        {
            RangeSum.BuildPrefix(_prefix, _data, N);
        }

        [Benchmark]
        public void RangeSum_Bench()
        {
            RangeSum.BuildPrefix(_prefix, _data, N);
            for (int i = 0; i < N; i += 16)
                RangeSum.Run(_prefix, i, i + 8);
        }

        [Benchmark]
        public void RangeAdd_Bench()
        {
            for (int j = 0; j < 10; j++)
                RangeAdd.Run(_diff, N, N / 4, N * 3 / 4, 1);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_data);
            Marshal.FreeHGlobal((nint)_prefix);
            Marshal.FreeHGlobal((nint)_diff);
        }
    }
}