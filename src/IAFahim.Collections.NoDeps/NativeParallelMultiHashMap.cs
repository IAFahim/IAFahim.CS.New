namespace Unity.Jobs
{
    using System.Runtime.CompilerServices;

    public struct JobHandle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete()
        {
        }
    }
}

namespace Unity.Collections
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.Jobs;

    public struct NativeParallelMultiHashMapIterator<TKey>
        where TKey : unmanaged
    {
        public TKey Key;
        public int EntryIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativeParallelMultiHashMap<TKey, TValue> : IDisposable
        where TKey : unmanaged
        where TValue : unmanaged
    {
        private const uint FnvOffsetBasis = 2166136261;
        private const uint FnvPrime = 16777619;
        private const int MinBucketCount = 16;
        private const int EmptyBucket = -1;

        [StructLayout(LayoutKind.Sequential)]
        private struct HashMapState
        {
            public void* Keys;
            public void* Values;
            public int* Next;
            public int* Buckets;
            public int Capacity;
            public int AllocatedLength;
            public int BucketCount;
        }

        private HashMapState* state;

        public readonly bool IsCreated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.state != null;
        }

        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => this.state != null ? this.state->Capacity : 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (this.state != null)
                {
                    this.state->Capacity = value;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeParallelMultiHashMap(int capacity, Allocator allocator)
        {
            this.state = (HashMapState*)Marshal.AllocHGlobal((IntPtr)sizeof(HashMapState));
            this.state->Keys = null;
            this.state->Values = null;
            this.state->Next = null;
            this.state->Buckets = null;
            this.state->Capacity = capacity;
            this.state->AllocatedLength = 0;
            this.state->BucketCount = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FreeBuffers(HashMapState* st)
        {
            if (st->Keys != null) Marshal.FreeHGlobal((IntPtr)st->Keys);
            if (st->Values != null) Marshal.FreeHGlobal((IntPtr)st->Values);
            if (st->Next != null) Marshal.FreeHGlobal((IntPtr)st->Next);
            if (st->Buckets != null) Marshal.FreeHGlobal((IntPtr)st->Buckets);
            st->Keys = null;
            st->Values = null;
            st->Next = null;
            st->Buckets = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AllocateBuffers(HashMapState* st, int length)
        {
            long keysSize = (long)length * sizeof(TKey);
            long valuesSize = (long)length * sizeof(TValue);
            long nextSize = (long)length * sizeof(int);

            st->Keys = (void*)Marshal.AllocHGlobal((IntPtr)keysSize);
            st->Values = (void*)Marshal.AllocHGlobal((IntPtr)valuesSize);
            st->Next = (int*)Marshal.AllocHGlobal((IntPtr)nextSize);

            byte* keysPtr = (byte*)st->Keys;
            for (long i = 0; i < keysSize; i++) keysPtr[i] = 0;

            byte* valuesPtr = (byte*)st->Values;
            for (long i = 0; i < valuesSize; i++) valuesPtr[i] = 0;

            for (int i = 0; i < length; i++) st->Next[i] = EmptyBucket;

            int bucketCount = length * 2;
            if (bucketCount < MinBucketCount) bucketCount = MinBucketCount;
            long bucketsSize = (long)bucketCount * sizeof(int);
            st->Buckets = (int*)Marshal.AllocHGlobal((IntPtr)bucketsSize);
            for (int i = 0; i < bucketCount; i++) st->Buckets[i] = EmptyBucket;

            st->AllocatedLength = length;
            st->BucketCount = bucketCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (this.state != null)
            {
                FreeBuffers(this.state);
                Marshal.FreeHGlobal((IntPtr)this.state);
                this.state = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly JobHandle Dispose(JobHandle dependency)
        {
            NativeParallelMultiHashMap<TKey, TValue> temp = this;
            temp.Dispose();
            return dependency;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Clear()
        {
            if (this.state != null)
            {
                this.state->AllocatedLength = 0;
                if (this.state->Buckets != null)
                {
                    for (int i = 0; i < this.state->BucketCount; i++)
                    {
                        this.state->Buckets[i] = EmptyBucket;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void SetAllocatedIndexLength(int length)
        {
            if (this.state == null) return;

            FreeBuffers(this.state);
            this.state->AllocatedLength = 0;
            this.state->BucketCount = 0;

            if (length > 0) AllocateBuffers(this.state, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly UnsafeBucketData GetUnsafeBucketData()
        {
            if (this.state == null)
            {
                UnsafeBucketData empty;
                empty.keys = null;
                empty.values = null;
                return empty;
            }
            UnsafeBucketData data;
            data.keys = this.state->Keys;
            data.values = this.state->Values;
            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void RecalculateBuckets()
        {
            if (this.state == null || this.state->AllocatedLength <= 0 || this.state->Buckets == null)
            {
                return;
            }

            for (int i = 0; i < this.state->BucketCount; i++)
            {
                this.state->Buckets[i] = EmptyBucket;
            }

            for (int i = 0; i < this.state->AllocatedLength; i++)
            {
                this.state->Next[i] = EmptyBucket;
            }

            TKey* keysPtr = (TKey*)this.state->Keys;

            for (int i = 0; i < this.state->AllocatedLength; i++)
            {
                int hashCode = HashKey(keysPtr + i);
                int bucketIndex = (hashCode & 0x7FFFFFFF) % this.state->BucketCount;

                this.state->Next[i] = this.state->Buckets[bucketIndex];
                this.state->Buckets[bucketIndex] = i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetFirstValue(TKey key, out TValue item, out NativeParallelMultiHashMapIterator<TKey> it)
        {
            it.Key = key;
            it.EntryIndex = EmptyBucket;

            if (this.state == null || this.state->AllocatedLength <= 0 || this.state->Buckets == null)
            {
                item = default;
                return false;
            }

            int hashCode = HashKey(&key);
            int bucketIndex = (hashCode & 0x7FFFFFFF) % this.state->BucketCount;
            int entryIndex = this.state->Buckets[bucketIndex];

            TKey* keysPtr = (TKey*)this.state->Keys;
            TValue* valuesPtr = (TValue*)this.state->Values;

            while (entryIndex != EmptyBucket)
            {
                TKey* entryKey = keysPtr + entryIndex;
                if (CompareKeys(entryKey, &key))
                {
                    item = valuesPtr[entryIndex];
                    it.EntryIndex = entryIndex;
                    return true;
                }
                entryIndex = this.state->Next[entryIndex];
            }

            item = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryGetNextValue(out TValue item, ref NativeParallelMultiHashMapIterator<TKey> it)
        {
            if (this.state == null || this.state->AllocatedLength <= 0 || this.state->Buckets == null || it.EntryIndex == EmptyBucket)
            {
                item = default;
                return false;
            }

            int entryIndex = this.state->Next[it.EntryIndex];
            TKey key = it.Key;

            TKey* keysPtr = (TKey*)this.state->Keys;
            TValue* valuesPtr = (TValue*)this.state->Values;

            while (entryIndex != EmptyBucket)
            {
                TKey* entryKey = keysPtr + entryIndex;
                if (CompareKeys(entryKey, &key))
                {
                    item = valuesPtr[entryIndex];
                    it.EntryIndex = entryIndex;
                    return true;
                }
                entryIndex = this.state->Next[entryIndex];
            }

            item = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int HashKey(TKey* keyPtr)
        {
            byte* bytePtr = (byte*)keyPtr;
            int size = sizeof(TKey);
            uint hash = FnvOffsetBasis;
            for (int i = 0; i < size; i++)
            {
                hash = (hash ^ bytePtr[i]) * FnvPrime;
            }
            return (int)hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CompareKeys(TKey* key1, TKey* key2)
        {
            byte* b1 = (byte*)key1;
            byte* b2 = (byte*)key2;
            int size = sizeof(TKey);
            for (int i = 0; i < size; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public struct UnsafeBucketData
        {
            public void* keys;
            public void* values;
        }
    }
}
