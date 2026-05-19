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
            Assert.Equal(24, ModPow.Run(2, 10, 1000));
            Assert.Equal(8, ModPow.Run(2, 3, 100));
            Assert.Equal(27, ModPow.Run(3, 3, 1000));
            Assert.Equal(1, ModPow.Run(5, 0, 100));
            Assert.Equal(7, ModPow.Run(7, 1, 100));
            Assert.Equal(65536, ModPow.Run(2, 16, 1000000007));
        }

        [Fact]
        public void ModPow_ModularInverseViaEuler()
        {
            // 3^(-1) mod 26 = 3^11 mod 26 = 9 (since phi(26)=12, inv = 3^(11))
            long inv3 = ModPow.Run(3, 11, 26);
            Assert.Equal(9, inv3);
            Assert.Equal(1, (3 * inv3) % 26);
        }

        [Fact]
        public void ModPow_LargeExponent()
        {
            // 2^30 mod (1e9+7)
            long expected = 1;
            for (int i = 0; i < 30; i++) expected = (expected * 2) % 1000000007;
            Assert.Equal(expected, ModPow.Run(2, 30, 1000000007));
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