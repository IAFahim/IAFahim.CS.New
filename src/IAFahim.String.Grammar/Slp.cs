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

        private static Rule* _rules;
        private static int _ruleCount;

        public static void Build(byte* s, int len, int maxRules)
        {
            _rules = (Rule*)Marshal.AllocHGlobal(sizeof(Rule) * maxRules);
            _ruleCount = 0;
            for (int i = 0; i < len; i++)
            {
                int j = i;
                while (j < len && !IsNewRule(s, j, len))
                    j++;
                int ruleId = FindOrCreateRule(s, i, j - i);
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

        private static int FindOrCreateRule(byte* s, int start, int len)
        {
            for (int i = 0; i < _ruleCount; i++)
            {
                if (!_rules[i].IsTerminal && _rules[i].Left == start && _rules[i].Right == len)
                    return i;
            }
            int id = _ruleCount++;
            _rules[id].IsTerminal = false;
            _rules[id].Left = start;
            _rules[id].Right = len;
            return id;
        }

        public static byte Query(int ruleId, int pos)
        {
            if (_rules[ruleId].IsTerminal)
                return _rules[ruleId].Char;
            return _rules[ruleId].Char;
        }
    }
}
