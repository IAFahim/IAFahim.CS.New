namespace IAFahim.String.Palindrome
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PalindromicAutomaton
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SeriesLink(int* linkPtr, int* diffPtr, int* slinkPtr, int stateCount)
        {
            for (int v = 2; v < stateCount; v++)
            {
                int link = linkPtr[v];
                if (linkPtr[link] != -1 && diffPtr[v] == diffPtr[link])
                    slinkPtr[v] = slinkPtr[link];
                else
                    slinkPtr[v] = link;
            }
            return stateCount;
        }
    }
}
