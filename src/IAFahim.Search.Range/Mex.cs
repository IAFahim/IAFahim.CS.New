namespace IAFahim.Search.Range
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RangeMex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int n, int* a, int l, int r)
        {
            if (l > r) return 0;
            long seen = 0;
            for (int i = l; i <= r && i < n; i++)
            {
                int val = a[i];
                if (val >= 0 && val < 64)
                    seen |= 1L << val;
            }
            int mex = 0;
            while ((seen & (1L << mex)) != 0) mex++;
            return mex;
        }
    }

    public static unsafe class MexMaintain
    {
        public static int Run(int n, int* a, int* res)
        {
            int mex = 0;
            long seen = 0;
            int left = 0;
            for (int right = 0; right < n; right++)
            {
                int val = a[right];
                if (val >= 0 && val < 64)
                    seen |= 1L << val;
                while ((seen & (1L << mex)) != 0 && left <= right)
                    mex++;
                res[right] = mex;
            }
            return n;
        }

        public static int RunWindow(int n, int* a, int* res, int k)
        {
            long seen = 0;
            for (int j = 0; j < k && j < n; j++)
            {
                int val = a[j];
                if (val >= 0 && val < 64)
                    seen |= 1L << val;
            }
            int mex = 0;
            while ((seen & (1L << mex)) != 0) mex++;
            res[0] = mex;
            for (int i = 1; i + k <= n; i++)
            {
                int remove = a[i - 1];
                int add = a[i + k - 1];
                if (remove >= 0 && remove < 64)
                {
                    bool stillPresent = false;
                    for (int j = i; j < i + k; j++)
                        if (a[j] == remove) { stillPresent = true; break; }
                    if (!stillPresent && (seen & (1L << remove)) != 0)
                        seen &= ~(1L << remove);
                }
                if (add >= 0 && add < 64)
                    seen |= 1L << add;
                while ((seen & (1L << mex)) != 0 && mex < 64) mex++;
                res[i] = mex;
            }
            return n - k + 1;
        }
    }
}