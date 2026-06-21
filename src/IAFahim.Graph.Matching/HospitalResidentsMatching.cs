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
                    Accept(held, heldCount, matchResident, h, r, numResidents);
                }
                else
                {
                    Contest(rankHosp, held, matchResident, queue, ref qt, h, r, heldCount[h], numResidents);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Accept(int* held, int* heldCount, int* matchResident, int h, int r, int numResidents)
        {
            held[h * numResidents + heldCount[h]++] = r;
            matchResident[r] = h;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindWorstSlot(int* rankHosp, int* held, int h, int count, int numResidents)
        {
            int worstSlot = 0;
            int worstRank = rankHosp[h * numResidents + held[h * numResidents]];
            for (int s = 1; s < count; s++)
            {
                int rr = rankHosp[h * numResidents + held[h * numResidents + s]];
                if (rr > worstRank) { worstRank = rr; worstSlot = s; }
            }
            return worstSlot;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Contest(int* rankHosp, int* held, int* matchResident, int* queue, ref int qt, int h, int r, int count, int numResidents)
        {
            int worstSlot = FindWorstSlot(rankHosp, held, h, count, numResidents);
            int worst = held[h * numResidents + worstSlot];
            if (rankHosp[h * numResidents + r] < rankHosp[h * numResidents + worst])
            {
                matchResident[worst] = -1;
                queue[qt++] = worst;
                held[h * numResidents + worstSlot] = r;
                matchResident[r] = h;
            }
            else
            {
                queue[qt++] = r;
            }
        }
    }
}
