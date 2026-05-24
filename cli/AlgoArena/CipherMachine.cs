namespace AlgoArena
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Math.NT;
    using IAFahim.Math.Modular;
    using IAFahim.Math.Transform;
    using IAFahim.String;

    public static unsafe class CipherMachine
    {
        public static void Run()
        {
            DisplayHeader();
            string choice = GetUserChoice();
            RouteChoice(choice);
        }

        private static void DisplayHeader()
        {
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════╗");
            Console.WriteLine("║        🔐  CIPHER MACHINE  🔐            ║");
            Console.WriteLine("║  Encode your secrets with math!          ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  1. XOR Cipher    — classic bit flip");
            Console.WriteLine("  2. Hash Forge    — create fingerprints");
            Console.WriteLine("  3. Caesar Shift  — ancient encryption");
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
                case "1": XorCipher(); break;
                case "2": HashForge(); break;
                case "3": CaesarShift(); break;
                default: Console.WriteLine("Unknown cipher."); break;
            }
        }

        private static void XorCipher()
        {
            Console.WriteLine("\n  ═══ XOR Cipher ═══");
            Console.Write("  Enter text: ");
            string plaintext = Console.ReadLine() ?? "";
            int key = GetCipherKey();

            int len = plaintext.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(len);
            byte* encoded = (byte*)Marshal.AllocHGlobal(len);
            byte* decoded = (byte*)Marshal.AllocHGlobal(len);

            try
            {
                InitializeTextBuffer(plaintext, text);
                ExecuteXorTransform(text, encoded, decoded, len, key);
                DisplayXorResults(text, encoded, decoded, len);
            }
            finally
            {
                FreeMemory(text, encoded, decoded);
            }
        }

        private static int GetCipherKey()
        {
            Console.Write("  Enter key (number): ");
            string input = Console.ReadLine()?.Trim() ?? "42";
            return int.TryParse(input, out int k) ? k : 42;
        }

        private static void InitializeTextBuffer(string source, byte* buffer)
        {
            for (int i = 0; i < source.Length; i++) buffer[i] = (byte)source[i];
        }

        private static void ExecuteXorTransform(byte* source, byte* encoded, byte* decoded, int len, int key)
        {
            for (int i = 0; i < len; i++) encoded[i] = (byte)(source[i] ^ key);
            for (int i = 0; i < len; i++) decoded[i] = (byte)(encoded[i] ^ key);
        }

        private static void DisplayXorResults(byte* original, byte* encoded, byte* decoded, int len)
        {
            Console.Write("\n  Original bytes:  ");
            PrintBytes(original, len);
            Console.Write("\n  Encoded bytes:   ");
            PrintBytes(encoded, len);
            Console.Write("\n  Encoded text:    ");
            PrintChars(encoded, len);
            Console.Write("\n  Decoded text:    ");
            PrintChars(decoded, len);
            Console.WriteLine();
        }

        private static void PrintBytes(byte* data, int len)
        {
            for (int i = 0; i < len; i++) Console.Write($"{data[i]:X2} ");
        }

        private static void PrintChars(byte* data, int len)
        {
            for (int i = 0; i < len; i++) Console.Write((char)data[i]);
        }

        private static void HashForge()
        {
            Console.WriteLine("\n  ═══ Hash Forge ═══");
            Console.Write("  Enter text: ");
            string plaintext = Console.ReadLine() ?? "";

            int len = plaintext.Length;
            byte* textBuffer = (byte*)Marshal.AllocHGlobal(len);
            ulong* hashBuffer = (ulong*)Marshal.AllocHGlobal((len + 1) * sizeof(ulong));
            ulong* powBuffer = (ulong*)Marshal.AllocHGlobal((len + 1) * sizeof(ulong));

            try
            {
                InitializeTextBuffer(plaintext, textBuffer);
                IAFahim.String.HashBuild.Run(textBuffer, len, hashBuffer, powBuffer);
                ulong fullHash = IAFahim.String.HashRange.Run(hashBuffer, powBuffer, 0, len);

                DisplayHashMetrics(fullHash);
                ExecuteBasisInsertion(fullHash, textBuffer, len);
            }
            finally
            {
                FreeMemory(textBuffer);
                Marshal.FreeHGlobal((nint)hashBuffer);
                Marshal.FreeHGlobal((nint)powBuffer);
            }
        }

        private static void DisplayHashMetrics(ulong hash)
        {
            Console.WriteLine($"\n  Hash (64-bit):  {hash:X16}");
            Console.WriteLine($"  Hash (decimal): {hash}");
            Console.WriteLine($"  Hash (binary):  {Convert.ToString((long)hash, 2).PadLeft(64, '0')}");

            int hInt = (int)(hash & 0x7FFFFFFF);
            Console.WriteLine($"  Fibonacci hash: {HashInt.Run(hInt)}");
            Console.WriteLine($"  Bit count:      {BitCount.Run(hInt)}");
        }

        private static void ExecuteBasisInsertion(ulong hash, byte* text, int len)
        {
            long* basis = (long*)Marshal.AllocHGlobal(64 * sizeof(long));
            int* basisSize = (int*)Marshal.AllocHGlobal(sizeof(int));
            *basisSize = 0;

            try
            {
                XorBasisInsert.Run(basis, basisSize, (long)hash);
                long maxXor = XorBasisMax.Run(basis);
                Console.WriteLine($"  Max XOR with basis: {maxXor}");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)basis);
                Marshal.FreeHGlobal((nint)basisSize);
            }
        }

        private static void CaesarShift()
        {
            Console.WriteLine("\n  ═══ Caesar Shift Cipher ═══");
            Console.Write("  Enter text: ");
            string plaintext = Console.ReadLine() ?? "";
            int shift = GetShiftValue();

            int len = plaintext.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(len);
            long* encoded = (long*)Marshal.AllocHGlobal(len);

            try
            {
                PrepareCaesarBuffer(plaintext, text, encoded, shift);
                DisplayCaesarResult(plaintext, text, encoded, len);
                DisplayModularInverse(shift);
                BruteForceAllShifts(plaintext, text, len);
            }
            finally
            {
                FreeMemory(text);
                Marshal.FreeHGlobal((nint)encoded);
            }
        }

        private static int GetShiftValue()
        {
            Console.Write("  Enter shift (0-25): ");
            string input = Console.ReadLine()?.Trim() ?? "3";
            int s = int.TryParse(input, out int result) ? result : 3;
            return ((s % 26) + 26) % 26;
        }

        private static void PrepareCaesarBuffer(string plaintext, byte* text, long* encoded, int shift)
        {
            for (int i = 0; i < plaintext.Length; i++)
            {
                char c = plaintext[i];
                if (char.IsLetter(c))
                {
                    text[i] = (byte)(char.ToUpper(c) - 'A');
                    encoded[i] = (text[i] + shift) % 26;
                }
                else
                {
                    text[i] = 255;
                }
            }
        }

        private static void DisplayCaesarResult(string original, byte* text, long* encoded, int len)
        {
            Console.Write("\n  Encoded: ");
            for (int i = 0; i < len; i++)
            {
                if (text[i] == 255) Console.Write(original[i]);
                else Console.Write((char)('A' + encoded[i]));
            }
            Console.WriteLine();
        }

        private static void DisplayModularInverse(int shift)
        {
            long mod = 26;
            long baseVal = shift == 0 ? 1 : shift;
            long invShift = IAFahim.Math.Modular.ModPow.Run(baseVal, mod - 2, mod);
            Console.WriteLine($"  Modular inverse of {shift} (mod 26): {invShift}");
        }

        private static void BruteForceAllShifts(string original, byte* text, int len)
        {
            Console.WriteLine("\n  Brute force all shifts:");
            for (int sh = 0; sh < 26; sh++)
            {
                Console.Write($"  Shift {sh,2}: ");
                PrintShiftedText(original, text, len, sh);
                Console.WriteLine();
            }
        }

        private static void PrintShiftedText(string original, byte* text, int len, int shift)
        {
            for (int i = 0; i < len; i++)
            {
                if (text[i] == 255) Console.Write(original[i]);
                else Console.Write((char)('A' + (text[i] + shift) % 26));
            }
        }

        private static void FreeMemory(params byte*[] pointers)
        {
            foreach (var ptr in pointers) Marshal.FreeHGlobal((nint)ptr);
        }
    }
}