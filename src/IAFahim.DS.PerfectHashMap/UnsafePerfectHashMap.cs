namespace IAFahim.DS.PerfectHashMap
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Unity.Burst.CompilerServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Jobs.LowLevel.Unsafe;
    using BovineLabs.Core.Memory;

    public unsafe struct UnsafePerfectHashMap<TKey, TValue> : IDisposable
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged, IEquatable<TValue>
    {
        [NativeDisableUnsafePtrRestriction]
        internal TKey* Keys;

        [NativeDisableUnsafePtrRestriction]
        internal TValue* Values;

        internal int Size;
        internal int Mask;
        internal TValue NullValue;

        private readonly AllocatorManager.AllocatorHandle allocator;

        public UnsafePerfectHashMap(NativeArray<TKey> keys, NativeArray<TValue> values, TValue nullValue, AllocatorManager.AllocatorHandle allocator)
        {
            int keyCount = keys.Length;
            NativeHashSet<int> uniqueSet = new NativeHashSet<int>(keyCount, Allocator.Temp);
            NativeArray<int> hashes = new NativeArray<int>(keyCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < keyCount; i++)
            {
                hashes[i] = keys[i].GetHashCode();
            }

            AssertCollisionFree(hashes, uniqueSet);

            int size = FindSize(hashes, uniqueSet);
            int valueOffset;
            long totalSize = CalculateDataSize(size, out valueOffset);

            void* ptr = Unmanaged.Allocate(totalSize, JobsUtility.CacheLineSize, allocator);
            this.allocator = allocator;
            this.Size = size;
            this.Mask = size - 1;
            this.NullValue = nullValue;
            this.Keys = (TKey*)ptr;
            this.Values = (TValue*)((byte*)ptr + valueOffset);

            // Zero-clear keys so empty slots never spuriously Equals a live key before insert.
            UnsafeUtility.MemClear(this.Keys, (long)size * sizeof(TKey));
            UnsafeUtility.MemCpyReplicate(this.Values, &nullValue, sizeof(TValue), size);

            int mask = size - 1;
            for (int i = 0; i < keyCount; i++)
            {
                TKey key = keys[i];
                int index = hashes[i] & mask;
                this.Keys[index] = key;
                this.Values[index] = values[i];
            }
        }

        public static UnsafePerfectHashMap<TKey, TValue>* Alloc(
            NativeArray<TKey> keys, NativeArray<TValue> values, TValue nullValue, AllocatorManager.AllocatorHandle allocator)
        {
            UnsafePerfectHashMap<TKey, TValue>* data = (UnsafePerfectHashMap<TKey, TValue>*)Unmanaged.Allocate(
                (long)sizeof(UnsafePerfectHashMap<TKey, TValue>),
                UnsafeUtility.AlignOf<UnsafePerfectHashMap<TKey, TValue>>(),
                allocator);

            *data = new UnsafePerfectHashMap<TKey, TValue>(keys, values, nullValue, allocator);
            return data;
        }

        public static void Free(UnsafePerfectHashMap<TKey, TValue>* data)
        {
            if (data == null)
            {
                throw new InvalidOperationException("Hash based container has yet to be created or has been destroyed!");
            }

            AllocatorManager.AllocatorHandle allocator = data->allocator;
            data->Dispose();
            Unmanaged.Free(data, allocator);
        }

        public readonly bool IsCreated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.Keys != null;
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (Hint.Unlikely(!this.TryGetValue(key, out value)))
                {
                    this.ThrowKeyNotPresent(key);
                    return default;
                }

                return value;
            }

            set
            {
                int index;
                if (!this.TryGetIndex(key, out index))
                {
                    this.ThrowKeyNotPresent(key);
                }

                this.Values[index] = value;
            }
        }

        public void Dispose()
        {
            if (!this.IsCreated)
            {
                return;
            }

            Unmanaged.Free(this.Keys, this.allocator);
            this = default;
        }

        public bool TryGetValue(TKey key, out TValue item)
        {
            int index = key.GetHashCode() & this.Mask;
            if (Hint.Unlikely(!this.Keys[index].Equals(key)))
            {
                item = this.NullValue;
                return false;
            }

            item = this.Values[index];
            return !item.Equals(this.NullValue);
        }

        private static int FindSize(NativeArray<int> hashes, NativeHashSet<int> unique)
        {
            int size = 1;

            while (HasCollisions(size, hashes, unique))
            {
                size <<= 1;
            }

            return size;
        }

        private static bool HasCollisions(int size, NativeArray<int> hashes, NativeHashSet<int> usedIndexes)
        {
            usedIndexes.Clear();

            int mask = size - 1;
            int count = hashes.Length;
            for (int i = 0; i < count; i++)
            {
                int index = hashes[i] & mask;

                if (!usedIndexes.Add(index))
                {
                    return true;
                }
            }

            return false;
        }

        private static long CalculateDataSize(int count, out int outValueOffset)
        {
            long sizeOfTKey = sizeof(TKey);
            long sizeOfTValue = sizeof(TValue);

            long keysSize = sizeOfTKey * count;
            long valuesSize = sizeOfTValue * count;

            long valueAlign = UnsafeUtility.AlignOf<TValue>();
            long valueOffset = (keysSize + (valueAlign - 1)) & ~(valueAlign - 1);
            long totalSize = valueOffset + valuesSize;

            outValueOffset = (int)valueOffset;

            return totalSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetIndex(TKey key, out int index)
        {
            index = key.GetHashCode() & this.Mask;
            return this.Keys[index].Equals(key) && !this.Values[index].Equals(this.NullValue);
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [Conditional("UNITY_DOTS_DEBUG")]
        private static void AssertCollisionFree(NativeArray<int> hashes, NativeHashSet<int> unique)
        {
            for (int i = 0; i < hashes.Length; i++)
            {
                if (!unique.Add(hashes[i]))
                {
                    throw new ArgumentException("HashCode collision.");
                }
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        [Conditional("UNITY_DOTS_DEBUG")]
        private void ThrowKeyNotPresent(TKey key)
        {
            throw new ArgumentException($"Key: {key} is not present.");
        }
    }
}
