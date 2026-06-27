namespace IAFahim.DS.PersistentTreap.Bench
{
    using IAFahim.DS.PersistentTreap;
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<PersistentTreapBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class PersistentTreapBench
    {
        [Params(128, 512, 2048)]
        public int N;

        private int* _nodes;
        private int* _left;
        private int* _right;
        private int* _prio;
        private int* _size;
        private int* _allocCnt;
        private int _capacity;
        private int _root;

        [GlobalSetup]
        public void Setup()
        {
            _capacity = 64 * N;
            _nodes = (int*)Marshal.AllocHGlobal(_capacity * sizeof(int));
            _left = (int*)Marshal.AllocHGlobal(_capacity * sizeof(int));
            _right = (int*)Marshal.AllocHGlobal(_capacity * sizeof(int));
            _prio = (int*)Marshal.AllocHGlobal(_capacity * sizeof(int));
            _size = (int*)Marshal.AllocHGlobal(_capacity * sizeof(int));
            _allocCnt = (int*)Marshal.AllocHGlobal(sizeof(int));
            *_allocCnt = 0;
            _root = 0;
        }

        [IterationSetup]
        public void ResetTree()
        {
            *_allocCnt = 0;
            _root = 0;
        }

        [Benchmark(Baseline = true)]
        public void PersistentTreapInsert()
        {
            ResetTree();
            Random rng = new Random(42);
            int root = _root;
            for (int i = 0; i < N; i++)
            {
                int val = rng.Next(N * 2);
                root = global::IAFahim.DS.PersistentTreap.PersistentTreapInsert.Run(_nodes, _left, _right, _prio, _size, _allocCnt, root, val);
            }
            _root = root;
        }

        [Benchmark]
        public void PersistentTreapFind()
        {
            for (int i = 0; i < N; i++)
                global::IAFahim.DS.PersistentTreap.PersistentTreapFind.Run(_nodes, _left, _right, _root, i * 2);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_nodes);
            Marshal.FreeHGlobal((nint)_left);
            Marshal.FreeHGlobal((nint)_right);
            Marshal.FreeHGlobal((nint)_prio);
            Marshal.FreeHGlobal((nint)_size);
            Marshal.FreeHGlobal((nint)_allocCnt);
        }
    }
}