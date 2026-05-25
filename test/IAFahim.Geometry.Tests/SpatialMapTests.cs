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
    }
}
