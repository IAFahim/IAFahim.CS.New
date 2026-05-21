namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ConvexHullTrick
    {
        public struct Line { public long M, B; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Eval(Line l, long x) { return l.M * x + l.B; }

        public static int Add(Line* hull, int* size, Line newLine)
        {
            while (*size >= 2)
            {
                int n = *size - 1;
                long m1 = hull[n - 1].M, b1 = hull[n - 1].B;
                long m2 = hull[n].M, b2 = hull[n].B;
                long m3 = newLine.M, b3 = newLine.B;
                if ((b3 - b1) * (m1 - m2) <= (b2 - b1) * (m1 - m3)) (*size)--;
                else break;
            }
            hull[(*size)++] = newLine;
            return *size;
        }

        public static long Query(Line* hull, int size, long x)
        {
            int lo = 0, hi = size - 1;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                long v1 = Eval(hull[mid], x);
                long v2 = Eval(hull[mid + 1], x);
                if (v1 <= v2) hi = mid;
                else lo = mid + 1;
            }
            return Eval(hull[lo], x);
        }
    }
}
