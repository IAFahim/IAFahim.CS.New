namespace IAFahim.String.Grammar
{
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
    using System;

    public static unsafe class StraightLineProgram
    {
        public struct Rule
        {
            public int Left;
            public int Right;
            public byte Char;
            public bool IsTerminal;
        }

        public static void Build(byte* s, int len, int maxRules, Rule* rules, ref int ruleCount)
        {
            ruleCount = 0;
            for (int i = 0; i < len; i++)
            {
                int j = i;
                while (j < len && !IsNewRule(s, j, len))
                    j++;
                int ruleId = FindOrCreateRule(s, i, j - i, rules, ref ruleCount);
                i = j - 1;
            }
        }

        private static bool IsNewRule(byte* s, int start, int len)
        {
            for (int l = 1; l <= (len - start) / 2; l++)
            {
                bool match = true;
                for (int k = 0; k < l; k++)
                {
                    if (s[start + k] != s[start + l + k])
                    {
                        match = false;
                        break;
                    }
                }
                if (match) return true;
            }
            return false;
        }

        private static int FindOrCreateRule(byte* s, int start, int len, Rule* rules, ref int ruleCount)
        {
            for (int i = 0; i < ruleCount; i++)
            {
                if (!rules[i].IsTerminal && rules[i].Left == start && rules[i].Right == len)
                    return i;
            }
            int id = ruleCount++;
            rules[id].IsTerminal = false;
            rules[id].Left = start;
            rules[id].Right = len;
            return id;
        }

        public static byte Query(Rule* rules, int ruleId, int pos)
        {
            if (rules[ruleId].IsTerminal)
                return rules[ruleId].Char;
            return rules[ruleId].Char;
        }
    }
}
