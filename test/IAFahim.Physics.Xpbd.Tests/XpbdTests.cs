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

        [Test]
        public void UpdateVelocity_FromPositions_MatchesFiniteDifference()
        {
            float3 oldPos = new float3(0, 0, 0);
            float3 newPos = new float3(1, 2, 3);
            float3 vel = float3.zero;
            float dt = 0.5f;
            XpbdIntegrator.UpdateVelocity(&vel, &oldPos, &newPos, dt);
            Assert.IsTrue(math.distance(vel, new float3(2, 4, 6)) < 1e-5f);
        }

        [Test]
        public void SolveDistanceConstraints_TwoParticles_CorrectsLength()
        {
            float3* positions = stackalloc float3[2];
            float3* velocities = stackalloc float3[2];
            float* invMasses = stackalloc float[2];
            int* a = stackalloc int[1];
            int* b = stackalloc int[1];
            float* rest = stackalloc float[1];
            float* comp = stackalloc float[1];
            positions[0] = new float3(0, 0, 0);
            positions[1] = new float3(3, 0, 0);
            velocities[0] = float3.zero;
            velocities[1] = float3.zero;
            invMasses[0] = 1.0f;
            invMasses[1] = 1.0f;
            a[0] = 0;
            b[0] = 1;
            rest[0] = 1.0f;
            comp[0] = 0.0f;
            XpbdIntegrator.SolveDistanceConstraints(
                positions, velocities, invMasses, a, b, rest, comp, 1, 0.016f);
            float dist = math.distance(positions[0], positions[1]);
            Assert.IsTrue(math.abs(dist - 1.0f) < 0.1f);
        }
    }

    public sealed unsafe class CollisionConstraintTests
    {
        [Test]
        public void SolvePlane_Penetrating_PushesOutAndReflects()
        {
            float3 pos = new float3(0, -0.5f, 0);
            float3 vel = new float3(0, -1, 0);
            float3 n = new float3(0, 1, 0);
            CollisionConstraint.SolvePlane(&pos, &vel, 1.0f, n, 0.0f, 1.0f, 0.0f, 0.016f);
            Assert.IsTrue(pos.y >= -1e-5f, "position corrected above plane");
            Assert.IsTrue(vel.y > 0.0f, "approach velocity reflected");
        }

        [Test]
        public void SolveSphere_OverlappingApproach_SeparatesAndUsesRestitution()
        {
            float3 posA = new float3(0, 0, 0);
            float3 posB = new float3(1.0f, 0, 0);
            float3 velA = new float3(1, 0, 0);
            float3 velB = new float3(-1, 0, 0);
            float radius = 1.0f;
            CollisionConstraint.SolveSphere(
                &posA, &posB, &velA, &velB,
                1.0f, 1.0f, radius, radius,
                1.0f, 0.0f);

            float dist = math.distance(posA, posB);
            Assert.IsTrue(dist >= 2.0f - 1e-4f, "spheres separated to non-overlap");

            float3 rel = velB - velA;
            float3 n = math.normalize(posB - posA);
            float vn = math.dot(rel, n);
            Assert.IsTrue(vn > 0.0f, "relative velocity becomes separating after bounce");
            Assert.IsTrue(math.abs(velA.x + 1.0f) < 1e-3f || math.abs(velB.x - 1.0f) < 1e-3f || vn > 0.5f,
                "restitution impulse flips approach along normal");
        }

        [Test]
        public void SolveSphere_Separating_NoVelocityChange()
        {
            float3 posA = new float3(0, 0, 0);
            float3 posB = new float3(1.5f, 0, 0);
            float3 velA = new float3(-1, 0, 0);
            float3 velB = new float3(1, 0, 0);
            float3 oldA = velA;
            float3 oldB = velB;
            CollisionConstraint.SolveSphere(
                &posA, &posB, &velA, &velB,
                1.0f, 1.0f, 1.0f, 1.0f,
                1.0f, 0.0f);
            Assert.IsTrue(math.distance(velA, oldA) < 1e-5f);
            Assert.IsTrue(math.distance(velB, oldB) < 1e-5f);
        }
    }

    public sealed unsafe class ShapeMatchingConstraintTests
    {
        [Test]
        public void Solve_RigidTranslation_NoDeltas()
        {
            const int N = 4;
            float3* positions = stackalloc float3[N];
            float3* rest = stackalloc float3[N];
            float* inv = stackalloc float[N];
            float3* deltas = stackalloc float3[N];
            rest[0] = new float3(0, 0, 0);
            rest[1] = new float3(1, 0, 0);
            rest[2] = new float3(0, 1, 0);
            rest[3] = new float3(0, 0, 1);
            float3 shift = new float3(2, 3, 4);
            for (int i = 0; i < N; i++)
            {
                positions[i] = rest[i] + shift;
                inv[i] = 1.0f;
                deltas[i] = float3.zero;
            }
            ShapeMatchingConstraint.Solve(positions, rest, inv, N, 0.0f, 0.016f, deltas);
            for (int i = 0; i < N; i++)
                Assert.IsTrue(math.length(deltas[i]) < 0.05f, $"particle {i}");
        }
    }
}