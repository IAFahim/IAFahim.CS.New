namespace IAFahim.Geometry.Intersect.Tests
{
    using NUnit.Framework;

    public sealed class PlaneTests
    {
        private const double Eps = 1e-9;

        // Plane z = 5 written as 0*x + 0*y + 1*z - 5 = 0 => n=(0,0,1), d=-5.
        [Test]
        public void PointPlaneDistance_PointAbovePlane_PositiveFive()
        {
            double dist = Plane.PointPlaneDistance(0, 0, 10, 0, 0, 1, -5);
            Assert.AreEqual(5.0, dist, Eps);
        }

        [Test]
        public void PointPlaneDistance_PointBelowPlane_NegativeFive()
        {
            double dist = Plane.PointPlaneDistance(0, 0, 0, 0, 0, 1, -5);
            Assert.AreEqual(-5.0, dist, Eps);
        }

        [Test]
        public void PointPlaneDistance_NonUnitNormal_ScalesCorrectly()
        {
            // Same plane z=5 but normal (0,0,2). |n|=2, so distance = (2*10-5)/2 = 15/2.
            double dist = Plane.PointPlaneDistance(0, 0, 10, 0, 0, 2, -10);
            Assert.AreEqual(5.0, dist, Eps);
        }

        [Test]
        public void PointPlaneDistanceNormalized_UnitNormal_MatchesRaw()
        {
            double dist = Plane.PointPlaneDistanceNormalized(3, 4, 10, 0, 0, 1, -5);
            // Signed = 3*0 + 4*0 + 10*1 - 5 = 5.
            Assert.AreEqual(5.0, dist, Eps);
        }

        [Test]
        public void LinePlaneIntersection_Perpendicular_HitsAtKnownT()
        {
            // Line from origin along +z, plane z=5 (n=(0,0,1), d=-5). t = 5.
            unsafe
            {
                double t;
                bool hit = Plane.LinePlaneIntersection(0, 0, 0, 0, 0, 1, 0, 0, 1, -5, &t);
                Assert.IsTrue(hit);
                Assert.AreEqual(5.0, t, Eps);
            }
        }

        [Test]
        public void LinePlaneIntersection_Parallel_NoHit()
        {
            // Line along +x, plane normal +z => denom=0, parallel, no intersection.
            unsafe
            {
                double t;
                bool hit = Plane.LinePlaneIntersection(0, 0, 0, 1, 0, 0, 0, 0, 1, -5, &t);
                Assert.IsFalse(hit);
            }
        }

        [Test]
        public void SegmentPlaneIntersection_CrossingSegment_HitsAtMidpoint()
        {
            // Segment (0,0,0)-(0,0,10), plane z=5. param = 0.5.
            unsafe
            {
                double t;
                bool hit = Plane.SegmentPlaneIntersection(0, 0, 0, 0, 0, 10, 0, 0, 1, -5, &t);
                Assert.IsTrue(hit);
                Assert.AreEqual(0.5, t, Eps);
            }
        }

        [Test]
        public void SegmentPlaneIntersection_BothEndsSameSide_NoHit()
        {
            // Segment (0,0,6)-(0,0,10), both above plane z=5. param > 1.
            unsafe
            {
                double t;
                bool hit = Plane.SegmentPlaneIntersection(0, 0, 6, 0, 0, 10, 0, 0, 1, -5, &t);
                Assert.IsFalse(hit);
            }
        }

        [Test]
        public unsafe void PlaneIntersection_TwoOrthogonalPlanes_LineAlongY()
        {
            // Plane x=0 (n=(1,0,0),d=0) and z=0 (n=(0,0,1),d=0).
            // Intersection is the y-axis: point (0,0,0), direction (0,1,0).
            double lpx, lpy, lpz, ldx, ldy, ldz;
            bool ok = Plane.PlaneIntersection(1, 0, 0, 0, 0, 0, 1, 0, &lpx, &lpy, &lpz, &ldx, &ldy, &ldz);
            Assert.IsTrue(ok);
            Assert.AreEqual(0.0, lpx, Eps);
            Assert.AreEqual(0.0, lpz, Eps);
            // Direction = cross(n1,n2) = (0*0-0*1, 0*0-1*0, 1*1-0*0) = (0,0,1)... wait.
            // Actually cross((1,0,0),(0,0,1)) = (0*1-0*0, 0*0-1*1, 1*0-0*0) = (0,-1,0).
            // Magnitude 1, direction along ±y.
            double dirLen = System.Math.Sqrt(ldx * ldx + ldy * ldy + ldz * ldz);
            Assert.AreEqual(1.0, dirLen, Eps);
            Assert.AreEqual(0.0, ldx, Eps);
            Assert.AreEqual(0.0, ldz, Eps);
        }
    }
}
