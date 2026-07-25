namespace IAFahim.Geometry.PolygonClip.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class SutherlandHodgmanTests
    {
        [Test]
        public void UnitSquareClip_ExactlyFourVerticesAtCorners()
        {
            double* sx = stackalloc double[4] { 0, 1, 1, 0 };
            double* sy = stackalloc double[4] { 0, 0, 1, 1 };
            double* cx = stackalloc double[4] { 0, 1, 1, 0 };
            double* cy = stackalloc double[4] { 0, 0, 1, 1 };
            double* ox = stackalloc double[16];
            double* oy = stackalloc double[16];
            int k = SutherlandHodgman.Clip(sx, sy, 4, cx, cy, 4, ox, oy, 16);
            Assert.AreEqual(4, k);
            // Each output vertex is one of the unit-square corners
            for (int i = 0; i < k; i++)
            {
                bool corner =
                    (Near(ox[i], 0) || Near(ox[i], 1)) &&
                    (Near(oy[i], 0) || Near(oy[i], 1));
                Assert.IsTrue(corner, $"v{i}=({ox[i]},{oy[i]})");
            }
            double area = 0;
            for (int i = 0; i < k; i++)
            {
                int j = (i + 1) % k;
                area += ox[i] * oy[j] - ox[j] * oy[i];
            }
            area = Math.Abs(area) * 0.5;
            Assert.IsTrue(Math.Abs(area - 1.0) < 1e-9, $"area={area}");
        }

        [Test]
        public void ClipHalf_CutsAreaToHalf()
        {
            // subject unit square, clip x<=0.5 (half-plane as thin tall rectangle)
            double* sx = stackalloc double[4] { 0, 1, 1, 0 };
            double* sy = stackalloc double[4] { 0, 0, 1, 1 };
            double* cx = stackalloc double[4] { 0, 0.5, 0.5, 0 };
            double* cy = stackalloc double[4] { 0, 0, 1, 1 };
            double* ox = stackalloc double[16];
            double* oy = stackalloc double[16];
            int k = SutherlandHodgman.Clip(sx, sy, 4, cx, cy, 4, ox, oy, 16);
            Assert.IsTrue(k >= 4);
            double area = 0;
            for (int i = 0; i < k; i++)
            {
                int j = (i + 1) % k;
                area += ox[i] * oy[j] - ox[j] * oy[i];
            }
            area = Math.Abs(area) * 0.5;
            Assert.IsTrue(Math.Abs(area - 0.5) < 1e-6, $"area={area}");
        }

        private static bool Near(double a, double b) => Math.Abs(a - b) < 1e-9;
    }
}
