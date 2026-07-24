namespace IAFahim.String.MinRotation.Tests
{
    using System.Text;
    using NUnit.Framework;

    public sealed unsafe class BoothTests
    {
        [Test]
        public void Empty_Zero()
        {
            Assert.AreEqual(0, Booth.Run((byte*)null, 0));
        }

        [Test]
        public void KnownRotation()
        {
            byte[] s = Encoding.ASCII.GetBytes("bbaaccaadd");
            fixed (byte* p = s)
            {
                int idx = Booth.Run(p, s.Length);
                Assert.IsTrue(idx >= 0 && idx < s.Length);
                string rot = Encoding.ASCII.GetString(s, idx, s.Length - idx) + Encoding.ASCII.GetString(s, 0, idx);
                for (int i = 0; i < s.Length; i++)
                {
                    string other = Encoding.ASCII.GetString(s, i, s.Length - i) + Encoding.ASCII.GetString(s, 0, i);
                    Assert.IsTrue(string.CompareOrdinal(rot, other) <= 0);
                }
            }
        }

        [Test]
        public void AlreadyMinimal()
        {
            byte[] s = Encoding.ASCII.GetBytes("abcde");
            fixed (byte* p = s)
            {
                Assert.AreEqual(0, Booth.Run(p, s.Length));
            }
        }
    }
}
