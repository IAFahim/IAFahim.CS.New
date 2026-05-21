namespace IAFahim.Math.NT.Bench
{
    using IAFahim.Math.NT;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    [MemoryDiagnoser]
    public unsafe class MathNTBench
    {
        [Params(100000, 1000000)]
        public int N;

    }
}
