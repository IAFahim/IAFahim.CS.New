namespace IAFahim.DS.Trie.Bench
{
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
            int* next0 = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* next1 = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* cnt = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            for (int i = 0; i < maxNodes; i++) { next0[i] = -1; next1[i] = -1; cnt[i] = 0; }
            int nodeCount = 1;
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
                TrieInsert.Run(&nodeCount, next0, next1, cnt, rng.Next(N));
            Marshal.FreeHGlobal((nint)next0);
            Marshal.FreeHGlobal((nint)next1);
            Marshal.FreeHGlobal((nint)cnt);
        }

        [GlobalCleanup]
        public void Cleanup() { }
    }
}