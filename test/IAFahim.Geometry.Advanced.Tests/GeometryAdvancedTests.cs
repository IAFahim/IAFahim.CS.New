namespace IAFahim.Geometry.Advanced.Tests
{
    using NUnit.Framework;

    public sealed unsafe class ClosestPairTests
    {
        [Test]
        public void TwoPoints_DistanceSquared()
        {
            long* x = stackalloc long[2];
            long* y = stackalloc long[2];
            x[0]=0; y[0]=0; x[1]=3; y[1]=4;
            long d = ClosestPair.Run(2, x, y);
            Assert.AreEqual(25, d);
        }

        [Test]
        public void ThreePoints_Closest()
        {
            long* x = stackalloc long[3];
            long* y = stackalloc long[3];
            x[0]=0; y[0]=0; x[1]=10; y[1]=0; x[2]=1; y[2]=0;
            Assert.AreEqual(1, ClosestPair.Run(3, x, y));
        }
    }
}
