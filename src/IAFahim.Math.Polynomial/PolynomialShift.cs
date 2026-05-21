namespace IAFahim.Math.Polynomial
{
    using System.Runtime.CompilerServices;

    public static unsafe class PolynomialShift
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunLeft(long* a, int n, int k)
        {
            if (k <= 0 || k >= n) return;
            for (int i = 0; i < n - k; i++) a[i] = a[i + k];
            for (int i = n - k; i < n; i++) a[i] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunRight(long* a, int n, int k)
        {
            if (k <= 0 || k >= n) return;
            for (int i = n - 1; i >= k; i--) a[i] = a[i - k];
            for (int i = 0; i < k; i++) a[i] = 0;
        }
    }
}
