namespace IAFahim.Math.Polynomial.Eval
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MultiPointEval
    {
        public static void Run(int n, long* poly, int m, long* x, long* res, long mod)
        {
            long* tree = stackalloc long[m * 4];
            BuildTree(1, 0, m, x, tree);
            EvalTree(1, 0, m, poly, n, x, res, tree, mod);
        }

        private static void BuildTree(int node, int l, int r, long* x, long* tree)
        {
            if (r - l == 1)
            {
                tree[node] = (mod - x[l]) % mod;
                return;
            }
            int mid = (l + r) >> 1;
            BuildTree(node * 2, l, mid, x, tree);
            BuildTree(node * 2 + 1, mid, r, x, tree);
            long* prod = stackalloc long[2];
            prod[0] = tree[node * 2];
            prod[1] = tree[node * 2 + 1];
            int len = PolynomialMulSingle(1, prod, 1, prod + 1, tree + node);
            tree[node] = prod[0];
        }

        private static int PolynomialMulSingle(int n, long* a, int m, long* b, long* res)
        {
            for (int i = 0; i < n + m - 1; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    res[i + j] = (res[i + j] + a[i] * b[j]) % mod;
            return n + m - 1;
        }

        private static long mod = 1000000007L;

        private static void EvalTree(int node, int l, int r, long* poly, int n, long* x, long* res, long* tree, long m)
        {
            mod = m;
            if (r - l == 1)
            {
                res[l] = EvaluatePoly(poly, n, x[l]);
                return;
            }
            int mid = (l + r) >> 1;
            long* rem = stackalloc long[n];
            int remLen = PolynomialRemainder(n, poly, GetNodeSize(node, tree), tree + node * 4, rem);
            EvalTree(node * 2, l, mid, rem, remLen, x, res, tree, mod);
            EvalTree(node * 2 + 1, mid, r, rem, remLen, x, res, tree, mod);
        }

        private static int GetNodeSize(int node, long* tree)
        {
            int size = 1;
            int n = 1;
            while (n < 4) n <<= 1;
            return n;
        }

        private static int PolynomialRemainder(int n, long* a, int m, long* b, long* r)
        {
            if (n < m)
            {
                for (int i = 0; i < n; i++) r[i] = a[i];
                return n;
            }
            long* q = stackalloc long[n];
            for (int i = 0; i < n; i++) q[i] = 0;
            long* tempA = stackalloc long[n];
            for (int i = 0; i < n; i++) tempA[i] = a[i];
            for (int i = n - 1; i >= m - 1; i--)
            {
                if (tempA[i] == 0) continue;
                long coef = tempA[i] * ModInverse(b[m - 1], mod) % mod;
                q[i - m + 1] = coef;
                for (int j = m - 1; j >= 0; j--)
                    tempA[i - m + 1 + j] = (tempA[i - m + 1 + j] - coef * b[j] + mod) % mod;
            }
            for (int i = 0; i < m - 1; i++) r[i] = tempA[i];
            return m - 1;
        }

        private static long EvaluatePoly(long* poly, int n, long x)
        {
            long res = 0, cur = 1;
            for (int i = 0; i < n; i++)
            {
                res = (res + poly[i] * cur) % mod;
                cur = cur * x % mod;
            }
            return res;
        }

        private static long ModInverse(long a, long m)
        {
            long b = m, u = 1, v = 0;
            while (b != 0)
            {
                long t = a / b;
                a -= t * b; long tmp = a; a = b; b = tmp;
                u -= t * v; tmp = u; u = v; v = tmp;
            }
            return (u % m + m) % m;
        }
    }

    public static unsafe class ChirpZTransform
    {
        public static int Run(int n, long* a, long c, long d, long* res, long mod)
        {
            long* g = stackalloc long[n];
            long* h = stackalloc long[n];
            long pow = 1;
            for (int i = 0; i < n; i++)
            {
                g[i] = a[i] * pow % mod;
                pow = pow * c % mod;
            }
            pow = 1;
            for (int i = 0; i < n; i++)
            {
                h[i] = FastPow(pow, (long)i, mod);
                pow = pow * d % mod;
            }
            long* prod = stackalloc long[2 * n];
            int prodLen = PolynomialMulMod(n, g, n, h, prod, mod);
            for (int i = 0; i < prodLen; i++) res[i] = prod[i] % mod;
            return prodLen;
        }

        private static int PolynomialMulMod(int n, long* a, int m, long* b, long* res, long mod)
        {
            for (int i = 0; i < n + m - 1; i++) res[i] = 0;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    res[i + j] = (res[i + j] + a[i] * b[j]) % mod;
            return n + m - 1;
        }

        private static long FastPow(long a, long e, long mod)
        {
            long res = 1 % mod;
            long b = a % mod;
            while (e > 0)
            {
                if ((e & 1) == 1) res = res * b % mod;
                b = b * b % mod;
                e >>= 1;
            }
            return res;
        }
    }
}