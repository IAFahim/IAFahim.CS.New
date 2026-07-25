namespace IAFahim.Geometry.SweepPrune
{
    using System.Runtime.CompilerServices;

    public static unsafe class SweepAndPrune
    {
        // Axis-aligned box: minX,maxX,minY,maxY per box i.
        // Writes overlapping pairs (i,j) i<j into outA/outB, returns pair count (capped).
        public static int FindOverlaps(
            double* minX, double* maxX, double* minY, double* maxY, int n,
            int* outA, int* outB, int outCap)
        {
            if (n <= 1 || outCap <= 0) return 0;

            int* order = stackalloc int[n];
            for (int i = 0; i < n; i++) order[i] = i;
            // Insertion sort by minX.
            for (int i = 1; i < n; i++)
            {
                int key = order[i];
                double kx = minX[key];
                int j = i - 1;
                while (j >= 0 && minX[order[j]] > kx)
                {
                    order[j + 1] = order[j];
                    j--;
                }
                order[j + 1] = key;
            }

            int count = 0;
            for (int a = 0; a < n; a++)
            {
                int i = order[a];
                for (int b = a + 1; b < n; b++)
                {
                    int j = order[b];
                    if (minX[j] > maxX[i]) break;
                    if (Overlaps1D(minY[i], maxY[i], minY[j], maxY[j]))
                    {
                        if (count < outCap)
                        {
                            outA[count] = i < j ? i : j;
                            outB[count] = i < j ? j : i;
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Overlaps1D(double a0, double a1, double b0, double b1)
            => a0 <= b1 && b0 <= a1;
    }
}
