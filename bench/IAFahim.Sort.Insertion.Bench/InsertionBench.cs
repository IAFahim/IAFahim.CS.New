namespace IAFahim.Sort.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<InsertionBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class InsertionBench
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
            Span<int> span = new Span<int>(_work, N);
            span.Sort();
        }

        [Benchmark]
        public void InsertionSort()
        {
            Insertion.Insertion.Run(_work, N);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((IntPtr)_source);
            Marshal.FreeHGlobal((IntPtr)_work);
        }
    }
}
