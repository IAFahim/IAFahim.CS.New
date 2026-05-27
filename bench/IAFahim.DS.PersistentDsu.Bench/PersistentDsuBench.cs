namespace IAFahim.DS.PersistentDsu.Bench
{
    using IAFahim.DS.PersistentDsu;
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<PersistentDsuBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class PersistentDsuBench
    {
        [Params(64, 256, 1024)]
        public int N;

        private int* _parent;
        private int* _size;
        private int* _lc;
        private int* _rc;
        private int* _allocCnt;
        private int _capacity;
        private int _root;

        [GlobalSetup]
        public void Setup()
        {
            _capacity = 4 * N;
            _parent = (int*)Marshal.AllocHGlobal(_capacity * sizeof(int));
            _size = (int*)Marshal.AllocHGlobal(_capacity * sizeof(int));
            _lc = (int*)Marshal.AllocHGlobal(_capacity * sizeof(int));
            _rc = (int*)Marshal.AllocHGlobal(_capacity * sizeof(int));
            _allocCnt = (int*)Marshal.AllocHGlobal(sizeof(int));
            *_allocCnt = 0;
            _root = PersistentDsu.Build(0, N - 1, _parent, _size, _allocCnt, _lc, _rc);
        }

        [IterationSetup]
        public void ResetTree()
        {
            *_allocCnt = 0;
            _root = PersistentDsu.Build(0, N - 1, _parent, _size, _allocCnt, _lc, _rc);
        }

        private int _prevRoot;

        [Benchmark(Baseline = true)]
        public void PersistentDsuUnion()
        {
            _prevRoot = _root;
            Random rng = new Random(42);
            int root = _root;
            for (int i = 0; i < N - 1; i++)
            {
                int a = rng.Next(N);
                int b = rng.Next(N);
                root = PersistentDsu.Union(root, N, a, b, _parent, _size, _allocCnt, _lc, _rc);
            }
            _root = root;
        }

        [Benchmark]
        public void PersistentDsuFind()
        {
            for (int i = 0; i < N; i++)
                PersistentDsu.Find(_root, N, i, _parent, _lc, _rc, _size, out _);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_parent);
            Marshal.FreeHGlobal((nint)_size);
            Marshal.FreeHGlobal((nint)_lc);
            Marshal.FreeHGlobal((nint)_rc);
            Marshal.FreeHGlobal((nint)_allocCnt);
        }
    }
}