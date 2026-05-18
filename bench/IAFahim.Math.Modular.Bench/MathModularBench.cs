namespace IAFahim.Math.Modular.Bench
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<MathModularBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class MathModularBench
    {
        [Params(100, 10000)]
        public int N;

        [Benchmark]
        public void Gcd()
        {
            int a = 123456, b = 789012;
            for (int i = 0; i < N; i++)
                Gcd.Run(a, b);
        }

        [Benchmark]
        public void ModPow()
        {
            for (int i = 0; i < N; i++)
                ModPow.Run(2, i, 1000000007);
        }

        [Benchmark]
        public void ModMul()
        {
            for (int i = 0; i < N; i++)
                ModMul.Run(i, i + 1, 1000000007);
        }
    }
}