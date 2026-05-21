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

        private static int* _roots;
        private static int _versionCount;

        public static void Init(int maxVersions)
        {
            _roots = (int*)Marshal.AllocHGlobal(sizeof(int) * maxVersions);
            _versionCount = 0;
        }

        public static void PushVersion(int root, int len)
        {
            _roots[_versionCount++] = root;
        }

        public static int GetVersion(int v)
        {
            return _roots[v];
        }
    }
}
