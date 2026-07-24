namespace IAFahim.Geometry.Frame.Tests
{
    using NUnit.Framework;
    using Unity.Mathematics;

    public sealed unsafe class ParallelTransportTests
    {
        [Test]
        public void StraightLine_PreservesFrames()
        {
            const int N = 4;
            float3* pos = stackalloc float3[N];
            float3* right = stackalloc float3[N];
            float3* up = stackalloc float3[N];
            float3* forward = stackalloc float3[N];
            for (int i = 0; i < N; i++) pos[i] = new float3(i, 0, 0);
            ParallelTransport.Compute(pos, N, new float3(0, 1, 0), right, up, forward);
            for (int i = 0; i < N; i++)
            {
                float len = math.length(forward[i]);
                Assert.IsTrue(len > 0.5f);
            }
        }
    }
}
