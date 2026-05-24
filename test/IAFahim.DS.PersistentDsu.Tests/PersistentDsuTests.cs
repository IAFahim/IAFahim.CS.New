namespace IAFahim.DS.PersistentDsu.Tests
{
    using IAFahim.DS.PersistentDsu;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class PersistentDsuTests
    {
        [Test]
        public void Init_Basic()
        {
            const int n = 10;
            int* parent = stackalloc int[n * 40], size = stackalloc int[n * 40], lc = stackalloc int[n * 40], rc = stackalloc int[n * 40];
            int allocCnt = 0;
            int root = PersistentDsu.Build(0, n - 1, parent, size, &allocCnt, lc, rc);
            for (int i = 0; i < n; i++)
            {
                int r = PersistentDsu.Find(root, n, i, parent, lc, rc, size, out int s);
                Assert.AreEqual(i, r);
            }
        }

        [Test]
        public void Union_SameComponent()
        {
            const int n = 10, maxNodes = 500;
            int* p = stackalloc int[maxNodes], s = stackalloc int[maxNodes], lc = stackalloc int[maxNodes], rc = stackalloc int[maxNodes];
            int allocCnt = 0;
            int root = PersistentDsu.Build(0, n - 1, p, s, &allocCnt, lc, rc);
            int newRoot = PersistentDsu.Union(root, n, 0, 1, p, s, &allocCnt, lc, rc);
            int ra = PersistentDsu.Find(newRoot, n, 0, p, lc, rc, s, out int sa);
            int rb = PersistentDsu.Find(newRoot, n, 1, p, lc, rc, s, out int sb);
            Assert.AreEqual(ra, rb);
        }

        [Test]
        public void MultipleUnionsInSameVersion()
        {
            const int n = 10, maxNodes = 500;
            int* p = stackalloc int[maxNodes], s = stackalloc int[maxNodes], lc = stackalloc int[maxNodes], rc = stackalloc int[maxNodes];
            int allocCnt = 0;
            int root = PersistentDsu.Build(0, n - 1, p, s, &allocCnt, lc, rc);
            root = PersistentDsu.Union(root, n, 0, 1, p, s, &allocCnt, lc, rc);
            root = PersistentDsu.Union(root, n, 1, 2, p, s, &allocCnt, lc, rc);
            int ra = PersistentDsu.Find(root, n, 0, p, lc, rc, s, out int sa);
            int rb = PersistentDsu.Find(root, n, 2, p, lc, rc, s, out int sb);
            Assert.AreEqual(ra, rb);
        }
    }
}
