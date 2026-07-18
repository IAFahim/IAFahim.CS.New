namespace IAFahim.Graph.Cactus
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class CactusLca
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int u, int v)
        {
            throw new NotImplementedException(
                "CactusLca needs cactus tree structure (parent/depth or Euler tour + RMQ); "
                + "Run(u,v) alone cannot compute LCA. Extend the contract with tree arrays.");
        }
    }
}
