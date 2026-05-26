namespace IAFahim.DS.Mo.Bench
{
    using System;
    using IAFahim.DS.Mo;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<MoBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class MoBench
    {
        [Params(1000, 5000)]
        public int Q;

        [Benchmark(Baseline = true)]
        public void MoSort()
        {
            int* l = (int*)Marshal.AllocHGlobal(Q * sizeof(int));
            int* r = (int*)Marshal.AllocHGlobal(Q * sizeof(int));
            int* block = (int*)Marshal.AllocHGlobal(Q * sizeof(int));
            int* queries = (int*)Marshal.AllocHGlobal(Q * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < Q; i++) { l[i] = rng.Next(100); r[i] = l[i] + rng.Next(50); block[i] = l[i] / 2; queries[i] = i; }
            IAFahim.DS.Mo.MoSort.Run(queries, l, r, block, Q, 2);
            Marshal.FreeHGlobal((nint)l);
            Marshal.FreeHGlobal((nint)r);
            Marshal.FreeHGlobal((nint)block);
            Marshal.FreeHGlobal((nint)queries);
        }
    }
}
