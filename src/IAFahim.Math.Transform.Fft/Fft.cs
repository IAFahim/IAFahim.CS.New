namespace IAFahim.Math.Transform.Fft
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FftTransform
    {
        public static void Forward(double* a, int n, double* real, double* imag)
        {
            for (int i = 0, j = 0; i < n; i++)
            {
                if (i < j) { double tmp = a[i]; a[i] = a[j]; a[j] = tmp; }
                for (int k = n >> 1; (j ^= k) < k; k >>= 1) ;
            }
            for (int len = 2; len <= n; len <<= 1)
            {
                double ang = 2 * Math.PI / len;
                double wlenRe = Math.Cos(ang);
                double wlenIm = Math.Sin(ang);
                for (int i = 0; i < n; i += len)
                {
                    double wRe = 1, wIm = 0;
                    for (int j = 0; j < len / 2; j++)
                    {
                        double uRe = a[i + j];
                        double uIm = 0;
                        double vRe = a[i + j + len / 2] * wRe - uIm * wIm;
                        double vIm = a[i + j + len / 2] * wIm + uIm * wRe;
                        a[i + j] = uRe + vRe;
                        a[i + j + len / 2] = uRe - vRe;
                        double newWRe = wRe * wlenRe - wIm * wlenIm;
                        wIm = wRe * wlenIm + wIm * wlenRe;
                        wRe = newWRe;
                    }
                }
            }
            for (int i = 0; i < n; i++)
            {
                real[i] = a[i];
                imag[i] = 0;
            }
        }

        public static void Inverse(double* a, int n, double* real, double* imag)
        {
            for (int i = 0, j = 0; i < n; i++)
            {
                if (i < j) { double tmp = a[i]; a[i] = a[j]; a[j] = tmp; }
                for (int k = n >> 1; (j ^= k) < k; k >>= 1) ;
            }
            for (int len = 2; len <= n; len <<= 1)
            {
                double ang = -2 * Math.PI / len;
                double wlenRe = Math.Cos(ang);
                double wlenIm = Math.Sin(ang);
                for (int i = 0; i < n; i += len)
                {
                    double wRe = 1, wIm = 0;
                    for (int j = 0; j < len / 2; j++)
                    {
                        double uRe = a[i + j];
                        double uIm = 0;
                        double vRe = a[i + j + len / 2] * wRe - uIm * wIm;
                        double vIm = a[i + j + len / 2] * wIm + uIm * wRe;
                        a[i + j] = uRe + vRe;
                        a[i + j + len / 2] = uRe - vRe;
                        double newWRe = wRe * wlenRe - wIm * wlenIm;
                        wIm = wRe * wlenIm + wIm * wlenRe;
                        wRe = newWRe;
                    }
                }
            }
            double invN = 1.0 / n;
            for (int i = 0; i < n; i++) a[i] *= invN;
            for (int i = 0; i < n; i++)
            {
                real[i] = a[i];
                imag[i] = 0;
            }
        }
    }

    public static unsafe class FftConvolution
    {
        public static int Run(double* a, int n, double* b, int m, double* res)
        {
            int size = 1;
            while (size < n + m - 1) size <<= 1;
            double* fa = stackalloc double[size];
            double* fb = stackalloc double[size];
            double* tempReal = stackalloc double[size];
            for (int i = 0; i < size; i++) { fa[i] = 0; fb[i] = 0; }
            for (int i = 0; i < n; i++) fa[i] = a[i];
            for (int i = 0; i < m; i++) fb[i] = b[i];
            FftTransform.Forward(fa, size, fa, tempReal);
            FftTransform.Forward(fb, size, fb, tempReal);
            for (int i = 0; i < size; i++)
            {
                double re = fa[i] * fb[i] - 0 * fb[i];
                double im = 0;
                fa[i] = re;
                fb[i] = im;
            }
            FftTransform.Inverse(fa, size, fa, tempReal);
            for (int i = 0; i < n + m - 1; i++) res[i] = fa[i];
            return n + m - 1;
        }
    }
}
