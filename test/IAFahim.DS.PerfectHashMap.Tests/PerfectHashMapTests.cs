namespace IAFahim.DS.PerfectHashMap.Tests
{
    using System;
    using NUnit.Framework;
    using Unity.Collections;

    public sealed unsafe class PerfectHashMapTests
    {
        [Test]
        public void TryGetValue_PresentKey_ReturnsValue()
        {
            NativeArray<int> keys = new NativeArray<int>(3, Allocator.Temp);
            NativeArray<int> values = new NativeArray<int>(3, Allocator.Temp);
            try
            {
                keys[0] = 10; keys[1] = 20; keys[2] = 30;
                values[0] = 100; values[1] = 200; values[2] = 300;
                UnsafePerfectHashMap<int, int> map = new UnsafePerfectHashMap<int, int>(
                    keys, values, -1, Allocator.Persistent);
                try
                {
                    Assert.IsTrue(map.TryGetValue(20, out int v));
                    Assert.AreEqual(200, v);
                    Assert.AreEqual(200, map[20]);
                }
                finally
                {
                    map.Dispose();
                }
            }
            finally
            {
                keys.Dispose();
                values.Dispose();
            }
        }

        [Test]
        public void TryGetValue_MissingKey_ReturnsFalseEvenIfHashCollidesSlot()
        {
            NativeArray<int> keys = new NativeArray<int>(1, Allocator.Temp);
            NativeArray<int> values = new NativeArray<int>(1, Allocator.Temp);
            try
            {
                keys[0] = 1;
                values[0] = 42;
                UnsafePerfectHashMap<int, int> map = new UnsafePerfectHashMap<int, int>(
                    keys, values, -1, Allocator.Persistent);
                try
                {
                    // Key equality is required: a key that is not in the map must miss
                    // even when its hash lands in a populated table slot range.
                    Assert.IsFalse(map.TryGetValue(999999, out int miss));
                    Assert.AreEqual(-1, miss);
                }
                finally
                {
                    map.Dispose();
                }
            }
            finally
            {
                keys.Dispose();
                values.Dispose();
            }
        }

        [Test]
        public void Alloc_Free_RoundTrip()
        {
            NativeArray<int> keys = new NativeArray<int>(2, Allocator.Temp);
            NativeArray<int> values = new NativeArray<int>(2, Allocator.Temp);
            try
            {
                keys[0] = 5; keys[1] = 7;
                values[0] = 50; values[1] = 70;
                UnsafePerfectHashMap<int, int>* data = UnsafePerfectHashMap<int, int>.Alloc(
                    keys, values, -1, Allocator.Persistent);
                Assert.IsTrue(data->TryGetValue(5, out int v));
                Assert.AreEqual(50, v);
                UnsafePerfectHashMap<int, int>.Free(data);
            }
            finally
            {
                keys.Dispose();
                values.Dispose();
            }
        }

        [Test]
        public void Indexer_Set_UpdatesValue()
        {
            NativeArray<int> keys = new NativeArray<int>(1, Allocator.Temp);
            NativeArray<int> values = new NativeArray<int>(1, Allocator.Temp);
            try
            {
                keys[0] = 3;
                values[0] = 1;
                UnsafePerfectHashMap<int, int> map = new UnsafePerfectHashMap<int, int>(
                    keys, values, -1, Allocator.Persistent);
                try
                {
                    map[3] = 99;
                    Assert.AreEqual(99, map[3]);
                }
                finally
                {
                    map.Dispose();
                }
            }
            finally
            {
                keys.Dispose();
                values.Dispose();
            }
        }
    }
}
