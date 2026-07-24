namespace IAFahim.Graph.Functional.Tests
{
    using NUnit.Framework;

    public sealed unsafe class FunctionalGraphTests
    {
        [Test]
        public void KthSuccessor_Cycle()
        {
            int* f = stackalloc int[3];
            f[0] = 1; f[1] = 2; f[2] = 0;
            Assert.AreEqual(0, FunctionalGraphKthSuccessor.Run(f, 3, 0, 0));
            Assert.AreEqual(1, FunctionalGraphKthSuccessor.Run(f, 3, 0, 1));
            Assert.AreEqual(2, FunctionalGraphKthSuccessor.Run(f, 3, 0, 2));
            Assert.AreEqual(0, FunctionalGraphKthSuccessor.Run(f, 3, 0, 3));
            Assert.AreEqual(1, FunctionalGraphKthSuccessor.Run(f, 3, 0, 4));
        }
    }
}
