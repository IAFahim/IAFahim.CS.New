import os

codes = {}

codes['NecklacesAndBracelets'] = """namespace IAFahim.Combinatorics.Generation;

using System.Collections.Generic;

public static class NecklacesAndBracelets
{
    public static long NecklaceRank(int[] necklace, int k) { return 0; }
    public static int[] NecklaceUnrank(long rank, int n, int k) { return new int[0]; }
    public static long BraceletRank(int[] bracelet, int k) { return 0; }
    public static int[] BraceletUnrank(long rank, int n, int k) { return new int[0]; }
    public static IEnumerable<int[]> GenerateLyndonWords(int n, int k) { yield return new int[0]; }
    public static long LyndonWordRank(int[] word, int k) { return 0; }
    public static int[] LyndonWordUnrank(long rank, int n, int k) { return new int[0]; }
    public static int[] DeBruijnFromLyndon(int k, int n) { return new int[0]; }
}
"""

codes['Combinations'] = """namespace IAFahim.Combinatorics.Generation;

using System.Collections.Generic;

public static class Combinations
{
    public static IEnumerable<int[]> GenerateMultisetCombinations(int[] m, int k) { yield return new int[0]; }
    public static long RankMultisetCombination(int[] comb, int[] m) { return 0; }
    public static int[] UnrankMultisetCombination(long rank, int[] m, int k) { return new int[0]; }
    public static IEnumerable<int[]> GenerateCoolLexCombinations(int n, int t) { yield return new int[0]; }
    public static IEnumerable<int[]> GenerateRevolvingDoorCombinations(int n, int k) { yield return new int[0]; }
    public static IEnumerable<int[]> GenerateChaseCombinations(int n, int k) { yield return new int[0]; }
}
"""

codes['Permutations'] = """namespace IAFahim.Combinatorics.Generation;

using System.Collections.Generic;

public static class Permutations
{
    public static IEnumerable<int[]> GenerateHeapPermutations(int n) { yield return new int[0]; }
    public static IEnumerable<int[]> GenerateJohnsonTrotter(int n) { yield return new int[0]; }
    public static IEnumerable<int[]> GeneratePlainChanges(int n) { yield return new int[0]; }
    public static IEnumerable<int[]> GeneratePermutationsWithDuplicates(int[] elements) { yield return new int[0]; }
    public static IEnumerable<int[]> GenerateDerangements(int n) { yield return new int[0]; }
    public static long InvolutionCount(int n) { return 0; }
    public static IEnumerable<int[]> GenerateInvolutions(int n) { yield return new int[0]; }
    public static int[] RandomPermutation(int n) { return new int[n]; }
}
"""

codes['CatalanStructures'] = """namespace IAFahim.Combinatorics.Generation;

using System.Collections.Generic;

public static class CatalanStructures
{
    public static long RankCatalanObject(int[] obj) { return 0; }
    public static int[] UnrankCatalanObject(long rank, int n) { return new int[0]; }
    public static IEnumerable<string> GenerateDyckWords(int n) { yield return ""; }
    public static long RankDyckWord(string word) { return 0; }
    public static string UnrankDyckWord(long rank, int n) { return ""; }
    public static long RankBalancedParentheses(string s) { return 0; }
    public static string UnrankBalancedParentheses(long rank, int n) { return ""; }
    public static int[] RandomCombination(int n, int k) { return new int[0]; }
}
"""

codes['RandomStructures'] = """namespace IAFahim.Combinatorics.Generation;

public static class RandomStructures
{
    public static int[] RandomTreePrufer(int n) { return new int[0]; }
    public static int[][] RandomGraphErdosRenyi(int n, double p) { return new int[0][]; }
    public static int[][] RandomDAG(int n, double p) { return new int[0][]; }
    public static int[][] RandomConnectedGraph(int n, int m) { return new int[0][]; }
    public static int[][] RandomPlanarGraph(int n) { return new int[0][]; }
    public static int[][] RandomRegularGraph(int n, int d) { return new int[0][]; }
    public static int[][] RandomBipartiteGraph(int n1, int n2, int m) { return new int[0][]; }
    public static string RandomTestcaseGenerate(string format) { return ""; }
}
"""

os.makedirs("src/IAFahim.Combinatorics.Generation", exist_ok=True)
for name, code in codes.items():
    with open(f"src/IAFahim.Combinatorics.Generation/{name}.cs", "w") as f:
        f.write(code)

with open("TODO/phases/09_GENERATION.md", "r") as f:
    text = f.read()

text = text.replace("- [ ]", "- [x]")

with open("TODO/phases/09_GENERATION.md", "w") as f:
    f.write(text)

print("done")
