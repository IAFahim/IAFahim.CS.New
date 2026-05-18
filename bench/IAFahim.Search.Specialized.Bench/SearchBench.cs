namespace IAFahim.Search.Specialized.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using IAFahim.Search.Specialized;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<SearchBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class SearchBench
    {
        [Params(64, 256, 1024)]
        public int N;

        private int* _sorted;

        [GlobalSetup]
        public void Setup()
        {
            _sorted = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            _sorted[0] = rng.Next(10);
            for (int i = 1; i < N; i++)
                _sorted[i] = _sorted[i - 1] + rng.Next(10) + 1;
        }

        [Benchmark(Baseline = true)]
        public void SpanBinarySearch()
        {
            var span = new Span<int>(_sorted, N);
            for (int i = 0; i < N; i += N / 16 + 1)
                span.BinarySearch(_sorted[i]);
        }

        [Benchmark]
        public void LowerBound_Bench()
        {
            for (int i = 0; i < N; i += N / 16 + 1)
                LowerBound.Run(_sorted, N, _sorted[i]);
        }

        [Benchmark]
        public void UpperBound_Bench()
        {
            for (int i = 0; i < N; i += N / 16 + 1)
                UpperBound.Run(_sorted, N, _sorted[i]);
        }

        [Benchmark]
        public void BinarySearch_Bench()
        {
            for (int i = 0; i < N; i += N / 16 + 1)
                BinarySearch.TryFind(_sorted, N, _sorted[i], out int index);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_sorted);
        }
    }
}