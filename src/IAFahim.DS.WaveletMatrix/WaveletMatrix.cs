namespace IAFahim.DS.WaveletMatrix
{
    public static unsafe class WaveletMatrixBuild
    {
        // Builds an MSB-first wavelet matrix over data[0..n) so that WaveletMatrixKth.Run
        // returns the numeric k-th smallest. Level b (b = 0..log-1) processes bit
        // (log - 1 - b): level 0 is the most-significant bit, level log-1 the least.
        //
        // Layout produced (level-major):
        //   ranks[b*(n+1) + i] = number of set bits among the first i elements of level b's
        //                        (stably reordered) array, for i in [0, n], where the bit
        //                        inspected at level b is bit (log-1-b).
        //                        ranks[b*(n+1)] == 0, ranks[b*(n+1)+n] == ones at level b.
        //   mids[b]            = number of ZEROS at level b == start index of the ones
        //                        bucket in the zeros-first stable partition of level b.
        //   bitmaps[0]         = n, stored as query metadata so WaveletMatrixKth.Run can
        //                        recover the per-level rank stride (n+1) without an extra
        //                        parameter. The rest of bitmaps is transient build scratch.
        //
        // data is NOT mutated. Caller guarantees: non-null buffers; n >= 0; log >= 1;
        // bitmaps and ranks each sized >= log*(n+1) ints; mids sized >= log ints.
        public static int Run(int* data, int n, int maxVal, int* bitmaps, int* ranks, int* mids, int log)
        {
            BuildLevels(data, n, bitmaps, ranks, mids, log);
            bitmaps[0] = n;
            return log;
        }

        // Builds the level tables using the (dead-after-build) bitmaps buffer as ping-pong
        // partition scratch so that data is never mutated. For log == 1 only one level runs
        // and no second buffer is needed.
        private static void BuildLevels(int* data, int n, int* bitmaps, int* ranks, int* mids, int log)
        {
            int* cur = bitmaps;
            int* next = bitmaps + (n + 1);

            for (int i = 0; i < n; i++) cur[i] = data[i];

            int last = log - 1;
            for (int b = 0; b < log; b++)
            {
                int offset = b * (n + 1);
                int bit = last - b;
                // The reordering is only consumed by the next level; on the final level
                // pass null so no second buffer is touched (keeps log == 1 in bounds).
                int* scratch = b < last ? next : null;
                mids[b] = BuildLevel(cur, n, bit, ranks + offset, scratch);

                if (b < last)
                {
                    int* tmp = cur;
                    cur = next;
                    next = tmp;
                }
            }
        }

        // MSB-first variant of Run that treats data itself as the level-0 working buffer
        // (data IS mutated, ending as the final-level reordering). bitmapPtr is a single
        // n-wide scratch buffer. Same table layout as Run except bitmaps[0] is not written.
        public static void RunFrom(int* data, int n, int* mids, int* bitmapPtr, int* rankPtr, int log)
        {
            int* cur = data;
            int* next = bitmapPtr;
            int last = log - 1;
            for (int b = 0; b < log; b++)
            {
                int offset = b * (n + 1);
                int bit = last - b;
                int* scratch = b < last ? next : null;
                mids[b] = BuildLevel(cur, n, bit, rankPtr + offset, scratch);
                if (b < last)
                    for (int i = 0; i < n; i++) cur[i] = next[i];
            }
        }

        // Builds one level for the working array data[0..n) inspecting the given absolute
        // bit index:
        //   levelRank[i] = count of set 'bit' among data[0..i)  (levelRank[0]=0, levelRank[n]=ones)
        //   scratch[]    = stable partition of data: all bit==0 (in order) then all bit==1
        //                  (in order). When scratch == null the partition is skipped (used
        //                  for the final level, whose reordering is never consumed).
        // Returns the number of zeros (== n - ones == start of the ones bucket in scratch).
        private static int BuildLevel(int* data, int n, int bit, int* levelRank, int* scratch)
        {
            int ones = 0;
            for (int i = 0; i < n; i++) ones += (data[i] >> bit) & 1;
            int zeros = n - ones;

            levelRank[0] = 0;
            if (scratch == null)
            {
                int accOnly = 0;
                for (int i = 0; i < n; i++)
                {
                    accOnly += (data[i] >> bit) & 1;
                    levelRank[i + 1] = accOnly;
                }
                return zeros;
            }

            int acc = 0;        // running count of ones over [0, i)
            int onePos = zeros; // next free slot in the ones bucket
            for (int i = 0; i < n; i++)
            {
                int v = data[i];
                int isOne = (v >> bit) & 1;
                if (isOne == 0)
                    scratch[i - acc] = v; // zeros bucket: i - (ones seen) is its stable rank
                else
                {
                    scratch[onePos] = v;
                    onePos++;
                }
                acc += isOne;
                levelRank[i + 1] = acc;
            }
            return zeros;
        }
    }

    public static unsafe class WaveletMatrixKth
    {
        // Returns the (k-th smallest, 0-based) value among data positions [l, r] (inclusive),
        // using the tables produced by WaveletMatrixBuild.
        // Caller guarantees 0 <= l <= r < n and 0 <= k <= r - l.
        public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int l, int r, int k, int log)
        {
            int stride = bitmapPtr[0] + 1; // n + 1, the per-level rank stride
            int li = l;
            int ri = r + 1;                // exclusive right boundary into the rank table
            int val = 0;
            int* levelRank = rankPtr;
            for (int b = 0; b < log; b++)
            {
                int r0 = levelRank[li];
                int r1 = levelRank[ri];
                int onesInRange = r1 - r0;
                int zerosInRange = (ri - li) - onesInRange;

                if (k < zerosInRange)
                {
                    // Descend into the zeros bucket (this level's inspected bit == 0).
                    // Zeros occupy the front of the next level's array; the new range is
                    // [zeros before li, zeros before ri) == [li - r0, ri - r1).
                    li = li - r0;
                    ri = ri - r1;
                }
                else
                {
                    // Descend into the ones bucket (this level's inspected bit == 1).
                    k -= zerosInRange;
                    int mid = mids[b]; // start index of the ones bucket at this level
                    li = mid + r0;
                    ri = mid + r1;
                    val |= 1 << (log - 1 - b); // level b inspects bit (log-1-b), MSB-first
                }

                levelRank += stride;
            }
            return val;
        }
    }
}
