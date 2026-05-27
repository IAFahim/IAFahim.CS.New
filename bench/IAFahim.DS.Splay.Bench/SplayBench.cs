namespace IAFahim.DS.Splay.Bench
{
    using IAFahim.DS.Splay;
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<SplayBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class SplayBench
    {
        [Params(512, 2048)]
        public int N;

        private SplayNode* _nodes;

        [GlobalSetup]
        public void Setup()
        {
            _nodes = (SplayNode*)Marshal.AllocHGlobal(N * sizeof(SplayNode));
        }

        [IterationSetup]
        public void Reset()
        {
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
            {
                _nodes[i].Key = i;
                _nodes[i].Size = 1;
                _nodes[i].Parent = null;
                _nodes[i].Left = null;
                _nodes[i].Right = null;
            }
        }

        [Benchmark]
        public void SplayOperations()
        {
            SplayNode* root = null;
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
            {
                int idx = rng.Next(N);
                Splay.Splay_(&root, &_nodes[idx]);
            }
        }

        [Benchmark]
        public void ReverseRange()
        {
            SplayRevNode* root = null;
            Random rng = new Random(42);
            for (int i = 0; i < Math.Min(N, 128); i++)
            {
                SplayRevNode* node = (SplayRevNode*)Marshal.AllocHGlobal(sizeof(SplayRevNode));
                node->Key = rng.Next(1000);
                node->Size = 1;
                node->Sum = node->Key;
                node->Rev = false;
                node->Parent = null;
                node->Left = root;
                if (root != null) root->Parent = node;
                node->Right = null;
                SplayRangeReverse.Update(node);
                root = node;
            }
            for (int i = 0; i < 10; i++)
            {
                int l = rng.Next(128);
                int r = l + rng.Next(128 - l);
                SplayRangeReverse.Reverse(&root, l, r);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_nodes);
        }
    }
}