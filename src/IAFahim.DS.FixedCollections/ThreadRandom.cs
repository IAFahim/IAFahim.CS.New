namespace IAFahim.DS.FixedCollections
{
    using System;
    using System.Runtime.InteropServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Jobs.LowLevel.Unsafe;
    using Unity.Mathematics;
    using BovineLabs.Core.Memory;

    public unsafe struct ThreadRandom : IDisposable
    {
        private readonly AllocatorManager.AllocatorHandle allocator;

        [NativeDisableUnsafePtrRestriction]
        private Randoms* buffer;

        public ThreadRandom(uint seed, AllocatorManager.AllocatorHandle allocator)
        {
            this.allocator = allocator;
            long totalSize = (long)sizeof(Randoms) * JobsUtility.ThreadIndexCount;
            this.buffer = (Randoms*)Unmanaged.Allocate(totalSize, UnsafeUtility.AlignOf<Randoms>(), allocator);

            seed = (uint)math.min(seed, uint.MaxValue - JobsUtility.ThreadIndexCount - 1);

            for (int i = 0; i < JobsUtility.ThreadIndexCount; i++)
            {
                this.buffer[i].Random = Unity.Mathematics.Random.CreateFromIndex((uint)(seed + i));
            }
        }

        public readonly bool IsCreated => this.buffer != null;

        public ref Unity.Mathematics.Random GetRandomRef()
        {
            ref Randoms randoms = ref UnsafeUtility.ArrayElementAsRef<Randoms>(this.buffer, JobsUtility.ThreadIndex);
            return ref randoms.Random;
        }

        public void Dispose()
        {
            if (!this.IsCreated)
            {
                return;
            }

            Unmanaged.Free(this.buffer, this.allocator);
            this.buffer = null;
        }

        [StructLayout(LayoutKind.Explicit, Size = JobsUtility.CacheLineSize)]
        private struct Randoms
        {
            [FieldOffset(0)]
            public Unity.Mathematics.Random Random;
        }
    }
}
