namespace IAFahim.DS.WaveletMatrix.Bench
{
    using IAFahim.DS.WaveletMatrix;
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<WaveletMatrixBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class WaveletMatrixBench
    {
        [Params(1024, 4096)]
        public int N;

        private int* _data;
        private int* _bitmaps;
        private int* _ranks;
        private int* _mids;

        [GlobalSetup]
        public void Setup()
        {
            _data = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _bitmaps = (int*)Marshal.AllocHGlobal(N * 20 * sizeof(int));
            _ranks = (int*)Marshal.AllocHGlobal(N * 20 * sizeof(int));
            _mids = (int*)Marshal.AllocHGlobal(20 * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) _data[i] = rng.Next(1024);
        }

        [Benchmark]
        public void Build()
        {
            int log = 10;
            WaveletMatrixBuild.Run(_data, N, 1024, _bitmaps, _ranks, _mids, log);
        }

        [Benchmark]
        public void KthQuery()
        {
            int log = 10;
            WaveletMatrixBuild.Run(_data, N, 1024, _bitmaps, _ranks, _mids, log);
            Random rng = new Random(42);
            for (int i = 0; i < 100; i++)
            {
                int l = rng.Next(N);
                int r = l + rng.Next(N - l);
                int k = rng.Next(r - l + 1);
                WaveletMatrixKth.Run(_bitmaps, _ranks, _mids, l, r, k, log);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_data);
            Marshal.FreeHGlobal((nint)_bitmaps);
            Marshal.FreeHGlobal((nint)_ranks);
            Marshal.FreeHGlobal((nint)_mids);
        }
    }
}