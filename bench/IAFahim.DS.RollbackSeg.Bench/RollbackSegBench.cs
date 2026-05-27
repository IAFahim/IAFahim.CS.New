namespace IAFahim.DS.RollbackSeg.Bench
{
    using IAFahim.DS.RollbackSeg;
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<RollbackSegBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class RollbackSegBench
    {
        [Params(1024, 4096)]
        public int N;

        private long* _tree;
        private long* _lazy;
        private int* _histNode;
        private long* _histVal;
        private int _top;
        private int _checkpoint;

        [GlobalSetup]
        public void Setup()
        {
            _tree = (long*)Marshal.AllocHGlobal((N * 4 + 100) * sizeof(long));
            _lazy = (long*)Marshal.AllocHGlobal((N * 4 + 100) * sizeof(long));
            _histNode = (int*)Marshal.AllocHGlobal(100000 * sizeof(int));
            _histVal = (long*)Marshal.AllocHGlobal(100000 * sizeof(long));
            _top = 0;
        }

        [IterationSetup]
        public void Reset()
        {
            _top = 0;
            for (int i = 0; i < N * 4 + 100; i++) { _tree[i] = 0; _lazy[i] = 0; }
        }

        [Benchmark]
        public void BuildAndUpdate()
        {
            long* arr = stackalloc long[N];
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) arr[i] = rng.Next(100);
            RollbackSegBuild.RunInt64(arr, _tree, 1, 0, N - 1);
            for (int i = 0; i < 10; i++)
            {
                int l = rng.Next(N / 4);
                int r = l + rng.Next(N / 4);
                int top = _top;
                RollbackSegUpdate.RangeAddInt64(_tree, _lazy, _histNode, _histVal, &top, 1, 0, N - 1, l, r, 1);
                _top = top;
                _checkpoint = RollbackSegRollback.GetCheckpoint(&top);
            }
        }

        [Benchmark]
        public void QueryRangeSum()
        {
            long* arr = stackalloc long[N];
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) arr[i] = rng.Next(100);
            RollbackSegBuild.RunInt64(arr, _tree, 1, 0, N - 1);
            for (int i = 0; i < 100; i++)
            {
                int l = rng.Next(N);
                int r = l + rng.Next(N - l);
                RollbackSegQuery.RangeSumInt64(_tree, _lazy, 1, 0, N - 1, l, r);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_tree);
            Marshal.FreeHGlobal((nint)_lazy);
            Marshal.FreeHGlobal((nint)_histNode);
            Marshal.FreeHGlobal((nint)_histVal);
        }
    }
}