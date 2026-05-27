namespace IAFahim.DS.UnsafeArray.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<UnsafeArrayBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class UnsafeArrayBench
    {
        [Params(1024, 4096, 16384)]
        public int N;

        [Benchmark(Baseline = true)]
        public void AllocateInt()
        {
            int* arr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            for (int i = 0; i < N; i++) arr[i] = i;
            long sum = 0;
            for (int i = 0; i < N; i++) sum += arr[i];
            Marshal.FreeHGlobal((nint)arr);
        }

        [Benchmark]
        public void FillAndSum()
        {
            int* arr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            for (int i = 0; i < N; i++) arr[i] = i;
            long sum = 0;
            for (int i = 0; i < N; i++) sum += arr[i];
            Marshal.FreeHGlobal((nint)arr);
        }

        [Benchmark]
        public void MemCopy()
        {
            int* src = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int* dst = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) src[i] = rng.Next();
            Buffer.MemoryCopy(src, dst, N * sizeof(int), N * sizeof(int));
            Marshal.FreeHGlobal((nint)src);
            Marshal.FreeHGlobal((nint)dst);
        }
    }
}