namespace IAFahim.DS.Dsu.Bench
{
    using IAFahim.DS.Dsu;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<DsuBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class DsuBench
    {
        [Params(1000, 10000, 100000)]
        public int N;

        private int* _parent;
        private int* _size;
        private int* _unionU;
        private int* _unionV;

        [GlobalSetup]
        public void Setup()
        {
            _parent = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _size = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _unionU = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _unionV = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) _unionU[i] = rng.Next(N);
            for (int i = 0; i < N; i++) _unionV[i] = rng.Next(N);
            DsuInit.Run(N, _parent, _size);
        }

        [IterationSetup]
        public void ResetDsu()
        {
            DsuInit.Run(N, _parent, _size);
        }

        [Benchmark(Baseline = true)]
        public void DsuUnion()
        {
            for (int i = 0; i < N; i++)
                DsuUnion.Run(_unionU[i], _unionV[i], _parent, _size);
        }

        [Benchmark]
        public void DsuFind()
        {
            for (int i = 0; i < N; i++)
                DsuFind.Run(i, _parent);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_parent);
            Marshal.FreeHGlobal((nint)_size);
            Marshal.FreeHGlobal((nint)_unionU);
            Marshal.FreeHGlobal((nint)_unionV);
        }
    }
}