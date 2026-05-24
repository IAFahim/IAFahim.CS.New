namespace AlgoArena
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.String;
    using IAFahim.Math.Transform;

    public static unsafe class TextForensics
    {
        public static void Run()
        {
            DisplayHeader();
            string choice = GetUserChoice();
            RouteChoice(choice);
        }

        private static void DisplayHeader()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════╗");
            Console.WriteLine("║        🔍  TEXT FORENSICS  🔍            ║");
            Console.WriteLine("║  Expose hidden patterns in text!         ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝\n");
            Console.WriteLine("  1. Palindrome Hunter  — find hidden mirrors");
            Console.WriteLine("  2. Substring Search  — KMP vs Z-Algorithm");
            Console.WriteLine("  3. Text Statistics   — length, period, borders");
        }

        private static string GetUserChoice()
        {
            Console.Write("  Choice: ");
            return Console.ReadLine()?.Trim() ?? "0";
        }

        private static void RouteChoice(string choice)
        {
            switch (choice)
            {
                case "1": PalindromeHunter(); break;
                case "2": SubstringSearch(); break;
                case "3": TextStatistics(); break;
                default: Console.WriteLine("Unknown forensics tool."); break;
            }
        }

        private static void PalindromeHunter()
        {
            Console.WriteLine("\n  ═══ Palindrome Hunter ═══");
            Console.Write("  Enter text: ");
            string input = Console.ReadLine() ?? "";
            int n = input.Length;

            byte* text = (byte*)Marshal.AllocHGlobal(n);
            int* radiiOdd = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* radiiEven = (int*)Marshal.AllocHGlobal(n * sizeof(int));

            try
            {
                InitializeTextBuffer(input, text);
                ManacherOdd.Run(text, n, radiiOdd);
                ManacherEven.Run(text, n, radiiEven);

                DisplayPalindromeResults(input, radiiOdd, radiiEven, n);
                CalculateZetaEnergy(radiiOdd, n);
            }
            finally
            {
                FreeMemory(text, (byte*)radiiOdd, (byte*)radiiEven);
            }
        }

        private static void InitializeTextBuffer(string source, byte* buffer)
        {
            for (int i = 0; i < source.Length; i++) buffer[i] = (byte)source[i];
        }

        private static void DisplayPalindromeResults(string input, int* radiiOdd, int* radiiEven, int n)
        {
            Console.WriteLine($"\n  Text: {input}\n");
            DisplayOddPalindromes(input, radiiOdd, n);
            DisplayEvenPalindromes(input, radiiEven, n);
        }

        private static void DisplayOddPalindromes(string input, int* radii, int n)
        {
            Console.WriteLine("  Odd-length palindromes (center on char):");
            int count = 0;
            for (int i = 0; i < n; i++)
            {
                int rad = radii[i];
                int len = rad * 2 - 1;
                if (len < 3) continue;

                Console.Write($"    Center[{i}]='{input[i]}': \"");
                PrintSubstring(input, i - rad + 1, i + rad - 1);
                Console.WriteLine($"\" (len={len})");
                count++;
            }
            Console.WriteLine($"  Found {count} odd palindromes\n");
        }

        private static void DisplayEvenPalindromes(string input, int* radii, int n)
        {
            Console.WriteLine("  Even-length palindromes (center between chars):");
            int count = 0;
            for (int i = 0; i < n - 1; i++)
            {
                int rad = radii[i];
                int len = rad * 2;
                if (len < 2) continue;

                Console.Write($"    Center between[{i},{i + 1}]: \"");
                PrintSubstring(input, i - rad + 1, i + rad);
                Console.WriteLine($"\" (len={len})");
                count++;
            }
            Console.WriteLine($"  Found {count} even palindromes");
        }

        private static void PrintSubstring(string source, int start, int end)
        {
            for (int j = start; j <= end && j < source.Length && j >= 0; j++)
                Console.Write(source[j]);
        }

        private static void CalculateZetaEnergy(int* radii, int n)
        {
            long* f = stackalloc long[32];
            int limit = Math.Min(n, 32);
            for (int i = 0; i < limit; i++) f[i] = radii[i];
            SubsetZeta.Run(f, 1);
            Console.WriteLine($"\n  Zeta transform energy: {f[0]}");
        }

        private static void SubstringSearch()
        {
            Console.WriteLine("\n  ═══ Substring Search — KMP vs Z ═══");
            Console.Write("  Enter text: ");
            string textInput = Console.ReadLine() ?? "";
            Console.Write("  Enter pattern: ");
            string patternInput = Console.ReadLine() ?? "";

            int n = textInput.Length, m = patternInput.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(n);
            byte* pattern = (byte*)Marshal.AllocHGlobal(m);
            int* matches = (int*)Marshal.AllocHGlobal(n * sizeof(int));

            try
            {
                InitializeTextBuffer(textInput, text);
                InitializeTextBuffer(patternInput, pattern);

                int kmpCount = KmpSearch.Run(text, n, pattern, m, matches);
                DisplayKmpMatches(textInput, matches, kmpCount, m);

                ExecuteZAlgorithmComparison(text, n, pattern, m, kmpCount);
            }
            finally
            {
                FreeMemory(text, pattern, (byte*)matches);
            }
        }

        private static void DisplayKmpMatches(string text, int* matches, int count, int patternLen)
        {
            Console.WriteLine($"\n  KMP found {count} matches:");
            for (int i = 0; i < count; i++)
            {
                int start = matches[i];
                Console.Write($"    [{start}] \"");
                PrintSubstringFixed(text, start, patternLen);
                Console.WriteLine("\"");
            }
        }

        private static void PrintSubstringFixed(string source, int start, int length)
        {
            int end = Math.Min(start + length, source.Length);
            for (int j = start; j < end; j++) Console.Write(source[j]);
        }

        private static void ExecuteZAlgorithmComparison(byte* text, int n, byte* pattern, int m, int kmpCount)
        {
            int combinedLen = n + m + 1;
            byte* combined = (byte*)Marshal.AllocHGlobal(combinedLen);
            int* combinedZ = (int*)Marshal.AllocHGlobal(combinedLen * sizeof(int));

            try
            {
                PrepareCombinedBuffer(combined, text, n, pattern, m);
                ZAlgorithm.Run(combined, combinedLen, combinedZ);

                int zCount = DisplayZMatches(combinedZ, combinedLen, m, text, n);
                Console.WriteLine($"\n  Z found {zCount} matches");
                Console.WriteLine($"  Both algorithms agree: {(kmpCount == zCount ? "✅ YES" : "❌ NO")}");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)combined);
                Marshal.FreeHGlobal((nint)combinedZ);
            }
        }

        private static void PrepareCombinedBuffer(byte* combined, byte* text, int n, byte* pattern, int m)
        {
            for (int i = 0; i < m; i++) combined[i] = pattern[i];
            combined[m] = 0;
            for (int i = 0; i < n; i++) combined[m + 1 + i] = text[i];
        }

        private static int DisplayZMatches(int* z, int totalLen, int patternLen, byte* text, int textLen)
        {
            Console.WriteLine("  Z-Algorithm matches:");
            int count = 0;
            for (int i = patternLen + 1; i < totalLen; i++)
            {
                if (z[i] >= patternLen)
                {
                    int pos = i - patternLen - 1;
                    Console.Write($"    [{pos}] \"");
                    PrintBytesAsChars(text + pos, patternLen);
                    Console.WriteLine("\"");
                    count++;
                }
            }
            return count;
        }

        private static void PrintBytesAsChars(byte* data, int len)
        {
            for (int j = 0; j < len; j++) Console.Write((char)data[j]);
        }

        private static void TextStatistics()
        {
            Console.WriteLine("\n  ═══ Text Statistics ═══");
            Console.Write("  Enter text: ");
            string input = Console.ReadLine() ?? "";
            int n = input.Length;

            byte* text = (byte*)Marshal.AllocHGlobal(n);
            byte* rleValues = (byte*)Marshal.AllocHGlobal(n);
            int* rleCounts = (int*)Marshal.AllocHGlobal(n * sizeof(int));

            try
            {
                InitializeTextBuffer(input, text);
                int rleCount = RunLengthEncode.Run(text, n, rleValues, rleCounts);

                DisplayRleResults(rleValues, rleCounts, rleCount, n);
                ExecutePeriodAnalysis(text, n);
                ExecuteBorderAnalysis(input, text, n);
            }
            finally
            {
                FreeMemory(text, rleValues, (byte*)rleCounts);
            }
        }

        private static void DisplayRleResults(byte* values, int* counts, int rleCount, int totalLen)
        {
            Console.WriteLine($"\n  Text length: {totalLen}");
            Console.WriteLine($"  Run-length encoded: {rleCount} runs");
            Console.Write("  RLE: ");
            for (int i = 0; i < rleCount; i++)
                Console.Write($"({(char)values[i]},{counts[i]}) ");
            Console.WriteLine();
        }

        private static void ExecutePeriodAnalysis(byte* text, int n)
        {
            int period = StringPeriod.Run(text, n);
            int minPeriod = MinPeriod.Run(text, n);
            Console.WriteLine($"\n  Minimal period: {minPeriod}");
            Console.WriteLine($"  Can be tiled:   {(period > 0 ? "YES (periodic)" : "NO")}");
            if (period > 0) Console.WriteLine($"  Period length:  {period}");
        }

        private static void ExecuteBorderAnalysis(string source, byte* text, int n)
        {
            int* borders = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                int count = Borders.Run(text, n, borders);
                Console.WriteLine($"\n  Borders (proper prefixes = suffixes): {count}");
                for (int i = 0; i < count; i++)
                    Console.WriteLine($"    len={borders[i]}: \"{source.Substring(0, borders[i])}\"");
            }
            finally { Marshal.FreeHGlobal((nint)borders); }
        }

        private static void FreeMemory(params byte*[] pointers)
        {
            foreach (var ptr in pointers) Marshal.FreeHGlobal((nint)ptr);
        }
    }
}