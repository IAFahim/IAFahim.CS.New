namespace IAFahim.DS.FixedCollections
{
    using System;
    using System.Runtime.InteropServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Jobs.LowLevel.Unsafe;
    using BovineLabs.Core.Memory;

    public unsafe struct ThreadList : IDisposable
    {
        private readonly AllocatorManager.AllocatorHandle allocator;

        [NativeDisableUnsafePtrRestriction]
        private Lists* buffer;

        public ThreadList(AllocatorManager.AllocatorHandle allocator)
        {
            this.allocator = allocator;
            long totalSize = (long)sizeof(Lists) * JobsUtility.ThreadIndexCount;
            this.buffer = (Lists*)Unmanaged.Allocate(totalSize, UnsafeUtility.AlignOf<Lists>(), allocator);

            for (int i = 0; i < JobsUtility.ThreadIndexCount; i++)
            {
                this.buffer[i].List = new UnsafeList<byte>(512, allocator);
            }
        }

        public readonly bool IsCreated => this.buffer != null;

        public ref UnsafeList<byte> GetList()
        {
            return ref this.GetList(JobsUtility.ThreadIndex);
        }

        public ref UnsafeList<byte> GetList(int threadIndex)
        {
            ref Lists listWrapper = ref UnsafeUtility.ArrayElementAsRef<Lists>(this.buffer, threadIndex);
            return ref listWrapper.List;
        }

        public void Dispose()
        {
            if (!this.IsCreated)
            {
                return;
            }

            for (int i = 0; i < JobsUtility.ThreadIndexCount; i++)
            {
                this.buffer[i].List.Dispose();
            }

            Unmanaged.Free(this.buffer, this.allocator);
            this.buffer = null;
        }

        [StructLayout(LayoutKind.Explicit, Size = JobsUtility.CacheLineSize)]
        private struct Lists
        {
            [FieldOffset(0)]
            public UnsafeList<byte> List;
        }
    }
}
