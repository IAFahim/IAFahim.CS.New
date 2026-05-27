namespace IAFahim.DS.HilbertOrder.Bench
{
    using IAFahim.DS.HilbertOrder;
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<HilbertOrderBench>(args: args);
    }

    [MemoryDiagnoser]
    public class HilbertOrderBench
    {
        [Params(10, 16, 20)]
        public int LogN;

        [Params(0, 1, 2, 3)]
        public int Rot;

        private long _x;
        private long _y;

        [GlobalSetup]
        public void Setup()
        {
            _x = 512;
            _y = 256;
        }

        [Benchmark(Baseline = true)]
        public void HilbertOrderRun()
        {
            HilbertOrder.Run(_x, _y, LogN, Rot);
        }

        [Benchmark]
        public void HilbertOrderEncode()
        {
            HilbertOrder.Encode(_x, _y, LogN);
        }

        [Benchmark]
        public void BlockOrderEncode()
        {
            BlockOrder.Encode((int)_x, (int)_y, 64);
        }
    }
}