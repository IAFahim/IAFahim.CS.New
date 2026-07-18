namespace IAFahim.Graph.Cactus
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BlockCutTreeLca
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int u, int v)
        {
            throw new NotImplementedException(
                "BlockCutTreeLca needs parent/depth (or RMQ) buffers for a prebuilt block-cut tree; "
                + "Run(u,v) alone cannot compute LCA. Extend the contract with tree arrays.");
        }
    }
}
