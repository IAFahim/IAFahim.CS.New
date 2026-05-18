namespace IAFahim.Math.NT
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class RandomShuffle
    {
        public static void Run<T>(T* ptr, int len) where T : unmanaged
        {
            for (int i = len - 1; i > 0; i--)
            {
                int j = RandomInt.Next(i + 1);
                T tmp = ptr[i];
                ptr[i] = ptr[j];
                ptr[j] = tmp;
            }
        }
    }
}