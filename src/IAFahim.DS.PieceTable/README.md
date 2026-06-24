# IAFahim.DS.PieceTable

## Description
A piece table data structure designed for text editing. It tracks changes using an original buffer, an append buffer, and a sequence of pieces pointing to segments of either buffer.

## Complexity
O(P) for insert and delete operations, where P is the number of pieces.

## API Signature
```csharp
public unsafe struct Piece
{
    public int Start;
    public int Length;
    public bool IsAddBuffer;
}
public unsafe struct PieceTableState
{
    public byte* OriginalBuffer;
    public int OriginalLength;
    public byte* AddBuffer;
    public int AddLength;
    public Piece* Pieces;
    public int PieceCount;
    public int PieceCapacity;
}
public static unsafe class PieceTableInsert
{
    public static void Run(ref PieceTableState s, int pos, byte* data, int len, Piece* pieceHistory, int* historyCount)
}
public static unsafe class PieceTableDelete
{
    public static void Run(ref PieceTableState s, int pos, int len, Piece* pieceHistory, int* historyCount)
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
        PieceTableState state = default;
        Piece* history = (Piece*)Marshal.AllocHGlobal(10 * sizeof(Piece));
        int* historyCount = (int*)Marshal.AllocHGlobal(sizeof(int));
        byte* data = (byte*)Marshal.AllocHGlobal(5 * sizeof(byte));
        try
        {
            *historyCount = 0;
            data[0] = 65;
            PieceTableInsert.Run(ref state, 0, data, 1, history, historyCount);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)history);
            Marshal.FreeHGlobal((IntPtr)historyCount);
            Marshal.FreeHGlobal((IntPtr)data);
        }
    }
}
```