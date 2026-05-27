import os

tree_iso_dir = "src/IAFahim.Graph.TreeIsomorphism"
cactus_dir = "src/IAFahim.Graph.Cactus"
func_dir = "src/IAFahim.Graph.Functional"

os.makedirs(tree_iso_dir, exist_ok=True)
os.makedirs(cactus_dir, exist_ok=True)
os.makedirs(func_dir, exist_ok=True)

files = {}

# 3.7 Tree Isomorphism
files[f"{tree_iso_dir}/TreeIsomorphismAhU.cs"] = """namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.CompilerServices;

    public static unsafe class TreeIsomorphismAhU
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* p1, int* p2, int n)
        {
            return false;
        }
    }
}
"""

files[f"{tree_iso_dir}/TreeIsomorphismCenterHash.cs"] = """namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.CompilerServices;

    public static unsafe class TreeIsomorphismCenterHash
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* p1, int* p2, int n)
        {
            return false;
        }
    }
}
"""

files[f"{tree_iso_dir}/RootedTreeCanonicalForm.cs"] = """namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.CompilerServices;

    public static unsafe class RootedTreeCanonicalForm
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* p, int n, int* outHash)
        {
        }
    }
}
"""

files[f"{tree_iso_dir}/UnrootedTreeCanonicalForm.cs"] = """namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.CompilerServices;

    public static unsafe class UnrootedTreeCanonicalForm
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* p, int n, int* outHash)
        {
        }
    }
}
"""

files[f"{tree_iso_dir}/RootedTreeAutomorphisms.cs"] = """namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.CompilerServices;

    public static unsafe class RootedTreeAutomorphisms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* p, int n)
        {
            return 1;
        }
    }
}
"""

files[f"{tree_iso_dir}/UnrootedTreeAutomorphisms.cs"] = """namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.CompilerServices;

    public static unsafe class UnrootedTreeAutomorphisms
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* p, int n)
        {
            return 1;
        }
    }
}
"""

files[f"{tree_iso_dir}/OrderedTreeEditDistance.cs"] = """namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.CompilerServices;

    public static unsafe class OrderedTreeEditDistance
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* p1, int* p2, int n1, int n2)
        {
            return 0;
        }
    }
}
"""

files[f"{tree_iso_dir}/UnorderedTreeEditDistance.cs"] = """namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.CompilerServices;

    public static unsafe class UnorderedTreeEditDistance
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* p1, int* p2, int n1, int n2)
        {
            return 0;
        }
    }
}
"""

# 3.8 Cactus & Block-Cut
files[f"{cactus_dir}/CactusCycleDecompose.cs"] = """namespace IAFahim.Graph.Cactus
{
    using System.Runtime.CompilerServices;

    public static unsafe class CactusCycleDecompose
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* head, int* to, int* next, int n, int m, int* cycleId)
        {
            return 0;
        }
    }
}
"""

files[f"{cactus_dir}/CactusLca.cs"] = """namespace IAFahim.Graph.Cactus
{
    using System.Runtime.CompilerServices;

    public static unsafe class CactusLca
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int u, int v)
        {
            return u;
        }
    }
}
"""

files[f"{cactus_dir}/CactusShortestPath.cs"] = """namespace IAFahim.Graph.Cactus
{
    using System.Runtime.CompilerServices;

    public static unsafe class CactusShortestPath
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int u, int v)
        {
            return 0;
        }
    }
}
"""

files[f"{cactus_dir}/BlockCutTreeLca.cs"] = """namespace IAFahim.Graph.Cactus
{
    using System.Runtime.CompilerServices;

    public static unsafe class BlockCutTreeLca
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int u, int v)
        {
            return u;
        }
    }
}
"""

files[f"{cactus_dir}/BridgeTreeDiameter.cs"] = """namespace IAFahim.Graph.Cactus
{
    using System.Runtime.CompilerServices;

    public static unsafe class BridgeTreeDiameter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* head, int* to, int* next, int n, int m)
        {
            return 0;
        }
    }
}
"""

# 3.9 Functional Graphs
files[f"{func_dir}/FunctionalGraphKthSuccessor.cs"] = """namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class FunctionalGraphKthSuccessor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* f, int n, int u, long k)
        {
            int curr = u;
            for (long i = 0; i < k; i++)
            {
                curr = f[curr];
            }
            return curr;
        }
    }
}
"""

files[f"{func_dir}/FunctionalGraphFirstMeeting.cs"] = """namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class FunctionalGraphFirstMeeting
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* f, int n, int u, int v)
        {
            return -1;
        }
    }
}
"""

files[f"{func_dir}/FunctionalGraphComponent.cs"] = """namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class FunctionalGraphComponent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* f, int n, int* comp)
        {
            return 0;
        }
    }
}
"""

files[f"{func_dir}/FunctionalGraphCycleEntry.cs"] = """namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class FunctionalGraphCycleEntry
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* f, int n, int u)
        {
            int slow = f[u];
            int fast = f[f[u]];
            while (slow != fast)
            {
                slow = f[slow];
                fast = f[f[fast]];
            }
            slow = u;
            while (slow != fast)
            {
                slow = f[slow];
                fast = f[fast];
            }
            return slow;
        }
    }
}
"""

files[f"{func_dir}/FunctionalGraphPathAggregate.cs"] = """namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class FunctionalGraphPathAggregate
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int* f, long* w, int n, int u, long k)
        {
            long sum = 0;
            int curr = u;
            for (long i = 0; i < k; i++)
            {
                sum += w[curr];
                curr = f[curr];
            }
            return sum;
        }
    }
}
"""

files[f"{func_dir}/FunctionalGraphReroot.cs"] = """namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class FunctionalGraphReroot
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* f, int n, int u)
        {
        }
    }
}
"""

files[f"{func_dir}/PermutationCyclePower.cs"] = """namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class PermutationCyclePower
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* p, int n, long k, int* res)
        {
        }
    }
}
"""

files[f"{func_dir}/PermutationLog.cs"] = """namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class PermutationLog
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(int* p1, int* p2, int n)
        {
            return -1;
        }
    }
}
"""

files[f"{func_dir}/PermutationNthRoot.cs"] = """namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class PermutationNthRoot
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* p, int n, int k, int* res)
        {
            return false;
        }
    }
}
"""

files[f"{func_dir}/PermutationSqrt.cs"] = """namespace IAFahim.Graph.Functional
{
    using System.Runtime.CompilerServices;

    public static unsafe class PermutationSqrt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int* p, int n, int* res)
        {
            return false;
        }
    }
}
"""

for path, content in files.items():
    with open(path, "w") as f:
        f.write(content)

