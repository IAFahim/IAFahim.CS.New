namespace BovineLabs.Core.Memory
{
    using System;
    using System.Runtime.InteropServices;
    using Unity.Collections;

    public static unsafe class Unmanaged
    {
        // Minimum alignment used when the caller requests a weaker (or invalid) alignment.
        // Must be a power of two and >= sizeof(void*) so the stashed base pointer always fits.
        private const int MinAlignment = 16;

        public static void* Allocate(long size, int alignment, AllocatorManager.AllocatorHandle allocator)
        {
            // Honor the requested alignment. Marshal.AllocHGlobal only guarantees CRT default
            // alignment (typically 8/16 bytes), so for stronger requested alignments we
            // over-allocate and round the user pointer up to the requested boundary, stashing
            // the original base pointer in the slot immediately preceding it for Free to recover.
            // By contract `alignment` is expected to be a power of two; weaker/invalid values are
            // clamped up to MinAlignment.
            int effectiveAlignment = alignment < MinAlignment ? MinAlignment : alignment;

            // Reserve room for: the payload, up to (alignment - 1) bytes of rounding slack, and a
            // pointer-sized slot to stash the original base pointer.
            long totalSize = size + effectiveAlignment + sizeof(void*);

            byte* basePtr = (byte*)Marshal.AllocHGlobal((IntPtr)totalSize);

            // Round up to the requested alignment, leaving at least sizeof(void*) bytes of headroom
            // before the aligned pointer to stash the original base.
            nuint raw = (nuint)(basePtr + sizeof(void*));
            nuint mask = (nuint)(effectiveAlignment - 1);
            byte* aligned = (byte*)((raw + mask) & ~mask);

            // Stash the original base pointer immediately before the aligned pointer.
            ((void**)aligned)[-1] = basePtr;

            return aligned;
        }

        public static void Free(void* ptr, AllocatorManager.AllocatorHandle allocator)
        {
            if (ptr != null)
            {
                // Recover the original base pointer stashed just before the user pointer by Allocate.
                void* basePtr = ((void**)ptr)[-1];
                Marshal.FreeHGlobal((IntPtr)basePtr);
            }
        }
    }
}
