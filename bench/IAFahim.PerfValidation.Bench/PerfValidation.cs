using System;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace IAFahim.PerfValidation.Bench
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var cfg = ManualConfig.Create(DefaultConfig.Instance)
                .AddJob(Job.Default
                    .WithWarmupCount(2)
                    .WithIterationCount(3)
                    .WithInvocationCount(16));
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, cfg);
        }
    }

    [MemoryDiagnoser]
    public unsafe class RankCompressBench
    {
        [Params(256, 2048)]
        public int N;

        private int* _src;
        private int* _dst;
        private int* _tmp;
        private int* _seed;

        [GlobalSetup]
        public void Setup()
        {
            _src = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _dst = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _tmp = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _seed = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) _seed[i] = rng.Next(N / 2);
        }

        [IterationSetup]
        public void CopySeed()
        {
            for (int i = 0; i < N; i++) _src[i] = _seed[i];
        }

        [Benchmark]
        public int RankCompress_Heapsort()
        {
            for (int i = 0; i < N; i++) _src[i] = _seed[i];
            return IAFahim.Compress.Coordinate.RankCompress.Run(_src, _dst, _tmp, N);
        }

        [Benchmark(Baseline = true)]
        public int RankCompress_InsertionSort_Reference()
        {
            for (int i = 0; i < N; i++) _tmp[i] = _seed[i];
            InsertionSortRef(_tmp, N);
            int unique = UniqueRef(_tmp, N);
            for (int i = 0; i < N; i++) _dst[i] = LowerBoundRef(_tmp, unique, _seed[i]);
            return unique;
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_src);
            Marshal.FreeHGlobal((nint)_dst);
            Marshal.FreeHGlobal((nint)_tmp);
            Marshal.FreeHGlobal((nint)_seed);
        }

        private static void InsertionSortRef(int* arr, int len)
        {
            for (int i = 1; i < len; i++)
            {
                int key = arr[i]; int j = i - 1;
                while (j >= 0 && arr[j] > key) { arr[j + 1] = arr[j]; j--; }
                arr[j + 1] = key;
            }
        }
        private static int UniqueRef(int* arr, int len)
        {
            int u = 1;
            for (int i = 1; i < len; i++) if (arr[i] != arr[i - 1]) arr[u++] = arr[i];
            return u;
        }
        private static int LowerBoundRef(int* arr, int len, int val)
        {
            int lo = 0, hi = len;
            while (lo < hi) { int mid = lo + ((hi - lo) >> 1); if (arr[mid] < val) lo = mid + 1; else hi = mid; }
            return lo;
        }
    }

    [MemoryDiagnoser]
    public unsafe class MeetInMiddleBench
    {
        private const int Items = 22;
        private int* _values;

        [GlobalSetup]
        public void Setup()
        {
            _values = (int*)Marshal.AllocHGlobal(Items * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < Items; i++) _values[i] = rng.Next(100);
        }

        [Benchmark]
        public int MeetInMiddle_SubsetSumCount() =>
            IAFahim.Search.MeetInMiddle.MeetInMiddle.SubsetSumCount(_values, Items, Items * 10);

        [GlobalCleanup]
        public void Cleanup() => Marshal.FreeHGlobal((nint)_values);
    }

    [MemoryDiagnoser]
    public class BellNumbersBench
    {
        [Params(50, 500)]
        public int N;

        private const long Mod = 1000000007L;

        [Benchmark]
        public long BellNumbers_BellTriangle() =>
            IAFahim.Math.Combinatorics.BellNumbers.Run(N, Mod);
    }

    [MemoryDiagnoser]
    public unsafe class SortIntsBench
    {
        [Params(256, 4096)]
        public int N;

        private int* _source;
        private int* _work;

        [GlobalSetup]
        public void Setup()
        {
            _source = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _work = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++) _source[i] = rng.Next();
        }

        [IterationSetup]
        public void CopySource()
        {
            for (int i = 0; i < N; i++) _work[i] = _source[i];
        }

        [Benchmark]
        public void SortInts_Heapsort() =>
            IAFahim.Sort.Specialized.SortInts.Run(_work, N);

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_source);
            Marshal.FreeHGlobal((nint)_work);
        }
    }

    [MemoryDiagnoser]
    public unsafe class PatternMatchBench
    {
        private const int Len = 4096;
        private byte* _a;
        private byte* _b;
        private int* _mapA;
        private int* _mapB;

        [GlobalSetup]
        public void Setup()
        {
            _a = (byte*)Marshal.AllocHGlobal(Len);
            _b = (byte*)Marshal.AllocHGlobal(Len);
            _mapA = (int*)Marshal.AllocHGlobal(Len * sizeof(int));
            _mapB = (int*)Marshal.AllocHGlobal(Len * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < Len; i++) { _a[i] = (byte)rng.Next(26); _b[i] = _a[i]; }
        }

        [Benchmark]
        public bool Patternized_LastSeenTable() =>
            IAFahim.String.Match.PatternMatch.Parameterized(_a, Len, _b, Len, _mapA, _mapB);

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_a);
            Marshal.FreeHGlobal((nint)_b);
            Marshal.FreeHGlobal((nint)_mapA);
            Marshal.FreeHGlobal((nint)_mapB);
        }
    }
}
