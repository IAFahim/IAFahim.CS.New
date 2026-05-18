namespace IAFahim.Permutation.Bench
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using IAFahim.Permutation;
    using GC = IAFahim.Permutation.GrayCode;
    using NP = IAFahim.Permutation.NextPermutation;
    using CP = IAFahim.Permutation.CartesianProduct;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<PermutationBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class PermutationBench
    {
        [Params(8, 10)]
        public int Bits;

        [Benchmark]
        public void GrayCodeGenerate_Bench()
        {
            int n = 1 << Bits;
            int* dst = stackalloc int[n];
            GC.Generate(dst, Bits);
        }

        [Benchmark]
        public void GrayCodeToAndFrom_Bench()
        {
            int x = 0;
            for (int i = 0; i < 256; i++)
            {
                int g = GC.ToGray(i);
                x = GC.FromGray(g);
            }
        }

        [Benchmark]
        public void NextPermutation_Bench()
        {
            int* ptr = stackalloc int[Bits];
            for (int i = 0; i < Bits; i++) ptr[i] = i;
            int count = 0;
            do { count++; } while (NP.Run(ptr, Bits) && count < 1000000);
        }

        [Benchmark]
        public void CartesianProduct_Bench()
        {
            int* sizes = stackalloc int[3];
            sizes[0] = 4; sizes[1] = 5; sizes[2] = 6;
            int* dst = stackalloc int[3];
            for (int i = 0; i < 120; i++)
                CP.GetAt(sizes, 3, i, dst);
        }
    }
}