namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;

    public static unsafe class HospitalResidentsMatching
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* residentPref, int* hospitalPref, int* hospitalCapacities, int numResidents, int numHospitals, int* matchResident)
        {
            for (int i = 0; i < numResidents; i++) matchResident[i] = -1;
        }
    }
}