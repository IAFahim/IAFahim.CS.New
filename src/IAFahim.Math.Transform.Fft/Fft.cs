namespace IAFahim.Math.Transform.Fft
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class FftTransform
    {
        public static void Forward(double* re, double* im, int n)
        {
            for (int i = 1, j = 0; i < n; i++)
            {
                int bit = n >> 1;
                while ((j & bit) != 0) { j ^= bit; bit >>= 1; }
                j ^= bit;
                if (i < j)
                {
                    double tr = re[i]; re[i] = re[j]; re[j] = tr;
                    double ti = im[i]; im[i] = im[j]; im[j] = ti;
                }
            }
            for (int len = 2; len <= n; len <<= 1)
            {
                double ang = 2.0 * Math.PI / len;
                double wlenRe = Math.Cos(ang);
                double wlenIm = Math.Sin(ang);
                for (int i = 0; i < n; i += len)
                {
                    double wRe = 1.0, wIm = 0.0;
                    for (int j = 0; j < len / 2; j++)
                    {
                        double uRe = re[i + j];
                        double uIm = im[i + j];
                        double vRe = re[i + j + len / 2] * wRe - im[i + j + len / 2] * wIm;
                        double vIm = re[i + j + len / 2] * wIm + im[i + j + len / 2] * wRe;
                        re[i + j] = uRe + vRe;
                        im[i + j] = uIm + vIm;
                        re[i + j + len / 2] = uRe - vRe;
                        im[i + j + len / 2] = uIm - vIm;
                        double newWRe = wRe * wlenRe - wIm * wlenIm;
                        wIm = wRe * wlenIm + wIm * wlenRe;
                        wRe = newWRe;
                    }
                }
            }
        }

        public static void Inverse(double* re, double* im, int n)
        {
            for (int i = 0; i < n; i++) im[i] = -im[i];
            Forward(re, im, n);
            double invN = 1.0 / n;
            for (int i = 0; i < n; i++)
            {
                re[i] *= invN;
                im[i] = -im[i] * invN;
            }
        }

        [Obsolete("Use Forward(double* re, double* im, int n) overload")]
        public static void Forward(double* a, int n, double* real, double* imag)
        {
            for (int i = 0; i < n; i++) { real[i] = a[i]; imag[i] = 0; }
            Forward(real, imag, n);
            for (int i = 0; i < n; i++) a[i] = real[i];
        }

        [Obsolete("Use Inverse(double* re, double* im, int n) overload")]
        public static void Inverse(double* a, int n, double* real, double* imag)
        {
            for (int i = 0; i < n; i++) { real[i] = a[i]; imag[i] = 0; }
            Inverse(real, imag, n);
            for (int i = 0; i < n; i++) a[i] = real[i];
        }
    }

    public static unsafe class FftConvolution
    {
        public static int Run(double* a, int n, double* b, int m, double* res)
        {
            int size = 1;
            while (size < n + m - 1) size <<= 1;
            double* faRe = stackalloc double[size];
            double* faIm = stackalloc double[size];
            double* fbRe = stackalloc double[size];
            double* fbIm = stackalloc double[size];
            for (int i = 0; i < size; i++) { faRe[i] = 0; faIm[i] = 0; fbRe[i] = 0; fbIm[i] = 0; }
            for (int i = 0; i < n; i++) faRe[i] = a[i];
            for (int i = 0; i < m; i++) fbRe[i] = b[i];
            FftTransform.Forward(faRe, faIm, size);
            FftTransform.Forward(fbRe, fbIm, size);
            for (int i = 0; i < size; i++)
            {
                double re = faRe[i] * fbRe[i] - faIm[i] * fbIm[i];
                double im = faRe[i] * fbIm[i] + faIm[i] * fbRe[i];
                faRe[i] = re;
                faIm[i] = im;
            }
            FftTransform.Inverse(faRe, faIm, size);
            int len = n + m - 1;
            for (int i = 0; i < len; i++) res[i] = faRe[i];
            return len;
        }
    }
}
