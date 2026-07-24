namespace IAFahim.String.Compress.Tests
{
    using System.Text;
    using NUnit.Framework;

    public sealed unsafe class MoveToFrontTests
    {
        [Test]
        public void EncodeDecode_RoundTrip()
        {
            byte[] s = Encoding.ASCII.GetBytes("banana");
            const int N = 6;
            byte* enc = stackalloc byte[N];
            byte* dec = stackalloc byte[N];
            fixed (byte* p = s)
            {
                MoveToFront.Encode(p, N, enc, 256);
                MoveToFront.Decode(enc, N, dec, 256);
            }
            for (int i = 0; i < N; i++)
                Assert.AreEqual(s[i], dec[i]);
        }

        [Test]
        public void Encode_FirstOccurrenceIsValue()
        {
            byte* inp = stackalloc byte[3];
            byte* outp = stackalloc byte[3];
            inp[0] = 2; inp[1] = 0; inp[2] = 2;
            MoveToFront.Encode(inp, 3, outp, 4);
            Assert.AreEqual(2, outp[0]);
            Assert.AreEqual(1, outp[1]);
            Assert.AreEqual(1, outp[2]);
        }
    }
}
