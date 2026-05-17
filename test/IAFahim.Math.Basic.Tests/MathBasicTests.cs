namespace IAFahim.Math.Basic.Tests
{
    using Xunit;

    public sealed unsafe class CeilDivTests
    {
        [Fact]
        public void Positive_Normal()
        {
            Assert.Equal(4, IAFahim.Math.Basic.CeilDiv.Run(7, 2));
        }

        [Fact]
        public void ExactDivision()
        {
            Assert.Equal(4, IAFahim.Math.Basic.CeilDiv.Run(8, 2));
        }

        [Fact]
        public void One()
        {
            Assert.Equal(1, IAFahim.Math.Basic.CeilDiv.Run(1, 5));
        }

        [Fact]
        public void Large()
        {
            Assert.Equal(1000001, IAFahim.Math.Basic.CeilDiv.Run(10000001, 10));
        }
    }

    public sealed unsafe class FloorDivTests
    {
        [Fact]
        public void Positive_Normal()
        {
            Assert.Equal(3, IAFahim.Math.Basic.FloorDiv.Run(7, 2));
        }

        [Fact]
        public void ExactDivision()
        {
            Assert.Equal(4, IAFahim.Math.Basic.FloorDiv.Run(8, 2));
        }

        [Fact]
        public void NegativeNumerator()
        {
            Assert.Equal(-4, IAFahim.Math.Basic.FloorDiv.Run(-7, 2));
        }

        [Fact]
        public void NegativeDenominator()
        {
            Assert.Equal(-4, IAFahim.Math.Basic.FloorDiv.Run(7, -2));
        }
    }

    public sealed unsafe class AbsIntTests
    {
        [Fact]
        public void Positive()
        {
            Assert.Equal(42, IAFahim.Math.Basic.AbsInt.Run(42));
        }

        [Fact]
        public void Negative()
        {
            Assert.Equal(42, IAFahim.Math.Basic.AbsInt.Run(-42));
        }

        [Fact]
        public void Zero()
        {
            Assert.Equal(0, IAFahim.Math.Basic.AbsInt.Run(0));
        }
    }

    public sealed unsafe class AbsInt64Tests
    {
        [Fact]
        public void Positive()
        {
            Assert.Equal(42L, IAFahim.Math.Basic.AbsInt64.Run(42));
        }

        [Fact]
        public void Negative()
        {
            Assert.Equal(42L, IAFahim.Math.Basic.AbsInt64.Run(-42));
        }

        [Fact]
        public void Zero()
        {
            Assert.Equal(0L, IAFahim.Math.Basic.AbsInt64.Run(0));
        }
    }

    public sealed unsafe class MinIntTests
    {
        [Fact]
        public void FirstIsMin() { Assert.Equal(1, IAFahim.Math.Basic.MinInt.Run(1, 5)); }

        [Fact]
        public void SecondIsMin() { Assert.Equal(2, IAFahim.Math.Basic.MinInt.Run(5, 2)); }

        [Fact]
        public void Equal() { Assert.Equal(5, IAFahim.Math.Basic.MinInt.Run(5, 5)); }
    }

    public sealed unsafe class MaxIntTests
    {
        [Fact]
        public void FirstIsMax() { Assert.Equal(5, IAFahim.Math.Basic.MaxInt.Run(5, 2)); }

        [Fact]
        public void SecondIsMax() { Assert.Equal(5, IAFahim.Math.Basic.MaxInt.Run(2, 5)); }

        [Fact]
        public void Equal() { Assert.Equal(5, IAFahim.Math.Basic.MaxInt.Run(5, 5)); }
    }

    public sealed unsafe class MinInt64Tests
    {
        [Fact]
        public void FirstIsMin() { Assert.Equal(1L, IAFahim.Math.Basic.MinInt64.Run(1, 5)); }
    }

    public sealed unsafe class MaxInt64Tests
    {
        [Fact]
        public void FirstIsMax() { Assert.Equal(5L, IAFahim.Math.Basic.MaxInt64.Run(5, 2)); }
    }

    public sealed unsafe class ClampTests
    {
        [Fact]
        public void InRange() { Assert.Equal(5, IAFahim.Math.Basic.Clamp.Run(5, 0, 10)); }

        [Fact]
        public void BelowRange() { Assert.Equal(0, IAFahim.Math.Basic.Clamp.Run(-5, 0, 10)); }

        [Fact]
        public void AboveRange() { Assert.Equal(10, IAFahim.Math.Basic.Clamp.Run(15, 0, 10)); }
    }
}