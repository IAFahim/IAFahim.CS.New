using System;
namespace IAFahim.Geometry.Hull.Bench
{
    using IAFahim.Geometry.Hull;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<GeometryHullBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class GeometryHullBench
    {
        [Params(64, 256)]
        public int N;

        private ConvexHullTrick.Line* _lines;
        private int* _size;
        private double* _xs;
        private double* _ys;
        private double* _zs;

        [GlobalSetup]
        public void Setup()
        {
            _lines = (ConvexHullTrick.Line*)Marshal.AllocHGlobal(N * sizeof(ConvexHullTrick.Line));
            _size = (int*)Marshal.AllocHGlobal(sizeof(int));
            _xs = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            _ys = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            _zs = (double*)Marshal.AllocHGlobal(N * sizeof(double));
            *(_size) = 0;
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
            {
                _lines[i] = new ConvexHullTrick.Line { M = rng.Next(1, 100), B = rng.Next(0, 1000) };
                _xs[i] = rng.Next(-10000, 10000);
                _ys[i] = rng.Next(-10000, 10000);
                _zs[i] = rng.Next(-10000, 10000);
            }
        }

        [Benchmark(Baseline = true)]
        public void ConvexHullTrickAdd()
        {
            *_size = 0;
            for (int i = 0; i < N; i++)
                ConvexHullTrick.Add(_lines, _size, _lines[i]);
        }

        [Benchmark]
        public void ConvexHull3D_Basic()
        {
            int maxFaces = 4 * N;
            ConvexHull3D.Face* outFaces = stackalloc ConvexHull3D.Face[maxFaces];
            ConvexHull3D.Face* scratchFaces = stackalloc ConvexHull3D.Face[maxFaces];
            int* scratchHead = stackalloc int[maxFaces];
            ConvexHull3D.Build(_xs, _ys, _zs, N, outFaces, scratchFaces, scratchHead);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_lines);
            Marshal.FreeHGlobal((nint)_size);
            Marshal.FreeHGlobal((nint)_xs);
            Marshal.FreeHGlobal((nint)_ys);
            Marshal.FreeHGlobal((nint)_zs);
        }
    }
}
