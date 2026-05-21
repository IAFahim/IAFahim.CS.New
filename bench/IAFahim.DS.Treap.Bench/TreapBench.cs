namespace IAFahim.DS.Treap.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<TreapBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class TreapBench
    {
        [Params(1024, 4096)]
        public int N;

        private TreapImplicitNode* _nodes;
        private TreapImplicitNode* _root;
        private Random _rng;

        [GlobalSetup]
        public void Setup()
        {
            _nodes = (TreapImplicitNode*)Marshal.AllocHGlobal(N * sizeof(TreapImplicitNode));
            _rng = new Random(42);
        }

        [IterationSetup]
        public void BuildTree()
        {
            _root = null;
            for (int i = 0; i < N; i++)
            {
                _nodes[i].Priority = _rng.Next();
                _nodes[i].Size = 1;
                _nodes[i].Value = i;
                _nodes[i].Sum = i;
                _nodes[i].Lazy = 0;
                _nodes[i].HasLazy = false;
                _nodes[i].Rev = false;
                _nodes[i].Left = null;
                _nodes[i].Right = null;
                _root = TreapImplicit.Merge(_root, &_nodes[i]);
            }
        }

        [Benchmark]
        public void RangeAddAndSum()
        {
            for (int i = 0; i < 100; i++)
            {
                int l = _rng.Next(0, N / 2);
                int r = _rng.Next(N / 2, N);
                TreapImplicit.AddRange(ref _root, l, r, 5);
                TreapImplicit.QueryRange(ref _root, l, r);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_nodes);
        }
    }
}
