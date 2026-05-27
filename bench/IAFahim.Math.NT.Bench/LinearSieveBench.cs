using System;
namespace IAFahim.Math.NT.Bench
{
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class LinearSieveBench
    {
        [Params(100_000, 1_000_000)]
        public int N;

        private int* _minPrime;
        private int* _primes;

        [GlobalSetup]
        public void Setup()
        {
            _minPrime = (int*)Marshal.AllocHGlobal((N + 1) * sizeof(int));
            _primes = (int*)Marshal.AllocHGlobal(N * sizeof(int));
        }

        [Benchmark]
        public void MinPrimeSieve()
        {
            LinearSieveMinPrime.Run(_minPrime, _primes, N, out int _);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_minPrime);
            Marshal.FreeHGlobal((nint)_primes);
        }
    }
}
