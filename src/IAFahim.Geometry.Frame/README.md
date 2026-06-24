# IAFahim.Geometry.Frame

## Description
This package provides methods for frame generation along a curve. It utilizes parallel transport to construct consistent orthogonal frames without twist.

## Complexity
The parallel transport frame solver runs in O(N) time complexity, where N is the point count.

## API Signature
public static class ParallelTransport
{
    public static void Compute(float3* positions, int count, float3 initialNormal, float3* outRight, float3* outUp, float3* outForward);
}

## Usage Example
```csharp
unsafe
{
    int size = 5;
    float3* pos = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(float3));
    float3* right = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(float3));
    float3* up = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(float3));
    float3* forward = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(float3));
    try
    {
        for (int i = 0; i < size; i++)
        {
            pos[i] = new float3((float)i, 0.0f, 0.0f);
        }
        float3 normal = new float3(0.0f, 1.0f, 0.0f);
        IAFahim.Geometry.Frame.ParallelTransport.Compute(pos, size, normal, right, up, forward);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)pos);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)right);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)up);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)forward);
    }
}
```
