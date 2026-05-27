namespace IAFahim.Math.PoissonDisk.Tests
{
    using Unity.Mathematics;
    using NUnit.Framework;

    public sealed unsafe class PoissonDisk2DTests
    {
        [Test]
        public void Run_Basic_CreatesPoints()
        {
            float2 min = new float2(0, 0);
            float2 max = new float2(10, 10);
            float2* output = stackalloc float2[100];

            int count = PoissonDisk2D.Run(min, max, 1.0f, output, 100, 42);
            Assert.IsTrue(count > 0);
        }

        [Test]
        public void Run_DenseSpacing_FewerPoints()
        {
            float2 min = new float2(0, 0);
            float2 max = new float2(10, 10);
            float2* output1 = stackalloc float2[100];
            float2* output2 = stackalloc float2[100];

            int count1 = PoissonDisk2D.Run(min, max, 0.5f, output1, 100, 42);
            int count2 = PoissonDisk2D.Run(min, max, 2.0f, output2, 100, 42);

            Assert.IsTrue(count1 > count2);
        }

        [Test]
        public void Run_SmallArea_LimitedPoints()
        {
            float2 min = new float2(0, 0);
            float2 max = new float2(1, 1);
            float2* output = stackalloc float2[10];

            int count = PoissonDisk2D.Run(min, max, 0.5f, output, 10, 42);
            Assert.IsTrue(count <= 10);
        }

        [Test]
        public void Run_PointsInsideBounds()
        {
            float2 min = new float2(0, 0);
            float2 max = new float2(10, 10);
            float2* output = stackalloc float2[50];

            int count = PoissonDisk2D.Run(min, max, 1.0f, output, 50, 42);

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(output[i].x >= 0.0f && output[i].x <= 10.0f);
                Assert.IsTrue(output[i].y >= 0.0f && output[i].y <= 10.0f);
            }
        }
    }

    public sealed unsafe class PoissonDisk3DTests
    {
        [Test]
        public void Run_Basic_CreatesPoints()
        {
            float3 min = new float3(0, 0, 0);
            float3 max = new float3(10, 10, 10);
            float3* output = stackalloc float3[100];

            int count = PoissonDisk3D.Run(min, max, 1.0f, output, 100, 42);
            Assert.IsTrue(count > 0);
        }

        [Test]
        public void Run_DenseSpacing_FewerPoints()
        {
            float3 min = new float3(0, 0, 0);
            float3 max = new float3(10, 10, 10);
            float3* output1 = stackalloc float3[100];
            float3* output2 = stackalloc float3[100];

            int count1 = PoissonDisk3D.Run(min, max, 0.5f, output1, 100, 42);
            int count2 = PoissonDisk3D.Run(min, max, 2.0f, output2, 100, 42);

            Assert.IsTrue(count1 > count2);
        }

        [Test]
        public void Run_PointsInsideBounds()
        {
            float3 min = new float3(0, 0, 0);
            float3 max = new float3(5, 5, 5);
            float3* output = stackalloc float3[50];

            int count = PoissonDisk3D.Run(min, max, 1.0f, output, 50, 42);

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(output[i].x >= 0.0f && output[i].x <= 5.0f);
                Assert.IsTrue(output[i].y >= 0.0f && output[i].y <= 5.0f);
                Assert.IsTrue(output[i].z >= 0.0f && output[i].z <= 5.0f);
            }
        }
    }
}