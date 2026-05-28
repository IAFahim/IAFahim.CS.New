namespace IAFahim.String.Match.Tests
{
    using System;
    using IAFahim.String.Compress;
    using NUnit.Framework;

    public sealed unsafe class CompressTests
    {
        [Test]
        public void Lz78_Roundtrip()
        {
            byte[] input = new byte[] { 97, 98, 97, 98, 97, 99 };
            fixed (byte* inp = input)
            {
                Lz78.Token* tokens = stackalloc Lz78.Token[6];
                int count = Lz78.Encode(inp, 6, tokens);
                byte* output = stackalloc byte[6];
                int outLen = Lz78.Decode(tokens, count, output);
                Assert.AreEqual(6, outLen);
                for (int i = 0; i < 6; i++)
                    Assert.AreEqual(input[i], output[i]);
            }
        }

        [Test]
        public void LzFactorization_NonEmpty()
        {
            byte[] input = new byte[] { 97, 98, 97, 98, 97, 99 };
            fixed (byte* inp = input)
            {
                LzFactorization.Factor* factors = stackalloc LzFactorization.Factor[6];
                int count = LzFactorization.Factorize(inp, 6, factors);
                Assert.IsTrue(count > 0);
                Assert.IsTrue(count <= 6);
            }
        }

        [Test]
        public void Arithmetic_EncodeDecode_Correct()
        {
            byte[] input = new byte[] { (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o' };
            long* output = stackalloc long[1000];
            int outLen = 0;
            long precision = 1L << 32;
            
            fixed (byte* inPtr = input)
            {
                Arithmetic.Encode(inPtr, 5, output, &outLen, precision);
            }
            
            byte* decoded = stackalloc byte[10];
            int decodedLen = 0;
            
            Arithmetic.Decode(output, outLen, decoded, &decodedLen, precision);
            
            Assert.AreEqual(5, decodedLen);
            Assert.AreEqual((byte)'h', decoded[0]);
            Assert.AreEqual((byte)'e', decoded[1]);
            Assert.AreEqual((byte)'l', decoded[2]);
            Assert.AreEqual((byte)'l', decoded[3]);
            Assert.AreEqual((byte)'o', decoded[4]);
        }
    }
}
