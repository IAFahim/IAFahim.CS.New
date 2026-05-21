namespace IAFahim.String.Match
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Crochemore
    {
        public struct Repetition
        {
            public int Position;
            public int Period;
            public int Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Find(byte* s, int n, Repetition* reps)
        {
            int count = 0;
            for (int period = 1; period * 2 <= n; period++)
            {
                for (int i = 0; i <= n - 2 * period; i++)
                {
                    int run = 0;
                    while (i + run < n && s[i + run] == s[i + (run % period)])
                        run++;
                    if (run >= 2 * period)
                    {
                        reps[count].Position = i;
                        reps[count].Period = period;
                        reps[count].Length = run;
                        count++;
                        i += run - 1;
                    }
                }
            }
            return count;
        }
    }
}
