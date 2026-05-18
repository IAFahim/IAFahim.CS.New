namespace IAFahim.DS.SegmentTree.Bench
{
    using IAFahim.DS.SegmentTree;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<SegmentTreeBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class SegmentTreeBench
    {
        [Params(1024, 4096, 16384)]
        public int N;

        private int* _arr;
        private int* _seg;

        [GlobalSetup]
        public void Setup()
        {
            _arr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _seg = (int*)Marshal.AllocHGlobal(N * 4 * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) _arr[i] = rng.Next();
        }

        [IterationSetup]
        public void Reset()
        {
            SegmentTreeBuild.Run(_arr, N, _seg);
        }

        [Benchmark(Baseline = true)]
        public void Build()
        {
            SegmentTreeBuild.Run(_arr, N, _seg);
        }

        [Benchmark]
        public void Query()
        {
            for (int i = 0; i < N; i++)
                SegmentTreeQuery.Run(_seg, 0, 0, N - 1, i / 2, N - 1 - i / 2);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_arr);
            Marshal.FreeHGlobal((nint)_seg);
        }
    }
}