namespace IAFahim.DS.Heap.Bench
{
    using System;
    using IAFahim.DS.Heap;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<HeapBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class HeapBench
    {
        [Params(1000, 10000, 100000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark(Baseline = true)]
        public void HeapPush()
        {
            int* heap = stackalloc int[N];
            int len = 0;
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
                IAFahim.DS.Heap.HeapPush.Run(heap, len++, rng.Next());
        }

        [Benchmark]
        public void HeapPop()
        {
            int* heap = stackalloc int[N];
            int len = 0;
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) IAFahim.DS.Heap.HeapPush.Run(heap, len++, rng.Next());
            for (int i = 0; i < N; i++)
                IAFahim.DS.Heap.HeapPop.Run(heap, len--);
        }

        [GlobalCleanup]
        public void Cleanup() { }
    }
}