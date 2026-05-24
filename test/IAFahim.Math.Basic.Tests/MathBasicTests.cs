namespace IAFahim.Math.Basic.Tests
{
    using NUnit.Framework;

    public sealed unsafe class CeilDivTests
    {
        [Test]
        public void Positive_Normal()
        {
            Assert.AreEqual(4, CeilDiv.Run(7, 2));
        }

        [Test]
        public void ExactDivision()
        {
            Assert.AreEqual(4, CeilDiv.Run(8, 2));
        }

        [Test]
        public void One()
        {
            Assert.AreEqual(1, CeilDiv.Run(1, 5));
        }

        [Test]
        public void Large()
        {
            Assert.AreEqual(1000001L, CeilDiv.Run(10000001L, 10L));
        }

        [Test]
        public void NegativeDivisors()
        {
            Assert.AreEqual(-3, CeilDiv.Run(-7, 2));
            Assert.AreEqual(-4, CeilDiv.Run(-8, 2));
            Assert.AreEqual(-3, CeilDiv.Run(7, -2));
            Assert.AreEqual(-4, CeilDiv.Run(8, -2));
            Assert.AreEqual(4, CeilDiv.Run(-7, -2));
            Assert.AreEqual(4, CeilDiv.Run(-8, -2));
        }
    }

    public sealed unsafe class FloorDivTests
    {
        [Test]
        public void Positive_Normal()
        {
            Assert.AreEqual(3, FloorDiv.Run(7, 2));
        }

        [Test]
        public void ExactDivision()
        {
            Assert.AreEqual(4, FloorDiv.Run(8, 2));
        }

        [Test]
        public void NegativeNumerator()
        {
            Assert.AreEqual(-4, FloorDiv.Run(-7, 2));
        }

        [Test]
        public void NegativeDenominator()
        {
            Assert.AreEqual(-4, FloorDiv.Run(7, -2));
        }
    }

    public sealed unsafe class AbsIntTests
    {
        [Test]
        public void Positive()
        {
            Assert.AreEqual(42, AbsInt.Run(42));
        }

        [Test]
        public void Negative()
        {
            Assert.AreEqual(42, AbsInt.Run(-42));
        }

        [Test]
        public void Zero()
        {
            Assert.AreEqual(0, AbsInt.Run(0));
        }
    }

    public sealed unsafe class AbsInt64Tests
    {
        [Test]
        public void Positive()
        {
            Assert.AreEqual(42L, AbsInt64.Run(42));
        }

        [Test]
        public void Negative()
        {
            Assert.AreEqual(42L, AbsInt64.Run(-42));
        }

        [Test]
        public void Zero()
        {
            Assert.AreEqual(0L, AbsInt64.Run(0));
        }
    }

    public sealed unsafe class MinIntTests
    {
        [Test]
        public void FirstIsMin() { Assert.AreEqual(1, MinInt.Run(1, 5)); }

        [Test]
        public void SecondIsMin() { Assert.AreEqual(2, MinInt.Run(5, 2)); }

        [Test]
        public void Equal() { Assert.AreEqual(5, MinInt.Run(5, 5)); }
    }

    public sealed unsafe class MaxIntTests
    {
        [Test]
        public void FirstIsMax() { Assert.AreEqual(5, MaxInt.Run(5, 2)); }

        [Test]
        public void SecondIsMax() { Assert.AreEqual(5, MaxInt.Run(2, 5)); }

        [Test]
        public void Equal() { Assert.AreEqual(5, MaxInt.Run(5, 5)); }
    }

    public sealed unsafe class MinInt64Tests
    {
        [Test]
        public void FirstIsMin() { Assert.AreEqual(1L, MinInt64.Run(1, 5)); }
    }

    public sealed unsafe class MaxInt64Tests
    {
        [Test]
        public void FirstIsMax() { Assert.AreEqual(5L, MaxInt64.Run(5, 2)); }
    }

    public sealed unsafe class ClampTests
    {
        [Test]
        public void InRange() { Assert.AreEqual(5, Clamp.Run(5, 0, 10)); }

        [Test]
        public void BelowRange() { Assert.AreEqual(0, Clamp.Run(-5, 0, 10)); }

        [Test]
        public void AboveRange() { Assert.AreEqual(10, Clamp.Run(15, 0, 10)); }
    }

    public sealed unsafe class IntegerSqrtTests
    {
        [Test]
        public void CorrectSquareRoots()
        {
            Assert.AreEqual(-1, IntegerSqrt.Run(-5));
            Assert.AreEqual(0, IntegerSqrt.Run(0));
            Assert.AreEqual(1, IntegerSqrt.Run(1));
            Assert.AreEqual(1, IntegerSqrt.Run(2));
            Assert.AreEqual(2, IntegerSqrt.Run(4));
            Assert.AreEqual(3, IntegerSqrt.Run(9));
            Assert.AreEqual(3037000499L, IntegerSqrt.Run(long.MaxValue));
        }
    }

    public sealed unsafe class IntegerCbrtTests
    {
        [Test]
        public void CorrectCubeRoots()
        {
            Assert.AreEqual(-1, IntegerCbrt.Run(-5));
            Assert.AreEqual(0, IntegerCbrt.Run(0));
            Assert.AreEqual(1, IntegerCbrt.Run(1));
            Assert.AreEqual(1, IntegerCbrt.Run(7));
            Assert.AreEqual(2, IntegerCbrt.Run(8));
            Assert.AreEqual(2097151L, IntegerCbrt.Run(long.MaxValue));
        }
    }

    public sealed unsafe class NthRootTests
    {
        [Test]
        public void CorrectRoots()
        {
            Assert.AreEqual(-1, NthRoot.Run(8, 0));
            Assert.AreEqual(8, NthRoot.Run(8, 1));
            Assert.AreEqual(2, NthRoot.Run(8, 3));
            Assert.AreEqual(1, NthRoot.Run(8, 4));
            Assert.AreEqual(1, NthRoot.Run(1, 100));
            Assert.AreEqual(2, NthRoot.Run(1024, 10));
            Assert.AreEqual(0, NthRoot.Run(0, 10));
        }
    }

    public sealed unsafe class IsPerfectSquareTests
    {
        [Test]
        public void PerfectSquares()
        {
            Assert.IsTrue(IsPerfectSquare.Run(0));
            Assert.IsTrue(IsPerfectSquare.Run(1));
            Assert.IsTrue(IsPerfectSquare.Run(4));
            Assert.IsTrue(IsPerfectSquare.Run(9));
            Assert.IsFalse(IsPerfectSquare.Run(2));
            Assert.IsFalse(IsPerfectSquare.Run(-1));
        }
    }

    public sealed unsafe class PowerOfTwoTests
    {
        [Test]
        public void IsPowerOfTwo_Tests()
        {
            Assert.IsTrue(IsPowerOfTwo.Run(1));
            Assert.IsTrue(IsPowerOfTwo.Run(2));
            Assert.IsTrue(IsPowerOfTwo.Run(4));
            Assert.IsTrue(IsPowerOfTwo.Run(1L << 60));
            Assert.IsFalse(IsPowerOfTwo.Run(0));
            Assert.IsFalse(IsPowerOfTwo.Run(-2));
            Assert.IsFalse(IsPowerOfTwo.Run(3));
        }

        [Test]
        public void NextPowerOfTwo_Tests()
        {
            Assert.AreEqual(1, NextPowerOfTwo.Run(0));
            Assert.AreEqual(1, NextPowerOfTwo.Run(-5));
            Assert.AreEqual(4, NextPowerOfTwo.Run(3));
            Assert.AreEqual(4, NextPowerOfTwo.Run(4));
            Assert.AreEqual(8, NextPowerOfTwo.Run(5));
            Assert.AreEqual(1L << 60, NextPowerOfTwo.Run((1L << 60) - 1));
        }

        [Test]
        public void PrevPowerOfTwo_Tests()
        {
            Assert.AreEqual(0, PrevPowerOfTwo.Run(0));
            Assert.AreEqual(0, PrevPowerOfTwo.Run(-5));
            Assert.AreEqual(2, PrevPowerOfTwo.Run(3));
            Assert.AreEqual(4, PrevPowerOfTwo.Run(4));
            Assert.AreEqual(4, PrevPowerOfTwo.Run(5));
            Assert.AreEqual(1L << 62, PrevPowerOfTwo.Run((1L << 62) + 1));
        }
    }

    public sealed unsafe class Log2Tests
    {
        [Test]
        public void FloorLog2_Tests()
        {
            Assert.AreEqual(0, FloorLog2.Run(0));
            Assert.AreEqual(0, FloorLog2.Run(-5));
            Assert.AreEqual(0, FloorLog2.Run(1));
            Assert.AreEqual(1, FloorLog2.Run(2));
            Assert.AreEqual(1, FloorLog2.Run(3));
            Assert.AreEqual(2, FloorLog2.Run(4));
            Assert.AreEqual(62, FloorLog2.Run(1L << 62));
        }

        [Test]
        public void CeilLog2_Tests()
        {
            Assert.AreEqual(0, CeilLog2.Run(0));
            Assert.AreEqual(0, CeilLog2.Run(-5));
            Assert.AreEqual(0, CeilLog2.Run(1));
            Assert.AreEqual(1, CeilLog2.Run(2));
            Assert.AreEqual(2, CeilLog2.Run(3));
            Assert.AreEqual(2, CeilLog2.Run(4));
            Assert.AreEqual(3, CeilLog2.Run(5));
            Assert.AreEqual(62, CeilLog2.Run(1L << 62));
        }
    }

    public sealed unsafe class SafeMulModTests
    {
        [Test]
        public void NormalMulMod()
        {
            Assert.AreEqual(1L, SafeMulMod.Run(2, 3, 5));
            Assert.AreEqual(4L, SafeMulMod.Run(-1, 1, 5));
            Assert.AreEqual(0L, SafeMulMod.Run(10, 10, 5));
        }
    }

    public sealed unsafe class PointerHelpersTests
    {
        [Test]
        public void TestMinimize()
        {
            long val = 10;
            Minimize.Run(&val, 5);
            Assert.AreEqual(5, val);
            Minimize.Run(&val, 8);
            Assert.AreEqual(5, val);
        }

        [Test]
        public void TestMaximize()
        {
            long val = 10;
            Maximize.Run(&val, 15);
            Assert.AreEqual(15, val);
            Maximize.Run(&val, 8);
            Assert.AreEqual(15, val);
        }

        [Test]
        public void TestRelaxMin()
        {
            long val = 10;
            Assert.IsTrue(RelaxMin.Run(&val, 5));
            Assert.AreEqual(5, val);
            Assert.IsFalse(RelaxMin.Run(&val, 8));
            Assert.AreEqual(5, val);
        }

        [Test]
        public void TestRelaxMax()
        {
            long val = 10;
            Assert.IsTrue(RelaxMax.Run(&val, 15));
            Assert.AreEqual(15, val);
            Assert.IsFalse(RelaxMax.Run(&val, 8));
            Assert.AreEqual(15, val);
        }

        [Test]
        public void TestSwapInts()
        {
            int a = 10, b = 20;
            SwapInts.Run(&a, &b);
            Assert.AreEqual(20, a);
            Assert.AreEqual(10, b);
        }

        [Test]
        public void TestSwapPairs()
        {
            long a = 10, b = 20;
            SwapPairs.Run(&a, &b);
            Assert.AreEqual(20L, a);
            Assert.AreEqual(10L, b);
        }
    }
}