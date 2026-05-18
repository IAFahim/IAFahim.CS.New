namespace IAFahim.DS.Grid.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using IAFahim.DS.Grid;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<GridBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class GridBench
    {
        [Params(64, 256)]
        public int N;

        private int* _flat;

        [GlobalSetup]
        public void Setup()
        {
            _flat = (int*)Marshal.AllocHGlobal(N * N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N * N; i++)
                _flat[i] = rng.Next();
        }

        [Benchmark]
        public void MakeGrid_Bench()
        {
            MakeGrid.Run(_flat, N * N, N, N);
        }

        [Benchmark]
        public void Reverse_Bench()
        {
            Reverse.Run(_flat, N * N);
        }

        [Benchmark]
        public void Shuffle_Bench()
        {
            Shuffle.Run(_flat, N * N, 42);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_flat);
        }
    }
}