namespace IAFahim.Geometry.Tests
{
    using System;
    using IAFahim.DS.FixedCollections;
    using NUnit.Framework;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Jobs;

    public struct Storage64Bytes
    {
        private long f0, f1, f2, f3, f4, f5, f6, f7;
    }

    public sealed unsafe class FixedCollectionsTests
    {
        [Test]
        public void FixedArray_SetAndGetCorrectly()
        {
            FixedArray<int, Storage64Bytes> arr = default;
            Assert.AreEqual(16, arr.Length);

            arr[0] = 100;
            arr[15] = 999;
            Assert.AreEqual(100, arr[0]);
            Assert.AreEqual(999, arr[15]);

            arr.ElementAt(5) = 555;
            Assert.AreEqual(555, arr[5]);
        }

        [Test]
        public void FixedHashMap_AddAndRetrieveCorrectly()
        {
            FixedHashMap<int, float, Storage64Bytes> map = new FixedHashMap<int, float, Storage64Bytes>(default);
            Assert.IsTrue(map.Capacity > 0);

            Assert.IsTrue(map.TryAdd(10, 1.5f));
            Assert.IsTrue(map.TryAdd(20, 2.5f));

            float val;
            Assert.IsTrue(map.TryGetValue(10, out val));
            Assert.AreEqual(1.5f, val);

            Assert.IsTrue(map.TryGetValue(20, out val));
            Assert.AreEqual(2.5f, val);

            Assert.IsFalse(map.TryGetValue(99, out val));
            Assert.IsFalse(map.TryAdd(10, 3.5f));
        }

        [Test]
        public void FixedBitMask_SetAndGetCorrectly()
        {
            FixedBitMask<Storage64Bytes> mask = default;
            Assert.AreEqual(512, mask.Length);

            Assert.IsFalse(mask.IsSet(10));
            mask.Set(10, true);
            Assert.IsTrue(mask.IsSet(10));

            mask.Set(10, false);
            Assert.IsFalse(mask.IsSet(10));

            mask.Set(500, true);
            Assert.IsTrue(mask.IsSet(500));

            mask.Reset();
            Assert.IsFalse(mask.IsSet(500));
        }

        [Test]
        public void NativeCounter_IncrementsCorrectly()
        {
            NativeCounter counter = new NativeCounter(Allocator.Persistent);
            try
            {
                Assert.IsTrue(counter.IsCreated);
                Assert.AreEqual(0, counter.Count);

                Assert.AreEqual(1, counter.Increment());
                Assert.AreEqual(1, counter.Count);

                NativeCounter.ParallelWriter writer = counter.AsParallelWriter();
                Assert.AreEqual(2, writer.Increment());
                Assert.AreEqual(2, counter.Count);
            }
            finally
            {
                counter.Dispose();
            }
        }

        [Test]
        public void SpinLock_AcquiresAndReleases()
        {
            SpinLock spinLock = default;
            Assert.IsTrue(spinLock.TryAcquire());
            Assert.IsFalse(spinLock.TryAcquire());
            spinLock.Release();
            Assert.IsTrue(spinLock.TryAcquire());
            spinLock.Release();
        }

        [Test]
        public void UnmanagedPool_PushesAndPops()
        {
            UnmanagedPool<int> pool = new UnmanagedPool<int>(10, Allocator.Persistent);
            try
            {
                Assert.IsTrue(pool.IsCreated);
                Assert.IsTrue(pool.TryAdd(42));
                Assert.IsTrue(pool.TryAdd(99));

                int val;
                Assert.IsTrue(pool.TryGet(out val));
                Assert.AreEqual(99, val); // LIFO behavior

                Assert.IsTrue(pool.TryGet(out val));
                Assert.AreEqual(42, val);

                Assert.IsFalse(pool.TryGet(out val));
            }
            finally
            {
                pool.Dispose();
            }
        }

        [Test]
        public void UnsafeListPool_ReusesList()
        {
            UnsafeListPool<int> pool = new UnsafeListPool<int>(5, Allocator.Persistent);
            try
            {
                Assert.IsTrue(pool.IsCreated);
                UnsafeList<int> list = pool.GetOrCreate(16, Allocator.Persistent);
                Assert.IsTrue(list.IsCreated);
                list.Add(50);
                pool.ReturnOrDispose(list);

                UnsafeList<int> list2;
                Assert.IsTrue(pool.TryGet(out list2));
                Assert.AreEqual(50, list2[0]);
                list2.Dispose();
            }
            finally
            {
                pool.Dispose();
            }
        }

        [Test]
        public void ThreadRandom_GeneratesRandomNumber()
        {
            ThreadRandom tr = new ThreadRandom(42, Allocator.Persistent);
            try
            {
                Assert.IsTrue(tr.IsCreated);
                ref Unity.Mathematics.Random random = ref tr.GetRandomRef();
                int val = random.NextInt(0, 100);
                Assert.IsTrue(val >= 0 && val < 100);
            }
            finally
            {
                tr.Dispose();
            }
        }

        [Test]
        public void ThreadList_RetrievesList()
        {
            ThreadList tl = new ThreadList(Allocator.Persistent);
            try
            {
                Assert.IsTrue(tl.IsCreated);
                ref UnsafeList<byte> list = ref tl.GetList();
                list.Add(12);
                Assert.AreEqual(12, list[0]);
            }
            finally
            {
                tl.Dispose();
            }
        }

        [Test]
        public void NativeLinearCongruentialGenerator_GeneratesNumbers()
        {
            NativeLinearCongruentialGenerator lcg = new NativeLinearCongruentialGenerator(1234, Allocator.Persistent);
            try
            {
                int val1 = lcg.Next();
                int val2 = lcg.Next();
                Assert.AreNotEqual(val1, val2);
            }
            finally
            {
                lcg.Dispose();
            }
        }
    }
}
