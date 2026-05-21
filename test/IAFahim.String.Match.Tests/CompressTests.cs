namespace IAFahim.String.Match.Tests
{
    using System;
    using IAFahim.String.Compress;
    using Xunit;

    public sealed unsafe class CompressTests
    {
        [Fact]
        public void Lz78_Roundtrip()
        {
            byte[] input = new byte[] { 97, 98, 97, 98, 97, 99 };
            fixed (byte* inp = input)
            {
                Lz78.Token* tokens = stackalloc Lz78.Token[6];
                int count = Lz78.Encode(inp, 6, tokens);
                byte* output = stackalloc byte[6];
                int outLen = Lz78.Decode(tokens, count, output);
                Assert.Equal(6, outLen);
                for (int i = 0; i < 6; i++)
                    Assert.Equal(input[i], output[i]);
            }
        }

        [Fact]
        public void LzFactorization_NonEmpty()
        {
            byte[] input = new byte[] { 97, 98, 97, 98, 97, 99 };
            fixed (byte* inp = input)
            {
                LzFactorization.Factor* factors = stackalloc LzFactorization.Factor[6];
                int count = LzFactorization.Factorize(inp, 6, factors);
                Assert.True(count > 0);
                Assert.True(count <= 6);
            }
        }
    }
}
