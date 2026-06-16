namespace IAFahim.String.FMIndex
{
    using System.Runtime.CompilerServices;

    public static unsafe class BurrowsWheeler
    {
        public const int DefaultSigma = 256;

        // Builds the BWT from a (sentinel-less, cyclic) suffix array of text and
        // returns the primary index: the row i where sa[i] == 0, i.e. the row that
        // corresponds to the original (un-rotated) string. Inverse must be given this
        // value to reconstruct the original instead of one of its cyclic rotations.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Transform(int* text, int len, int* bwt, int* sa)
        {
            int primary = 0;
            for (int i = 0; i < len; i++)
            {
                int p = sa[i];
                // (p - 1 + len) % len with p in [0, len): equals p - 1 when p >= 1,
                // else len - 1. Branch/select avoids a hardware divide in the hot loop.
                bwt[i] = text[p != 0 ? p - 1 : len - 1];
                if (p == 0) primary = i;
            }
            return primary;
        }

        // Reconstructs the original text from its BWT via LF-mapping. The walk must
        // start at primary (the row Transform returned, where sa==primary-row maps to
        // offset 0); starting at a fixed row 0 would yield a cyclic rotation instead.
        // sigma must be at least max(bwt[i]) + 1 (unchecked Run contract).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Inverse(int* bwt, int len, int primary, int sigma, int* text, int* count, int* LF)
        {
            for (int c = 0; c < sigma; c++) count[c] = 0;
            for (int i = 0; i < len; i++) count[bwt[i]]++;

            int sum = 0;
            for (int c = 0; c < sigma; c++)
            {
                int t = count[c];
                count[c] = sum;
                sum += t;
            }
            for (int i = 0; i < len; i++)
            {
                LF[i] = count[bwt[i]]++;
            }
            int pos = primary;
            for (int i = len - 1; i >= 0; i--)
            {
                text[i] = bwt[pos];
                pos = LF[pos];
            }
        }
    }
}
