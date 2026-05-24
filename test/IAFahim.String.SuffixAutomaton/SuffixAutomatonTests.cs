namespace IAFahim.String.SuffixAutomaton.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class SuffixAutomatonTests
    {
        [Test]
        public void Build_Empty_NoCrash()
        {
            SuffixAutomaton.Build(null, 0);
        }

        [Test]
        public void Build_SingleChar_Builds()
        {
            int* ptr = (int*)Marshal.AllocHGlobal(sizeof(int));
            ptr[0] = 97;
            SuffixAutomaton.Build(ptr, 1);
            Marshal.FreeHGlobal((nint)ptr);
        }

        [Test]
        public void Build_MultipleChars_Builds()
        {
            int len = 5;
            int* ptr = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            for (int i = 0; i < len; i++) ptr[i] = 97 + i;
            SuffixAutomaton.Build(ptr, len);
            Marshal.FreeHGlobal((nint)ptr);
        }
    }
}
