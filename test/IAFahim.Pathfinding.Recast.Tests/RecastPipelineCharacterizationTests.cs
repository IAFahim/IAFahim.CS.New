// <copyright file="RecastPipelineCharacterizationTests.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace IAFahim.Pathfinding.Recast.Tests
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using Unity.Collections;
    using Unity.Mathematics;

    // Golden-master characterization tests: run the full Recast pipeline over a fixed
    // deterministic terrain and FNV-1a-hash every stage output. The hash constants are
    // the locked baseline. Any refactor that changes even one output byte fails here.
    public unsafe class RecastPipelineCharacterizationTests
    {
        private const uint FnvOffset = 2166136261u;
        private const uint FnvPrime = 16777619u;

        private const int GridExtent = 12;
        private const float CellSize = 1f;
        private const float CellHeight = 1f;
        private const float WalkableSlopeAngle = 50f;

        [Test]
        public void PipelineOutputsMatchGoldenMaster()
        {
            BuildInputs(out float3* verts, out int3* tris, out int vertCount, out int triCount, out float3 bmin, out float3 bmax);
            try
            {
                Assert.Multiple(() =>
                {
                    AssertHash("regions", 0x1076963au, HashRegions(verts, tris, vertCount, triCount, bmin, bmax));
                    AssertHash("contours", 0x050c5d1fu, HashContours(verts, tris, vertCount, triCount, bmin, bmax));
                    AssertHash("polymesh", 0x48b0f491u, HashPolyMesh(verts, tris, vertCount, triCount, bmin, bmax));
                    AssertHash("detail", 0x4ab0f7b7u, HashDetail(verts, tris, vertCount, triCount, bmin, bmax));
                    AssertHash("layers", 0xeb741d64u, HashLayers(verts, tris, vertCount, triCount, bmin, bmax));
                });
            }
            finally
            {
                Marshal.FreeHGlobal((nint)verts);
                Marshal.FreeHGlobal((nint)tris);
            }
        }

        // ---- input mesh: deterministic stepped terrain, three height tiers ----
        private static void BuildInputs(out float3* verts, out int3* tris, out int vertCount, out int triCount, out float3 bmin, out float3 bmax)
        {
            int side = GridExtent + 1;
            vertCount = side * side;
            verts = (float3*)Marshal.AllocHGlobal(vertCount * sizeof(float3));

            for (int z = 0; z < side; z++)
            {
                for (int x = 0; x < side; x++)
                {
                    int tier = ((x / 3) + (z / 3)) % 3;
                    float h = tier * 3f;
                    verts[(z * side) + x] = new float3(x, h, z);
                }
            }

            int quads = GridExtent * GridExtent;
            triCount = quads * 2;
            tris = (int3*)Marshal.AllocHGlobal(triCount * sizeof(int3));

            int t = 0;
            for (int z = 0; z < GridExtent; z++)
            {
                for (int x = 0; x < GridExtent; x++)
                {
                    int v00 = (z * side) + x;
                    int v10 = (z * side) + x + 1;
                    int v01 = ((z + 1) * side) + x;
                    int v11 = ((z + 1) * side) + x + 1;
                    tris[t++] = new int3(v00, v10, v11);
                    tris[t++] = new int3(v00, v11, v01);
                }
            }

            Recast.CalcBounds(verts, vertCount, out bmin, out bmax);
        }

        private static RcCompactHeightfield BuildCompactHeightfield(float3* verts, int3* tris, int triCount, float3 bmin, float3 bmax, byte* triAreas)
        {
            Recast.CalcGridSize(in bmin, in bmax, CellSize, out int width, out int height);

            RcHeightfield heightfield = new RcHeightfield(Allocator.Temp);
            RcCompactHeightfield compactHeightfield = new RcCompactHeightfield(Allocator.Temp);
            try
            {
                Recast.CreateHeightfield(&heightfield, width, height, bmin, bmax, CellSize, CellHeight);

                Recast.MarkWalkableTriangles(WalkableSlopeAngle, verts, tris, triAreas, triCount);
                Recast.RasterizeTriangles(verts, tris, triAreas, triCount, &heightfield);
                Recast.FilterLedgeSpans(2, 2, &heightfield);
                Recast.FilterLowHangingWalkableObstacles(2, &heightfield);
                Recast.FilterWalkableLowHeightSpans(2, &heightfield);

                Recast.BuildCompactHeightfield(2, 2, &heightfield, &compactHeightfield);
            }
            finally
            {
                heightfield.Dispose();
            }
            return compactHeightfield;
        }

        private static uint HashRegions(float3* verts, int3* tris, int vertCount, int triCount, float3 bmin, float3 bmax)
        {
            byte* triAreas = (byte*)Marshal.AllocHGlobal(triCount);
            for (int i = 0; i < triCount; i++) triAreas[i] = 0;

            RcCompactHeightfield compactHeightfield = BuildCompactHeightfield(verts, tris, triCount, bmin, bmax, triAreas);
            try
            {
                Recast.BuildRegions(&compactHeightfield, 0, 0, 0);

                uint h = FnvOffset;
                h = Mix(h, (uint)compactHeightfield.SpanCount);
                h = Mix(h, (uint)compactHeightfield.MaxRegions);
                for (int i = 0; i < compactHeightfield.SpanCount; i++)
                {
                    h = Mix(h, compactHeightfield.Spans[i].Reg);
                    h = Mix(h, compactHeightfield.Spans[i].Y);
                    h = Mix(h, compactHeightfield.Spans[i].H);
                    h = Mix(h, compactHeightfield.Spans[i].Con);
                    h = Mix(h, compactHeightfield.Areas[i]);
                }
                return h;
            }
            finally
            {
                compactHeightfield.Dispose();
                Marshal.FreeHGlobal((nint)triAreas);
            }
        }

        private static uint HashContours(float3* verts, int3* tris, int vertCount, int triCount, float3 bmin, float3 bmax)
        {
            byte* triAreas = (byte*)Marshal.AllocHGlobal(triCount);
            for (int i = 0; i < triCount; i++) triAreas[i] = 0;

            RcCompactHeightfield compactHeightfield = BuildCompactHeightfield(verts, tris, triCount, bmin, bmax, triAreas);
            RcContourSet contourSet = new RcContourSet(Allocator.Temp);
            try
            {
                Recast.BuildRegions(&compactHeightfield, 0, 0, 0);
                Recast.BuildContours(&compactHeightfield, 1.2f, 12, &contourSet);

                uint h = FnvOffset;
                h = Mix(h, (uint)contourSet.Nconts);
                for (int i = 0; i < contourSet.Nconts; i++)
                {
                    RcContour* c = &contourSet.Conts[i];
                    h = Mix(h, (uint)c->NVerts);
                    h = Mix(h, (uint)c->NRVerts);
                    h = Mix(h, c->Reg);
                    h = Mix(h, c->Area);
                    for (int v = 0; v < c->NVerts; v++) h = MixInt4(h, c->Verts[v]);
                    for (int v = 0; v < c->NRVerts; v++) h = MixInt4(h, c->RVerts[v]);
                }
                return h;
            }
            finally
            {
                contourSet.Dispose();
                compactHeightfield.Dispose();
                Marshal.FreeHGlobal((nint)triAreas);
            }
        }

        private static uint HashPolyMesh(float3* verts, int3* tris, int vertCount, int triCount, float3 bmin, float3 bmax)
        {
            byte* triAreas = (byte*)Marshal.AllocHGlobal(triCount);
            for (int i = 0; i < triCount; i++) triAreas[i] = 0;

            RcCompactHeightfield compactHeightfield = BuildCompactHeightfield(verts, tris, triCount, bmin, bmax, triAreas);
            RcContourSet contourSet = new RcContourSet(Allocator.Temp);
            RcPolyMesh mesh = new RcPolyMesh(Allocator.Temp);
            try
            {
                Recast.BuildRegions(&compactHeightfield, 0, 0, 0);
                Recast.BuildContours(&compactHeightfield, 1.2f, 12, &contourSet);
                Recast.BuildPolyMesh(&contourSet, 6, &mesh);

                uint h = FnvOffset;
                h = Mix(h, (uint)mesh.NVerts);
                h = Mix(h, (uint)mesh.NPolys);
                h = Mix(h, (uint)mesh.Nvp);
                for (int i = 0; i < mesh.NVerts; i++) h = MixUshort3(h, mesh.Verts[i]);
                for (int i = 0; i < mesh.NPolys * mesh.Nvp * 2; i++) h = Mix(h, mesh.Polys[i]);
                for (int i = 0; i < mesh.NPolys; i++) h = Mix(h, mesh.Regs[i]);
                for (int i = 0; i < mesh.NPolys; i++) h = Mix(h, mesh.Flags[i]);
                for (int i = 0; i < mesh.NPolys; i++) h = Mix(h, mesh.Areas[i]);
                return h;
            }
            finally
            {
                mesh.Dispose();
                contourSet.Dispose();
                compactHeightfield.Dispose();
                Marshal.FreeHGlobal((nint)triAreas);
            }
        }

        private static uint HashDetail(float3* verts, int3* tris, int vertCount, int triCount, float3 bmin, float3 bmax)
        {
            byte* triAreas = (byte*)Marshal.AllocHGlobal(triCount);
            for (int i = 0; i < triCount; i++) triAreas[i] = 0;

            RcCompactHeightfield compactHeightfield = BuildCompactHeightfield(verts, tris, triCount, bmin, bmax, triAreas);
            RcContourSet contourSet = new RcContourSet(Allocator.Temp);
            RcPolyMesh mesh = new RcPolyMesh(Allocator.Temp);
            RcPolyMeshDetail detail = new RcPolyMeshDetail(Allocator.Temp);
            try
            {
                Recast.BuildRegions(&compactHeightfield, 0, 0, 0);
                Recast.BuildContours(&compactHeightfield, 1.2f, 12, &contourSet);
                Recast.BuildPolyMesh(&contourSet, 6, &mesh);
                Recast.BuildPolyMeshDetail(&mesh, &compactHeightfield, 6f, 1f, &detail);

                uint h = FnvOffset;
                h = Mix(h, (uint)detail.NMeshes);
                h = Mix(h, (uint)detail.NVerts);
                h = Mix(h, (uint)detail.NTris);
                for (int i = 0; i < detail.NMeshes; i++) h = MixUint4(h, detail.Meshes[i]);
                for (int i = 0; i < detail.NVerts; i++) h = MixFloat3(h, detail.Verts[i]);
                for (int i = 0; i < detail.NTris; i++) h = MixByte4(h, detail.Tris[i]);
                return h;
            }
            finally
            {
                detail.Dispose();
                mesh.Dispose();
                contourSet.Dispose();
                compactHeightfield.Dispose();
                Marshal.FreeHGlobal((nint)triAreas);
            }
        }

        private static uint HashLayers(float3* verts, int3* tris, int vertCount, int triCount, float3 bmin, float3 bmax)
        {
            byte* triAreas = (byte*)Marshal.AllocHGlobal(triCount);
            for (int i = 0; i < triCount; i++) triAreas[i] = 0;

            RcCompactHeightfield compactHeightfield = BuildCompactHeightfield(verts, tris, triCount, bmin, bmax, triAreas);
            RcHeightfieldLayerSet layerSet = new RcHeightfieldLayerSet(Allocator.Temp);
            try
            {
                bool ok = Recast.BuildHeightfieldLayers(&compactHeightfield, 0, 2, &layerSet);

                uint h = FnvOffset;
                h = Mix(h, ok ? 1u : 0u);
                h = Mix(h, (uint)layerSet.NLayers);
                for (int i = 0; i < layerSet.NLayers; i++)
                {
                    RcHeightfieldLayer* layer = &layerSet.Layers[i];
                    h = MixFloat3(h, layer->BoundMin);
                    h = MixFloat3(h, layer->BoundMax);
                    h = Mix(h, (uint)layer->Width);
                    h = Mix(h, (uint)layer->Height);
                    h = Mix(h, (uint)layer->MinX);
                    h = Mix(h, (uint)layer->MaxX);
                    h = Mix(h, (uint)layer->MinY);
                    h = Mix(h, (uint)layer->MaxY);
                    h = Mix(h, (uint)layer->HeightMin);
                    h = Mix(h, (uint)layer->HeightMax);
                    int cells = layer->Width * layer->Height;
                    for (int c = 0; c < cells; c++) h = Mix(h, layer->Heights[c]);
                    for (int c = 0; c < cells; c++) h = Mix(h, layer->Areas[c]);
                    for (int c = 0; c < cells * 4; c++) h = Mix(h, layer->Cons[c]);
                }
                return h;
            }
            finally
            {
                layerSet.Dispose();
                compactHeightfield.Dispose();
                Marshal.FreeHGlobal((nint)triAreas);
            }
        }

        // ---- hashing primitives ----
        private static void AssertHash(string label, uint expected, uint actual)
        {
            if (expected == 0)
            {
                TestContext.Out.WriteLine($"CAPTURE[{label}] = 0x{actual:x8}u");
                return;
            }
            Assert.AreEqual(expected, actual, label);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Mix(uint h, uint v) => (h ^ v) * FnvPrime;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixInt4(uint h, int4 v) => Mix(Mix(Mix(h, (uint)v.x), (uint)v.y), (uint)v.z) ^ ((uint)v.w * FnvPrime);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixUint4(uint h, uint4 v) => Mix(Mix(Mix(h, v.x), v.y), v.z) ^ (v.w * FnvPrime);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixByte4(uint h, byte4 v) => Mix(Mix(Mix(h, v.x), v.y), v.z) ^ (v.w * FnvPrime);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixUshort3(uint h, ushort3 v) => Mix(Mix(h, v.x), v.y) ^ (v.z * FnvPrime);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint MixFloat3(uint h, float3 v) => Mix(Mix(Mix(h, FloatBits(v.x)), FloatBits(v.y)), FloatBits(v.z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint FloatBits(float f) => *(uint*)&f;
    }
}
