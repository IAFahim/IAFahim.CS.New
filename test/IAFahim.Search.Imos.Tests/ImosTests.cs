namespace IAFahim.Search.Imos.Tests
{
    using Xunit;

    public sealed unsafe class Imos1DTests
    {
        [Fact]
        public void Add_Normal()
        {
            int* diff = stackalloc int[10];
            for (int i = 0; i < 10; i++) diff[i] = 0;
            Imos1D.Add(diff, 10, 2, 5, 3);
            Assert.Equal(3, diff[2]);
            Assert.Equal(-3, diff[6]);
        }

        [Fact]
        public void Build_Normal()
        {
            int* diff = stackalloc int[10];
            int* dst = stackalloc int[10];
            for (int i = 0; i < 10; i++) diff[i] = 0;
            Imos1D.Add(diff, 10, 2, 5, 3);
            Imos1D.Build(dst, diff, 10);
            for (int i = 0; i < 10; i++)
            {
                if (i < 2 || i > 5)
                    Assert.Equal(0, dst[i]);
                else
                    Assert.Equal(3, dst[i]);
            }
        }
    }

    public sealed unsafe class Imos2DTests
    {
        [Fact]
        public void Add_Normal()
        {
            int* diff = stackalloc int[16];
            for (int i = 0; i < 16; i++) diff[i] = 0;
            Imos2D.Add(diff, 4, 4, 1, 1, 2, 2, 5);
            Assert.Equal(5, diff[1 * 4 + 1]);
        }

        [Fact]
        public void Build_Normal()
        {
            int* diff = stackalloc int[16];
            int* dst = stackalloc int[16];
            for (int i = 0; i < 16; i++) diff[i] = 0;
            Imos2D.Add(diff, 4, 4, 1, 1, 2, 2, 5);
            Imos2D.Build(dst, diff, 4, 4);
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (r >= 1 && r <= 2 && c >= 1 && c <= 2)
                        Assert.Equal(5, dst[r * 4 + c]);
                    else
                        Assert.Equal(0, dst[r * 4 + c]);
                }
            }
        }
    }
}