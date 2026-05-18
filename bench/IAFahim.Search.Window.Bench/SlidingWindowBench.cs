namespace IAFahim.Search.Window.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<SlidingWindowBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class SlidingWindowBench
    {
        [Params(1024, 4096)]
        public int N;

        [Params(16, 64)]
        public int WindowSize;

        private int* _src;
        private int* _dst;

        [GlobalSetup]
        public void Setup()
        {
            _src = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _dst = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
                _src[i] = rng.Next(1000);
        }

        [Benchmark]
        public void SlidingWindowMin()
        {
            SlidingWindowMin.Run(_src, _dst, N, WindowSize);
        }

        [Benchmark]
        public void SlidingWindowMax()
        {
            SlidingWindowMax.Run(_src, _dst, N, WindowSize);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_src);
            Marshal.FreeHGlobal((nint)_dst);
        }
    }
}