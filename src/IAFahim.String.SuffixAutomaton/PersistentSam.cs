namespace IAFahim.String.SuffixAutomaton
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class PersistentSam
    {
        public struct Version
        {
            public int Root;
            public int Len;
        }

        public static void PushVersion(int* roots, ref int versionCount, int root, int len)
        {
            roots[versionCount++] = root;
        }

        public static int GetVersion(int* roots, int v)
        {
            return roots[v];
        }
    }
}
