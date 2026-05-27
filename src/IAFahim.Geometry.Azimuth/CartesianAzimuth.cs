namespace IAFahim.Geometry.Azimuth
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class CartesianAzimuth
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Run(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            double angle = Math.Atan2(dx, dy);
            if (angle < 0.0)
            {
                angle += 2.0 * Math.PI;
            }
            return angle;
        }
    }
}
