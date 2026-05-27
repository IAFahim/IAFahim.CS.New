using System;
namespace IAFahim.Geometry.Intersect.Bench
{
    using IAFahim.Geometry.Intersect;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<GeometryIntersectBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class GeometryIntersectBench
    {
        [Params(256, 1024)]
        public int N;

        private double* _xs1;
        private double* _ys1;
        private double* _zs1;
        private double* _xs2;
        private double* _ys2;
        private double* _zs2;

        [GlobalSetup]
        public void Setup()
        {
            _xs1 = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            _ys1 = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            _zs1 = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            _xs2 = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            _ys2 = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            _zs2 = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
            {
                _xs1[i] = rng.Next(-1000, 1000); _ys1[i] = rng.Next(-1000, 1000); _zs1[i] = rng.Next(-1000, 1000);
                _xs2[i] = rng.Next(-1000, 1000); _ys2[i] = rng.Next(-1000, 1000); _zs2[i] = rng.Next(-1000, 1000);
            }
        }

        [Benchmark(Baseline = true)]
        public void PointPlaneDistance()
        {
            for (int i = 0; i < N; i++)
                Plane.PointPlaneDistance(_xs1[i], _ys1[i], _zs1[i], 1, 0, 0, 0);
        }

        [Benchmark]
        public void SegmentPlaneIntersection()
        {
            double t;
            for (int i = 0; i < N; i++)
                Plane.SegmentPlaneIntersection(_xs1[i], _ys1[i], _zs1[i], _xs2[i], _ys2[i], _zs2[i], 1, 0, 0, 0, &t);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_xs1);
            Marshal.FreeHGlobal((nint)_ys1);
            Marshal.FreeHGlobal((nint)_zs1);
            Marshal.FreeHGlobal((nint)_xs2);
            Marshal.FreeHGlobal((nint)_ys2);
            Marshal.FreeHGlobal((nint)_zs2);
        }
    }
}
