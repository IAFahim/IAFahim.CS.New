namespace IAFahim.Geometry.Tests
{
    using System;
    using IAFahim.Geometry.Frame;
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class FrameTests
    {
        [Test]
        public void ParallelTransport_StraightLine_OrthogonalFrames()
        {
            const int count = 5;
            float3* positions = stackalloc float3[count];
            for (int i = 0; i < count; i++)
            {
                positions[i] = new float3((float)i, 0.0f, 0.0f);
            }

            float3* right = stackalloc float3[count];
            float3* up = stackalloc float3[count];
            float3* forward = stackalloc float3[count];

            ParallelTransport.Compute(positions, count, new float3(0.0f, 1.0f, 0.0f), right, up, forward);

            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(1.0f, math.length(forward[i]), 1e-5f);
                Assert.AreEqual(1.0f, math.length(up[i]), 1e-5f);
                Assert.AreEqual(1.0f, math.length(right[i]), 1e-5f);

                Assert.AreEqual(0.0f, math.dot(forward[i], up[i]), 1e-5f);
                Assert.AreEqual(0.0f, math.dot(forward[i], right[i]), 1e-5f);
                Assert.AreEqual(0.0f, math.dot(up[i], right[i]), 1e-5f);

                Assert.AreEqual(1.0f, forward[i].x, 1e-5f);
            }
        }
    }
}
