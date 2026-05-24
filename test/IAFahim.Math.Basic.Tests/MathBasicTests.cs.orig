namespace IAFahim.Math.Basic.Tests
{
    using Xunit;

    public sealed unsafe class CeilDivTests
    {
        [Fact]
        public void Positive_Normal()
        {
            Assert.Equal(4, CeilDiv.Run(7, 2));
        }

        [Fact]
        public void ExactDivision()
        {
            Assert.Equal(4, CeilDiv.Run(8, 2));
        }

        [Fact]
        public void One()
        {
            Assert.Equal(1, CeilDiv.Run(1, 5));
        }

        [Fact]
        public void Large()
        {
            Assert.Equal(1000001L, CeilDiv.Run(10000001L, 10L));
        }

        [Fact]
        public void NegativeDivisors()
        {
            Assert.Equal(-3, CeilDiv.Run(-7, 2));
            Assert.Equal(-4, CeilDiv.Run(-8, 2));
            Assert.Equal(-3, CeilDiv.Run(7, -2));
            Assert.Equal(-4, CeilDiv.Run(8, -2));
            Assert.Equal(4, CeilDiv.Run(-7, -2));
            Assert.Equal(4, CeilDiv.Run(-8, -2));
        }
    }

    public sealed unsafe class FloorDivTests
    {
        [Fact]
        public void Positive_Normal()
        {
            Assert.Equal(3, FloorDiv.Run(7, 2));
        }

        [Fact]
        public void ExactDivision()
        {
            Assert.Equal(4, FloorDiv.Run(8, 2));
        }

        [Fact]
        public void NegativeNumerator()
        {
            Assert.Equal(-4, FloorDiv.Run(-7, 2));
        }

        [Fact]
        public void NegativeDenominator()
        {
            Assert.Equal(-4, FloorDiv.Run(7, -2));
        }
    }

    public sealed unsafe class AbsIntTests
    {
        [Fact]
        public void Positive()
        {
            Assert.Equal(42, AbsInt.Run(42));
        }

        [Fact]
        public void Negative()
        {
            Assert.Equal(42, AbsInt.Run(-42));
        }

        [Fact]
        public void Zero()
        {
            Assert.Equal(0, AbsInt.Run(0));
        }
    }

    public sealed unsafe class AbsInt64Tests
    {
        [Fact]
        public void Positive()
        {
            Assert.Equal(42L, AbsInt64.Run(42));
        }

        [Fact]
        public void Negative()
        {
            Assert.Equal(42L, AbsInt64.Run(-42));
        }

        [Fact]
        public void Zero()
        {
            Assert.Equal(0L, AbsInt64.Run(0));
        }
    }

    public sealed unsafe class MinIntTests
    {
        [Fact]
        public void FirstIsMin() { Assert.Equal(1, MinInt.Run(1, 5)); }

        [Fact]
        public void SecondIsMin() { Assert.Equal(2, MinInt.Run(5, 2)); }

        [Fact]
        public void Equal() { Assert.Equal(5, MinInt.Run(5, 5)); }
    }

    public sealed unsafe class MaxIntTests
    {
        [Fact]
        public void FirstIsMax() { Assert.Equal(5, MaxInt.Run(5, 2)); }

        [Fact]
        public void SecondIsMax() { Assert.Equal(5, MaxInt.Run(2, 5)); }

        [Fact]
        public void Equal() { Assert.Equal(5, MaxInt.Run(5, 5)); }
    }

    public sealed unsafe class MinInt64Tests
    {
        [Fact]
        public void FirstIsMin() { Assert.Equal(1L, MinInt64.Run(1, 5)); }
    }

    public sealed unsafe class MaxInt64Tests
    {
        [Fact]
        public void FirstIsMax() { Assert.Equal(5L, MaxInt64.Run(5, 2)); }
    }

    public sealed unsafe class ClampTests
    {
        [Fact]
        public void InRange() { Assert.Equal(5, Clamp.Run(5, 0, 10)); }

        [Fact]
        public void BelowRange() { Assert.Equal(0, Clamp.Run(-5, 0, 10)); }

        [Fact]
        public void AboveRange() { Assert.Equal(10, Clamp.Run(15, 0, 10)); }
    }

    public sealed unsafe class IntegerSqrtTests
    {
        [Fact]
        public void CorrectSquareRoots()
        {
            Assert.Equal(-1, IntegerSqrt.Run(-5));
            Assert.Equal(0, IntegerSqrt.Run(0));
            Assert.Equal(1, IntegerSqrt.Run(1));
            Assert.Equal(1, IntegerSqrt.Run(2));
            Assert.Equal(2, IntegerSqrt.Run(4));
            Assert.Equal(3, IntegerSqrt.Run(9));
            Assert.Equal(3037000499L, IntegerSqrt.Run(long.MaxValue));
        }
    }

    public sealed unsafe class IntegerCbrtTests
    {
        [Fact]
        public void CorrectCubeRoots()
        {
            Assert.Equal(-1, IntegerCbrt.Run(-5));
            Assert.Equal(0, IntegerCbrt.Run(0));
            Assert.Equal(1, IntegerCbrt.Run(1));
            Assert.Equal(1, IntegerCbrt.Run(7));
            Assert.Equal(2, IntegerCbrt.Run(8));
            Assert.Equal(2097151L, IntegerCbrt.Run(long.MaxValue));
        }
    }

    public sealed unsafe class NthRootTests
    {
        [Fact]
        public void CorrectRoots()
        {
            Assert.Equal(-1, NthRoot.Run(8, 0));
            Assert.Equal(8, NthRoot.Run(8, 1));
            Assert.Equal(2, NthRoot.Run(8, 3));
            Assert.Equal(1, NthRoot.Run(8, 4));
            Assert.Equal(1, NthRoot.Run(1, 100));
            Assert.Equal(2, NthRoot.Run(1024, 10));
            Assert.Equal(0, NthRoot.Run(0, 10));
        }
    }

    public sealed unsafe class IsPerfectSquareTests
    {
        [Fact]
        public void PerfectSquares()
        {
            Assert.True(IsPerfectSquare.Run(0));
            Assert.True(IsPerfectSquare.Run(1));
            Assert.True(IsPerfectSquare.Run(4));
            Assert.True(IsPerfectSquare.Run(9));
            Assert.False(IsPerfectSquare.Run(2));
            Assert.False(IsPerfectSquare.Run(-1));
        }
    }

    public sealed unsafe class PowerOfTwoTests
    {
        [Fact]
        public void IsPowerOfTwo_Tests()
        {
            Assert.True(IsPowerOfTwo.Run(1));
            Assert.True(IsPowerOfTwo.Run(2));
            Assert.True(IsPowerOfTwo.Run(4));
            Assert.True(IsPowerOfTwo.Run(1L << 60));
            Assert.False(IsPowerOfTwo.Run(0));
            Assert.False(IsPowerOfTwo.Run(-2));
            Assert.False(IsPowerOfTwo.Run(3));
        }

        [Fact]
        public void NextPowerOfTwo_Tests()
        {
            Assert.Equal(1, NextPowerOfTwo.Run(0));
            Assert.Equal(1, NextPowerOfTwo.Run(-5));
            Assert.Equal(4, NextPowerOfTwo.Run(3));
            Assert.Equal(4, NextPowerOfTwo.Run(4));
            Assert.Equal(8, NextPowerOfTwo.Run(5));
            Assert.Equal(1L << 60, NextPowerOfTwo.Run((1L << 60) - 1));
        }

        [Fact]
        public void PrevPowerOfTwo_Tests()
        {
            Assert.Equal(0, PrevPowerOfTwo.Run(0));
            Assert.Equal(0, PrevPowerOfTwo.Run(-5));
            Assert.Equal(2, PrevPowerOfTwo.Run(3));
            Assert.Equal(4, PrevPowerOfTwo.Run(4));
            Assert.Equal(4, PrevPowerOfTwo.Run(5));
            Assert.Equal(1L << 62, PrevPowerOfTwo.Run((1L << 62) + 1));
        }
    }

    public sealed unsafe class Log2Tests
    {
        [Fact]
        public void FloorLog2_Tests()
        {
            Assert.Equal(0, FloorLog2.Run(0));
            Assert.Equal(0, FloorLog2.Run(-5));
            Assert.Equal(0, FloorLog2.Run(1));
            Assert.Equal(1, FloorLog2.Run(2));
            Assert.Equal(1, FloorLog2.Run(3));
            Assert.Equal(2, FloorLog2.Run(4));
            Assert.Equal(62, FloorLog2.Run(1L << 62));
        }

        [Fact]
        public void CeilLog2_Tests()
        {
            Assert.Equal(0, CeilLog2.Run(0));
            Assert.Equal(0, CeilLog2.Run(-5));
            Assert.Equal(0, CeilLog2.Run(1));
            Assert.Equal(1, CeilLog2.Run(2));
            Assert.Equal(2, CeilLog2.Run(3));
            Assert.Equal(2, CeilLog2.Run(4));
            Assert.Equal(3, CeilLog2.Run(5));
            Assert.Equal(62, CeilLog2.Run(1L << 62));
        }
    }

    public sealed unsafe class SafeMulModTests
    {
        [Fact]
        public void NormalMulMod()
        {
            Assert.Equal(1L, SafeMulMod.Run(2, 3, 5));
            Assert.Equal(4L, SafeMulMod.Run(-1, 1, 5));
            Assert.Equal(0L, SafeMulMod.Run(10, 10, 5));
        }
    }

    public sealed unsafe class PointerHelpersTests
    {
        [Fact]
        public void TestMinimize()
        {
            long val = 10;
            Minimize.Run(&val, 5);
            Assert.Equal(5, val);
            Minimize.Run(&val, 8);
            Assert.Equal(5, val);
        }

        [Fact]
        public void TestMaximize()
        {
            long val = 10;
            Maximize.Run(&val, 15);
            Assert.Equal(15, val);
            Maximize.Run(&val, 8);
            Assert.Equal(15, val);
        }

        [Fact]
        public void TestRelaxMin()
        {
            long val = 10;
            Assert.True(RelaxMin.Run(&val, 5));
            Assert.Equal(5, val);
            Assert.False(RelaxMin.Run(&val, 8));
            Assert.Equal(5, val);
        }

        [Fact]
        public void TestRelaxMax()
        {
            long val = 10;
            Assert.True(RelaxMax.Run(&val, 15));
            Assert.Equal(15, val);
            Assert.False(RelaxMax.Run(&val, 8));
            Assert.Equal(15, val);
        }

        [Fact]
        public void TestSwapInts()
        {
            int a = 10, b = 20;
            SwapInts.Run(&a, &b);
            Assert.Equal(20, a);
            Assert.Equal(10, b);
        }

        [Fact]
        public void TestSwapPairs()
        {
            long a = 10, b = 20;
            SwapPairs.Run(&a, &b);
            Assert.Equal(20L, a);
            Assert.Equal(10L, b);
        }
    }
}