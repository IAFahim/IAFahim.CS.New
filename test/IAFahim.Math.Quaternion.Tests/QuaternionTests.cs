namespace IAFahim.Math.Quaternion.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Unity.Mathematics;
    using NUnit.Framework;

    public sealed unsafe class QuaternionOpsTests
    {
        [Test]
        public void FromAxisAngle_Identity()
        {
            quaternion q = QuaternionOps.FromAxisAngle(new float3(0, 1, 0), 0.0f);
            Assert.IsTrue(math.all(q.value == quaternion.identity.value));
        }

        [Test]
        public void FromAxisAngle_90Degrees()
        {
            quaternion q = QuaternionOps.FromAxisAngle(new float3(0, 1, 0), math.PI * 0.5f);
            float3 rotated = QuaternionOps.RotateVector(q, new float3(1, 0, 0));
            Assert.IsTrue(math.abs(rotated.x) < 0.01f);
            Assert.IsTrue(math.abs(rotated.z - (-1.0f)) < 0.01f);
        }

        [Test]
        public void Conjugate_IsCorrect()
        {
            quaternion q = new quaternion(1, 2, 3, 4);
            quaternion c = QuaternionOps.Conjugate(q);
            Assert.AreEqual(-1.0f, c.value.x);
            Assert.AreEqual(-2.0f, c.value.y);
            Assert.AreEqual(-3.0f, c.value.z);
            Assert.AreEqual(4.0f, c.value.w);
        }

        [Test]
        public void Dot_SameQuat_ReturnsOne()
        {
            quaternion q = math.normalize(new quaternion(1, 2, 3, 4));
            float d = QuaternionOps.Dot(q, q);
            Assert.IsTrue(math.abs(d - 1.0f) < 1e-6f);
        }

        [Test]
        public void Normalize_UnitQuaternion()
        {
            quaternion q = new quaternion(1, 0, 0, 0);
            quaternion n = QuaternionOps.Normalize(q);
            float len = math.length(n.value);
            Assert.IsTrue(math.abs(len - 1.0f) < 1e-6f);
        }

        [Test]
        public void RotateVector_90DegreesX()
        {
            quaternion q = QuaternionOps.FromAxisAngle(new float3(1, 0, 0), math.PI * 0.5f);
            float3 v = new float3(0, 1, 0);
            float3 result = QuaternionOps.RotateVector(q, v);
            Assert.IsTrue(math.abs(result.y) < 0.01f);
            Assert.IsTrue(math.abs(result.z - 1.0f) < 0.01f);
        }
    }

    public sealed unsafe class QuaternionSlerpTests
    {
        [Test]
        public void Slerp_SameQuat_ReturnsSame()
        {
            quaternion q = new quaternion(0, 0, 0, 1);
            quaternion result = QuaternionSlerp.Run(q, q, 0.5f);
            float d = math.dot(q.value, result.value);
            Assert.IsTrue(math.abs(d - 1.0f) < 1e-6f);
        }

        [Test]
        public void Slerp_Start_ReturnsStart()
        {
            quaternion q0 = quaternion.identity;
            quaternion q1 = new quaternion(0, 1, 0, 0);
            quaternion result = QuaternionSlerp.Run(q0, q1, 0.0f);
            float d = math.dot(q0.value, result.value);
            Assert.IsTrue(math.abs(d - 1.0f) < 1e-6f);
        }

        [Test]
        public void Slerp_End_ReturnsEnd()
        {
            quaternion q0 = quaternion.identity;
            quaternion q1 = new quaternion(0, 1, 0, 0);
            quaternion result = QuaternionSlerp.Run(q0, q1, 1.0f);
            float d = math.dot(q1.value, result.value);
            Assert.IsTrue(math.abs(d - 1.0f) < 1e-6f);
        }

        [Test]
        public void Slerp_Midpoint_IsMidway()
        {
            quaternion q0 = quaternion.identity;
            quaternion q1 = new quaternion(0, 1, 0, 0);
            quaternion result = QuaternionSlerp.Run(q0, q1, 0.5f);
            float angle = QuaternionOps.AngleBetween(q0, result);
            float halfAngle = QuaternionOps.AngleBetween(q0, q1) * 0.5f;
            Assert.IsTrue(math.abs(angle - halfAngle) < 0.01f);
        }
    }

    public sealed unsafe class SwingTwistDecompositionTests
    {
        [Test]
        public void Run_TwistOnly_SplitCorrectly()
        {
            quaternion twist = QuaternionOps.FromAxisAngle(new float3(0, 1, 0), math.PI * 0.5f);
            quaternion q = twist;
            SwingTwistDecomposition.Run(q, new float3(0, 1, 0), out quaternion swing, out quaternion twistOut);
            float swingAngle = math.abs(QuaternionOps.AngleBetween(swing, quaternion.identity));
            float twistAngle = QuaternionOps.AngleBetween(twistOut, quaternion.identity);
            Assert.IsTrue(swingAngle < 0.01f);
            Assert.IsTrue(twistAngle > 1.0f);
        }

        [Test]
        public void FromTwistAngle_RoundTrip()
        {
            float angle = math.PI * 0.75f;
            float3 axis = math.normalize(new float3(1, 1, 1));
            quaternion q = SwingTwistDecomposition.FromTwistAngle(angle, axis);
            float recovered = SwingTwistDecomposition.TwistAngle(q, axis);
            Assert.IsTrue(math.abs(recovered - angle) < 1e-4f);
        }
    }
}