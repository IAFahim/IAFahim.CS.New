namespace IAFahim.Geometry.Intersect.Tests
{
    using NUnit.Framework;

    public sealed class SphereTests
    {
        private const double Eps = 1e-9;

        [Test]
        public unsafe void LineIntersection_ThroughCenter_TwoHits()
        {
            // Sphere at origin radius 5. Line from (-10,0,0) along +x.
            // Hits at x=-5 (t=5) and x=+5 (t=15).
            double t1, t2;
            int n = Sphere.LineIntersection(0, 0, 0, 5, -10, 0, 0, 1, 0, 0, &t1, &t2);
            Assert.AreEqual(2, n);
            Assert.AreEqual(5.0, t1, Eps);
            Assert.AreEqual(15.0, t2, Eps);
        }

        [Test]
        public unsafe void LineIntersection_Tangent_OneHit()
        {
            // Sphere at origin radius 5. Line from (0,5,0) along +x (tangent at top).
            // Touches at (0,5,0): t=0. discriminant = 0.
            double t1, t2;
            int n = Sphere.LineIntersection(0, 0, 0, 5, 0, 5, 0, 1, 0, 0, &t1, &t2);
            Assert.AreEqual(1, n);
            Assert.AreEqual(0.0, t1, Eps);
        }

        [Test]
        public unsafe void LineIntersection_Miss_NoHit()
        {
            // Sphere at origin radius 5. Line from (0,10,0) along +x (misses entirely).
            double t1, t2;
            int n = Sphere.LineIntersection(0, 0, 0, 5, 0, 10, 0, 1, 0, 0, &t1, &t2);
            Assert.AreEqual(0, n);
        }

        [Test]
        public unsafe void LineIntersection_OffCenter_TwoHits()
        {
            // Sphere at origin radius 5. Line from (-10,3,0) along +x.
            // Hits at x = -4 (t=6) and x = +4 (t=14) since 3^2 + 4^2 = 5^2.
            double t1, t2;
            int n = Sphere.LineIntersection(0, 0, 0, 5, -10, 3, 0, 1, 0, 0, &t1, &t2);
            Assert.AreEqual(2, n);
            Assert.AreEqual(6.0, t1, 1e-6);
            Assert.AreEqual(14.0, t2, 1e-6);
        }

        [Test]
        public unsafe void SphereIntersection_Overlapping_CircleAtMidpoint()
        {
            // Two equal spheres radius 5, centers at (-3,0,0) and (3,0,0). Distance=6.
            // Overlap circle at x=0 (midpoint), radius = sqrt(25-9) = 4.
            double cx, cy, cz, radius, nx, ny, nz;
            bool ok = Sphere.SphereIntersection(-3, 0, 0, 5, 3, 0, 0, 5,
                &cx, &cy, &cz, &radius, &nx, &ny, &nz);
            Assert.IsTrue(ok);
            Assert.AreEqual(0.0, cx, Eps);
            Assert.AreEqual(0.0, cy, Eps);
            Assert.AreEqual(0.0, cz, Eps);
            Assert.AreEqual(4.0, radius, 1e-6);
        }

        [Test]
        public unsafe void SphereIntersection_Separated_NoIntersection()
        {
            // Centers at (-10,0,0) and (10,0,0), radius 5 each. Distance 20 > 2*5.
            double cx, cy, cz, radius, nx, ny, nz;
            bool ok = Sphere.SphereIntersection(-10, 0, 0, 5, 10, 0, 0, 5,
                &cx, &cy, &cz, &radius, &nx, &ny, &nz);
            Assert.IsFalse(ok);
        }
    }
}
