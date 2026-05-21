namespace IAFahim.String.Match
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MainLorentz
    {
        public struct Run
        {
            public int Start;
            public int Period;
            public int Length;
        }

        public static int Find(byte* s, int n, Run* runs)
        {
            int count = 0;
            for (int period = 1; period * 2 <= n; period++)
            {
                for (int start = 0; start + 2 * period <= n; start++)
                {
                    bool match = true;
                    for (int k = 0; k < period; k++)
                    {
                        if (s[start + k] != s[start + period + k])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        int len = 2 * period;
                        while (start + len < n && s[start + len] == s[start + len % period])
                            len++;
                        bool subsumed = false;
                        for (int r = 0; r < count; r++)
                        {
                            if (runs[r].Period == period &&
                                runs[r].Start <= start &&
                                runs[r].Start + runs[r].Length >= start + len)
                            {
                                subsumed = true;
                                break;
                            }
                        }
                        if (!subsumed)
                        {
                            runs[count].Start = start;
                            runs[count].Period = period;
                            runs[count].Length = len;
                            count++;
                        }
                        break;
                    }
                }
            }
            return count;
        }
    }
}
