namespace IAFahim.Math.NT.Bench
{
    using IAFahim.Math.NT;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<MathNTBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class MathNTBench
    {
        [Params(100000, 1000000)]
        public int N;

        [Benchmark(Baseline = true)]
        public void IsPrime()
        {
            for (int i = 2; i < N; i++)
                IsPrime.Run(i);
        }

        [Benchmark]
        public void Gcd()
        {
            for (int i = 1; i < N; i++)
                Gcd.Run(i, i + 1);
        }
    }
}
