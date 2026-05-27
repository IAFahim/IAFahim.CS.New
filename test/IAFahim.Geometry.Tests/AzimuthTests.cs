namespace IAFahim.Geometry.Tests
{
    using System;
    using IAFahim.Geometry.Azimuth;
    using NUnit.Framework;

    public sealed unsafe class AzimuthTests
    {
        [Test]
        public void CartesianAzimuth_CardinalDirections()
        {
            // North: (0,0) -> (0,1) => 0
            Assert.AreEqual(0.0, CartesianAzimuth.Run(0.0, 0.0, 0.0, 1.0), 1e-9);

            // East: (0,0) -> (1,0) => pi/2
            Assert.AreEqual(Math.PI * 0.5, CartesianAzimuth.Run(0.0, 0.0, 1.0, 0.0), 1e-9);

            // South: (0,0) -> (0,-1) => pi
            Assert.AreEqual(Math.PI, CartesianAzimuth.Run(0.0, 0.0, 0.0, -1.0), 1e-9);

            // West: (0,0) -> (-1,0) => 3*pi/2
            Assert.AreEqual(Math.PI * 1.5, CartesianAzimuth.Run(0.0, 0.0, -1.0, 0.0), 1e-9);

            // North-East: (0,0) -> (1,1) => pi/4
            Assert.AreEqual(Math.PI * 0.25, CartesianAzimuth.Run(0.0, 0.0, 1.0, 1.0), 1e-9);
        }

        [Test]
        public void SphericalAzimuth_CardinalDirections()
        {
            // North along Prime Meridian: (0,0) -> (pi/4, 0) => 0
            Assert.AreEqual(0.0, SphericalAzimuth.Run(0.0, 0.0, Math.PI * 0.25, 0.0), 1e-9);

            // East along Equator: (0,0) -> (0, pi/4) => pi/2
            Assert.AreEqual(Math.PI * 0.5, SphericalAzimuth.Run(0.0, 0.0, 0.0, Math.PI * 0.25), 1e-9);
        }

        [Test]
        public void SphericalDistance_KnownDistances()
        {
            double r = 6371000.0; // Earth radius in meters

            // Quarter of circumference along equator
            double expectedDist = r * (Math.PI * 0.5);
            Assert.AreEqual(expectedDist, SphericalDistance.Run(0.0, 0.0, 0.0, Math.PI * 0.5, r), 1e-9);

            // Quarter of circumference along meridian
            Assert.AreEqual(expectedDist, SphericalDistance.Run(0.0, 0.0, Math.PI * 0.5, 0.0, r), 1e-9);

            // Same point distance should be zero
            Assert.AreEqual(0.0, SphericalDistance.Run(0.123, 0.456, 0.123, 0.456, r), 1e-9);
        }
    }
}
