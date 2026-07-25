namespace IAFahim.Memory.Allocators.Tests
{
    using NUnit.Framework;
    using Unity.Collections;

    public sealed unsafe class AllocatorTests
    {
        [Test]
        public void FixedPool_AllocFree_ReusesSlot()
        {
            UnsafeFixedPoolAllocator<int> pool = new UnsafeFixedPoolAllocator<int>(4, Allocator.Persistent);
            try
            {
                Assert.IsTrue(pool.IsCreated);
                int* a = pool.Alloc();
                int* b = pool.Alloc();
                Assert.IsTrue(a != null);
                Assert.IsTrue(b != null);
                Assert.IsTrue(a != b);
                *a = 11;
                *b = 22;
                Assert.AreEqual(11, *a);
                Assert.AreEqual(22, *b);
                pool.Free(a);
                int* c = pool.Alloc();
                Assert.IsTrue(a == c);
            }
            finally
            {
                pool.Dispose();
            }
        }

        [Test]
        public void FixedPool_Exhausted_ReturnsNull()
        {
            UnsafeFixedPoolAllocator<int> pool = new UnsafeFixedPoolAllocator<int>(2, Allocator.Persistent);
            try
            {
                Assert.IsTrue(pool.Alloc() != null);
                Assert.IsTrue(pool.Alloc() != null);
                Assert.IsTrue(pool.Alloc() == null);
            }
            finally
            {
                pool.Dispose();
            }
        }

        [Test]
        public void MemoryAllocator_Create_FreeAll()
        {
            MemoryAllocator alloc = new MemoryAllocator(Allocator.Persistent);
            try
            {
                int* p = alloc.Create<int>(8);
                Assert.IsTrue(p != null);
                p[0] = 7;
                Assert.AreEqual(7, p[0]);
                alloc.FreeAll();
            }
            finally
            {
                alloc.Dispose();
            }
        }

        [Test]
        public void SlabAllocator_AllocIncreasesCount()
        {
            UnsafeSlabAllocator<int> slab = new UnsafeSlabAllocator<int>(4, Allocator.Persistent);
            try
            {
                Assert.IsTrue(slab.IsCreated);
                int* p = slab.Alloc();
                Assert.IsTrue(p != null);
                Assert.IsTrue(slab.AllocationCount >= 1);
            }
            finally
            {
                slab.Dispose();
            }
        }

        [Test]
        public void Ptr_Zero_Equals()
        {
            Ptr z = Ptr.Zero;
            Assert.IsTrue(z.Equals(Ptr.Zero));
            void* n = null;
            Ptr fromNull = n;
            Assert.IsTrue(fromNull.Equals(Ptr.Zero));
        }
    }
}
