namespace IAFahim.Sort.Specialized.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

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

        private int* _source;
        private int* _work;

        [GlobalSetup]
        public void Setup()
        {
            _source = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _work = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
                _source[i] = rng.Next();
        }

        [IterationSetup]
        public void CopySource()
        {
            Buffer.MemoryCopy(_source, _work, N * sizeof(int), N * sizeof(int));
        }

        [Benchmark(Baseline = true)]
        public void SpanSort()
        {
            new Span<int>(_work, N).Sort();
        }

        [Benchmark]
        public void SortInts()
        {
            SortInts.Run(_work, N);
        }

        [Benchmark]
        public void SortInt64s()
        {
            SortInt64s.Run(_work, N);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_source);
            Marshal.FreeHGlobal((nint)_work);
        }
    }
}