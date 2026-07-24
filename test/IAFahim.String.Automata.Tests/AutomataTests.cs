namespace IAFahim.String.Automata.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class FiniteAutomatonTests
    {
        [Test]
        public void Accepts_SimpleDfa()
        {
            const int States = 2;
            const int Sigma = 2;
            int** transitions = (int**)Marshal.AllocHGlobal(States * sizeof(nint));
            int* row0 = (int*)Marshal.AllocHGlobal(Sigma * sizeof(int));
            int* row1 = (int*)Marshal.AllocHGlobal(Sigma * sizeof(int));
            bool* accept = stackalloc bool[States];
            try
            {
                transitions[0] = row0;
                transitions[1] = row1;
                row0[0] = 0; row0[1] = 1;
                row1[0] = 0; row1[1] = 1;
                accept[0] = false;
                accept[1] = true;
                FiniteAutomaton.Dfa dfa;
                dfa.Transitions = transitions;
                dfa.IsAccept = accept;
                dfa.StateCount = States;
                dfa.AlphabetSize = Sigma;
                byte* inp = stackalloc byte[3];
                inp[0] = 0; inp[1] = 1; inp[2] = 0;
                Assert.IsFalse(FiniteAutomaton.Accepts(&dfa, 0, inp, 1));
                Assert.IsTrue(FiniteAutomaton.Accepts(&dfa, 0, inp + 1, 1));
                inp[0] = 1; inp[1] = 1;
                Assert.IsTrue(FiniteAutomaton.Accepts(&dfa, 0, inp, 2));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)row0);
                Marshal.FreeHGlobal((nint)row1);
                Marshal.FreeHGlobal((nint)transitions);
            }
        }
    }
}
