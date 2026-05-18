namespace IAFahim.Permutation.Bench
{
    using System;
    using System.Runtime.CompilerServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

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
        public void GrayCodeGenerate()
        {
            int n = 1 << Bits;
            int* dst = stackalloc int[n];
            GrayCode.Generate(dst, Bits);
        }

        [Benchmark]
        public void GrayCodeToAndFrom()
        {
            int x = 0;
            for (int i = 0; i < 256; i++)
            {
                int g = GrayCode.ToGray(i);
                x = GrayCode.FromGray(g);
            }
        }

        [Benchmark]
        public void NextPermutation()
        {
            int* ptr = stackalloc int[Bits];
            for (int i = 0; i < Bits; i++) ptr[i] = i;
            int count = 0;
            do { count++; } while (NextPermutation.Run(ptr, Bits) && count < 1000000);
        }

        [Benchmark]
        public void CartesianProduct()
        {
            int* sizes = stackalloc int[3];
            sizes[0] = 4; sizes[1] = 5; sizes[2] = 6;
            int* dst = stackalloc int[3];
            for (int i = 0; i < 120; i++)
                CartesianProduct.GetAt(sizes, 3, i, dst);
        }
    }
}