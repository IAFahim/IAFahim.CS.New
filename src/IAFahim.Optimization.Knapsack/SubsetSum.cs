namespace IAFahim.Optimization.Knapsack
{
    using System.Runtime.CompilerServices;

    public static unsafe class SubsetSum
    {
        private const int BitsPerWord = 64;
        private const int WordShift = 6;
        private const int WordMask = 63;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanSingleWord(long* w, int n, long target)
        {
            ulong word = 1UL;
            int t = (int)target;
            for (int i = 0; i < n; i++)
            {
                long wi = w[i];
                if (wi > target) continue;
                word |= word << (int)wi;
            }
            return ((word >> t) & 1UL) != 0UL;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ShiftItemAligned(ulong* bits, int size, int wordOffset)
        {
            for (int k = size - 1; k >= wordOffset; k--)
                bits[k] |= bits[k - wordOffset];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ShiftItemCarry(ulong* bits, int size, int wordOffset, int bitShift)
        {
            int invShift = BitsPerWord - bitShift;
            for (int k = size - 1; k >= wordOffset; k--)
            {
                ulong shifted = bits[k - wordOffset] << bitShift;
                if (k - wordOffset - 1 >= 0)
                    shifted |= bits[k - wordOffset - 1] >> invShift;
                bits[k] |= shifted;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TestBit(ulong* bits, long target)
        {
            int targetWord = (int)(target >> WordShift);
            int targetBit = (int)(target & WordMask);
            return ((bits[targetWord] >> targetBit) & 1UL) != 0UL;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Can(long* w, int n, long target)
        {
            if (target < 0) return false;

            // Single-word fast path: all reachable sums in [0, target] fit in one 64-bit
            // word. Requires target <= 63 so that every shift (by w[i] <= target) is a
            // valid C# shift count and target itself indexes a real bit.
            if (target < BitsPerWord)
            {
                return CanSingleWord(w, n, target);
            }

            // Multi-word bitset DP: word k, bit b represents the reachability of sum
            // k * 64 + b. Reachable sums form a set; adding item wi unions the set with
            // itself shifted up by wi (a word-parallel shift-OR).
            int size = (int)((target >> WordShift) + 1);
            ulong* bits = stackalloc ulong[size];
            for (int k = 0; k < size; k++) bits[k] = 0UL;
            bits[0] = 1UL;

            for (int i = 0; i < n; i++)
            {
                long wi = w[i];
                if (wi > target) continue;

                int wordOffset = (int)(wi >> WordShift);
                int bitShift = (int)(wi & WordMask);

                if (bitShift == 0)
                {
                    ShiftItemAligned(bits, size, wordOffset);
                }
                else
                {
                    ShiftItemCarry(bits, size, wordOffset, bitShift);
                }
            }

            return TestBit(bits, target);
        }
    }
}
