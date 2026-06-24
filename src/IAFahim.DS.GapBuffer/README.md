# IAFahim.DS.GapBuffer

## Description
This package provides a gap buffer structure for efficient text editing operations. It keeps an empty gap at the current edit position, enabling fast insertion and deletion at that cursor offset. It avoids copying the entire buffer on consecutive edits.

## Complexity
- Insertion at cursor: O(K) where K is the length of inserted data.
- Deletion at cursor: O(L) where L is the length of deleted data.
- Moving cursor: O(D) where D is the distance moved.

## API Signature
```csharp
public unsafe struct GapBufferState
{
    public int Capacity;
    public int GapStart;
    public int GapEnd;
}

public static unsafe class GapBufferInsert
{
    public static void Run(ref GapBufferState s, int pos, byte* data, int len);
}

public static unsafe class GapBufferDelete
{
    public static void Run(ref GapBufferState s, int pos, int len);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.GapBuffer;

public static unsafe class Example
{
    public static void Run()
    {
        GapBufferState state = default;
        state.Capacity = 100;
        state.GapStart = 0;
        state.GapEnd = 100;
        byte* buffer = (byte*)Marshal.AllocHGlobal(state.Capacity * sizeof(byte));
        try
        {
            byte val = 65;
            GapBufferInsert.Run(ref state, 0, &val, 1);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)buffer);
        }
    }
}
```