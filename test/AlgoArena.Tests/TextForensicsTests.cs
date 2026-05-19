namespace AlgoArena.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;
    using IAFahim.String;

    public sealed unsafe class TextForensicsTests
    {
        [Theory]
        [InlineData("racecar", 7)]
        [InlineData("aba", 3)]
        [InlineData("a", 1)]
        [InlineData("babad", 3)]
        [InlineData("hello", 1)]
        [InlineData("ab", 1)]
        public void ManacherOdd_MaxOddPalindrome(string s, int expectedMaxOdd)
        {
            int n = s.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(n);
            int* radiiOdd = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) text[i] = (byte)s[i];
                ManacherOdd.Run(text, n, radiiOdd);

                int maxOdd = 0;
                for (int i = 0; i < n; i++)
                    maxOdd = Math.Max(maxOdd, radiiOdd[i] * 2 - 1);

                Assert.Equal(expectedMaxOdd, maxOdd);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)radiiOdd);
            }
        }

        [Theory]
        [InlineData("abba", 4)]
        [InlineData("ab", 0)]
        [InlineData("aabb", 2)]
        [InlineData("aaaa", 4)]
        public void ManacherEven_MaxEvenPalindrome(string s, int expectedMaxEven)
        {
            int n = s.Length;
            if (n < 2) return;
            byte* text = (byte*)Marshal.AllocHGlobal(n);
            int* radiiEven = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) text[i] = (byte)s[i];
                ManacherEven.Run(text, n, radiiEven);

                int maxEven = 0;
                for (int i = 0; i < n - 1; i++)
                    maxEven = Math.Max(maxEven, radiiEven[i] * 2);

                Assert.Equal(expectedMaxEven, maxEven);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)radiiEven);
            }
        }

        [Theory]
        [InlineData("hello", "ll", new[] { 2 })]
        [InlineData("hello", "lo", new[] { 3 })]
        [InlineData("ababab", "ab", new[] { 0, 2, 4 })]
        [InlineData("aaaa", "aa", new[] { 0, 1, 2 })]
        [InlineData("abc", "d", new int[] { })]
        [InlineData("abcabc", "abc", new[] { 0, 3 })]
        public void KmpSearch_FindsAllMatches(string textStr, string patternStr, int[] expectedPositions)
        {
            int n = textStr.Length;
            int m = patternStr.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(n);
            byte* pattern = (byte*)Marshal.AllocHGlobal(m);
            int* matches = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) text[i] = (byte)textStr[i];
                for (int i = 0; i < m; i++) pattern[i] = (byte)patternStr[i];

                int count = KmpSearch.Run(text, n, pattern, m, matches);

                Assert.Equal(expectedPositions.Length, count);
                for (int i = 0; i < count; i++)
                    Assert.Equal(expectedPositions[i], matches[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)pattern);
                Marshal.FreeHGlobal((nint)matches);
            }
        }

        [Theory]
        [InlineData("abcabc", 3)]
        [InlineData("aaaa", 1)]
        [InlineData("abc", 3)]
        [InlineData("ababab", 2)]
        public void StringPeriod_ReturnsMinimalPeriod(string s, int expectedPeriod)
        {
            int n = s.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(n);
            try
            {
                for (int i = 0; i < n; i++) text[i] = (byte)s[i];
                int period = StringPeriod.Run(text, n);
                Assert.Equal(expectedPeriod, period);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
            }
        }

        [Theory]
        [InlineData("abab", 1)]
        [InlineData("abcdab", 1)]
        [InlineData("aaa", 2)]
        [InlineData("abc", 0)]
        [InlineData("aaaa", 3)]
        public void Borders_CountsCorrectly(string s, int expectedCount)
        {
            int n = s.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(n);
            int* borders = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) text[i] = (byte)s[i];
                int count = Borders.Run(text, n, borders);

                Assert.Equal(expectedCount, count);
                for (int i = 0; i < count; i++)
                    Assert.True(borders[i] > 0 && borders[i] < n);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)borders);
            }
        }

        [Theory]
        [InlineData("aabbaa", 'a', 2)]
        [InlineData("hello", 'h', 1)]
        [InlineData("aaaa", 'a', 4)]
        [InlineData("abc", 'a', 1)]
        public void RunLengthEncode_FirstRun(string s, char expectedChar, int expectedCount)
        {
            int n = s.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(n);
            byte* values = (byte*)Marshal.AllocHGlobal(n);
            int* counts = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) text[i] = (byte)s[i];
                int runs = RunLengthEncode.Run(text, n, values, counts);

                Assert.True(runs > 0);
                Assert.Equal((byte)expectedChar, values[0]);
                Assert.Equal(expectedCount, counts[0]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)values);
                Marshal.FreeHGlobal((nint)counts);
            }
        }

        [Fact]
        public void RunLengthEncode_EmptyInput()
        {
            byte* values = (byte*)Marshal.AllocHGlobal(1);
            int* counts = (int*)Marshal.AllocHGlobal(sizeof(int));
            try
            {
                int runs = RunLengthEncode.Run(null, 0, values, counts);
                Assert.Equal(0, runs);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)values);
                Marshal.FreeHGlobal((nint)counts);
            }
        }
    }
}