# IAFahim.Math.Quaternion

## Description
Offers mathematical operations for quaternions. Includes spherical linear interpolation (SLERP), conversions between quaternions and Euler angles or axis-angle representations, look rotation solvers, vector rotation, negating vector parts, normalization, and swing-twist decomposition.

## Complexity
- All operations: O(1) time, O(1) space.

## API Signature
- public static quaternion QuaternionSlerp.Run(quaternion from, quaternion to, float t)
- public static quaternion QuaternionOps.FromAxisAngle(float3 axis, float angleRadians)
- public static void QuaternionOps.ToAxisAngle(quaternion q, out float3 axis, out float angle)
- public static quaternion QuaternionOps.FromEuler(float3 eulerRadians)
- public static float3 QuaternionOps.ToEuler(quaternion q)
- public static quaternion QuaternionOps.LookRotation(float3 forward, float3 up)
- public static float3 QuaternionOps.RotateVector(quaternion q, float3 v)
- public static quaternion QuaternionOps.Conjugate(quaternion q)
- public static float QuaternionOps.Dot(quaternion a, quaternion b)
- public static float QuaternionOps.Length(quaternion q)
- public static quaternion QuaternionOps.Normalize(quaternion q)
- public static float QuaternionOps.AngleBetween(quaternion a, quaternion b)
- public static void SwingTwistDecomposition.Run(quaternion q, float3 twistAxis, out quaternion swing, out quaternion twist)
- public static float SwingTwistDecomposition.TwistAngle(quaternion q, float3 twistAxis)
- public static quaternion SwingTwistDecomposition.FromTwistAngle(float angle, float3 twistAxis)

## Usage Example
```csharp
using Unity.Mathematics;
using IAFahim.Math.Quaternion;

public unsafe class Example
{
    public static void Main()
    {
        quaternion q1 = quaternion.identity;
        float3 axis = new float3(0.0f, 1.0f, 0.0f);
        quaternion q2 = QuaternionOps.FromAxisAngle(axis, 1.57f);
        quaternion result = QuaternionSlerp.Run(q1, q2, 0.5f);
    }
}
```