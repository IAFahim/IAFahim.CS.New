# IAFahim.DS.Trie

## Description
A trie structure supporting byte sequences, binary values, and persistent versions. Useful for prefix matching, word tracking, and bitwise XOR query operations.

## Complexity
O(L) for inserts, deletions, and searches, where L is the length of the key or the number of bits.

## API Signature
```csharp
public static unsafe class TrieInsert
{
    public static void Run(int* trie, int node, byte* s, int len)
}
public static unsafe class TrieFind
{
    public static bool Run(int* trie, int node, byte* s, int len)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        int* trie = (int*)Marshal.AllocHGlobal(100 * sizeof(int));
        byte* word = (byte*)Marshal.AllocHGlobal(5 * sizeof(byte));
        try
        {
            for (int i = 0; i < 100; i++)
            {
                trie[i] = 0;
            }
            word[0] = 97;
            word[1] = 98;
            TrieInsert.Run(trie, 0, word, 2);
            bool found = TrieFind.Run(trie, 0, word, 2);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)trie);
            Marshal.FreeHGlobal((IntPtr)word);
        }
    }
}
```