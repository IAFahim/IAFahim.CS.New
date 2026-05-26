namespace IAFahim.DS.Trie.Bench
{
    using System;
    using IAFahim.DS.Trie;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<TrieBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class TrieBench
    {
        [Params(1000, 10000)]
        public int N;

        [GlobalSetup]
        public void Setup() { }

        [Benchmark(Baseline = true)]
        public void TrieInsert()
        {
            const int maxNodes = 50000;
            int* trie = (int*)Marshal.AllocHGlobal(maxNodes * 27 * sizeof(int));
            for (int i = 0; i < maxNodes * 27; i++) trie[i] = 0;
            trie[0] = 1;
            int root = 1;
            Random rng = new Random(42);
            byte* word = stackalloc byte[1];
            for (int i = 0; i < N; i++)
            {
                word[0] = (byte)('a' + (rng.Next() % 26));
                IAFahim.DS.Trie.TrieInsert.Run(trie, root, word, 1);
            }
            Marshal.FreeHGlobal((nint)trie);
        }

        [GlobalCleanup]
        public void Cleanup() { }
    }
}
