namespace IAFahim.Optimization.Approximation.Tests
{
    using NUnit.Framework;

    public sealed unsafe class FreivaldsTests
    {
        [Test]
        public void Verify_CorrectProduct()
        {
            const int N = 2;
            int* a = stackalloc int[4];
            int* b = stackalloc int[4];
            int* c = stackalloc int[4];
            int* r = stackalloc int[2];
            uint seed = 42;
            a[0]=1; a[1]=2; a[2]=3; a[3]=4;
            b[0]=5; b[1]=6; b[2]=7; b[3]=8;
            // C = A*B = [[19,22],[43,50]]
            c[0]=19; c[1]=22; c[2]=43; c[3]=50;
            Assert.IsTrue(Freivalds.Verify(N, a, b, c, r, 20, &seed));
        }

        [Test]
        public void Verify_WrongProduct_False()
        {
            const int N = 2;
            int* a = stackalloc int[4];
            int* b = stackalloc int[4];
            int* c = stackalloc int[4];
            int* r = stackalloc int[2];
            uint seed = 7;
            a[0]=1; a[1]=0; a[2]=0; a[3]=1;
            b[0]=1; b[1]=0; b[2]=0; b[3]=1;
            c[0]=0; c[1]=0; c[2]=0; c[3]=0;
            Assert.IsFalse(Freivalds.Verify(N, a, b, c, r, 30, &seed));
        }
    }
}
