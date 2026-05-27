using System;
namespace IAFahim.Math.Combinatorics.Bench
{
    using IAFahim.Math.Combinatorics;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<CombinatoricsBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class CombinatoricsBench
    {
        [Params(1000, 10000)]
        public int N;

        [Benchmark(Baseline = true)]
        public void Binom()
        {
            for (int i = 0; i < N; i++)
                IAFahim.Math.Combinatorics.Binom.Run(i, i / 2, 1000000007L);
        }

        [Benchmark]
        public void Factorial()
        {
            for (int i = 0; i < N; i++)
                IAFahim.Math.Combinatorics.Factorial.Run(i % 20, 1000000007L);
        }
    }
}
