namespace IAFahim.Geometry.Frame.Tests
{
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class ParallelTransportTests
    {
        [Test]
        public void StraightX_ForwardAlongX()
        {
            const int N = 5;
            float3* pos = stackalloc float3[N];
            float3* right = stackalloc float3[N];
            float3* up = stackalloc float3[N];
            float3* forward = stackalloc float3[N];
            for (int i = 0; i < N; i++) pos[i] = new float3(i, 0, 0);
            ParallelTransport.Compute(pos, N, new float3(0, 1, 0), right, up, forward);
            for (int i = 0; i < N; i++)
            {
                Assert.AreEqual(1f, forward[i].x, 0.15f);
                Assert.AreEqual(0f, forward[i].y, 0.15f);
                Assert.AreEqual(0f, forward[i].z, 0.15f);
                float upLen = math.length(up[i]);
                Assert.AreEqual(1f, upLen, 0.15f);
                float ortho = math.dot(forward[i], up[i]);
                Assert.AreEqual(0f, ortho, 0.2f);
            }
        }

        [Test]
        public void Empty_NoOp()
        {
            ParallelTransport.Compute(null, 0, new float3(0, 1, 0), null, null, null);
        }
    }
}
