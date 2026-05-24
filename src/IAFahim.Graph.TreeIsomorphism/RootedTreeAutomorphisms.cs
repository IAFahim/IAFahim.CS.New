namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.CompilerServices;
    using IAFahim.Graph.TreeQueries;

    public static unsafe class RootedTreeAutomorphisms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int n, int root, int* head, int* to, int* next, long mod)
        {
            ulong* subHash = stackalloc ulong[n];
            TreeHashing.CanonicalHashRooted(root, -1, head, to, next, subHash);
            return TreeHashing.AutomorphismCountRooted(root, -1, head, to, next, subHash, mod);
        }
    }
}
