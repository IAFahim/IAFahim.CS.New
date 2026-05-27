using System;
namespace IAFahim.Geometry.Advanced.Bench
{
    using IAFahim.Geometry.Advanced;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<GeometryAdvancedBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class GeometryAdvancedBench
    {
        [Params(64, 256)]
        public int N;

        private long* _x;
        private long* _y;

        [GlobalSetup]
        public void Setup()
        {
            _x = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            _y = (long*)Marshal.AllocHGlobal(N * sizeof(long));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) { _x[i] = rng.Next(-10000, 10000); _y[i] = rng.Next(-10000, 10000); }
        }

        [Benchmark(Baseline = true)]
        public void ConvexDiameter()
        {
            long result = global::IAFahim.Geometry.Advanced.ConvexDiameter.Run(N, _x, _y);
        }

        [Benchmark]
        public void RotatingCalipers()
        {
            long* res = stackalloc long[2];
            global::IAFahim.Geometry.Advanced.RotatingCalipers.Run(N, _x, _y, res);
        }

        [Benchmark]
        public void ClosestPair()
        {
            long result = global::IAFahim.Geometry.Advanced.ClosestPair.Run(N, _x, _y);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_x);
            Marshal.FreeHGlobal((nint)_y);
        }
    }
}