namespace IAFahim.Graph.Matching
{
    using System.Runtime.CompilerServices;

    public static unsafe class HospitalResidentsMatching
    {
        // Resident-proposing Gale-Shapley for the Hospital/Residents problem.
        // residentPref[i * numHospitals + t]   = t-th preferred hospital of resident i.
        // hospitalPref[h * numResidents + t]   = t-th preferred resident of hospital h.
        // hospitalCapacities[h]                = capacity of hospital h.
        // matchResident[i]                     = assigned hospital index, or -1.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* residentPref, int* hospitalPref, int* hospitalCapacities, int numResidents, int numHospitals, int* matchResident)
        {
            for (int i = 0; i < numResidents; i++) matchResident[i] = -1;

            if (numResidents <= 0 || numHospitals <= 0) return;

            // rankHosp[h * numResidents + r] = preference rank of resident r at hospital h (lower = preferred).
            int* rankHosp = stackalloc int[numHospitals * numResidents];
            for (int i = 0; i < numHospitals * numResidents; i++) rankHosp[i] = int.MaxValue;
            for (int h = 0; h < numHospitals; h++)
                for (int t = 0; t < numResidents; t++)
                    rankHosp[h * numResidents + hospitalPref[h * numResidents + t]] = t;

            int* nextProp = stackalloc int[numResidents];
            int* heldCount = stackalloc int[numHospitals];
            // held[h * numResidents + slot] holds the currently-accepted resident indices at hospital h.
            int* held = stackalloc int[numHospitals * numResidents];
            for (int i = 0; i < numResidents; i++) nextProp[i] = 0;
            for (int h = 0; h < numHospitals; h++) heldCount[h] = 0;

            int* queue = stackalloc int[numResidents];
            int qh = 0, qt = 0;
            for (int i = 0; i < numResidents; i++) queue[qt++] = i;

            while (qh < qt)
            {
                int r = queue[qh++];
                if (nextProp[r] >= numHospitals) continue;
                int h = residentPref[r * numHospitals + nextProp[r]];
                nextProp[r]++;

                if (heldCount[h] < hospitalCapacities[h])
                {
                    held[h * numResidents + heldCount[h]++] = r;
                    matchResident[r] = h;
                }
                else
                {
                    // Find the worst-ranked currently-held resident at hospital h.
                    int worstSlot = 0;
                    int worstRank = rankHosp[h * numResidents + held[h * numResidents]];
                    for (int s = 1; s < heldCount[h]; s++)
                    {
                        int rr = rankHosp[h * numResidents + held[h * numResidents + s]];
                        if (rr > worstRank) { worstRank = rr; worstSlot = s; }
                    }
                    int worst = held[h * numResidents + worstSlot];
                    if (rankHosp[h * numResidents + r] < worstRank)
                    {
                        // r is preferred over the worst held -> evict worst, accept r.
                        matchResident[worst] = -1;
                        queue[qt++] = worst;
                        held[h * numResidents + worstSlot] = r;
                        matchResident[r] = h;
                    }
                    else
                    {
                        // h rejects r; r will propose again next round.
                        queue[qt++] = r;
                    }
                }
            }
        }
    }
}
