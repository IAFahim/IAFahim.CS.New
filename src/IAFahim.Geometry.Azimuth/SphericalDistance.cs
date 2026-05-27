namespace IAFahim.Geometry.Azimuth
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SphericalDistance
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Run(double lat1, double lon1, double lat2, double lon2, double radius)
        {
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;
            double sinLat = Math.Sin(dLat * 0.5);
            double sinLon = Math.Sin(dLon * 0.5);
            double a = sinLat * sinLat + Math.Cos(lat1) * Math.Cos(lat2) * sinLon * sinLon;
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
            return radius * c;
        }
    }
}
