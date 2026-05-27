namespace IAFahim.Math.SphericalHarmonics
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class SHEvaluation
    {
        private const float Pi = 3.14159265f;
        private const float SqrtPi = 1.77245385f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float BasisL0M0()
        {
            return 0.28209479f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float BasisL1M1(float x, float y, float z)
        {
            return -0.48860251f * y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float BasisL1M0(float x, float y, float z)
        {
            return 0.48860251f * z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float BasisL1P1(float x, float y, float z)
        {
            return -0.48860251f * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float BasisL2M2(float x, float y, float z)
        {
            return 1.09254843f * x * y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float BasisL2M1(float x, float y, float z)
        {
            return -1.09254843f * y * z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float BasisL2M0(float x, float y, float z)
        {
            return 0.31539157f * (3.0f * z * z - 1.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float BasisL2P1(float x, float y, float z)
        {
            return -1.09254843f * x * z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float BasisL2P2(float x, float y, float z)
        {
            return 0.54627421f * (x * x - y * y);
        }

        public static void EvaluateL2(float3 direction, float* outCoeffs)
        {
            float x = direction.x, y = direction.y, z = direction.z;
            outCoeffs[0] = BasisL0M0();
            outCoeffs[1] = BasisL1M1(x, y, z);
            outCoeffs[2] = BasisL1M0(x, y, z);
            outCoeffs[3] = BasisL1P1(x, y, z);
            outCoeffs[4] = BasisL2M2(x, y, z);
            outCoeffs[5] = BasisL2M1(x, y, z);
            outCoeffs[6] = BasisL2M0(x, y, z);
            outCoeffs[7] = BasisL2P1(x, y, z);
            outCoeffs[8] = BasisL2P2(x, y, z);
        }

        public static void ProjectL2(float3* directions, float* values, int sampleCount, float* outCoeffs)
        {
            for (int i = 0; i < 9; i++)
            {
                outCoeffs[i] = 0.0f;
            }

            float* basis = stackalloc float[9];

            for (int i = 0; i < sampleCount; i++)
            {
                EvaluateL2(directions[i], basis);
                for (int j = 0; j < 9; j++)
                {
                    outCoeffs[j] += values[i] * basis[j];
                }
            }

            float weight = 4.0f * Pi / (float)sampleCount;
            for (int i = 0; i < 9; i++)
            {
                outCoeffs[i] *= weight;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EvalL2(float3 direction, float* coeffs)
        {
            float* basis = stackalloc float[9];
            EvaluateL2(direction, basis);
            float result = 0.0f;
            for (int i = 0; i < 9; i++)
            {
                result += coeffs[i] * basis[i];
            }

            return result;
        }

        public static void ConvolveWithCosineKernelL2(float* irradianceCoeffs, float* radianceCoeffs)
        {
            float a0 = Pi;
            float a1 = (2.0f * Pi) / 3.0f;
            float a2 = Pi * 0.25f;

            irradianceCoeffs[0] = radianceCoeffs[0] * a0;
            irradianceCoeffs[1] = radianceCoeffs[1] * a1;
            irradianceCoeffs[2] = radianceCoeffs[2] * a1;
            irradianceCoeffs[3] = radianceCoeffs[3] * a1;
            irradianceCoeffs[4] = radianceCoeffs[4] * a2;
            irradianceCoeffs[5] = radianceCoeffs[5] * a2;
            irradianceCoeffs[6] = radianceCoeffs[6] * a2;
            irradianceCoeffs[7] = radianceCoeffs[7] * a2;
            irradianceCoeffs[8] = radianceCoeffs[8] * a2;
        }
    }
}
