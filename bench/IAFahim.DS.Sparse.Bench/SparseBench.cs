namespace IAFahim.DS.Sparse.Bench
{
    using IAFahim.DS.Sparse;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<SparseBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class SparseBench
    {
        [Params(1024, 4096, 16384)]
        public int N;

        private int* _arr;
        private int* _st;

        [GlobalSetup]
        public void Setup()
        {
            _arr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int logN = 1;
            while ((1 << logN) <= N) logN++;
            _st = (int*)Marshal.AllocHGlobal(N * logN * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) _arr[i] = rng.Next();
        }

        [Benchmark(Baseline = true)]
        public void SparseTableBuild()
        {
            int logN = 1;
            while ((1 << logN) <= N) logN++;
            SparseTableBuild.RunInt32(_arr, _st, null, N);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_arr);
            Marshal.FreeHGlobal((nint)_st);
        }
    }
}