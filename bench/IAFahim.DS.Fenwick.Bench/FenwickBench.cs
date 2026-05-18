namespace IAFahim.DS.Fenwick.Bench
{
    using IAFahim.DS.Fenwick;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<FenwickBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class FenwickBench
    {
        [Params(1024, 4096, 16384)]
        public int N;

        private int* _bit;
        private int* _updates;

        [GlobalSetup]
        public void Setup()
        {
            _bit = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _updates = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) _updates[i] = rng.Next(N);
        }

        [IterationSetup]
        public void Reset()
        {
            for (int i = 0; i < N; i++) _bit[i] = 0;
        }

        [Benchmark(Baseline = true)]
        public void FenwickAdd()
        {
            for (int i = 0; i < N; i++)
                FenwickAdd.Run(_bit, _updates[i], 1);
        }

        [Benchmark]
        public void FenwickSum()
        {
            for (int i = 1; i <= N; i++)
                FenwickSum.Run(_bit, i);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_bit);
            Marshal.FreeHGlobal((nint)_updates);
        }
    }
}