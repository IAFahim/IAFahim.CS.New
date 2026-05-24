namespace IAFahim.DS.GapBuffer.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class GapBufferTests
    {
        [Test]
        public void GapBufferInsert_Basic()
        {
            const int Cap = 64;
            byte* buf = (byte*)Marshal.AllocHGlobal(Cap);
            try
            {
                GapBufferState s;
                s.Buffer = buf;
                s.Capacity = Cap;
                s.GapStart = 0;
                s.GapEnd = Cap;

                byte* data = stackalloc byte[3];
                data[0] = (byte)'A';
                data[1] = (byte)'B';
                data[2] = (byte)'C';
                GapBufferInsert.Run(ref s, 0, data, 3);
                Assert.AreEqual(3, s.GapStart);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)buf);
            }
        }

        [Test]
        public void GapBufferDelete_Basic()
        {
            const int Cap = 64;
            byte* buf = (byte*)Marshal.AllocHGlobal(Cap);
            try
            {
                GapBufferState s;
                s.Buffer = buf;
                s.Capacity = Cap;
                s.GapStart = 0;
                s.GapEnd = Cap;

                byte* data = stackalloc byte[5];
                for (int i = 0; i < 5; i++) data[i] = (byte)('A' + i);
                GapBufferInsert.Run(ref s, 0, data, 5);

                int gapBefore = s.GapEnd - s.GapStart;

                GapBufferInsert.Run(ref s, 5, data, 3);

                int gapAfter = s.GapEnd - s.GapStart;
                Assert.IsTrue(gapAfter < gapBefore);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)buf);
            }
        }
    }
}
