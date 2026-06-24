# IAFahim.Math.SphericalHarmonics

## Description
Implements Spherical Harmonics projection and evaluation up to band 2 (9 coefficients). Provides functions for basis function evaluation, projection of directional samples, irradiance convolution, and reconstruction.

## Complexity
- Basis / EvaluateL2 / EvalL2 / Convolve: O(1) time, O(1) space.
- ProjectL2: O(sampleCount) time, O(1) space.

## API Signature
- public static float SHEvaluation.BasisL0M0()
- public static void SHEvaluation.EvaluateL2(float3 direction, float* outCoeffs)
- public static void SHEvaluation.ProjectL2(float3* directions, float* values, int sampleCount, float* outCoeffs)
- public static float SHEvaluation.EvalL2(float3 direction, float* coeffs)
- public static void SHEvaluation.ConvolveWithCosineKernelL2(float* irradianceCoeffs, float* radianceCoeffs)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using IAFahim.Math.SphericalHarmonics;

public unsafe class Example
{
    public static void Main()
    {
        float3 dir = new float3(0.0f, 1.0f, 0.0f);
        float* coeffs = (float*)Marshal.AllocHGlobal(9 * sizeof(float));
        try
        {
            SHEvaluation.EvaluateL2(dir, coeffs);
            float val = SHEvaluation.EvalL2(dir, coeffs);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)coeffs);
        }
    }
}
```