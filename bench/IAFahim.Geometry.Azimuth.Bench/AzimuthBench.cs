namespace IAFahim.Geometry.Azimuth.Bench
{
    using System;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using IAFahim.Geometry.Azimuth;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<AzimuthBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class AzimuthBench
    {
        [Params(10000)]
        public int N;

        [Benchmark]
        public void CartesianAzimuth_Bench()
        {
            double sum = 0.0;
            for (int i = 0; i < N; i++)
            {
                sum += CartesianAzimuth.Run(0.0, 0.0, (double)i, (double)(i + 1));
            }
        }

        [Benchmark]
        public void SphericalAzimuth_Bench()
        {
            double sum = 0.0;
            double lat1 = 0.1;
            double lon1 = 0.2;
            for (int i = 0; i < N; i++)
            {
                double lat2 = 0.1 + (double)i * 0.0001;
                double lon2 = 0.2 + (double)i * 0.0001;
                sum += SphericalAzimuth.Run(lat1, lon1, lat2, lon2);
            }
        }

        [Benchmark]
        public void SphericalDistance_Bench()
        {
            double sum = 0.0;
            double lat1 = 0.1;
            double lon1 = 0.2;
            double r = 6371000.0;
            for (int i = 0; i < N; i++)
            {
                double lat2 = 0.1 + (double)i * 0.0001;
                double lon2 = 0.2 + (double)i * 0.0001;
                sum += SphericalDistance.Run(lat1, lon1, lat2, lon2, r);
            }
        }
    }
}
