namespace IAFahim.Math.BigInt.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Math.BigInt;
    using Xunit;

    public sealed unsafe class BigIntTests
    {
        [Fact]
        public void Add_SingleDigits_CorrectResult()
        {
            int* a = stackalloc int[1] { 5 };
            int* b = stackalloc int[1] { 7 };
            int* res = stackalloc int[2];
            int len = BigIntAdd.Run(1, a, 1, b, res);
            Assert.Equal(2, len);
            Assert.Equal(1, res[0]);
            Assert.Equal(2, res[1]);
        }

        [Fact]
        public void Add_VaryingLengths_CorrectResult()
        {
            int* a = stackalloc int[3] { 9, 9, 9 };
            int* b = stackalloc int[1] { 1 };
            int* res = stackalloc int[4];
            int len = BigIntAdd.Run(3, a, 1, b, res);
            Assert.Equal(4, len);
            Assert.Equal(1, res[0]);
            Assert.Equal(0, res[1]);
            Assert.Equal(0, res[2]);
            Assert.Equal(0, res[3]);
        }

        [Fact]
        public void Sub_BasicSubtraction_CorrectResult()
        {
            int* a = stackalloc int[3] { 1, 0, 0 };
            int* b = stackalloc int[2] { 9, 9 };
            int* res = stackalloc int[3];
            int len = BigIntSub.Run(3, a, 2, b, res);
            Assert.Equal(1, len);
            Assert.Equal(1, res[0]);
        }

        [Fact]
        public void Mul_MultiplicationOverlap_CorrectResult()
        {
            int* a = stackalloc int[2] { 1, 2 }; // 12
            int* b = stackalloc int[2] { 1, 2 }; // 12
            int* res = stackalloc int[4];
            // We pass the same buffer to check if it handles self-overlap correctly
            // (since the result can be written directly to a if we use a temp buffer)
            int len = BigIntMul.Run(2, a, 2, b, res);
            Assert.Equal(3, len);
            // 12 * 12 = 144
            Assert.Equal(1, res[0]);
            Assert.Equal(4, res[1]);
            Assert.Equal(4, res[2]);
        }

        [Fact]
        public void Pow_PowerCalculation_CorrectResult()
        {
            int* res = stackalloc int[1000];
            for (int i = 0; i < 1000; i++) res[i] = 0;
            // 2^10 = 1024
            int len = BigIntPow.Run(2, 10, res);
            Assert.Equal(4, len);
            Assert.Equal(1, res[0]);
            Assert.Equal(0, res[1]);
            Assert.Equal(2, res[2]);
            Assert.Equal(4, res[3]);
        }

        [Fact]
        public void Div_Division_CorrectResult()
        {
            int* a = stackalloc int[3] { 1, 2, 5 }; // 125
            int* res = stackalloc int[3];
            int len = BigIntDiv.Run(3, a, 5, res);
            Assert.Equal(2, len); // 25
            Assert.Equal(2, res[0]);
            Assert.Equal(5, res[1]);
        }

        [Fact]
        public void Mod_Modulo_CorrectResult()
        {
            int* a = stackalloc int[3] { 1, 2, 7 }; // 127
            int rem = BigIntMod.Run(3, a, 10);
            Assert.Equal(7, rem);
        }
    }
}
