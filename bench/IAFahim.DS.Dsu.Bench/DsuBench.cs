namespace IAFahim.DS.Dsu.Bench
{
    using System;
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
            IAFahim.DS.Dsu.DsuInit.Run(_parent, _size, N);
        }

        [IterationSetup]
        public void ResetDsu()
        {
            IAFahim.DS.Dsu.DsuInit.Run(_parent, _size, N);
        }

        [Benchmark(Baseline = true)]
        public void DsuUnion()
        {
            for (int i = 0; i < N; i++)
                IAFahim.DS.Dsu.DsuUnion.Run(_parent, _size, _unionU[i], _unionV[i]);
        }

        [Benchmark]
        public void DsuFind()
        {
            for (int i = 0; i < N; i++)
                IAFahim.DS.Dsu.DsuFind.Run(_parent, i);
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