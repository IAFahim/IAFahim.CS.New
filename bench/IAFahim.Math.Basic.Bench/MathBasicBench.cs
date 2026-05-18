namespace IAFahim.Math.Basic.Bench
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<MathBasicBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class MathBasicBench
    {
        [Params(1000, 1000000)]
        public int N;

        [Benchmark]
        public void CeilDiv()
        {
            for (int i = 1; i < N; i++)
                CeilDiv.Run(i, 7);
        }

        [Benchmark]
        public void FloorDiv()
        {
            for (int i = 1; i < N; i++)
                FloorDiv.Run(i, 7);
        }

        [Benchmark]
        public void AbsInt()
        {
            for (int i = -500; i < 500; i++)
                AbsInt.Run(i);
        }

        [Benchmark]
        public void MinInt()
        {
            int x = 0;
            for (int i = 0; i < N; i++)
                x = MinInt.Run(x, i);
        }

        [Benchmark]
        public void MaxInt()
        {
            int x = 0;
            for (int i = 0; i < N; i++)
                x = MaxInt.Run(x, i);
        }

        [Benchmark]
        public void Clamp()
        {
            for (int i = 0; i < N; i++)
                Clamp.Run(i, 0, 1000);
        }
    }
}