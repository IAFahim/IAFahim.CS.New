namespace IAFahim.Collections.NoDeps.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }

    [MemoryDiagnoser]
    [ShortRunJob]
    public unsafe class NativeArrayBench
    {
        [Params(64, 1024)]
        public int N;

        private int* _source;

        [GlobalSetup]
        public void Setup()
        {
            _source = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
                _source[i] = rng.Next();
        }

        [Benchmark(Baseline = true)]
        public void DirectPointer()
        {
            int* work = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Buffer.MemoryCopy(_source, work, N * sizeof(int), N * sizeof(int));
            for (int i = 0; i < N; i++)
                work[i] = work[i] + 1;
            Marshal.FreeHGlobal((IntPtr)work);
        }

        [Benchmark]
        public void NativeArray_ReadWrite()
        {
            var arr = new NativeArray<int>(N, Allocator.Persistent);
            for (int i = 0; i < N; i++)
                arr[i] = _source[i];
            for (int i = 0; i < N; i++)
                arr[i] = arr[i] + 1;
            arr.Dispose();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((IntPtr)_source);
        }
    }

    [MemoryDiagnoser]
    [ShortRunJob]
    public unsafe class NativeListBench
    {
        [Params(64, 1024)]
        public int N;

        private int* _source;

        [GlobalSetup]
        public void Setup()
        {
            _source = (int*)Marshal.AllocHGlobal(1024 * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < 1024; i++)
                _source[i] = rng.Next();
        }

        [Benchmark(Baseline = true)]
        public void NativeList_Add()
        {
            var list = new NativeList<int>(N, Allocator.Persistent);
            for (int i = 0; i < N; i++)
                list.Add(_source[i]);
            list.Dispose();
        }

        [Benchmark]
        public void NativeList_AddThenRemoveAt()
        {
            var list = new NativeList<int>(N, Allocator.Persistent);
            for (int i = 0; i < N; i++)
                list.Add(_source[i]);
            for (int i = 0; i < N / 2; i++)
                list.RemoveAt(0);
            list.Dispose();
        }

        [Benchmark]
        public void NativeList_ResizeUninitialized()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.ResizeUninitialized(N);
            for (int i = 0; i < N; i++)
                list[i] = _source[i];
            list.Dispose();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((IntPtr)_source);
        }
    }

    [MemoryDiagnoser]
    [ShortRunJob]
    public unsafe class UnsafeListBench
    {
        [Params(64, 1024)]
        public int N;

        private int* _source;

        [GlobalSetup]
        public void Setup()
        {
            _source = (int*)Marshal.AllocHGlobal(1024 * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < 1024; i++)
                _source[i] = rng.Next();
        }

        [Benchmark(Baseline = true)]
        public void UnsafeList_Add()
        {
            var list = new UnsafeList<int>(N, Allocator.Persistent);
            for (int i = 0; i < N; i++)
                list.Add(_source[i]);
            list.Dispose();
        }

        [Benchmark]
        public void UnsafeList_AddRange()
        {
            var list = new UnsafeList<int>(N, Allocator.Persistent);
            list.AddRange(_source, N);
            list.Dispose();
        }

        [Benchmark]
        public void UnsafeList_ResizeUninitialized()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            list.ResizeUninitialized(N);
            for (int i = 0; i < N; i++)
                list[i] = _source[i];
            list.Dispose();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((IntPtr)_source);
        }
    }
}