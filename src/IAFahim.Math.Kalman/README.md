# IAFahim.Math.Kalman

## Description
Implements 1D scalar and 3D vector Kalman filtering for noise reduction and state estimation. Provides prediction and update steps, as well as utility functions to filter a series of input measurements.

## Complexity
- Predict / Update / PredictCovariance: O(1) time, O(1) space.
- Run: O(N) time, O(1) space.

## API Signature
- public static float ScalarKalmanFilter.Predict(float state, float velocity, float processNoise, float dt)
- public static float ScalarKalmanFilter.PredictCovariance(float covariance, float processNoise, float dt)
- public static float ScalarKalmanFilter.Update(float predictedState, float predictedCovariance, float measurement, float measurementNoise, out float updatedCovariance)
- public static void ScalarKalmanFilter.Run(float* measurements, int count, float processNoise, float measurementNoise, float* outFiltered)
- public static float3 VectorKalmanFilter.Predict(float3 state, float3 velocity, float processNoise, float dt)
- public static float3 VectorKalmanFilter.PredictCovariance(float3 covariance, float3 processNoise, float dt)
- public static float3 VectorKalmanFilter.Update(float3 predictedState, float3 predictedCov, float3 measurement, float measurementNoise, out float3 updatedCov)
- public static void VectorKalmanFilter.Run(float3* measurements, int count, float processNoise, float measurementNoise, float3* outFiltered)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Math.Kalman;

public unsafe class Example
{
    public static void Main()
    {
        int count = 5;
        float* measurements = (float*)Marshal.AllocHGlobal(count * sizeof(float));
        float* filtered = (float*)Marshal.AllocHGlobal(count * sizeof(float));
        try
        {
            measurements[0] = 1.0f;
            measurements[1] = 1.1f;
            measurements[2] = 0.9f;
            measurements[3] = 1.0f;
            measurements[4] = 1.2f;
            ScalarKalmanFilter.Run(measurements, count, 0.1f, 0.2f, filtered);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)measurements);
            Marshal.FreeHGlobal((IntPtr)filtered);
        }
    }
}
```