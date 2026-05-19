namespace AlgoArena.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;
    using IAFahim.Math.Modular;
    using IAFahim.String;

    public sealed unsafe class CipherMachineTests
    {
        [Theory]
        [InlineData("Hello", 42, "Hello")]
        [InlineData("Test", 7, "Test")]
        [InlineData("ABC", 255, "ABC")]
        public void XorCipher_RoundTrip(string plaintext, int key, string expected)
        {
            int len = plaintext.Length;
            byte* original = (byte*)Marshal.AllocHGlobal(len);
            byte* encoded = (byte*)Marshal.AllocHGlobal(len);
            byte* decoded = (byte*)Marshal.AllocHGlobal(len);
            try
            {
                for (int i = 0; i < len; i++) original[i] = (byte)plaintext[i];

                for (int i = 0; i < len; i++) encoded[i] = (byte)(original[i] ^ key);
                for (int i = 0; i < len; i++) decoded[i] = (byte)(encoded[i] ^ key);

                for (int i = 0; i < len; i++)
                    Assert.Equal((byte)expected[i], decoded[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)original);
                Marshal.FreeHGlobal((nint)encoded);
                Marshal.FreeHGlobal((nint)decoded);
            }
        }

        [Fact]
        public void ModPow_SimplePowers()
        {
            Assert.Equal(8, ModPow.Run(2, 3, 100));
            Assert.Equal(27, ModPow.Run(3, 3, 1000));
            Assert.Equal(1, ModPow.Run(5, 0, 100));
            Assert.Equal(7, ModPow.Run(7, 1, 100));
        }

        [Theory]
        [InlineData(1, 26, 1)]
        public void ModInverse_KnownValues(long a, long mod, long expected)
        {
            long inv = ModPow.Run(a, mod - 2, mod);
            Assert.Equal(expected, inv);
            Assert.Equal(1, (a * inv) % mod);
        }

        [Fact]
        public void CaesarShift_Encrypt()
        {
            string plaintext = "ABC";
            int shift = 3;
            char[] expected = { 'D', 'E', 'F' };

            int len = plaintext.Length;
            byte* text = (byte*)Marshal.AllocHGlobal(len);
            long* enc = (long*)Marshal.AllocHGlobal(len);
            try
            {
                for (int i = 0; i < len; i++) text[i] = (byte)(plaintext[i] - 'A');
                for (int i = 0; i < len; i++)
                    enc[i] = (text[i] + shift) % 26;

                for (int i = 0; i < len; i++)
                    Assert.Equal(expected[i], (char)('A' + enc[i]));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)enc);
            }
        }
    }
}