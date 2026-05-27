namespace IAFahim.DS.LinkCut.Bench
{
    using IAFahim.DS.LinkCut;
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<LinkCutBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class LinkCutBench
    {
        [Params(100, 1000, 5000)]
        public int N;

        private LctNode* _nodes;
        private Random _rng;

        [GlobalSetup]
        public void Setup()
        {
            _nodes = (LctNode*)Marshal.AllocHGlobal(N * sizeof(LctNode));
            _rng = new Random(42);
            for (int i = 0; i < N; i++)
            {
                _nodes[i].Index = i;
                _nodes[i].Value = _rng.Next(1000);
                _nodes[i].PathSum = _nodes[i].Value;
                _nodes[i].Rev = false;
                _nodes[i].Left = null;
                _nodes[i].Right = null;
                _nodes[i].Parent = null;
            }
        }

        [IterationSetup]
        public void ResetParents()
        {
            for (int i = 0; i < N; i++)
            {
                _nodes[i].Left = null;
                _nodes[i].Right = null;
                _nodes[i].Parent = null;
                _nodes[i].Rev = false;
                _nodes[i].PathSum = _nodes[i].Value;
            }
        }

        [Benchmark(Baseline = true)]
        public void LinkCutAccess()
        {
            for (int i = 0; i < N; i += 4)
                LinkCut.Access(&_nodes[i]);
        }

        [Benchmark]
        public void LinkCutQuery()
        {
            for (int i = 1; i < N; i += 4)
                LinkCut.Query(&_nodes[0], &_nodes[i]);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_nodes);
        }
    }
}