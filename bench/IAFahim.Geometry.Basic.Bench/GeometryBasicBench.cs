using System;
namespace IAFahim.Geometry.Basic.Bench
{
    using IAFahim.Geometry.Basic;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<GeometryBasicBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class GeometryBasicBench
    {
        [Params(256, 1024)]
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
        public void PointDot()
        {
            for (int i = 0; i < N - 1; i++)
                PointDot.Run(_x[i], _y[i], _x[i + 1], _y[i + 1]);
        }

        [Benchmark]
        public void PointCross()
        {
            for (int i = 0; i < N - 1; i++)
                PointCross.Run(_x[i], _y[i], _x[i + 1], _y[i + 1]);
        }

        [Benchmark]
        public void PolygonArea()
        {
            PolygonArea.Run(N, _x, _y);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_x);
            Marshal.FreeHGlobal((nint)_y);
        }
    }
}