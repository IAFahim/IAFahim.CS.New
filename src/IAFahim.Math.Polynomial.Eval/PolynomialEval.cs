namespace IAFahim.Math.Polynomial.Eval
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MultiPointEval
    {
        public static void Run(int n, long* poly, int m, long* x, long* res, long mod)
        {
            long* pn = stackalloc long[n];
            for (int j = 0; j < n; j++) pn[j] = (poly[j] % mod + mod) % mod;
            for (int i = 0; i < m; i++) res[i] = EvaluateAt(n, pn, x[i], mod);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long EvaluateAt(int n, long* poly, long x, long mod)
        {
            long val = 0, xi = (x % mod + mod) % mod;
            for (int j = n - 1; j >= 0; j--)
            {
                val = (val * xi + poly[j]) % mod;
            }
            return val;
        }
    }

    public static unsafe class ChirpZTransform
    {
        // Bluestein chirp-Z: res[k] = sum_i a[i] * c^i * d^(i*k) (mod), for k in [0, n).
        // Uses the identity i*k = C(i+k,2) - C(i,2) - C(k,2) with C(x,2) = x(x-1)/2, so
        //   res[k] = invD^C(k,2) * sum_i (a[i] * c^i * invD^C(i,2)) * d^C(i+k,2).
        // The sum_i g[i]*d^C(i+k,2) is a correlation; mapping it onto a (convolution)
        // NaiveConvolve requires reversing g into G[i] = g[n-1-i]. Then
        //   prod[n-1+k] = sum_i G[i] * H[n-1+k-i] = sum_p g[p] * H[p+k]   (H[j] = d^C(j,2)).
        public static int Run(int n, long* a, long c, long d, long* res, long mod)
        {
            int convLen = 2 * n - 1;
            long* gRev = stackalloc long[convLen];
            long* h = stackalloc long[convLen];
            long* invDTri = stackalloc long[n]; // invD^C(k,2), reused for the post-multiply
            for (int i = 0; i < convLen; i++) { gRev[i] = 0; h[i] = 0; }

            long invD = FastPow(d, mod - 2, mod);

            // Build g[i] = a[i] * c^i * invD^C(i,2) using incremental running powers, and
            // store it reversed into gRev so the convolution realises the correlation.
            // C(i+1,2) - C(i,2) = i, so invD^C(i,2) advances by multiplying invD^i each step.
            long cPow = 1;        // c^i
            long invDPow = 1;     // invD^C(i,2)
            long invDStep = 1;    // invD^i (multiplier to advance invDPow)
            for (int i = 0; i < n; i++)
            {
                invDTri[i] = invDPow;
                long g = a[i] % mod * cPow % mod * invDPow % mod;
                gRev[n - 1 - i] = g;
                cPow = cPow * c % mod;
                invDPow = invDPow * invDStep % mod;
                invDStep = invDStep * invD % mod;
            }

            // Build kernel H[j] = d^C(j,2) for j in [0, convLen).
            // C(j+1,2) - C(j,2) = j, so d^C(j,2) advances by multiplying d^j each step.
            long dPow = 1;        // d^C(j,2)
            long dStep = 1;       // d^j
            for (int j = 0; j < convLen; j++)
            {
                h[j] = dPow;
                dPow = dPow * dStep % mod;
                dStep = dStep * d % mod;
            }

            long* prod = stackalloc long[convLen];
            NaiveConvolve(gRev, n, h, convLen, prod, convLen, mod);

            for (int k = 0; k < n; k++)
                res[k] = prod[n - 1 + k] * invDTri[k] % mod;
            return n;
        }

        private static void NaiveConvolve(long* a, int an, long* b, int bn, long* res, int resLen, long mod)
        {
            for (int i = 0; i < resLen; i++) res[i] = 0;
            for (int i = 0; i < an; i++)
            {
                int maxJ = resLen - i;
                if (maxJ > bn) maxJ = bn;
                for (int j = 0; j < maxJ; j++)
                    res[i + j] = (res[i + j] + a[i] * b[j]) % mod;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long FastPow(long a, long e, long mod)
        {
            long res = 1 % mod, b = a % mod;
            while (e > 0) { if ((e & 1) == 1) res = res * b % mod; b = b * b % mod; e >>= 1; }
            return res;
        }
    }
}
