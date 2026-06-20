namespace IAFahim.Geometry.Azimuth.Tests
{
    using System;
    using NUnit.Framework;

    // Haversine formula for great-circle distance on a sphere.
    public sealed class SphericalDistanceTests
    {
        private const double EarthRadius = 6371.0; // km
        private const double Eps = 1e-6;

        private static double Deg(double d) => d * Math.PI / 180.0;

        [Test]
        public void SamePoint_Zero()
        {
            double dist = SphericalDistance.Run(Deg(45), Deg(90), Deg(45), Deg(90), EarthRadius);
            Assert.AreEqual(0.0, dist, Eps);
        }

        [Test]
        public void Antipodal_HalfCircumference()
        {
            // Opposite points on the equator: distance = pi * R ≈ 20015.09 km.
            double dist = SphericalDistance.Run(Deg(0), Deg(0), Deg(0), Deg(180), EarthRadius);
            Assert.AreEqual(Math.PI * EarthRadius, dist, 1e-3);
        }

        [Test]
        public void QuarterEquator_QuarterCircumference()
        {
            // Two points on equator 90° apart: distance = R * pi/2.
            double dist = SphericalDistance.Run(Deg(0), Deg(0), Deg(0), Deg(90), EarthRadius);
            Assert.AreEqual(EarthRadius * Math.PI / 2.0, dist, 1e-3);
        }

        [Test]
        public void LondonToParis_Approx343km()
        {
            // London (51.5074°N, 0.1278°W) to Paris (48.8566°N, 2.3522°E) ≈ 343 km.
            double dist = SphericalDistance.Run(Deg(51.5074), Deg(-0.1278), Deg(48.8566), Deg(2.3522), EarthRadius);
            Assert.AreEqual(343.0, dist, 5.0); // ±5 km tolerance
        }

        [Test]
        public void UnitRadius_MatchesAngle()
        {
            // On unit sphere, distance between two points = central angle (radians).
            // North pole to equator = pi/2.
            double dist = SphericalDistance.Run(Deg(90), Deg(0), Deg(0), Deg(0), 1.0);
            Assert.AreEqual(Math.PI / 2.0, dist, Eps);
        }

        [Test]
        public void Symmetric_SameBothDirections()
        {
            double d1 = SphericalDistance.Run(Deg(30), Deg(45), Deg(60), Deg(-30), EarthRadius);
            double d2 = SphericalDistance.Run(Deg(60), Deg(-30), Deg(30), Deg(45), EarthRadius);
            Assert.AreEqual(d1, d2, Eps);
        }
    }
}
