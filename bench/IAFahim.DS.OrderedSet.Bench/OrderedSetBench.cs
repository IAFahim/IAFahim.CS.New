namespace IAFahim.DS.OrderedSet.Bench
{
    using IAFahim.DS.OrderedSet;
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<OrderedSetBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class OrderedSetBench
    {
        [Params(64, 256, 1024)]
        public int N;

        private int* _data;
        private int* _sorted;
        private int _sortedLen;

        [GlobalSetup]
        public void Setup()
        {
            _data = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _sorted = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _sortedLen = 0;
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
                _data[i] = rng.Next(N * 2);
        }

        [IterationSetup]
        public void ResetSorted()
        {
            _sortedLen = 0;
        }

        [Benchmark(Baseline = true)]
        public void OrderedSetInsert()
        {
            ResetSorted();
            for (int i = 0; i < N; i++)
                _sortedLen = OrderedSet.Insert(_sorted, _sortedLen, _data[i]);
        }

        [Benchmark]
        public void OrderedSetRank()
        {
            int sum = 0;
            for (int i = 0; i < _sortedLen; i++)
                sum += OrderedSet.Rank(_sorted, _sortedLen, _data[i]);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_data);
            Marshal.FreeHGlobal((nint)_sorted);
        }
    }
}