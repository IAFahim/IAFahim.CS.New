namespace AlgoArena.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using IAFahim.String;

    public sealed unsafe class TextForensicsTests
    {
        [TestCase("racecar", 7)]
        [TestCase("aba", 3)]
        [TestCase("a", 1)]
        [TestCase("babad", 3)]
        [TestCase("hello", 1)]
        [TestCase("ab", 1)]
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

                Assert.AreEqual(expectedMaxOdd, maxOdd);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)radiiOdd);
            }
        }

        [TestCase("abba", 4)]
        [TestCase("ab", 0)]
        [TestCase("aabb", 2)]
        [TestCase("aaaa", 4)]
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

                Assert.AreEqual(expectedMaxEven, maxEven);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)radiiEven);
            }
        }

        [TestCase("hello", "ll", new[] { 2 })]
        [TestCase("hello", "lo", new[] { 3 })]
        [TestCase("ababab", "ab", new[] { 0, 2, 4 })]
        [TestCase("aaaa", "aa", new[] { 0, 1, 2 })]
        [TestCase("abc", "d", new int[] { })]
        [TestCase("abcabc", "abc", new[] { 0, 3 })]
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

                Assert.AreEqual(expectedPositions.Length, count);
                for (int i = 0; i < count; i++)
                    Assert.AreEqual(expectedPositions[i], matches[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)pattern);
                Marshal.FreeHGlobal((nint)matches);
            }
        }

        [TestCase("abcabc", 3)]
        [TestCase("aaaa", 1)]
        [TestCase("abc", 3)]
        [TestCase("ababab", 2)]
        public void StringPeriod_ReturnsMinimalPeriod(string s, int expectedPeriod)
        {
            int n = s.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(n);
            try
            {
                for (int i = 0; i < n; i++) text[i] = (byte)s[i];
                int period = StringPeriod.Run(text, n);
                Assert.AreEqual(expectedPeriod, period);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
            }
        }

        [TestCase("abab", 1)]
        [TestCase("abcdab", 1)]
        [TestCase("aaa", 2)]
        [TestCase("abc", 0)]
        [TestCase("aaaa", 3)]
        public void Borders_CountsCorrectly(string s, int expectedCount)
        {
            int n = s.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(n);
            int* borders = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) text[i] = (byte)s[i];
                int count = Borders.Run(text, n, borders);

                Assert.AreEqual(expectedCount, count);
                for (int i = 0; i < count; i++)
                    Assert.IsTrue(borders[i] > 0 && borders[i] < n);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)borders);
            }
        }

        [TestCase("aabbaa", 'a', 2)]
        [TestCase("hello", 'h', 1)]
        [TestCase("aaaa", 'a', 4)]
        [TestCase("abc", 'a', 1)]
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

                Assert.IsTrue(runs > 0);
                Assert.AreEqual((byte)expectedChar, values[0]);
                Assert.AreEqual(expectedCount, counts[0]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)values);
                Marshal.FreeHGlobal((nint)counts);
            }
        }

        [Test]
        public void RunLengthEncode_EmptyInput()
        {
            byte* values = (byte*)Marshal.AllocHGlobal(1);
            int* counts = (int*)Marshal.AllocHGlobal(sizeof(int));
            try
            {
                int runs = RunLengthEncode.Run(null, 0, values, counts);
                Assert.AreEqual(0, runs);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)values);
                Marshal.FreeHGlobal((nint)counts);
            }
        }
    }
}