namespace IAFahim.DS.SpatialMap.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Memory.Allocators;
    using BenchmarkDotNet.Attributes;
    using Unity.Collections;

    [MemoryDiagnoser]
    public unsafe class AllocatorBench
    {
        [Params(64, 256, 1024)]
        public int N;

        private UnsafePoolAllocator<int> pool;
        private int** ptrs;

        [GlobalSetup]
        public void Setup()
        {
            this.pool = new UnsafePoolAllocator<int>(this.N, Allocator.Persistent);
            this.ptrs = (int**)Marshal.AllocHGlobal(this.N * sizeof(int*));
        }

        [Benchmark(Baseline = true)]
        public void MarshalAllocFree()
        {
            for (int i = 0; i < this.N; i++)
            {
                this.ptrs[i] = (int*)Marshal.AllocHGlobal(sizeof(int));
            }
            for (int i = 0; i < this.N; i++)
            {
                Marshal.FreeHGlobal((IntPtr)this.ptrs[i]);
            }
        }

        [Benchmark]
        public void PoolAllocFree()
        {
            for (int i = 0; i < this.N; i++)
            {
                this.ptrs[i] = this.pool.Alloc();
            }
            for (int i = 0; i < this.N; i++)
            {
                this.pool.Free(this.ptrs[i]);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.pool.Dispose();
            Marshal.FreeHGlobal((IntPtr)this.ptrs);
        }
    }
}
