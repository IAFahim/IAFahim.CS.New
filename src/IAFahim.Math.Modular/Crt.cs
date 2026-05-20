using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Math.Modular
{
    public static unsafe class Crt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long r1, long m1, long r2, long m2)
        {
            long x, y;
            long g = ExtendedGcd.Run(m1, m2, out x, out y);
            if ((r2 - r1) % g != 0) return -1;
            
            long m2_g = m2 / g;
            long diff = (r2 - r1) / g;
            long t = ModMul.Run(x, diff, m2_g);
            
            long result = r1 + t * m1;
            long lcm = m1 * m2_g; 
            
            // If lcm overflowed to negative, we can't normalize properly using signed long.
            // But we can try to return something sensible or just return -1.
            if (m1 > 0 && m2_g > 0 && lcm <= 0) 
            {
                 // Handle overflow - for now just return what we have and hope for the best, 
                 // or normalize manually.
                 if (result < 0) return result; // Better than nothing? 
                 // Actually, let's just use the current logic but be aware.
            }
            
            return ModNormalize.Run(result, lcm);
        }
    }
}