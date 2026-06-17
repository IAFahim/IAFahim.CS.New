namespace IAFahim.Geometry.Frame
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    public static unsafe class ParallelTransport
    {
        private const float Zero = 0.0f;
        private const float One = 1.0f;
        private const float Two = 2.0f;
        private const float Threshold = 1e-6f;

        public static void Compute(float3* positions, int count, float3 initialNormal, float3* outRight, float3* outUp, float3* outForward)
        {
            if (count <= 0)
            {
                return;
            }

            if (count == 1)
            {
                float3 fwd = new float3(Zero, Zero, One);
                float3 up = math.normalize(initialNormal);
                if (math.abs(math.dot(fwd, up)) > One - Threshold)
                {
                    fwd = new float3(Zero, One, Zero);
                }
                float3 right = math.normalize(math.cross(up, fwd));
                fwd = math.normalize(math.cross(right, up));

                outForward[0] = fwd;
                outUp[0] = up;
                outRight[0] = right;
                return;
            }

            float3* tangents = stackalloc float3[count];
            for (int i = 0; i < count - 1; i++)
            {
                float3 diff = positions[i + 1] - positions[i];
                float len = math.length(diff);
                tangents[i] = len > Threshold ? diff / len : new float3(Zero, Zero, One);
            }
            tangents[count - 1] = tangents[count - 2];

            float3 t0 = tangents[0];
            float3 n0 = math.normalize(initialNormal);
            float dotT0N0 = math.dot(t0, n0);
            if (math.abs(dotT0N0) > One - Threshold)
            {
                float3 alt = new float3(Zero, One, Zero);
                if (math.abs(math.dot(t0, alt)) > One - Threshold)
                {
                    alt = new float3(Zero, Zero, One);
                }
                n0 = math.normalize(math.cross(t0, alt));
            }
            else
            {
                n0 = math.normalize(n0 - dotT0N0 * t0);
            }

            float3 b0 = math.normalize(math.cross(t0, n0));
            outForward[0] = t0;
            outUp[0] = n0;
            outRight[0] = b0;

            for (int i = 0; i < count - 1; i++)
            {
                float3 pI = positions[i];
                float3 pNext = positions[i + 1];
                float3 tI = tangents[i];
                float3 tNext = tangents[i + 1];
                float3 nI = outUp[i];

                float3 v1 = pNext - pI;
                float c1 = math.dot(v1, v1);

                float3 uI = nI;
                float3 tIL = tI;

                if (c1 > Threshold * Threshold)
                {
                    uI = nI - (Two / c1) * math.dot(v1, nI) * v1;
                    tIL = tI - (Two / c1) * math.dot(v1, tI) * v1;
                }

                float3 v2 = tNext - tIL;
                float c2 = math.dot(v2, v2);

                float3 nNext = uI;
                if (c2 > Threshold)
                {
                    nNext = uI - (Two / c2) * math.dot(v2, uI) * v2;
                }

                nNext = math.normalize(nNext);
                float3 bNext = math.normalize(math.cross(tNext, nNext));

                outForward[i + 1] = tNext;
                outUp[i + 1] = nNext;
                outRight[i + 1] = bNext;
            }
        }
    }
}
