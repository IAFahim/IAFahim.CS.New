namespace IAFahim.Geometry.Tests
{
    using System;
    using IAFahim.DS.SpatialMap;
    using NUnit.Framework;
    using Unity.Collections;
    using Unity.Jobs;
    using Unity.Mathematics;

    public struct TestPosition2D : ISpatialPosition
    {
        public float2 Pos;
        public float2 Position => Pos;
    }

    public struct TestPosition3D : ISpatialPosition3
    {
        public float3 Pos;
        public float3 Position => Pos;
    }

    public sealed unsafe class SpatialMapTests
    {
        [Test]
        public void SpatialMap2D_BuildsAndHashesCorrectly()
        {
            NativeArray<TestPosition2D> positions = new NativeArray<TestPosition2D>(4, Allocator.TempJob);
            positions[0] = new TestPosition2D { Pos = new float2(-1.0f, -1.0f) };
            positions[1] = new TestPosition2D { Pos = new float2(1.0f, 1.0f) };
            positions[2] = new TestPosition2D { Pos = new float2(10.0f, 10.0f) };
            positions[3] = new TestPosition2D { Pos = new float2(-10.0f, -10.0f) };

            SpatialMap<TestPosition2D> sm = new SpatialMap<TestPosition2D>(2.0f, 100, Allocator.TempJob);
            try
            {
                JobHandle handle = sm.Build(positions, default, 2);
                handle.Complete();

                Assert.IsTrue(sm.IsCreated);

                SpatialMapReadOnly reader = sm.AsReadOnly();
                for (int i = 0; i < positions.Length; i++)
                {
                    float2 pos = positions[i].Pos;
                    int2 quantized = reader.Quantized(pos);
                    int hash = reader.Hash(quantized);

                    bool found = false;
                    int val;
                    NativeParallelMultiHashMapIterator<int> it;
                    if (reader.Map.TryGetFirstValue(hash, out val, out it))
                    {
                        do
                        {
                            if (val == i)
                            {
                                found = true;
                                break;
                            }
                        }
                        while (reader.Map.TryGetNextValue(out val, ref it));
                    }
                    Assert.IsTrue(found);
                }
            }
            finally
            {
                positions.Dispose();
                sm.Dispose();
            }
        }

        [Test]
        public void SpatialMap3D_BuildsAndHashesCorrectly()
        {
            NativeArray<TestPosition3D> positions = new NativeArray<TestPosition3D>(4, Allocator.TempJob);
            positions[0] = new TestPosition3D { Pos = new float3(-1.0f, -1.0f, -1.0f) };
            positions[1] = new TestPosition3D { Pos = new float3(1.0f, 1.0f, 1.0f) };
            positions[2] = new TestPosition3D { Pos = new float3(10.0f, 10.0f, 10.0f) };
            positions[3] = new TestPosition3D { Pos = new float3(-10.0f, -10.0f, -10.0f) };

            SpatialMap3<TestPosition3D> sm3 = new SpatialMap3<TestPosition3D>(2.0f, 100, Allocator.TempJob);
            try
            {
                JobHandle handle = sm3.Build(positions, default, 2);
                handle.Complete();

                Assert.IsTrue(sm3.IsCreated);

                SpatialMapReadOnly3 reader3 = sm3.AsReadOnly();
                for (int i = 0; i < positions.Length; i++)
                {
                    float3 pos = positions[i].Pos;
                    int3 quantized = reader3.Quantized(pos);
                    long hash = reader3.Hash(quantized);

                    bool found = false;
                    int val;
                    NativeParallelMultiHashMapIterator<long> it;
                    if (reader3.Map.TryGetFirstValue(hash, out val, out it))
                    {
                        do
                        {
                            if (val == i)
                            {
                                found = true;
                                break;
                            }
                        }
                        while (reader3.Map.TryGetNextValue(out val, ref it));
                    }
                    Assert.IsTrue(found);
                }
            }
            finally
            {
                positions.Dispose();
                sm3.Dispose();
            }
        }

        [Test]
        public void SpatialHexMap_BuildsAndHashesCorrectly()
        {
            NativeArray<TestPosition2D> positions = new NativeArray<TestPosition2D>(4, Allocator.TempJob);
            positions[0] = new TestPosition2D { Pos = new float2(-1.0f, -1.0f) };
            positions[1] = new TestPosition2D { Pos = new float2(1.0f, 1.0f) };
            positions[2] = new TestPosition2D { Pos = new float2(10.0f, 10.0f) };
            positions[3] = new TestPosition2D { Pos = new float2(-10.0f, -10.0f) };

            SpatialHexMap<TestPosition2D> sm = new SpatialHexMap<TestPosition2D>(2.0f, 100, Allocator.TempJob);
            try
            {
                JobHandle handle = sm.Build(positions, default);
                handle.Complete();

                Assert.IsTrue(sm.IsCreated);

                SpatialHexMap.ReadOnly reader = sm.AsReadOnly();
                for (int i = 0; i < positions.Length; i++)
                {
                    float2 pos = positions[i].Pos;
                    int2 quantized = reader.Quantized(pos);
                    int hash = reader.Hash(quantized);

                    bool found = false;
                    int val;
                    NativeParallelMultiHashMapIterator<int> it;
                    if (reader.Map.TryGetFirstValue(hash, out val, out it))
                    {
                        do
                        {
                            if (val == i)
                            {
                                found = true;
                                break;
                            }
                        }
                        while (reader.Map.TryGetNextValue(out val, ref it));
                    }
                    Assert.IsTrue(found);
                }
            }
            finally
            {
                positions.Dispose();
                sm.Dispose();
            }
        }

        [Test]
        public void SpatialKeyedMap_BuildsAndHashesCorrectly()
        {
            NativeArray<TestPosition2D> positions = new NativeArray<TestPosition2D>(4, Allocator.TempJob);
            positions[0] = new TestPosition2D { Pos = new float2(-1.0f, -1.0f) };
            positions[1] = new TestPosition2D { Pos = new float2(1.0f, 1.0f) };
            positions[2] = new TestPosition2D { Pos = new float2(10.0f, 10.0f) };
            positions[3] = new TestPosition2D { Pos = new float2(-10.0f, -10.0f) };

            SpatialKeyedMap<TestPosition2D> sm = new SpatialKeyedMap<TestPosition2D>(2.0f, 100, Allocator.TempJob);
            try
            {
                JobHandle handle = sm.Build(positions, default);
                handle.Complete();

                Assert.IsTrue(sm.IsCreated);
            }
            finally
            {
                positions.Dispose();
                sm.Dispose();
            }
        }

        [Test]
        public void LocalSpatialMap_BuildsAndHashesCorrectly()
        {
            NativeArray<TestPosition2D> positions = new NativeArray<TestPosition2D>(4, Allocator.TempJob);
            positions[0] = new TestPosition2D { Pos = new float2(-1.0f, -1.0f) };
            positions[1] = new TestPosition2D { Pos = new float2(1.0f, 1.0f) };
            positions[2] = new TestPosition2D { Pos = new float2(10.0f, 10.0f) };
            positions[3] = new TestPosition2D { Pos = new float2(-10.0f, -10.0f) };

            LocalSpatialMap<TestPosition2D> sm = new LocalSpatialMap<TestPosition2D>(2.0f, 100, Allocator.TempJob);
            try
            {
                JobHandle handle = sm.Build(positions, default);
                handle.Complete();

                Assert.IsTrue(sm.IsCreated);
            }
            finally
            {
                positions.Dispose();
                sm.Dispose();
            }
        }

        [Test]
        public void PerfectHashMap_BuildsAndHashesCorrectly()
        {
            NativeArray<int> keys = new NativeArray<int>(3, Allocator.TempJob);
            keys[0] = 5;
            keys[1] = 12;
            keys[2] = 99;

            NativeArray<int> values = new NativeArray<int>(3, Allocator.TempJob);
            values[0] = 100;
            values[1] = 200;
            values[2] = 300;

            IAFahim.DS.PerfectHashMap.NativePerfectHashMap<int, int> map = new IAFahim.DS.PerfectHashMap.NativePerfectHashMap<int, int>(keys, values, -1, Allocator.TempJob);
            try
            {
                Assert.IsTrue(map.IsCreated);
                Assert.AreEqual(100, map[5]);
                Assert.AreEqual(200, map[12]);
                Assert.AreEqual(300, map[99]);

                int val;
                Assert.IsTrue(map.TryGetValue(5, out val));
                Assert.AreEqual(100, val);

                Assert.IsFalse(map.TryGetValue(6, out val));
                Assert.AreEqual(-1, val);
            }
            finally
            {
                keys.Dispose();
                values.Dispose();
                map.Dispose();
            }
        }

        [Test]
        public void Allocators_AllocAndFreeCorrectly()
        {
            IAFahim.Memory.Allocators.NativeSlabAllocator<int> slab = new IAFahim.Memory.Allocators.NativeSlabAllocator<int>(64, Allocator.Persistent);
            try
            {
                Assert.IsTrue(slab.IsCreated);
                int* p1 = slab.Alloc();
                int* p2 = slab.Alloc();
                *p1 = 42;
                *p2 = 99;
                Assert.AreEqual(42, *p1);
                Assert.AreEqual(99, *p2);
                Assert.AreEqual(2, slab.AllocationCount);

                slab.Clear();
                Assert.AreEqual(0, slab.AllocationCount);
            }
            finally
            {
                slab.Dispose();
            }

            IAFahim.Memory.Allocators.UnsafePoolAllocator<int> pool = new IAFahim.Memory.Allocators.UnsafePoolAllocator<int>(32, Allocator.Persistent);
            try
            {
                Assert.IsTrue(pool.IsCreated);
                int* p1 = pool.Alloc();
                int* p2 = pool.Alloc();
                *p1 = 123;
                *p2 = 456;
                Assert.AreEqual(123, *p1);
                Assert.AreEqual(456, *p2);
                Assert.AreEqual(32, pool.Allocated() / sizeof(int));

                pool.Free(p1);
                int* p3 = pool.Alloc();
                Assert.IsTrue(p3 == p1);
            }
            finally
            {
                pool.Dispose();
            }

            IAFahim.Memory.Allocators.UnsafeFixedPoolAllocator<int> fixedPool = new IAFahim.Memory.Allocators.UnsafeFixedPoolAllocator<int>(10, Allocator.Persistent);
            try
            {
                Assert.IsTrue(fixedPool.IsCreated);
                int* p1 = fixedPool.Alloc();
                *p1 = 789;
                Assert.AreEqual(789, *p1);
                fixedPool.Free(p1);
            }
            finally
            {
                fixedPool.Dispose();
            }
        }
    }
}
