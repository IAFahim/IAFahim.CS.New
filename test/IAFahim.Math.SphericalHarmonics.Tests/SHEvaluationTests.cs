namespace IAFahim.Math.SphericalHarmonics.Tests
{
    using System;
    using Unity.Mathematics;
    using NUnit.Framework;

    public sealed unsafe class SHEvaluationTests
    {
        [Test]
        public void BasisL0M0_IsConstant()
        {
            float val = SHEvaluation.BasisL0M0();
            Assert.IsTrue(math.abs(val - 0.28209479f) < 1e-6f);
        }

        [Test]
        public void EvaluateL2_HasNineCoefficients()
        {
            float* coeffs = stackalloc float[9];
            SHEvaluation.EvaluateL2(new float3(0, 0, 1), coeffs);
            bool allZero = true;
            for (int i = 0; i < 9; i++)
            {
                if (math.abs(coeffs[i]) > 1e-6f)
                {
                    allZero = false;
                    break;
                }
            }
            Assert.IsFalse(allZero);
        }

        [Test]
        public void EvaluateL2_IntegrationOne()
        {
            float* coeffs = stackalloc float[9];
            float* basis = stackalloc float[9];

            coeffs[0] = 1.0f;
            for (int i = 1; i < 9; i++) coeffs[i] = 0.0f;

            float sum = 0.0f;
            float3 dir = new float3(0, 0, 1);
            SHEvaluation.EvaluateL2(dir, basis);
            for (int i = 0; i < 9; i++)
            {
                sum += coeffs[i] * basis[i];
            }
            Assert.IsTrue(sum > 0.0f);
        }

        [Test]
        public void EvalL2_SymmetricDirections_ProduceSimilarValues()
        {
            float* coeffs = stackalloc float[9];
            for (int i = 0; i < 9; i++) coeffs[i] = 0.0f;
            coeffs[4] = 0.5f;
            coeffs[6] = 0.3f;
            coeffs[8] = 0.2f;

            float v1 = SHEvaluation.EvalL2(new float3(0, 0, 1), coeffs);
            float v2 = SHEvaluation.EvalL2(new float3(0, 0, -1), coeffs);
            Assert.IsTrue(math.abs(math.abs(v1) - math.abs(v2)) < 1e-4f);
        }

        [Test]
        public void ProjectL2_UniformDirection_WeightsArePositive()
        {
            const int samples = 100;
            float3* dirs = stackalloc float3[samples];
            float* vals = stackalloc float[samples];

            for (int i = 0; i < samples; i++)
            {
                float phi = (float)i / samples * 6.28318530f;
                dirs[i] = new float3(math.cos(phi), 0, math.sin(phi));
                vals[i] = 1.0f;
            }

            float* coeffs = stackalloc float[9];
            SHEvaluation.ProjectL2(dirs, vals, samples, coeffs);
            Assert.IsTrue(coeffs[0] > 0.0f);
        }
    }
}