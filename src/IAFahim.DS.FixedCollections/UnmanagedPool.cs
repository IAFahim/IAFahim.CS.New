namespace IAFahim.DS.FixedCollections
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Mathematics;
    using BovineLabs.Core.Memory;

    public readonly unsafe struct UnmanagedPool<T> : IDisposable
        where T : unmanaged
    {
        private readonly int capacity;
        private readonly Allocator allocator;

        [NativeDisableUnsafePtrRestriction]
        private readonly T* buffer;

        [NativeDisableUnsafePtrRestriction]
        private readonly int* length;

        [NativeDisableUnsafePtrRestriction]
        private readonly SpinLock* spinner;

        public UnmanagedPool(int capacity, Allocator allocator = Allocator.Persistent)
        {
            capacity = GetCapacity(capacity);
            this.capacity = capacity;
            this.allocator = allocator;

            this.buffer = (T*)Unmanaged.Allocate((long)sizeof(T) * capacity, UnsafeUtility.AlignOf<T>(), allocator);
            this.length = (int*)Unmanaged.Allocate((long)sizeof(int), UnsafeUtility.AlignOf<int>(), allocator);
            *this.length = 0;
            this.spinner = (SpinLock*)Unmanaged.Allocate((long)sizeof(SpinLock), UnsafeUtility.AlignOf<SpinLock>(), allocator);
            *this.spinner = default;
        }

        public bool IsCreated => this.buffer != null;

        public void Dispose()
        {
            Unmanaged.Free(this.buffer, this.allocator);
            Unmanaged.Free(this.length, this.allocator);
            Unmanaged.Free(this.spinner, this.allocator);
        }

        public bool TryAdd(T element)
        {
            this.spinner->Acquire();

            if (*this.length < this.capacity)
            {
                this.buffer[*this.length] = element;
                *this.length += 1;
                this.spinner->Release();
                return true;
            }

            this.spinner->Release();
            return false;
        }

        public bool TryGet(out T element)
        {
            this.spinner->Acquire();

            if (*this.length > 0)
            {
                *this.length -= 1;
                element = this.buffer[*this.length];
                this.spinner->Release();
                return true;
            }

            element = default;
            this.spinner->Release();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetCapacity(int newCapacity)
        {
            newCapacity = math.max(newCapacity, CollectionHelper.CacheLineSize / sizeof(T));
            newCapacity = math.ceilpow2(newCapacity);
            return newCapacity;
        }
    }
}
