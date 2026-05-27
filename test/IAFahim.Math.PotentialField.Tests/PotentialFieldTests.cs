namespace IAFahim.Math.PotentialField.Tests
{
    using Unity.Mathematics;
    using NUnit.Framework;

    public sealed unsafe class PotentialField2DTests
    {
        [Test]
        public void Attractive_TowardTarget()
        {
            float2 pos = new float2(0, 0);
            float2 target = new float2(10, 0);
            float2 force = PotentialField2D.Attractive(pos, target, 1.0f);

            Assert.IsTrue(force.x > 0.0f);
        }

        [Test]
        public void Repulsive_OutsideRadius_IsZero()
        {
            float2 pos = new float2(10, 0);
            float2 obstacle = float2.zero;
            float2 force = PotentialField2D.Repulsive(pos, obstacle, 5.0f, 1.0f);

            Assert.IsTrue(math.length(force) < 1e-6f);
        }

        [Test]
        public void Repulsive_InsideRadius_PushesAway()
        {
            float2 pos = new float2(1, 0);
            float2 obstacle = float2.zero;
            float2 force = PotentialField2D.Repulsive(pos, obstacle, 5.0f, 1.0f);

            Assert.IsTrue(force.x > 0.0f);
        }

        [Test]
        public void Tangential_IsPerpendicular()
        {
            float2 pos = new float2(1, 0);
            float2 obstacle = float2.zero;
            float2 force = PotentialField2D.Tangential(pos, obstacle, 5.0f, 1.0f);

            float dotProduct = math.dot(force, pos - obstacle);
            Assert.IsTrue(math.abs(dotProduct) < 1e-6f);
        }

        [Test]
        public void GradientDescent_MovesTowardGoal()
        {
            float2 start = new float2(0, 0);
            float2 target = new float2(5, 5);
            float2* path = stackalloc float2[100];
            float2* attractors = stackalloc float2[1];
            attractors[0] = target;

            int steps = PotentialField2D.GradientDescent(
                start, attractors, 1, 1.0f,
                null, 0, 0, 0,
                null, 0, 0, 0,
                0.1f, 0.01f, 100, path);

            Assert.IsTrue(steps > 0);
            float2 last = path[steps - 1];
            float dist = math.distance(last, target);
            Assert.IsTrue(dist < 2.0f);
        }
    }

    public sealed unsafe class PotentialField3DTests
    {
        [Test]
        public void Attractive_TowardTarget()
        {
            float3 pos = new float3(0, 0, 0);
            float3 target = new float3(10, 0, 0);
            float3 force = PotentialField3D.Attractive(pos, target, 1.0f);

            Assert.IsTrue(force.x > 0.0f);
        }

        [Test]
        public void Repulsive_OutsideRadius_IsZero()
        {
            float3 pos = new float3(10, 0, 0);
            float3 obstacle = float3.zero;
            float3 force = PotentialField3D.Repulsive(pos, obstacle, 5.0f, 1.0f);

            Assert.IsTrue(math.length(force) < 1e-6f);
        }

        [Test]
        public void ComputeGradient_SingleAttractor_PointsToward()
        {
            float3 pos = new float3(0, 0, 0);
            float3* attractors = stackalloc float3[1];
            attractors[0] = new float3(10, 0, 0);

            PotentialField3D.ComputeGradient(pos, attractors, 1, 1.0f, null, 0, 0, 0, out float3 grad);

            Assert.IsTrue(grad.x > 0.0f);
        }

        [Test]
        public void GradientDescent_MovesTowardGoal()
        {
            float3 start = new float3(0, 0, 0);
            float3 target = new float3(5, 5, 5);
            float3* path = stackalloc float3[100];
            float3* attractors = stackalloc float3[1];
            attractors[0] = target;

            int steps = PotentialField3D.GradientDescent(
                start, attractors, 1, 1.0f,
                null, 0, 0, 0,
                0.1f, 0.01f, 100, path);

            Assert.IsTrue(steps > 0);
            float3 last = path[steps - 1];
            float dist = math.distance(last, target);
            Assert.IsTrue(dist < 2.0f);
        }
    }
}