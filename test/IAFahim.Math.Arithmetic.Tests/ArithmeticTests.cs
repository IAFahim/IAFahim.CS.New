namespace IAFahim.Math.Arithmetic.Tests
{
    using Xunit;

    public sealed unsafe class TryAddTests
    {
        [Fact]
        public void Normal_Int()
        {
            int res;
            Assert.True(TryAdd.Run(5, 10, out res));
            Assert.Equal(15, res);

            Assert.True(TryAdd.Run(-5, -10, out res));
            Assert.Equal(-15, res);
        }

        [Fact]
        public void Overflow_Int()
        {
            int res;
            Assert.False(TryAdd.Run(int.MaxValue, 1, out res));
            Assert.False(TryAdd.Run(int.MinValue, -1, out res));
        }

        [Fact]
        public void Normal_Long()
        {
            long res;
            Assert.True(TryAdd.Run(5L, 10L, out res));
            Assert.Equal(15L, res);
        }

        [Fact]
        public void Overflow_Long()
        {
            long res;
            Assert.False(TryAdd.Run(long.MaxValue, 1L, out res));
            Assert.False(TryAdd.Run(long.MinValue, -1L, out res));
        }
    }

    public sealed unsafe class TrySubTests
    {
        [Fact]
        public void Normal_Int()
        {
            int res;
            Assert.True(TrySub.Run(10, 5, out res));
            Assert.Equal(5, res);
        }

        [Fact]
        public void Overflow_Int()
        {
            int res;
            Assert.False(TrySub.Run(int.MinValue, 1, out res));
            Assert.False(TrySub.Run(int.MaxValue, -1, out res));
        }

        [Fact]
        public void Normal_Long()
        {
            long res;
            Assert.True(TrySub.Run(10L, 5L, out res));
            Assert.Equal(5L, res);
        }

        [Fact]
        public void Overflow_Long()
        {
            long res;
            Assert.False(TrySub.Run(long.MinValue, 1L, out res));
            Assert.False(TrySub.Run(long.MaxValue, -1L, out res));
        }
    }

    public sealed unsafe class TryMulTests
    {
        [Fact]
        public void Normal_Int()
        {
            int res;
            Assert.True(TryMul.Run(5, 6, out res));
            Assert.Equal(30, res);

            Assert.True(TryMul.Run(0, 100, out res));
            Assert.Equal(0, res);
        }

        [Fact]
        public void Overflow_Int()
        {
            int res;
            Assert.False(TryMul.Run(int.MaxValue, 2, out res));
            Assert.False(TryMul.Run(int.MinValue, 2, out res));
        }

        [Fact]
        public void Normal_Long()
        {
            long res;
            Assert.True(TryMul.Run(5L, 6L, out res));
            Assert.Equal(30L, res);
        }

        [Fact]
        public void Overflow_Long()
        {
            long res;
            Assert.False(TryMul.Run(long.MaxValue, 2L, out res));
            Assert.False(TryMul.Run(long.MinValue, 2L, out res));
            Assert.False(TryMul.Run(long.MinValue, -1L, out res));
        }
    }

    public sealed unsafe class TryDivTests
    {
        [Fact]
        public void Normal_Int()
        {
            int res;
            Assert.True(TryDiv.Run(10, 2, out res));
            Assert.Equal(5, res);
        }

        [Fact]
        public void DivisionByZero_Int()
        {
            int res;
            Assert.False(TryDiv.Run(10, 0, out res));
        }

        [Fact]
        public void Overflow_Int()
        {
            int res;
            Assert.False(TryDiv.Run(int.MinValue, -1, out res));
        }

        [Fact]
        public void Normal_Long()
        {
            long res;
            Assert.True(TryDiv.Run(10L, 2L, out res));
            Assert.Equal(5L, res);
        }

        [Fact]
        public void DivisionByZero_Long()
        {
            long res;
            Assert.False(TryDiv.Run(10L, 0L, out res));
        }

        [Fact]
        public void Overflow_Long()
        {
            long res;
            Assert.False(TryDiv.Run(long.MinValue, -1L, out res));
        }
    }
}
