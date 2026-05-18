namespace IAFahim.Math.NT.Bench
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<BitOpsBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public class BitOpsBench
    {
        [Params(1000, 10000)]
        public int N;

        [Benchmark(Baseline = true)]
        public void BitCount()
        {
            int sum = 0;
            for (int i = 0; i < N; i++)
                sum += IAFahim.Math.NT.BitCount.Run(i);
        }

        [Benchmark]
        public void BitLength()
        {
            int sum = 0;
            for (int i = 0; i < N; i++)
                sum += IAFahim.Math.NT.BitLength.Run(i);
        }

        [Benchmark]
        public void HighestBit()
        {
            int sum = 0;
            for (int i = 0; i < N; i++)
                sum += IAFahim.Math.NT.HighestBit.Run(i);
        }

        [Benchmark]
        public void Gcd()
        {
            int sum = 0;
            for (int i = 1; i < N; i++)
                sum += IAFahim.Math.NT.Gcd.Run(i, i + 1);
        }
    }
}