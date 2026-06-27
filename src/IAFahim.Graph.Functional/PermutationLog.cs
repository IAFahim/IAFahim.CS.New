namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class PermutationLog
    {
        // Smallest k>=0 with p1^k == p2 (both permutations of 0..n-1), else -1.
        //
        // p1^k rotates each cycle of p1 by (k mod L). So p1^k == p2 requires that p2,
        // restricted to every cycle of p1, is a uniform rotation of that same cycle by a
        // fixed offset d. That yields one congruence k ≡ d (mod L) per cycle; the answer
        // is the smallest non-negative simultaneous solution (CRT over possibly non-coprime
        // moduli). p2 leaving a cycle, a non-uniform rotation, or an unsolvable CRT => -1.
        //
        // Scratch: one n-sized 'seen' marker (cannot clobber the const inputs and there is no
        // output buffer). Everything else is O(1) two-pointer walking — no per-node position
        // table is needed because a pointer kept exactly d steps ahead validates the rotation.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int* p1, int* p2, int n)
        {
            if (n <= 0) return 0;

            byte* seen = (byte*)Marshal.AllocHGlobal(n);
            try
            {
                for (int i = 0; i < n; i++) seen[i] = 0;

                // Running CRT solution: k ≡ rem (mod mod). Empty constraint to start.
                long rem = 0;
                long mod = 1;

                for (int start = 0; start < n; start++)
                {
                    if (seen[start] != 0) continue; // cycle already processed

                    // 1) Measure cycle length L and find d = steps from 'start' to p2[start]
                    //    while marking the cycle as seen. If p2[start] never appears on the
                    //    walk it lies on a different cycle => no solution.
                    int t = p2[start];
                    long len = 0;
                    int d = -1;
                    int node = start;
                    do
                    {
                        seen[node] = 1;
                        if (node == t) d = (int)len;
                        node = p1[node];
                        len++;
                    } while (node != start);

                    if (d < 0) return -1; // p2 maps 'start' off its p1-cycle

                    // 2) Verify the rotation is uniform: a second pointer 'ahead', kept exactly
                    //    d steps in front of 'node', must equal p2[node] for every member.
                    int ahead = start;
                    for (int s = 0; s < d; s++) ahead = p1[ahead];
                    node = start;
                    for (long s = 0; s < len; s++)
                    {
                        if (p2[node] != ahead) return -1; // non-uniform rotation
                        node = p1[node];
                        ahead = p1[ahead];
                    }

                    // 3) Fold k ≡ d (mod len) into the running CRT solution.
                    if (!CrtMerge(ref rem, ref mod, d, len)) return -1;
                }

                return rem;
            }
            finally
            {
                Marshal.FreeHGlobal(new System.IntPtr((void*)seen));
            }
        }

        // Merge k ≡ rem (mod mod) with k ≡ r2 (mod m2). Returns false if inconsistent;
        // on success updates (rem, mod) to the combined congruence (mod := lcm(mod, m2)).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CrtMerge(ref long rem, ref long mod, long r2, long m2)
        {
            long g = Gcd(mod, m2);
            long diff = r2 - rem;
            if (diff % g != 0) return false; // no common solution

            // lcm(mod, m2) = (mod/g) * m2. The combined period can exceed long.MaxValue for
            // permutations with many coprime cycle lengths; the 64-bit modular arithmetic below
            // (and the result itself) is then unrepresentable. Detect the overflow and report no
            // solution rather than silently returning a corrupted value. (Caller maps false -> -1;
            // a smaller representable k may exist in theory but cannot be computed without bignum.)
            long mg = mod / g;
            if (m2 != 0 && mg > long.MaxValue / m2) return false;
            long lcm = mg * m2;
            // Solve rem + mod*t ≡ r2 (mod m2)  =>  t ≡ (diff/g) * inv(mod/g) (mod m2/g).
            long m2g = m2 / g;
            long inv = ModInverse((mod / g) % m2g, m2g);
            long t = Mul((((diff / g) % m2g) + m2g) % m2g, inv, m2g);
            long result = (rem + Mul(mod % lcm, t, lcm)) % lcm;
            if (result < 0) result += lcm;
            rem = result;
            mod = lcm;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Gcd(long a, long b)
        {
            while (b != 0)
            {
                long tmp = a % b;
                a = b;
                b = tmp;
            }
            return a < 0 ? -a : a;
        }

        // (a*b) mod m via add-and-double, avoiding 64-bit overflow when m is a large lcm.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Mul(long a, long b, long m)
        {
            if (m == 1) return 0;
            a %= m; if (a < 0) a += m;
            b %= m; if (b < 0) b += m;
            long result = 0;
            while (b > 0)
            {
                if ((b & 1) != 0)
                {
                    result += a;
                    if (result >= m) result -= m;
                }
                a += a;
                if (a >= m) a -= m;
                b >>= 1;
            }
            return result;
        }

        // Modular inverse of a mod m via extended Euclid; m == 1 yields 0.
        // Caller guarantees gcd(a, m) == 1 (CrtMerge reduces by the gcd first).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long ModInverse(long a, long m)
        {
            if (m == 1) return 0;
            long g = m, x = 0, x1 = 1;
            long r = a % m;
            if (r < 0) r += m;
            while (r != 0)
            {
                long q = g / r;
                long tmp = g - q * r; g = r; r = tmp;
                tmp = x - q * x1; x = x1; x1 = tmp;
            }
            x %= m;
            if (x < 0) x += m;
            return x;
        }
    }
}
