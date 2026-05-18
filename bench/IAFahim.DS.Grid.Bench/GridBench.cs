namespace IAFahim.DS.Grid.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

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
        public void MakeGrid()
        {
            int rows, cols;
            MakeGrid.Run(_flat, N * N, &rows, &cols);
        }

        [Benchmark]
        public void Reverse()
        {
            Reverse.Run(_flat, N * N);
        }

        [Benchmark]
        public void Shuffle()
        {
            Shuffle.Run(_flat, N * N);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_flat);
        }
    }
}