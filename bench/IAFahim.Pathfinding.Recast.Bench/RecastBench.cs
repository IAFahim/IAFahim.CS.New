namespace IAFahim.Pathfinding.Recast.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using IAFahim.Pathfinding.Recast;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using Unity.Collections;
    using Unity.Mathematics;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<RecastBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class RecastBench
    {
        [Params(64, 256, 1024)]
        public int N;

        private DtNavMesh* _navMesh;
        private DtNavMeshQuery _query;
        private DtQueryFilter _filter;
        private DtPolyRef _startRef;
        private DtPolyRef _endRef;
        private float3 _startPos;
        private float3 _endPos;
        private DtPolyRef* _pathBuffer;

        [GlobalSetup]
        public void Setup()
        {
            _navMesh = DtNavMesh.Alloc(Allocator.Persistent);
            
            int vertCount = 2 * (N + 1);
            ushort3* verts = (ushort3*)Marshal.AllocHGlobal(vertCount * sizeof(ushort3));
            for (int x = 0; x <= N; ++x)
            {
                verts[x] = new ushort3((ushort)x, 0, 0);
                verts[x + N + 1] = new ushort3((ushort)x, 0, 1);
            }

            int polyCount = N;
            ushort* polys = (ushort*)Marshal.AllocHGlobal(polyCount * 8 * sizeof(ushort));
            for (int i = 0; i < polyCount * 8; ++i)
            {
                polys[i] = Detour.MeshNullIDX;
            }

            const ushort border = Detour.DTExtLink | 0xf;
            for (int i = 0; i < polyCount; ++i)
            {
                ushort v0 = (ushort)i;
                ushort v1 = (ushort)(i + N + 1);
                ushort v2 = (ushort)(i + N + 2);
                ushort v3 = (ushort)(i + 1);
                ushort n0 = i > 0 ? (ushort)(i - 1) : border;
                ushort n1 = border;
                ushort n2 = i < N - 1 ? (ushort)(i + 1) : border;
                ushort n3 = border;

                ushort* poly = polys + (i * 8);
                poly[0] = v0;
                poly[1] = v1;
                poly[2] = v2;
                poly[3] = v3;
                poly[4] = n0;
                poly[5] = n1;
                poly[6] = n2;
                poly[7] = n3;
            }

            ushort* polyFlags = (ushort*)Marshal.AllocHGlobal(polyCount * sizeof(ushort));
            byte* polyAreas = (byte*)Marshal.AllocHGlobal(polyCount * sizeof(byte));
            for (int i = 0; i < polyCount; ++i)
            {
                polyFlags[i] = 1;
                polyAreas[i] = Recast.RCWalkableArea;
            }

            var createParams = new DtNavMeshCreateParams
            {
                Verts = verts,
                VertCount = vertCount,
                Polys = polys,
                PolyFlags = polyFlags,
                PolyAreas = polyAreas,
                PolyCount = polyCount,
                Nvp = 4,
                TileX = 0,
                TileY = 0,
                TileLayer = 0,
                Bmin = float3.zero,
                Bmax = new float3((float)N, 1f, 1f),
                WalkableHeight = 2f,
                WalkableRadius = 0f,
                WalkableClimb = 0f,
                Cs = 1f,
                Ch = 1f,
                BuildBvTree = true,
            };

            if (!Detour.CreateNavMeshData(&createParams, out var navData, out var navDataSize, Allocator.Persistent))
            {
                throw new Exception();
            }

            if (Detour.StatusFailed(_navMesh->InitSingleTile(navData, navDataSize, DtTileFlags.TileFreeData)))
            {
                throw new Exception();
            }

            Marshal.FreeHGlobal((IntPtr)verts);
            Marshal.FreeHGlobal((IntPtr)polys);
            Marshal.FreeHGlobal((IntPtr)polyFlags);
            Marshal.FreeHGlobal((IntPtr)polyAreas);

            _query = new DtNavMeshQuery(_navMesh, N * 2 + 64, Allocator.Persistent);
            _filter = DtQueryFilter.CreateDefault();

            var tile = _navMesh->GetTileAt(0, 0, 0);
            var tileRef = _navMesh->GetTileRef(tile);
            _navMesh->DecodePolyId(tileRef, out var salt, out var tileIndex, out _);

            _startRef = _navMesh->EncodePolyId(salt, tileIndex, 0u);
            _endRef = _navMesh->EncodePolyId(salt, tileIndex, (uint)(N - 1));

            _startPos = new float3(0.5f, 0f, 0.5f);
            _endPos = new float3((float)N - 0.5f, 0f, 0.5f);

            _pathBuffer = (DtPolyRef*)Marshal.AllocHGlobal(N * sizeof(DtPolyRef));
        }

        [Benchmark(Baseline = true)]
        public void FindPath()
        {
            var status = _query.FindPath(_startRef, _endRef, _startPos, _endPos, ref _filter, _pathBuffer, out var pathCount, N);
        }

        [Benchmark]
        public void Raycast()
        {
            var status = _query.Raycast(_startRef, _startPos, _endPos, ref _filter, out var t, out _, _pathBuffer, out var pathCount, N);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _query.Dispose();
            DtNavMesh.Free(_navMesh);
            Marshal.FreeHGlobal((IntPtr)_pathBuffer);
        }
    }
}
