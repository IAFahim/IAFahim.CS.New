namespace IAFahim.Physics.Xpbd.Tests
{
    using Unity.Mathematics;
    using NUnit.Framework;

    public sealed unsafe class DistanceConstraintTests
    {
        [Test]
        public void Solve_EqualMasses_CorrectsEqually()
        {
            float3 posA = new float3(0, 0, 0);
            float3 posB = new float3(2, 0, 0);
            float3 velA = float3.zero;
            float3 velB = float3.zero;
            float restLen = 1.0f;
            float compliance = 0.0f;
            float dt = 0.016f;

            DistanceConstraint.Solve(&posA, &posB, &velA, &velB, 1.0f, 1.0f, restLen, compliance, dt);

            float dist = math.distance(posA, posB);
            Assert.IsTrue(math.abs(dist - 1.0f) < 0.1f);
        }

        [Test]
        public void Solve_InfiniteMass_DoesNotMove()
        {
            float3 posA = new float3(0, 0, 0);
            float3 posB = new float3(2, 0, 0);
            float3 velA = float3.zero;
            float3 velB = float3.zero;

            DistanceConstraint.Solve(&posA, &posB, &velA, &velB, 0.0f, 1.0f, 1.0f, 0.0f, 0.016f);

            Assert.IsTrue(math.abs(posA.x) < 0.01f);
        }

        [Test]
        public void Solve_AlreadyAtRest_NoChange()
        {
            float3 posA = new float3(0, 0, 0);
            float3 posB = new float3(1, 0, 0);
            float3 velA = float3.zero;
            float3 velB = float3.zero;

            float3 oldA = posA;
            float3 oldB = posB;

            DistanceConstraint.Solve(&posA, &posB, &velA, &velB, 1.0f, 1.0f, 1.0f, 0.0f, 0.016f);

            float changeA = math.distance(posA, oldA);
            float changeB = math.distance(posB, oldB);
            Assert.IsTrue(changeA < 0.01f && changeB < 0.01f);
        }
    }

    public sealed unsafe class VolumeConstraintTests
    {
        [Test]
        public void ComputeRestVolume_Tetrahedron_Correct()
        {
            float3 p0 = new float3(0, 0, 0);
            float3 p1 = new float3(1, 0, 0);
            float3 p2 = new float3(0, 1, 0);
            float3 p3 = new float3(0, 0, 1);

            float volume = VolumeConstraint.ComputeRestVolume(p0, p1, p2, p3);
            Assert.IsTrue(math.abs(volume - (1.0f / 6.0f)) < 0.01f);
        }
    }

    public sealed unsafe class XpbdIntegratorTests
    {
        [Test]
        public void PredictPosition_NoForces_NoChange()
        {
            float3 pos = new float3(1, 2, 3);
            float3 vel = float3.zero;

            XpbdIntegrator.PredictPosition(&pos, &vel, float3.zero, 1.0f, 0.016f);

            Assert.IsTrue(math.all(pos == new float3(1, 2, 3)));
            Assert.IsTrue(math.all(vel == float3.zero));
        }

        [Test]
        public void ApplyDamping_ZeroTime_NoEffect()
        {
            float3 vel = new float3(1, 2, 3);
            XpbdIntegrator.ApplyDamping(&vel, 0.5f, 0.0f);
            Assert.IsTrue(math.all(vel == new float3(1, 2, 3)));
        }

        [Test]
        public void ApplyDamping_WithDamping_ReducesVelocity()
        {
            float3 vel = new float3(1, 0, 0);
            XpbdIntegrator.ApplyDamping(&vel, 10.0f, 0.016f);
            Assert.IsTrue(vel.x < 1.0f);
        }
    }
}