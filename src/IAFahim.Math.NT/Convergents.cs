namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Convergents
    {
        public static int Run(long* cf, int cfLen, long* num, long* den)
        {
            num[0] = cf[0];
            den[0] = 1;
            if (cfLen == 1) return 1;
            num[1] = cf[0] * cf[1] + 1;
            den[1] = cf[1];
            for (int i = 2; i < cfLen; i++)
            {
                num[i] = cf[i] * num[i - 1] + num[i - 2];
                den[i] = cf[i] * den[i - 1] + den[i - 2];
            }
            return cfLen;
        }
    }
}