namespace IAFahim.DP.Knapsack.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class KnapsackTests
    {
        [Test]
        public void Knapsack01_Empty_NoOp()
        {
            long* w = stackalloc long[0];
            long* v = stackalloc long[0];
            long* dp = stackalloc long[11];
            long result = Knapsack01.Run(0, 10, w, v, dp);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Knapsack01_ZeroCapacity_Returns0()
        {
            long* w = stackalloc long[3];
            long* v = stackalloc long[3];
            long* dp = stackalloc long[1];
            w[0] = 3; v[0] = 10;
            w[1] = 4; v[1] = 15;
            w[2] = 5; v[2] = 20;
            long result = Knapsack01.Run(3, 0, w, v, dp);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Knapsack01_SingleItem_Fits_ReturnsValue()
        {
            long* w = stackalloc long[1];
            long* v = stackalloc long[1];
            long* dp = stackalloc long[11];
            w[0] = 5; v[0] = 10;
            long result = Knapsack01.Run(1, 10, w, v, dp);
            Assert.AreEqual(10, result);
        }

        [Test]
        public void Knapsack01_SingleItem_TooHeavy_Returns0()
        {
            long* w = stackalloc long[1];
            long* v = stackalloc long[1];
            long* dp = stackalloc long[11];
            w[0] = 15; v[0] = 10;
            long result = Knapsack01.Run(1, 10, w, v, dp);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Knapsack01_TwoItems_ChoosesBetter()
        {
            long* w = stackalloc long[2];
            long* v = stackalloc long[2];
            long* dp = stackalloc long[11];
            w[0] = 6; v[0] = 10;
            w[1] = 10; v[1] = 21;
            long result = Knapsack01.Run(2, 10, w, v, dp);
            Assert.AreEqual(21, result);
        }

        [Test]
        public void Knapsack01_SpaceEfficient_ConsistentWithDp()
        {
            long* w = stackalloc long[3];
            long* v = stackalloc long[3];
            long* dp = stackalloc long[11];
            w[0] = 3; v[0] = 10;
            w[1] = 4; v[1] = 15;
            w[2] = 5; v[2] = 20;
            long result1 = Knapsack01.Run(3, 10, w, v, dp);
            long* dp2 = stackalloc long[11];
            long result2 = Knapsack01.RunSpaceEfficient(3, 10, w, v, dp2);
            Assert.AreEqual(result1, result2);
        }

        [Test]
        public void Knapsack01_FractionalWeight_ChoosesPartial()
        {
            long* w = stackalloc long[2];
            long* v = stackalloc long[2];
            long* dp = stackalloc long[11];
            w[0] = 5; v[0] = 10;
            w[1] = 3; v[1] = 8;
            long result = Knapsack01.Run(2, 7, w, v, dp);
            Assert.AreEqual(10, result);
        }

        [Test]
        public void KnapsackUnbounded_Empty_Returns0()
        {
            long* w = stackalloc long[0];
            long* v = stackalloc long[0];
            long* dp = stackalloc long[11];
            long result = KnapsackUnbounded.Run(0, 10, w, v, dp);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void KnapsackUnbounded_SingleItem_Maximizes()
        {
            long* w = stackalloc long[1];
            long* v = stackalloc long[1];
            long* dp = stackalloc long[11];
            w[0] = 3; v[0] = 5;
            long result = KnapsackUnbounded.Run(1, 10, w, v, dp);
            Assert.AreEqual(15, result);
        }

        [Test]
        public void KnapsackUnbounded_TwoItems_ChoosesBetter()
        {
            long* w = stackalloc long[2];
            long* v = stackalloc long[2];
            long* dp = stackalloc long[11];
            w[0] = 3; v[0] = 5;
            w[1] = 5; v[1] = 10;
            long result = KnapsackUnbounded.Run(2, 10, w, v, dp);
            Assert.AreEqual(20, result);
        }

        [Test]
        public void KnapsackUnbounded_SpaceEfficient_Consistent()
        {
            long* w = stackalloc long[2];
            long* v = stackalloc long[2];
            w[0] = 3; v[0] = 5;
            w[1] = 5; v[1] = 10;
            long* dp = stackalloc long[11];
            long result1 = KnapsackUnbounded.Run(2, 10, w, v, dp);
            long* dp2 = stackalloc long[11];
            long result2 = KnapsackUnbounded.RunSpaceEfficient(2, 10, w, v, dp2);
            Assert.AreEqual(result1, result2);
        }

        [Test]
        public void SubsetSum_EmptyTarget_True()
        {
            long* a = stackalloc long[3];
            bool* dp = stackalloc bool[11];
            bool result = SubsetSum.Run(0, 0, a, dp);
            Assert.IsTrue(result);
        }

        [Test]
        public void SubsetSum_Target0_True()
        {
            long* a = stackalloc long[3];
            a[0] = 1; a[1] = 2; a[2] = 3;
            bool* dp = stackalloc bool[11];
            bool result = SubsetSum.Run(3, 0, a, dp);
            Assert.IsTrue(result);
        }

        [Test]
        public void SubsetSum_Normal_ReturnsCorrect()
        {
            long* a = stackalloc long[4];
            a[0] = 3; a[1] = 5; a[2] = 7; a[3] = 8;
            bool* dp = stackalloc bool[11];
            Assert.IsTrue(SubsetSum.Run(4, 10, a, dp));
            Assert.IsFalse(SubsetSum.Run(4, 9, a, dp));
        }

        [Test]
        public void SubsetSum_CanMakeAll()
        {
            long* a = stackalloc long[3];
            a[0] = 1; a[1] = 2; a[2] = 3;
            bool* dp = stackalloc bool[11];
            for (int s = 0; s <= 6; s++)
            {
                dp[s] = false;
            }
            SubsetSum.Run(3, 6, a, dp);
            for (int s = 0; s <= 6; s++)
            {
                Assert.IsTrue(dp[s], $"Should be able to make sum {s}");
            }
        }

        [Test]
        public void KnapsackBounded_SingleWithCount1_Effective()
        {
            long* w = stackalloc long[1];
            long* v = stackalloc long[1];
            long* cnt = stackalloc long[1];
            long* dp = stackalloc long[11];
            w[0] = 5; v[0] = 10; cnt[0] = 1;
            long result = KnapsackBounded.Run(1, 10, w, v, cnt, dp);
            Assert.AreEqual(10, result);
        }

        [Test]
        public void KnapsackBounded_MultipleItems_ReturnsMax()
        {
            long* w = stackalloc long[3];
            long* v = stackalloc long[3];
            long* cnt = stackalloc long[3];
            long* dp = stackalloc long[11];
            w[0] = 3; v[0] = 5; cnt[0] = 1;
            w[1] = 4; v[1] = 8; cnt[1] = 2;
            w[2] = 5; v[2] = 12; cnt[2] = 1;
            long result = KnapsackBounded.Run(3, 10, w, v, cnt, dp);
            Assert.AreEqual(20, result);
        }

        [Test]
        public void BitsetSubsetSum_MatchesBoolDp()
        {
            long* a = stackalloc long[] { 3, 5, 7, 8 };
            long* bits = stackalloc long[1];
            Assert.AreEqual(1, BitsetSubsetSum.Run(4, 10, a, bits));
            Assert.AreEqual(0, BitsetSubsetSum.Run(4, 9, a, bits));
            Assert.AreEqual(1, BitsetSubsetSum.Run(4, 0, a, bits));
        }

        [Test]
        public void BitsetSubsetSum_TargetMultipleOf64_HoldsTopBit()
        {
            long* a = stackalloc long[] { 64 };
            long* bits = stackalloc long[2];
            long hit = BitsetSubsetSum.Run(1, 64, a, bits);
            Assert.AreEqual(1, hit);
            Assert.AreEqual(0, BitsetSubsetSum.Run(1, 63, a, bits));
            Assert.AreEqual(0, BitsetSubsetSum.Run(1, 128, a, bits));
        }

        [Test]
        public void BitsetSubsetSum_Empty_OnlyZero()
        {
            long* a = stackalloc long[1];
            long* bits = stackalloc long[1];
            Assert.AreEqual(1, BitsetSubsetSum.Run(0, 0, a, bits));
            Assert.AreEqual(0, BitsetSubsetSum.Run(0, 5, a, bits));
        }
    }
}