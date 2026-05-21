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
            if (n < Threshold || m < Threshold)
            {
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < m; j++)
                        res[i + j] += a[i] * b[j];
                return;
            }

            int half = (n > m ? n : m) >> 1;

            int n0 = half < n ? half : n;
            int n1 = n - n0;
            int m0 = half < m ? half : m;
            int m1 = m - m0;

            long* a1 = a + n0;
            long* b1 = b + m0;

            int sumALen = n0 > n1 ? n0 : n1;
            int sumBLen = m0 > m1 ? m0 : m1;

            long* sumA = tmp;
            long* sumB = tmp + sumALen;
            long* midRes = sumB + sumBLen;
            long* innerTmp = midRes + (sumALen + sumBLen - 1);

            for (int i = 0; i < sumALen; i++) sumA[i] = 0;
            for (int i = 0; i < sumBLen; i++) sumB[i] = 0;

            for (int i = 0; i < n0; i++) sumA[i] += a[i];
            for (int i = 0; i < n1; i++) sumA[i] += a1[i];
            for (int i = 0; i < m0; i++) sumB[i] += b[i];
            for (int i = 0; i < m1; i++) sumB[i] += b1[i];

            int midLen = sumALen + sumBLen - 1;
            for (int i = 0; i < midLen; i++) midRes[i] = 0;
            Multiply(sumA, sumALen, sumB, sumBLen, midRes, innerTmp);

            int lo = n0 + m0 - 1;
            int hi = n1 + m1 - 1;
            long* loRes = res;
            long* hiRes = res + 2 * half;

            long* loTmp = innerTmp;
            long* hiTmp = loTmp + lo;
            for (int i = 0; i < lo; i++) loTmp[i] = 0;
            for (int i = 0; i < hi; i++) hiTmp[i] = 0;

            Multiply(a, n0, b, m0, loTmp, hiTmp + hi);
            Multiply(a1, n1, b1, m1, hiTmp, hiTmp + hi);

            for (int i = 0; i < lo; i++) loRes[i] += loTmp[i];
            for (int i = 0; i < hi; i++) hiRes[i] += hiTmp[i];
            for (int i = 0; i < midLen; i++)
                res[i + half] += midRes[i] - loTmp[i < lo ? i : 0] * (i < lo ? 1 : 0)
                                            - hiTmp[i < hi ? i : 0] * (i < hi ? 1 : 0);

            for (int i = 0; i < midLen; i++)
            {
                long sub = 0;
                if (i < lo) sub += loTmp[i];
                if (i < hi) sub += hiTmp[i];
                res[i + half] += midRes[i] - sub;
            }
        }
    }
}
