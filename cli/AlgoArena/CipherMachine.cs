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
            Console.WriteLine();
            Console.WriteLine("╔═══════════════════════════════════════════╗");
            Console.WriteLine("║        🔐  CIPHER MACHINE  🔐            ║");
            Console.WriteLine("║  Encode your secrets with math!          ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  1. XOR Cipher    — classic bit flip");
            Console.WriteLine("  2. Hash Forge    — create fingerprints");
            Console.WriteLine("  3. Caesar Shift  — ancient encryption");
            Console.Write("  Choice: ");
            string choice = Console.ReadLine()?.Trim() ?? "0";

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
            Console.WriteLine();
            Console.WriteLine("  ═══ XOR Cipher ═══");
            Console.Write("  Enter text: ");
            string plaintext = Console.ReadLine() ?? "";
            Console.Write("  Enter key (number): ");
            string keyInput = Console.ReadLine()?.Trim() ?? "42";
            int key = int.TryParse(keyInput, out int k) ? k : 42;

            int len = plaintext.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(len);
            byte* encoded = (byte*)Marshal.AllocHGlobal(len);
            byte* decoded = (byte*)Marshal.AllocHGlobal(len);
            try
            {
                for (int i = 0; i < len; i++) text[i] = (byte)plaintext[i];

                Console.WriteLine();
                Console.Write("  Original bytes:  ");
                for (int i = 0; i < len; i++) Console.Write($"{text[i]:X2} ");
                Console.WriteLine();

                for (int i = 0; i < len; i++) encoded[i] = (byte)(text[i] ^ key);
                Console.Write("  Encoded bytes:  ");
                for (int i = 0; i < len; i++) Console.Write($"{encoded[i]:X2} ");
                Console.WriteLine();
                Console.Write("  Encoded text:   ");
                for (int i = 0; i < len; i++) Console.Write((char)encoded[i]);
                Console.WriteLine();

                for (int i = 0; i < len; i++) decoded[i] = (byte)(encoded[i] ^ key);
                Console.Write("  Decoded text:   ");
                for (int i = 0; i < len; i++) Console.Write((char)decoded[i]);
                Console.WriteLine();
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)encoded);
                Marshal.FreeHGlobal((nint)decoded);
            }
        }

        private static void HashForge()
        {
            Console.WriteLine();
            Console.WriteLine("  ═══ Hash Forge ═══");
            Console.Write("  Enter text: ");
            string plaintext = Console.ReadLine() ?? "";

            int len = plaintext.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(len);
            ulong* hash = (ulong*)Marshal.AllocHGlobal((len + 1) * sizeof(ulong));
            ulong* pow = (ulong*)Marshal.AllocHGlobal((len + 1) * sizeof(ulong));
            try
            {
                for (int i = 0; i < len; i++) text[i] = (byte)plaintext[i];

                IAFahim.String.HashBuild.Run(text, len, hash, pow);
                ulong h = IAFahim.String.HashRange.Run(hash, pow, 0, len);

                Console.WriteLine();
                Console.WriteLine($"  Hash (64-bit):  {h:X16}");
                Console.WriteLine($"  Hash (decimal): {h}");
                Console.WriteLine($"  Hash (binary):  {Convert.ToString((long)h, 2).PadLeft(64, '0')}");
                int hInt = (int)(h & 0x7FFFFFFF);
                Console.WriteLine($"  Fibonacci hash: {HashInt.Run(hInt)}");
                Console.WriteLine($"  Bit count: {BitCount.Run(hInt)}");

                long* f = (long*)Marshal.AllocHGlobal(len * sizeof(long));
                long* basis = (long*)Marshal.AllocHGlobal(64 * sizeof(long));
                int* basisSize = (int*)Marshal.AllocHGlobal(sizeof(int));
                *basisSize = 0;
                try
                {
                    for (int i = 0; i < len; i++) f[i] = text[i] % 26;
                    IAFahim.Math.Transform.XorBasisInsert.Run(basis, basisSize, (long)h);
                    long maxXor = IAFahim.Math.Transform.XorBasisMax.Run(basis);
                    Console.WriteLine($"  Max XOR with basis: {maxXor}");
                }
                finally { Marshal.FreeHGlobal((nint)f); Marshal.FreeHGlobal((nint)basis); Marshal.FreeHGlobal((nint)basisSize); }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)hash);
                Marshal.FreeHGlobal((nint)pow);
            }
        }

        private static void CaesarShift()
        {
            Console.WriteLine();
            Console.WriteLine("  ═══ Caesar Shift Cipher ═══");
            Console.Write("  Enter text: ");
            string plaintext = Console.ReadLine() ?? "";
            Console.Write("  Enter shift (0-25): ");
            string shiftInput = Console.ReadLine()?.Trim() ?? "3";
            int shift = int.TryParse(shiftInput, out int s) ? ((s % 26) + 26) % 26 : 3;

            int len = plaintext.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(len);
            long* enc = (long*)Marshal.AllocHGlobal(len);
            try
            {
                for (int i = 0; i < len; i++)
                {
                    char c = plaintext[i];
                    if (c >= 'A' && c <= 'Z') text[i] = (byte)(c - 'A');
                    else if (c >= 'a' && c <= 'z') text[i] = (byte)(c - 'a');
                    else { text[i] = 255; continue; }
                    enc[i] = (text[i] + shift) % 26;
                }

                Console.WriteLine();
                Console.Write("  Encoded: ");
                for (int i = 0; i < len; i++)
                {
                    if (text[i] == 255) Console.Write(plaintext[i]);
                    else Console.Write((char)('A' + enc[i]));
                }
                Console.WriteLine();

                long mod = 26;
                long invShift = IAFahim.Math.Modular.ModPow.Run(shift == 0 ? 1 : shift, mod - 2, mod);
                Console.WriteLine($"  Modular inverse of {shift}: {invShift}");

                Console.WriteLine();
                Console.Write("  Brute force all shifts:");
                for (int sh = 0; sh < 26; sh++)
                {
                    Console.Write($"\n  Shift {sh,2}: ");
                    for (int i = 0; i < len; i++)
                    {
                        if (text[i] == 255) Console.Write(plaintext[i]);
                        else Console.Write((char)('A' + (text[i] + sh) % 26));
                    }
                }
                Console.WriteLine();
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)enc);
            }
        }
    }
}