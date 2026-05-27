using System;
namespace IAFahim.Geometry.Arrangement.Bench
{
    using IAFahim.Geometry.Arrangement;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<GeometryArrangementBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class GeometryArrangementBench
    {
        [Params(256, 1024)]
        public int N;

        private int* _xs;
        private int* _ys;
        private int* _grid;
        private int* _outX;
        private int* _outY;

        [GlobalSetup]
        public void Setup()
        {
            int gridSize = 32;
            _xs = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _ys = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _grid = (int*)Marshal.AllocHGlobal(gridSize * gridSize * sizeof(int));
            _outX = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _outY = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) { _xs[i] = rng.Next(-1000, 1000); _ys[i] = rng.Next(-1000, 1000); }
        }

        [Benchmark(Baseline = true)]
        public void PointLocationBuild()
        {
            global::IAFahim.Geometry.Arrangement.PointLocationBuild.Run(_xs, _ys, N, _grid, 32);
        }

        [Benchmark]
        public void VerticalDecomposition()
        {
            global::IAFahim.Geometry.Arrangement.VerticalDecomposition.Run(_xs, _ys, N, _outX, _outY);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_xs);
            Marshal.FreeHGlobal((nint)_ys);
            Marshal.FreeHGlobal((nint)_grid);
            Marshal.FreeHGlobal((nint)_outX);
            Marshal.FreeHGlobal((nint)_outY);
        }
    }
}