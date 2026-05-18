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
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════╗");
            Console.WriteLine("║        🔍  TEXT FORENSICS  🔍            ║");
            Console.WriteLine("║  Expose hidden patterns in text!         ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine("  1. Palindrome Hunter  — find hidden mirrors");
            Console.WriteLine("  2. Substring Search  — KMP vs Z-Algorithm");
            Console.WriteLine("  3. Text Statistics   — length, period, borders");
            Console.Write("  Choice: ");
            string choice = Console.ReadLine()?.Trim() ?? "0";

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
            Console.WriteLine();
            Console.WriteLine("  ═══ Palindrome Hunter ═══");
            Console.WriteLine("  Enter text: ");
            string s = Console.ReadLine() ?? "";
            int n = s.Length;

            byte* text = (byte*)Marshal.AllocHGlobal(n);
            int* radiiOdd = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* radiiEven = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) text[i] = (byte)s[i];
                IAFahim.String.ManacherOdd.Run(text, n, radiiOdd);
                IAFahim.String.ManacherEven.Run(text, n, radiiEven);

                Console.WriteLine();
                Console.WriteLine($"  Text: {s}");
                Console.WriteLine();
                Console.WriteLine("  Odd-length palindromes (center on char):");
                int oddCount = 0;
                for (int i = 0; i < n; i++)
                {
                    int rad = radiiOdd[i];
                    int len = rad * 2 - 1;
                    if (len >= 3)
                    {
                        Console.Write($"    Center[{i}]='{s[i]}': \"");
                        for (int j = i - rad + 1; j <= i + rad - 1 && j < n && j >= 0; j++)
                            Console.Write(s[j]);
                        Console.Write($"\" (len={len})");
                        Console.WriteLine();
                        oddCount++;
                    }
                }
                Console.WriteLine($"  Found {oddCount} odd palindromes");

                Console.WriteLine();
                Console.WriteLine("  Even-length palindromes (center between chars):");
                int evenCount = 0;
                for (int i = 0; i < n - 1; i++)
                {
                    int rad = radiiEven[i];
                    int len = rad * 2;
                    if (len >= 2)
                    {
                        Console.Write($"    Center between[{i},{i+1}]: \"");
                        for (int j = i - rad + 1; j <= i + rad && j < n && j >= 0; j++)
                            Console.Write(s[j]);
                        Console.Write($"\" (len={len})");
                        Console.WriteLine();
                        evenCount++;
                    }
                }
                Console.WriteLine($"  Found {evenCount} even palindromes");

                long* f = stackalloc long[32];
                int limit = Math.Min(n, 32);
                for (int i = 0; i < limit; i++) f[i] = radiiOdd[i];
                IAFahim.Math.Transform.SubsetZeta.Run(f, 1);
                Console.WriteLine();
                Console.WriteLine($"  Zeta transform energy: {f[0]}");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)radiiOdd);
                Marshal.FreeHGlobal((nint)radiiEven);
            }
        }

        private static void SubstringSearch()
        {
            Console.WriteLine();
            Console.WriteLine("  ═══ Substring Search — KMP vs Z ═══");
            Console.WriteLine("  Enter text: ");
            string textStr = Console.ReadLine() ?? "";
            Console.WriteLine("  Enter pattern: ");
            string patternStr = Console.ReadLine() ?? "";

            int n = textStr.Length;
            int m = patternStr.Length;

            byte* text = (byte*)Marshal.AllocHGlobal(n);
            byte* pattern = (byte*)Marshal.AllocHGlobal(m);
            int* matches = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) text[i] = (byte)textStr[i];
                for (int i = 0; i < m; i++) pattern[i] = (byte)patternStr[i];

                Console.WriteLine();
                Console.WriteLine($"  Text length:   {n}");
                Console.WriteLine($"  Pattern length: {m}");

                int kmpCount = IAFahim.String.KmpSearch.Run(text, n, pattern, m, matches);
                Console.WriteLine();
                Console.WriteLine($"  KMP found {kmpCount} matches at positions:");
                for (int i = 0; i < kmpCount; i++)
                {
                    int start = matches[i];
                    int len = Math.Min(m, n - start);
                    Console.Write("    [");
                    Console.Write(start);
                    Console.Write("] \"");
                    for (int j = 0; j < len; j++) Console.Write((char)text[start + j]);
                    Console.WriteLine("\"");
                }

                int combinedLen = n + m + 1;
                byte* combined = (byte*)Marshal.AllocHGlobal(combinedLen);
                int* combinedZ = (int*)Marshal.AllocHGlobal(combinedLen * sizeof(int));
                try
                {
                    for (int i = 0; i < m; i++) combined[i] = pattern[i];
                    combined[m] = 0;
                    for (int i = 0; i < n; i++) combined[m + 1 + i] = text[i];

                    IAFahim.String.ZAlgorithm.Run(combined, combinedLen, combinedZ);

                    Console.WriteLine("  Z-Algorithm matches:");
                    int zCount = 0;
                    for (int i = m + 1; i < combinedLen; i++)
                    {
                        if (combinedZ[i] >= m)
                        {
                            int pos = i - m - 1;
                            Console.Write("    [");
                            Console.Write(pos);
                            Console.Write("] \"");
                            for (int j = 0; j < m; j++) Console.Write((char)text[pos + j]);
                            Console.WriteLine("\"");
                            zCount++;
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine($"  Z found {zCount} matches");
                    Console.WriteLine($"  Both algorithms agree: {(kmpCount == zCount ? "✅ YES" : "❌ NO")}");
                }
                finally { Marshal.FreeHGlobal((nint)combined); Marshal.FreeHGlobal((nint)combinedZ); }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)pattern);
                Marshal.FreeHGlobal((nint)matches);
            }
        }

        private static void TextStatistics()
        {
            Console.WriteLine();
            Console.WriteLine("  ═══ Text Statistics ═══");
            Console.WriteLine("  Enter text: ");
            string s = Console.ReadLine() ?? "";
            int n = s.Length;

            byte* text = (byte*)Marshal.AllocHGlobal(n);
            byte* values = (byte*)Marshal.AllocHGlobal(n);
            int* counts = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) text[i] = (byte)s[i];

                int rleCount = IAFahim.String.RunLengthEncode.Run(text, n, values, counts);
                Console.WriteLine();
                Console.WriteLine($"  Text length: {n}");
                Console.WriteLine($"  Run-length encoded: {rleCount} runs");
                Console.Write("  RLE: ");
                for (int i = 0; i < rleCount; i++)
                {
                    Console.Write("(");
                    Console.Write((char)values[i]);
                    Console.Write(",");
                    Console.Write(counts[i]);
                    Console.Write(") ");
                }
                Console.WriteLine();

                int period = IAFahim.String.StringPeriod.Run(text, n);
                int minPeriod = IAFahim.String.MinPeriod.Run(text, n);
                Console.WriteLine();
                Console.WriteLine($"  Minimal period: {minPeriod}");
                Console.WriteLine($"  Can be tiled: {(period > 0 ? "YES (periodic)" : "NO")}");
                if (period > 0) Console.WriteLine($"  Period length: {period}");

                int* borders = (int*)Marshal.AllocHGlobal(n * sizeof(int));
                int borderCount = IAFahim.String.Borders.Run(text, n, borders);
                Console.WriteLine();
                Console.WriteLine($"  Borders (proper prefixes = suffixes): {borderCount}");
                for (int i = 0; i < borderCount; i++)
                {
                    string border = s.Substring(0, borders[i]);
                    Console.WriteLine($"    len={borders[i]}: \"{border}\"");
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)values);
                Marshal.FreeHGlobal((nint)counts);
            }
        }
    }
}