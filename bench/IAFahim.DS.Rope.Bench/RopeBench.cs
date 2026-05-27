namespace IAFahim.DS.Rope.Bench
{
    using IAFahim.DS.Rope;
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<RopeBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class RopeBench
    {
        [Params(1024, 4096)]
        public int N;

        private byte* _text;

        [GlobalSetup]
        public void Setup()
        {
            _text = (byte*)Marshal.AllocHGlobal(N * sizeof(byte));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) _text[i] = (byte)(rng.Next(26) + 65);
        }

        [Benchmark]
        public void BuildAndInsert()
        {
            RopeNode* root = null;
            Random rng = new Random(42);
            for (int i = 0; i < Math.Min(N, 256); i++)
            {
                RopeNode* node = (RopeNode*)Marshal.AllocHGlobal(sizeof(RopeNode));
                node->Size = 1;
                node->Priority = rng.Next();
                node->Value = _text[i];
                node->Left = null;
                node->Right = null;
                root = RopeInsert.Run(root, rng.Next(256), node);
            }
            Marshal.FreeHGlobal((nint)root);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_text);
        }
    }
}