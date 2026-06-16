namespace IAFahim.String.Grammar
{
    /// <summary>
    /// Straight-line program (balanced binary grammar) over a byte string.
    ///
    /// A <see cref="Rule"/> is either a terminal (a single byte) or a
    /// non-terminal that concatenates a left child rule and a right child rule.
    /// <see cref="Build"/> produces a forest of rules in <paramref name="rules"/>
    /// and returns the id of the root rule whose expansion equals the input.
    /// <see cref="Query"/> walks the tree to return the byte at a given position.
    ///
    /// Identical sub-trees are shared (structural deduplication): two rules with
    /// the same children — or two terminals with the same byte — are merged into
    /// a single id, which is what makes the program "straight-line" (a DAG, not a
    /// tree) and gives compression of repeated factors.
    ///
    /// Build is "unchecked": the caller guarantees s is non-null with at least
    /// len readable bytes, len &gt;= 1, and rules has capacity for at least
    /// maxRules entries (a balanced tree over len leaves needs at most 2*len-1
    /// rules). Query assumes rules/ruleId/pos are valid for the built program.
    /// </summary>
    public static unsafe class StraightLineProgram
    {
        /// <summary>An invalid rule id, returned by Build when the input is empty.</summary>
        public const int NoRule = -1;

        public struct Rule
        {
            /// <summary>Left child rule id (non-terminals); unused for terminals.</summary>
            public int Left;

            /// <summary>Right child rule id (non-terminals); unused for terminals.</summary>
            public int Right;

            /// <summary>Number of bytes this rule expands to.</summary>
            public int Len;

            /// <summary>The byte value for terminal rules.</summary>
            public byte Char;

            /// <summary>True for a single-byte terminal, false for a concatenation.</summary>
            public bool IsTerminal;
        }

        /// <summary>
        /// Builds a balanced SLP over s[0..len) into rules and returns the root
        /// rule id (or <see cref="NoRule"/> when len &lt;= 0).
        /// </summary>
        public static int Build(byte* s, int len, int maxRules, Rule* rules, ref int ruleCount)
        {
            ruleCount = 0;
            if (len <= 0)
                return NoRule;
            return BuildRange(s, 0, len, maxRules, rules, ref ruleCount);
        }

        /// <summary>
        /// Builds a rule whose expansion equals s[lo..hi) by recursively
        /// splitting the range in half, then deduplicating against existing rules.
        /// </summary>
        private static int BuildRange(byte* s, int lo, int hi, int maxRules, Rule* rules, ref int ruleCount)
        {
            int span = hi - lo;
            if (span == 1)
                return FindOrCreateTerminal(s[lo], maxRules, rules, ref ruleCount);

            int mid = lo + (span >> 1);
            int left = BuildRange(s, lo, mid, maxRules, rules, ref ruleCount);
            int right = BuildRange(s, mid, hi, maxRules, rules, ref ruleCount);
            return FindOrCreateConcat(left, right, span, maxRules, rules, ref ruleCount);
        }

        /// <summary>
        /// Returns the id of an existing terminal rule for value c, or creates one.
        /// </summary>
        private static int FindOrCreateTerminal(byte c, int maxRules, Rule* rules, ref int ruleCount)
        {
            for (int i = 0; i < ruleCount; i++)
            {
                if (rules[i].IsTerminal && rules[i].Char == c)
                    return i;
            }
            if (ruleCount >= maxRules)
                return ruleCount - 1;
            int id = ruleCount++;
            rules[id].IsTerminal = true;
            rules[id].Char = c;
            rules[id].Len = 1;
            rules[id].Left = NoRule;
            rules[id].Right = NoRule;
            return id;
        }

        /// <summary>
        /// Returns the id of an existing non-terminal rule with the given children,
        /// or creates one. Equal child pairs expand to equal content, so this shares
        /// repeated sub-trees by content rather than by position.
        /// </summary>
        private static int FindOrCreateConcat(int left, int right, int totalLen, int maxRules, Rule* rules, ref int ruleCount)
        {
            for (int i = 0; i < ruleCount; i++)
            {
                if (!rules[i].IsTerminal && rules[i].Left == left && rules[i].Right == right)
                    return i;
            }
            if (ruleCount >= maxRules)
                return ruleCount - 1;
            int id = ruleCount++;
            rules[id].IsTerminal = false;
            rules[id].Left = left;
            rules[id].Right = right;
            rules[id].Len = totalLen;
            rules[id].Char = 0;
            return id;
        }

        /// <summary>
        /// Returns the byte at position pos within the expansion of ruleId.
        /// Walks down the DAG choosing the left or right child by the running
        /// offset. Unchecked: caller guarantees 0 &lt;= pos &lt; rules[ruleId].Len.
        /// </summary>
        public static byte Query(Rule* rules, int ruleId, int pos)
        {
            while (!rules[ruleId].IsTerminal)
            {
                int left = rules[ruleId].Left;
                int leftLen = rules[left].Len;
                if (pos < leftLen)
                {
                    ruleId = left;
                }
                else
                {
                    pos -= leftLen;
                    ruleId = rules[ruleId].Right;
                }
            }
            return rules[ruleId].Char;
        }
    }
}
