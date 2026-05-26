namespace IAFahim.DS.FixedCollections
{
    using System.Runtime.CompilerServices;
    using System.Threading;

    public struct SpinLock
    {
        private int @lock;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Acquire()
        {
            for (;;)
            {
                if (Interlocked.CompareExchange(ref this.@lock, 1, 0) == 0)
                {
                    return;
                }

                while (Volatile.Read(ref this.@lock) == 1)
                {
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAcquire()
        {
            return Volatile.Read(ref this.@lock) == 0 && Interlocked.CompareExchange(ref this.@lock, 1, 0) == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAcquire(bool spin)
        {
            if (spin)
            {
                this.Acquire();
                return true;
            }

            return this.TryAcquire();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Release()
        {
            Volatile.Write(ref this.@lock, 0);
        }
    }
}
