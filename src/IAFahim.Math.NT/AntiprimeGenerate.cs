namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class AntiprimeGenerate
    {
        public static void Run(int k, long* result)
        {
            if (k <= 0)
            {
                return;
            }
            long limit = 20000000000000000L;
            long* temp = stackalloc long[500];
            int count = HighlyCompositeNumbers.Run(limit, temp);
            int writeCount = k < count ? k : count;
            for (int i = 0; i < writeCount; i++)
            {
                result[i] = temp[i];
            }
        }
    }
}
