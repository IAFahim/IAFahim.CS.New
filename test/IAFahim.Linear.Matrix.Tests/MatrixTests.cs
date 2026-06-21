namespace IAFahim.Linear.Matrix.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class MatrixTests
    {
        [Test]
        public void Mul_Identity_NoChange()
        {
            long* a = stackalloc long[4] { 1, 2, 3, 4 };
            long* id = stackalloc long[4] { 1, 0, 0, 1 };
            long* c = stackalloc long[4];
            MatrixMul.Run(2, 2, 2, a, id, c);
            Assert.AreEqual(new long[] { 1, 2, 3, 4 }, AsArray(c, 4));
        }

        [Test]
        public void Mul_TwoByTwo_KnownProduct()
        {
            // [[1,2],[3,4]] * [[5,6],[7,8]] = [[19,22],[43,50]]
            long* a = stackalloc long[4] { 1, 2, 3, 4 };
            long* b = stackalloc long[4] { 5, 6, 7, 8 };
            long* c = stackalloc long[4];
            MatrixMul.Run(2, 2, 2, a, b, c);
            Assert.AreEqual(new long[] { 19, 22, 43, 50 }, AsArray(c, 4));
        }

        [Test]
        public void Mul_NonSquare_2x3Times3x2()
        {
            // [[1,2,3],[4,5,6]] (2x3) * [[1,0],[0,1],[1,1]] (3x2) = [[4,5],[10,11]]
            long* a = stackalloc long[6] { 1, 2, 3, 4, 5, 6 };
            long* b = stackalloc long[6] { 1, 0, 0, 1, 1, 1 };
            long* c = stackalloc long[4];
            MatrixMul.Run(2, 3, 2, a, b, c);
            Assert.AreEqual(new long[] { 4, 5, 10, 11 }, AsArray(c, 4));
        }

        [Test]
        public void Pow_Identity_Exp0()
        {
            long* a = stackalloc long[4] { 1, 2, 3, 4 };
            long* r = stackalloc long[4];
            MatrixPow.Run(2, a, r, 0);
            Assert.AreEqual(new long[] { 1, 0, 0, 1 }, AsArray(r, 4));
        }

        [Test]
        public void Pow_Exp1_ReturnsSame()
        {
            long* a = stackalloc long[4] { 1, 2, 3, 4 };
            long* r = stackalloc long[4];
            MatrixPow.Run(2, a, r, 1);
            Assert.AreEqual(new long[] { 1, 2, 3, 4 }, AsArray(r, 4));
        }

        [Test]
        public void Pow_Fibonacci_KnownValue()
        {
            // [[1,1],[1,0]]^n gives Fibonacci. [[1,1],[1,0]]^10 = [[89,55],[55,34]].
            long* a = stackalloc long[4] { 1, 1, 1, 0 };
            long* r = stackalloc long[4];
            MatrixPow.Run(2, a, r, 10);
            Assert.AreEqual(new long[] { 89, 55, 55, 34 }, AsArray(r, 4));
        }

        [Test]
        public void Determinant_2x2_Correct()
        {
            // det([[1,2],[3,4]]) = 1*4 - 2*3 = -2.
            long* a = stackalloc long[4] { 1, 2, 3, 4 };
            Assert.AreEqual(-2, MatrixDeterminant.Run(2, a));
        }

        [Test]
        public void Determinant_3x3_Correct()
        {
            // det([[6,1,1],[4,-2,5],[2,8,7]]) = 6*(-14-40) - 1*(28-10) + 1*(32+4) = -306.
            long* a = stackalloc long[9] { 6, 1, 1, 4, -2, 5, 2, 8, 7 };
            Assert.AreEqual(-306, MatrixDeterminant.Run(3, a));
        }

        [Test]
        public void Determinant_Identity_One()
        {
            long* a = stackalloc long[4] { 1, 0, 0, 1 };
            Assert.AreEqual(1, MatrixDeterminant.Run(2, a));
        }

        [Test]
        public void Determinant_Singular_Zero()
        {
            // det([[1,2],[2,4]]) = 0 (rank 1).
            long* a = stackalloc long[4] { 1, 2, 2, 4 };
            Assert.AreEqual(0, MatrixDeterminant.Run(2, a));
        }

        [Test]
        public unsafe void Determinant_EmptyMatrix_One()
        {
            long* a = null;
            Assert.AreEqual(1, MatrixDeterminant.Run(0, a));
        }

        [Test]
        public void Determinant_UpperTriangular_ProductOfDiagonal()
        {
            // det([[2,3,5],[0,4,6],[0,0,7]]) = 2*4*7 = 56.
            long* a = stackalloc long[9] { 2, 3, 5, 0, 4, 6, 0, 0, 7 };
            Assert.AreEqual(56, MatrixDeterminant.Run(3, a));
        }

        private static long[] AsArray(long* p, int n)
        {
            long[] a = new long[n];
            for (int i = 0; i < n; i++) a[i] = p[i];
            return a;
        }
    }
}
