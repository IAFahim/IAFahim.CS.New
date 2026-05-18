namespace IAFahim.Sort.Partition.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<PartitionBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class PartitionBench
    {
        [Params(256, 1024)]
        public int N;

        private int* _source;
        private int* _work;

        [GlobalSetup]
        public void Setup()
        {
            _source = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _work = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
                _source[i] = rng.Next();
        }

        [IterationSetup]
        public void CopySource()
        {
            Buffer.MemoryCopy(_source, _work, N * sizeof(int), N * sizeof(int));
        }

        [Benchmark]
        public void Partition()
        {
            Partition.Run(_work, N, N / 2);
        }

        [Benchmark]
        public void NthElement()
        {
            int val;
            Partition.TryGetNthElement(_work, N, N / 2, out val);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_source);
            Marshal.FreeHGlobal((nint)_work);
        }
    }
}