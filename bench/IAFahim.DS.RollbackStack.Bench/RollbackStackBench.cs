namespace IAFahim.DS.RollbackStack.Bench
{
    using IAFahim.DS.RollbackStack;
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<RollbackStackBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class RollbackStackBench
    {
        [Params(1000, 10000)]
        public int N;

        private int* _parent;
        private int* _size;
        private int* _history;
        private int _histSize;

        [GlobalSetup]
        public void Setup()
        {
            _parent = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _size = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _history = (int*)Marshal.AllocHGlobal(N * 4 * sizeof(int));
            _histSize = 0;
        }

        [IterationSetup]
        public void Reset()
        {
            for (int i = 0; i < N; i++) { _parent[i] = i; _size[i] = 1; }
            _histSize = 0;
        }

        [Benchmark]
        public void UnionWithSnapshot()
        {
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
            {
                int snap = UndoableUnionFind.Snapshot(_parent, _size, _history, _histSize);
                UndoableUnionFind.Union(_parent, _size, _history, &_histSize, rng.Next(N), rng.Next(N));
                if (i % 10 == 0)
                    UndoableUnionFind.Rollback(_parent, _size, _history, snap, &_histSize);
            }
        }

        [Benchmark]
        public void BipartiteDsu()
        {
            Random rng = new Random(42);
            int* parity = stackalloc int[N];
            for (int i = 0; i < N; i++) { _parent[i] = i; parity[i] = 0; }
            int bHistSize = 0;
            int* bHistory = stackalloc int[N * 6];
            for (int i = 0; i < N; i++)
            {
                UndoableBipartiteDsu.Union(_parent, parity, bHistory, &bHistSize, rng.Next(N), rng.Next(N));
                if (i % 5 == 0)
                    UndoableBipartiteDsu.Rollback(_parent, parity, bHistory, bHistSize - 10, &bHistSize);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_parent);
            Marshal.FreeHGlobal((nint)_size);
            Marshal.FreeHGlobal((nint)_history);
        }
    }
}