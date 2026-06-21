namespace IAFahim.Search.Range
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RangeMex
    {
        private const int MexUniverse = 64;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Add(ref long seen, int val)
        {
            if (val >= 0 && val < MexUniverse)
                seen |= 1L << val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Advance(long seen, int mex)
        {
            while (mex < MexUniverse && (seen & (1L << mex)) != 0) mex++;
            return mex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int* a, int l, int r)
        {
            if (l > r) return 0;
            long seen = 0;
            for (int i = l; i <= r && i < n; i++)
                Add(ref seen, a[i]);
            return Advance(seen, 0);
        }
    }

    public static unsafe class MexMaintain
    {
        private const int MexUniverse = 64;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Add(ref long seen, int val)
        {
            if (val >= 0 && val < MexUniverse)
                seen |= 1L << val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Advance(long seen, int mex)
        {
            while (mex < MexUniverse && (seen & (1L << mex)) != 0) mex++;
            return mex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool StillPresent(int* a, int from, int to, int value)
        {
            for (int j = from; j < to; j++)
                if (a[j] == value) return true;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Remove(int* a, int i, int k, int remove, ref long seen, ref int mex)
        {
            if (remove < 0 || remove >= MexUniverse) return;
            if (StillPresent(a, i, i + k, remove)) return;
            if ((seen & (1L << remove)) == 0) return;
            seen &= ~(1L << remove);
            if (remove < mex) mex = remove;
        }

        public static int Run(int n, int* a, int* res)
        {
            int mex = 0;
            long seen = 0;
            for (int right = 0; right < n; right++)
            {
                Add(ref seen, a[right]);
                mex = Advance(seen, mex);
                res[right] = mex;
            }
            return n;
        }

        public static int RunWindow(int n, int* a, int* res, int k)
        {
            long seen = 0;
            for (int j = 0; j < k && j < n; j++)
                Add(ref seen, a[j]);
            int mex = Advance(seen, 0);
            res[0] = mex;
            for (int i = 1; i + k <= n; i++)
            {
                Remove(a, i, k, a[i - 1], ref seen, ref mex);
                Add(ref seen, a[i + k - 1]);
                mex = Advance(seen, mex);
                res[i] = mex;
            }
            return n - k + 1;
        }
    }
}