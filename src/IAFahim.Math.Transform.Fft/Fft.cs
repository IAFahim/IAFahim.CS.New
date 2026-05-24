namespace IAFahim.Math.Transform.Fft
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FftTransform
    {
        public static void Forward(double* re, double* im, int n)
        {
            BitReverse(re, im, n);
            for (int len = 2; len <= n; len <<= 1)
            {
                double ang = 2.0 * Math.PI / len;
                double wlenRe = Math.Cos(ang), wlenIm = Math.Sin(ang);
                for (int i = 0; i < n; i += len)
                    PerformButterflyStep(re, im, i, len / 2, wlenRe, wlenIm);
            }
        }

        private static void PerformButterflyStep(double* re, double* im, int i, int half, double wlenRe, double wlenIm)
        {
            double wRe = 1.0, wIm = 0.0;
            for (int j = 0; j < half; j++)
            {
                int idx1 = i + j, idx2 = i + j + half;
                double uRe = re[idx1], uIm = im[idx1];
                double vRe = re[idx2] * wRe - im[idx2] * wIm, vIm = re[idx2] * wIm + im[idx2] * wRe;
                re[idx1] = uRe + vRe; im[idx1] = uIm + vIm;
                re[idx2] = uRe - vRe; im[idx2] = uIm - vIm;
                double nextWRe = wRe * wlenRe - wIm * wlenIm;
                wIm = wRe * wlenIm + wIm * wlenRe; wRe = nextWRe;
            }
        }

        public static void Inverse(double* re, double* im, int n)
        {
            for (int i = 0; i < n; i++) im[i] = -im[i];
            Forward(re, im, n);
            double invN = 1.0 / n;
            for (int i = 0; i < n; i++) { re[i] *= invN; im[i] = -im[i] * invN; }
        }

        private static void BitReverse(double* re, double* im, int n)
        {
            for (int i = 1, j = 0; i < n; i++)
            {
                int bit = n >> 1;
                while ((j & bit) != 0) { j ^= bit; bit >>= 1; }
                j ^= bit;
                if (i < j) { Swap(ref re[i], ref re[j]); Swap(ref im[i], ref im[j]); }
            }
        }

        private static void Swap(ref double a, ref double b) { double t = a; a = b; b = t; }
    }

    public static unsafe class FftConvolution
    {
        public static int Run(double* a, int n, double* b, int m, double* res)
        {
            int size = 1; while (size < n + m - 1) size <<= 1;
            double* faRe = stackalloc double[size], faIm = stackalloc double[size];
            double* fbRe = stackalloc double[size], fbIm = stackalloc double[size];
            for (int i = 0; i < size; i++) { faRe[i] = i < n ? a[i] : 0; faIm[i] = 0; fbRe[i] = i < m ? b[i] : 0; fbIm[i] = 0; }
            FftTransform.Forward(faRe, faIm, size); FftTransform.Forward(fbRe, fbIm, size);
            for (int i = 0; i < size; i++)
            {
                double re = faRe[i] * fbRe[i] - faIm[i] * fbIm[i], im = faRe[i] * fbIm[i] + faIm[i] * fbRe[i];
                faRe[i] = re; faIm[i] = im;
            }
            FftTransform.Inverse(faRe, faIm, size);
            for (int i = 0; i < n + m - 1; i++) res[i] = faRe[i];
            return n + m - 1;
        }
    }
}
