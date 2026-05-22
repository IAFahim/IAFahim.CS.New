namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class AllFactorizations
    {
        public static int Run(long n, long* outBuffer, out int outOffset)
        {
            outOffset = 0;
            if (n <= 1)
            {
                return 0;
            }
            int factorizationCount = 0;
            long* path = stackalloc long[64];
            Generate(n, 2, path, 0, outBuffer, ref outOffset, ref factorizationCount);
            return factorizationCount;
        }

        private static void Generate(
            long currentN,
            long minFactor,
            long* path,
            int pathLen,
            long* outBuffer,
            ref int outOffset,
            ref int factorizationCount)
        {
            path[pathLen] = currentN;
            outBuffer[outOffset] = (long)(pathLen + 1);
            outOffset++;
            for (int i = 0; i <= pathLen; i++)
            {
                outBuffer[outOffset + i] = path[i];
            }
            outOffset += pathLen + 1;
            factorizationCount++;

            for (long d = minFactor; d * d <= currentN; d++)
            {
                if (currentN % d == 0)
                {
                    path[pathLen] = d;
                    Generate(currentN / d, d, path, pathLen + 1, outBuffer, ref outOffset, ref factorizationCount);
                }
            }
        }
    }
}
