namespace IAFahim.Math.Polynomial
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class KaratsubaMultiply
    {
        private const int Threshold = 32;

        public static int Run(int n, long* a, int m, long* b, long* res, long* scratch)
        {
            int outLen = n + m - 1;
            for (int i = 0; i < outLen; i++) res[i] = 0;
            Multiply(a, n, b, m, res, scratch);
            return outLen;
        }

        private static void Multiply(long* a, int n, long* b, int m, long* res, long* tmp)
        {
            if (n == 0 || m == 0) return;
            if (n < Threshold || m < Threshold) { BaseMultiply(a, n, b, m, res); return; }

            int half = Math.Max(n, m) >> 1;
            int n0 = Math.Min(half, n), n1 = n - n0;
            int m0 = Math.Min(half, m), m1 = m - m0;

            long* a0 = a, a1 = a + n0, b0 = b, b1 = b + m0;
            int sALen = Math.Max(n0, n1), sBLen = Math.Max(m0, m1);
            long* sA = tmp, sB = tmp + sALen, midRes = sB + sBLen, innerTmp = midRes + (sALen + sBLen - 1);

            PrepareSums(n0, n1, m0, m1, sALen, sBLen, a0, a1, b0, b1, sA, sB);
            
            int midLen = sALen + sBLen - 1;
            for (int i = 0; i < midLen; i++) midRes[i] = 0;
            Multiply(sA, sALen, sB, sBLen, midRes, innerTmp);
            
            long* loRes = tmp + sALen + sBLen + (sALen + sBLen - 1); // Reuse tmp
            long* hiRes = loRes + (n0 + m0 - 1);
            for (int i = 0; i < (n0 + m0 - 1); i++) loRes[i] = 0;
            for (int i = 0; i < (n1 + m1 - 1); i++) hiRes[i] = 0;

            Multiply(a0, n0, b0, m0, loRes, hiRes + (n1 + m1 - 1));
            Multiply(a1, n1, b1, m1, hiRes, hiRes + (n1 + m1 - 1));

            CombineResults(n0, n1, m0, m1, half, res, loRes, hiRes, midRes);
        }

        private static void BaseMultiply(long* a, int n, long* b, int m, long* res)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++) res[i + j] += a[i] * b[j];
        }

        private static void PrepareSums(int n0, int n1, int m0, int m1, int sALen, int sBLen, long* a0, long* a1, long* b0, long* b1, long* sA, long* sB)
        {
            for (int i = 0; i < sALen; i++) sA[i] = (i < n0 ? a0[i] : 0) + (i < n1 ? a1[i] : 0);
            for (int i = 0; i < sBLen; i++) sB[i] = (i < m0 ? b0[i] : 0) + (i < m1 ? b1[i] : 0);
        }

        private static void CombineResults(int n0, int n1, int m0, int m1, int half, long* res, long* lo, long* hi, long* mid)
        {
            int loLen = n0 + m0 - 1, hiLen = n1 + m1 - 1, midLen = Math.Max(n0, n1) + Math.Max(m0, m1) - 1;
            for (int i = 0; i < loLen; i++) res[i] += lo[i];
            for (int i = 0; i < hiLen; i++) res[i + 2 * half] += hi[i];
            for (int i = 0; i < midLen; i++)
            {
                long sub = (i < loLen ? lo[i] : 0) + (i < hiLen ? hi[i] : 0);
                res[i + half] += mid[i] - sub;
            }
        }
    }
}
