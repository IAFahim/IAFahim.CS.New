namespace IAFahim.String.Compress.Lz77.Tests
{
    using IAFahim.String.Compress;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class Lz77Tests
    {
        [Test]
        public void EncodeDecode_RoundTrip_RandomAndRepetitive()
        {
            Random rng = new Random(17);
            for (int t = 0; t < 40; t++)
            {
                int len = rng.Next(1, 4000);
                byte[] src = new byte[len];
                int alphabet = rng.Next(2, 256);
                for (int i = 0; i < len; i++) src[i] = (byte)rng.Next(0, alphabet);

                Lz77.Token* tokens = (Lz77.Token*)Marshal.AllocHGlobal(sizeof(Lz77.Token) * len);
                byte* outBuf = (byte*)Marshal.AllocHGlobal(len);
                try
                {
                    fixed (byte* sp = src)
                    {
                        int n = Lz77.Encode(sp, len, tokens, 4096);
                        int decoded = Lz77.Decode(tokens, n, outBuf);
                        Assert.AreEqual(len, decoded, $"decoded length t={t}");
                        for (int i = 0; i < len; i++)
                            Assert.AreEqual(src[i], outBuf[i], $"byte mismatch t={t} i={i}");
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)tokens);
                    Marshal.FreeHGlobal((nint)outBuf);
                }
            }
        }

        [Test]
        public void Encode_MatchesGreedyBrute_SmallWindow()
        {
            int[] text = { 1, 2, 3, 1, 2, 3, 1, 2, 3, 4, 1, 2, 3, 1, 2, 3 };
            int len = text.Length;
            byte[] src = new byte[len];
            for (int i = 0; i < len; i++) src[i] = (byte)text[i];

            Lz77.Token* fast = (Lz77.Token*)Marshal.AllocHGlobal(sizeof(Lz77.Token) * len);
            Lz77.Token* brute = (Lz77.Token*)Marshal.AllocHGlobal(sizeof(Lz77.Token) * len);
            try
            {
                fixed (byte* sp = src)
                {
                    int nFast = Lz77.Encode(sp, len, fast, 1024);
                    int nBrute = EncodeBrute(sp, len, brute, 1024);
                    Assert.AreEqual(nBrute, nFast, "token count: greedy hash-chain must match greedy brute");
                    for (int i = 0; i < nFast; i++)
                    {
                        Assert.AreEqual(brute[i].Length, fast[i].Length, $"tok {i} length (coverage must match greedy)");
                        if (brute[i].Length == 0)
                            Assert.AreEqual(brute[i].Literal, fast[i].Literal, $"tok {i} literal");
                    }
                    byte* check = (byte*)Marshal.AllocHGlobal(len);
                    try { int d = Lz77.Decode(fast, nFast, check); Assert.AreEqual(len, d); }
                    finally { Marshal.FreeHGlobal((nint)check); }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)fast);
                Marshal.FreeHGlobal((nint)brute);
            }
        }

        private static int EncodeBrute(byte* input, int len, Lz77.Token* output, int windowSize)
        {
            int outCount = 0;
            int i = 0;
            while (i < len)
            {
                int bestLen = 0, bestDist = 0;
                int start = i - windowSize; if (start < 0) start = 0;
                for (int j = start; j < i; j++)
                {
                    int l = 0;
                    while (i + l < len && input[j + l] == input[i + l]) { l++; if (l >= 255) break; }
                    if (l > bestLen) { bestLen = l; bestDist = i - j; }
                }
                if (bestLen >= 2) { output[outCount++] = new Lz77.Token { Offset = bestDist, Length = bestLen }; i += bestLen; }
                else { output[outCount++] = new Lz77.Token { Literal = input[i] }; i++; }
            }
            return outCount;
        }
    }
}
