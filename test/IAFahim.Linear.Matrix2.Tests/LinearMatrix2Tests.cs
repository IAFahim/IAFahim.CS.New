namespace IAFahim.Linear.Matrix2.Tests
{
    using NUnit.Framework;

    public sealed unsafe class Matrix2Tests
    {
        [Test]
        public void Identity_DiagonalOnes()
        {
            long* a = stackalloc long[9];
            MatrixIdentity.Run(3, a);
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Assert.AreEqual(i == j ? 1L : 0L, a[i * 3 + j]);
        }

        [Test]
        public void Mul_Identity()
        {
            long* a = stackalloc long[4];
            long* b = stackalloc long[4];
            long* c = stackalloc long[4];
            a[0] = 1; a[1] = 2; a[2] = 3; a[3] = 4;
            MatrixIdentity.Run(2, b);
            MatrixMul.Run(2, 2, 2, a, b, c);
            Assert.AreEqual(1, c[0]); Assert.AreEqual(2, c[1]);
            Assert.AreEqual(3, c[2]); Assert.AreEqual(4, c[3]);
        }
    }
}
