namespace IAFahim.Math.Arithmetic.Tests
{
    using NUnit.Framework;

    public sealed unsafe class TryAddTests
    {
        [Test]
        public void Normal_Int()
        {
            int res;
            Assert.IsTrue(TryAdd.Run(5, 10, out res));
            Assert.AreEqual(15, res);

            Assert.IsTrue(TryAdd.Run(-5, -10, out res));
            Assert.AreEqual(-15, res);
        }

        [Test]
        public void Overflow_Int()
        {
            int res;
            Assert.IsFalse(TryAdd.Run(int.MaxValue, 1, out res));
            Assert.IsFalse(TryAdd.Run(int.MinValue, -1, out res));
        }

        [Test]
        public void Normal_Long()
        {
            long res;
            Assert.IsTrue(TryAdd.Run(5L, 10L, out res));
            Assert.AreEqual(15L, res);
        }

        [Test]
        public void Overflow_Long()
        {
            long res;
            Assert.IsFalse(TryAdd.Run(long.MaxValue, 1L, out res));
            Assert.IsFalse(TryAdd.Run(long.MinValue, -1L, out res));
        }
    }

    public sealed unsafe class TrySubTests
    {
        [Test]
        public void Normal_Int()
        {
            int res;
            Assert.IsTrue(TrySub.Run(10, 5, out res));
            Assert.AreEqual(5, res);
        }

        [Test]
        public void Overflow_Int()
        {
            int res;
            Assert.IsFalse(TrySub.Run(int.MinValue, 1, out res));
            Assert.IsFalse(TrySub.Run(int.MaxValue, -1, out res));
        }

        [Test]
        public void Normal_Long()
        {
            long res;
            Assert.IsTrue(TrySub.Run(10L, 5L, out res));
            Assert.AreEqual(5L, res);
        }

        [Test]
        public void Overflow_Long()
        {
            long res;
            Assert.IsFalse(TrySub.Run(long.MinValue, 1L, out res));
            Assert.IsFalse(TrySub.Run(long.MaxValue, -1L, out res));
        }
    }

    public sealed unsafe class TryMulTests
    {
        [Test]
        public void Normal_Int()
        {
            int res;
            Assert.IsTrue(TryMul.Run(5, 6, out res));
            Assert.AreEqual(30, res);

            Assert.IsTrue(TryMul.Run(0, 100, out res));
            Assert.AreEqual(0, res);
        }

        [Test]
        public void Overflow_Int()
        {
            int res;
            Assert.IsFalse(TryMul.Run(int.MaxValue, 2, out res));
            Assert.IsFalse(TryMul.Run(int.MinValue, 2, out res));
        }

        [Test]
        public void Normal_Long()
        {
            long res;
            Assert.IsTrue(TryMul.Run(5L, 6L, out res));
            Assert.AreEqual(30L, res);
        }

        [Test]
        public void Overflow_Long()
        {
            long res;
            Assert.IsFalse(TryMul.Run(long.MaxValue, 2L, out res));
            Assert.IsFalse(TryMul.Run(long.MinValue, 2L, out res));
            Assert.IsFalse(TryMul.Run(long.MinValue, -1L, out res));
        }
    }

    public sealed unsafe class TryDivTests
    {
        [Test]
        public void Normal_Int()
        {
            int res;
            Assert.IsTrue(TryDiv.Run(10, 2, out res));
            Assert.AreEqual(5, res);
        }

        [Test]
        public void DivisionByZero_Int()
        {
            int res;
            Assert.IsFalse(TryDiv.Run(10, 0, out res));
        }

        [Test]
        public void Overflow_Int()
        {
            int res;
            Assert.IsFalse(TryDiv.Run(int.MinValue, -1, out res));
        }

        [Test]
        public void Normal_Long()
        {
            long res;
            Assert.IsTrue(TryDiv.Run(10L, 2L, out res));
            Assert.AreEqual(5L, res);
        }

        [Test]
        public void DivisionByZero_Long()
        {
            long res;
            Assert.IsFalse(TryDiv.Run(10L, 0L, out res));
        }

        [Test]
        public void Overflow_Long()
        {
            long res;
            Assert.IsFalse(TryDiv.Run(long.MinValue, -1L, out res));
        }
    }
}
