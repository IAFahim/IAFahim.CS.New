namespace IAFahim.Search.Imos.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using Imos1D = IAFahim.Search.Imos.Imos1D;
    using Imos2D = IAFahim.Search.Imos.Imos2D;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<ImosBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class ImosBench
    {
        [Params(1024, 4096)]
        public int N;

        [Benchmark]
        public void Imos1D_Bench()
        {
            int* diff = stackalloc int[N];
            for (int j = 0; j < 100; j++)
            {
                Imos1D.Add(diff, N, 0, N / 2, 1);
                Imos1D.Add(diff, N, N / 4, N * 3 / 4, 1);
            }
        }

        [Benchmark]
        public void Imos2D_Bench()
        {
            int size = 64;
            int* diff = stackalloc int[size * size];
            for (int j = 0; j < 10; j++)
            {
                Imos2D.Add(diff, size, size, 0, 0, size / 2, size / 2, 1);
                Imos2D.Add(diff, size, size, size / 4, size / 4, size * 3 / 4, size * 3 / 4, 1);
            }
        }
    }
}