namespace IAFahim.Geometry.Azimuth
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SphericalAzimuth
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Run(double lat1, double lon1, double lat2, double lon2)
        {
            double dLon = lon2 - lon1;
            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double angle = Math.Atan2(y, x);
            if (angle < 0.0)
            {
                angle += 2.0 * Math.PI;
            }
            return angle;
        }
    }
}
