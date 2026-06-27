import json
import re
from validator import check_text

readmes = {}

# 1. IAFahim.Geometry.Arrangement
readmes["IAFahim.Geometry.Arrangement"] = """# IAFahim.Geometry.Arrangement

## Description
This package provides algorithms for subdivision arrangement analysis. It constructs partitions, builds query grids, computes vertical decomposition, builds trapezoidal maps, and solves polygon union and intersection.

## Complexity
Grid build: O(N) where N is point count. KdTree build: O(N log N). Query: O(log N) for KdTree, O(1) for grid. Trapezoidal Map build: O(N log N) average. Decomposition: O(N log N).

## API Signature
public static class PointLocationBuild
{
    public static int Run(int* xs, int* ys, int n, int* grid, int gridSize);
    public static void BuildKdTree(long* points, int* tree, int node, int l, int r, int depth);
}
public static class PointLocationQuery
{
    public static int Run(int* grid, int gridSize, int minX, int minY, int cellW, int cellH, int px, int py);
    public static int QueryKdTree(long* points, int* tree, int node, int depth, long px, long py);
}

## Usage Example
```csharp
unsafe
{
    int size = 100;
    int* xs = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(int));
    int* ys = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(int));
    int* outX = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(int));
    int* outY = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(int));
    try
    {
        for (int i = 0; i < size; i++)
        {
            xs[i] = i;
            ys[i] = i * 2;
        }
        int result = IAFahim.Geometry.Arrangement.VerticalDecomposition.Run(xs, ys, size, outX, outY);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)xs);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ys);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)outX);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)outY);
    }
}
```"""

# 2. IAFahim.Geometry.Azimuth
readmes["IAFahim.Geometry.Azimuth"] = """# IAFahim.Geometry.Azimuth

## Description
This package provides methods for azimuth solving. It supports spherical azimuth, spherical distance on a sphere, and planar 2D azimuth.

## Complexity
All methods execute in O(1) time complexity.

## API Signature
public static class SphericalAzimuth
{
    public static double Run(double lat1, double lon1, double lat2, double lon2);
}
public static class SphericalDistance
{
    public static double Run(double lat1, double lon1, double lat2, double lon2, double radius);
}
public static class CartesianAzimuth
{
    public static double Run(double x1, double y1, double x2, double y2);
}

## Usage Example
```csharp
unsafe
{
    double* coords = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(4 * sizeof(double));
    try
    {
        coords[0] = 0.0;
        coords[1] = 0.0;
        coords[2] = 1.0;
        coords[3] = 1.0;
        double result = IAFahim.Geometry.Azimuth.CartesianAzimuth.Run(coords[0], coords[1], coords[2], coords[3]);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)coords);
    }
}
```"""

# 3. IAFahim.Geometry.Basic
readmes["IAFahim.Geometry.Basic"] = """# IAFahim.Geometry.Basic

## Description
This package provides basic geometry operations. It includes point arithmetic, dot products, cross products, point rotation, orientation tests, segment intersection checks, projection and reflection, distance formulas, polygon area, centroid solving, and inclusion checks.

## Complexity
All primitive operations run in O(1) time complexity. Polygon operations like area, centroid, and inclusion run in O(N) time complexity where N is the vertex count.

## API Signature
public static class GeometryPoint
{
    public static void Run(long* x, long* y, long px, long py);
}
public static class SegmentIntersect
{
    public static bool Run(long ax, long ay, long bx, long by, long cx, long cy, long dx, long dy);
}
public static class PolygonArea
{
    public static long Run(int n, long* x, long* y);
}

## Usage Example
```csharp
unsafe
{
    int size = 3;
    long* xs = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(long));
    long* ys = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(long));
    try
    {
        xs[0] = 0; ys[0] = 0;
        xs[1] = 10; ys[1] = 0;
        xs[2] = 0; ys[2] = 10;
        long area = IAFahim.Geometry.Basic.PolygonArea.Run(size, xs, ys);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)xs);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ys);
    }
}
```"""

# 4. IAFahim.Geometry.Bvh
readmes["IAFahim.Geometry.Bvh"] = """# IAFahim.Geometry.Bvh

## Description
This package provides a bounding volume hierarchy tree for 3D meshes. It enables efficient ray query operations and spatial partitioning for collision tests.

## Complexity
Tree construction runs in O(N log N) time complexity. Ray query runs in O(log N) average time complexity, where N is the triangle count.

## API Signature
public struct BvhNode
{
    public float3 Min;
    public float3 Max;
    public int Left;
    public int Right;
    public int TriangleIndex;
}
public static class BvhTree
{
    public static int Build(BvhNode* nodes, float3* centroids, int* triangleIndices, int count);
}

## Usage Example
```csharp
unsafe
{
    int count = 2;
    IAFahim.Geometry.Bvh.BvhNode* nodes = (IAFahim.Geometry.Bvh.BvhNode*)System.Runtime.InteropServices.Marshal.AllocHGlobal(count * sizeof(IAFahim.Geometry.Bvh.BvhNode));
    try
    {
        nodes[0].Left = -1;
        nodes[0].Right = -1;
        nodes[0].TriangleIndex = 0;
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)nodes);
    }
}
```"""

# 5. IAFahim.Geometry.Curve
readmes["IAFahim.Geometry.Curve"] = """# IAFahim.Geometry.Curve

## Description
This package provides curve evaluation algorithms. It includes cubic Bezier curve evaluation, tangent evaluation, arc length integration, and uniform sampling along a path.

## Complexity
Cubic curve evaluation and tangent solving run in O(1) time complexity. Arc length integration runs in O(S) where S is the step count.

## API Signature
public static class CubicBezier
{
    public static float3 Evaluate(float3 p0, float3 p1, float3 p2, float3 p3, float t);
    public static float3 EvaluateTangent(float3 p0, float3 p1, float3 p2, float3 p3, float t);
    public static float IntegrateArcLength(float3 p0, float3 p1, float3 p2, float3 p3);
}

## Usage Example
```csharp
unsafe
{
    float3* points = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(4 * sizeof(float3));
    try
    {
        points[0] = new float3(0.0f, 0.0f, 0.0f);
        points[1] = new float3(1.0f, 0.0f, 0.0f);
        points[2] = new float3(1.0f, 1.0f, 0.0f);
        points[3] = new float3(2.0f, 2.0f, 0.0f);
        float3 res = IAFahim.Geometry.Curve.CubicBezier.Evaluate(points[0], points[1], points[2], points[3], 0.5f);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)points);
    }
}
```"""

# 6. IAFahim.Geometry.Frame
readmes["IAFahim.Geometry.Frame"] = """# IAFahim.Geometry.Frame

## Description
This package provides methods for frame generation along a curve. It utilizes parallel transport to construct consistent orthogonal frames without twist.

## Complexity
The parallel transport frame solver runs in O(N) time complexity, where N is the point count.

## API Signature
public static class ParallelTransport
{
    public static void Compute(float3* positions, int count, float3 initialNormal, float3* outRight, float3* outUp, float3* outForward);
}

## Usage Example
```csharp
unsafe
{
    int size = 5;
    float3* pos = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(float3));
    float3* right = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(float3));
    float3* up = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(float3));
    float3* forward = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(float3));
    try
    {
        for (int i = 0; i < size; i++)
        {
            pos[i] = new float3((float)i, 0.0f, 0.0f);
        }
        float3 normal = new float3(0.0f, 1.0f, 0.0f);
        IAFahim.Geometry.Frame.ParallelTransport.Compute(pos, size, normal, right, up, forward);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)pos);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)right);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)up);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)forward);
    }
}
```"""

# 7. IAFahim.Geometry.Hull
readmes["IAFahim.Geometry.Hull"] = """# IAFahim.Geometry.Hull

## Description
This package provides geometric hull and partition algorithms. It includes Minkowski sum solving, straight skeleton construction, convex hull trick with rollback history, half-space intersection, rotating calipers for bounding boxes, and 3D convex hull generation.

## Complexity
Minkowski sum runs in O(N + M) time. Straight skeleton construction runs in O(N^2 log N) worst-case time. Rotating calipers run in O(N) time. Convex hull 3D construction runs in O(N^2) time.

## API Signature
public static class RotatingCalipers
{
    public struct Rect
    {
        public double X, Y, W, H, Angle;
    }
    public static Rect MinArea(double* xs, double* ys, int n);
    public static double MinWidth(double* xs, double* ys, int n);
}

## Usage Example
```csharp
unsafe
{
    int size = 4;
    double* xs = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(double));
    double* ys = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(double));
    try
    {
        xs[0] = 0.0; ys[0] = 0.0;
        xs[1] = 10.0; ys[1] = 0.0;
        xs[2] = 10.0; ys[2] = 10.0;
        xs[3] = 0.0; ys[3] = 10.0;
        IAFahim.Geometry.Hull.RotatingCalipers.Rect r = IAFahim.Geometry.Hull.RotatingCalipers.MinArea(xs, ys, size);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)xs);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ys);
    }
}
```"""

# 8. IAFahim.Geometry.Intersect
readmes["IAFahim.Geometry.Intersect"] = """# IAFahim.Geometry.Intersect

## Description
This package provides methods for geometric intersection solving. It computes polyhedron volume, line-sphere intersection, sphere-sphere intersection, point-plane distances, line-plane intersection, segment-plane intersection, and plane-plane intersections.

## Complexity
Intersection and distance methods run in O(1) time complexity. Polyhedron volume solver runs in O(F) where F is the face count.

## API Signature
public static class Plane
{
    public static double PointPlaneDistance(double px, double py, double pz, double nx, double ny, double nz, double d);
    public static bool LinePlaneIntersection(double lx, double ly, double lz, double ldx, double ldy, double ldz, double nx, double ny, double nz, double d, double* t);
}

## Usage Example
```csharp
unsafe
{
    double* t = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(double));
    try
    {
        bool hit = IAFahim.Geometry.Intersect.Plane.LinePlaneIntersection(0.0, 0.0, 5.0, 0.0, 0.0, -1.0, 0.0, 0.0, 1.0, 0.0, t);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)t);
    }
}
```"""

# 9. IAFahim.Geometry.Mesh
readmes["IAFahim.Geometry.Mesh"] = """# IAFahim.Geometry.Mesh

## Description
This package provides algorithms for mesh updates. It supports vertex deformation and normal recomputing.

## Complexity
All methods run in O(N) time complexity where N is the vertex count.

## API Signature
public static class MeshProjection
{
    public static void DeformVertices(float3* positions, int count, float3 direction, float force);
    public static void RecalculateNormals(float3* positions, int* indices, int indexCount, float3* normals);
}

## Usage Example
```csharp
unsafe
{
    int count = 3;
    float3* pos = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(count * sizeof(float3));
    try
    {
        pos[0] = new float3(0.0f, 0.0f, 0.0f);
        pos[1] = new float3(1.0f, 0.0f, 0.0f);
        pos[2] = new float3(0.0f, 1.0f, 0.0f);
        IAFahim.Geometry.Mesh.MeshProjection.DeformVertices(pos, count, new float3(0.0f, 0.0f, 1.0f), 0.5f);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)pos);
    }
}
```"""

# 10. IAFahim.Geometry.Spatial
readmes["IAFahim.Geometry.Spatial"] = """# IAFahim.Geometry.Spatial

## Description
This package provides spatial query data structures. It includes cover trees, kd-trees, quadtrees, range trees, segment trees, octrees, ball trees, 3D binary indexed trees, and methods for Euclidean, Manhattan, and rectilinear minimum spanning trees.

## Complexity
Tree building algorithms run in O(N log N) or O(N log^2 N) time complexity. Nearest neighbor and range queries run in O(log N) average time complexity.

## API Signature
public static class KdTree
{
    public struct Node
    {
        public double X, Y;
        public int PointIndex;
        public int Left, Right;
        public int Axis;
    }
    public static int Build(double* xs, double* ys, int n, Node* nodes);
}

## Usage Example
```csharp
unsafe
{
    int size = 2;
    double* xs = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(double));
    double* ys = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(double));
    IAFahim.Geometry.Spatial.KdTree.Node* nodes = (IAFahim.Geometry.Spatial.KdTree.Node*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(IAFahim.Geometry.Spatial.KdTree.Node));
    try
    {
        xs[0] = 1.0; ys[0] = 2.0;
        xs[1] = 3.0; ys[1] = 4.0;
        int root = IAFahim.Geometry.Spatial.KdTree.Build(xs, ys, size, nodes);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)xs);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ys);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)nodes);
    }
}
```"""

# 11. IAFahim.Geometry.Triangulation
readmes["IAFahim.Geometry.Triangulation"] = """# IAFahim.Geometry.Triangulation

## Description
This package provides methods for polygon triangulation. It implements ear clipping to decompose simple polygons into triangles.

## Complexity
Ear clipping triangulation runs in O(N^2) worst-case time complexity, where N is the vertex count.

## API Signature
public static class EarClipping
{
    public static void Triangulate(float3* positions, int count, int* outIndices);
}

## Usage Example
```csharp
unsafe
{
    int count = 3;
    float3* pos = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(count * sizeof(float3));
    int* indices = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(count * sizeof(int));
    try
    {
        pos[0] = new float3(0.0f, 0.0f, 0.0f);
        pos[1] = new float3(1.0f, 0.0f, 0.0f);
        pos[2] = new float3(0.0f, 1.0f, 0.0f);
        IAFahim.Geometry.Triangulation.EarClipping.Triangulate(pos, count, indices);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)pos);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)indices);
    }
}
```"""

# 12. IAFahim.Geometry.Voronoi
readmes["IAFahim.Geometry.Voronoi"] = """# IAFahim.Geometry.Voronoi

## Description
This package provides Voronoi diagrams and related spatial graph algorithms. It includes Delaunay triangulation, Fortune's sweep-line solver, visibility graph construction, nearest neighbor search on KD-trees, and shortest path solving.

## Complexity
Delaunay triangulation and Fortune's algorithm run in O(N log N) time complexity. Visibility graph construction runs in O(N^2 log N) time complexity.

## API Signature
public static class Delaunay
{
    public struct Triangle
    {
        public int A, B, C;
    }
    public static int Build(double* xs, double* ys, int n, Triangle* outTri);
}

## Usage Example
```csharp
unsafe
{
    int size = 3;
    double* xs = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(double));
    double* ys = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(double));
    IAFahim.Geometry.Voronoi.Delaunay.Triangle* tris = (IAFahim.Geometry.Voronoi.Delaunay.Triangle*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(IAFahim.Geometry.Voronoi.Delaunay.Triangle));
    try
    {
        xs[0] = 0.0; ys[0] = 0.0;
        xs[1] = 10.0; ys[1] = 0.0;
        xs[2] = 0.0; ys[2] = 10.0;
        int triCount = IAFahim.Geometry.Voronoi.Delaunay.Build(xs, ys, size, tris);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)xs);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ys);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)tris);
    }
}
```"""

# 13. IAFahim.Graph
readmes["IAFahim.Graph"] = """# IAFahim.Graph

## Description
This package provides core graph algorithms. It includes adjacency builders, minimum cut solvers, Eulerian path detection, 2-SAT solvers, minimum spanning tree variants, bipartite matching, shortest path routines, graph traversals, tournament analysis, topological sorting, and planar graph utilities.

## Complexity
BFS and DFS traversals run in O(V + E) time. Dijkstra shortest path runs in O(E log V) time. Minimum spanning tree algorithms run in O(E log V) or O(E log* V) time.

## API Signature
public static class Dijkstra
{
    public static void Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist, int* parent);
}

## Usage Example
```csharp
unsafe
{
    int n = 5;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* weight = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    long* dist = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(long));
    int* parent = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        for (int i = 0; i < n; i++)
        {
            head[i] = -1;
            dist[i] = long.MaxValue;
            parent[i] = -1;
        }
        IAFahim.Graph.Dijkstra.Run(n, 0, head, to, next, weight, dist, parent);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)weight);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)dist);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)parent);
    }
}
```"""

# 14. IAFahim.Graph.Bridges
readmes["IAFahim.Graph.Bridges"] = """# IAFahim.Graph.Bridges

## Description
This package provides methods for identifying bridges and cut vertices in graphs. It supports static search, incremental dynamic bridge maintenance, and biconnectivity augmentation solving.

## Complexity
Static bridge search runs in O(V + E) time complexity. Dynamic bridge updates run in O(log V) amortized time.

## API Signature
public static class BridgeAndArticulation
{
    public static void Find(int n, int* head, int* next, int* to, bool* isBridge, bool* isCutVertex);
}

## Usage Example
```csharp
unsafe
{
    int n = 4;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    bool* isBridge = (bool*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(bool));
    bool* isCutVertex = (bool*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(bool));
    try
    {
        for (int i = 0; i < n; i++)
        {
            head[i] = -1;
            isBridge[i] = false;
            isCutVertex[i] = false;
        }
        IAFahim.Graph.Bridges.BridgeAndArticulation.Find(n, head, next, to, isBridge, isCutVertex);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)isBridge);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)isCutVertex);
    }
}
```"""

# 15. IAFahim.Graph.Cactus
readmes["IAFahim.Graph.Cactus"] = """# IAFahim.Graph.Cactus

## Description
This package provides algorithms for graphs where any two simple cycles share at most one vertex. It includes cycle decomposition, shortest path queries, bridge tree diameter solving, and lowest common ancestor query support.

## Complexity
Cycle decomposition and bridge tree diameter solving run in O(V + E) time. Shortest path and ancestor queries run in O(log V) time.

## API Signature
public static class CactusCycleDecompose
{
    public static int Run(int* head, int* to, int* next, int n, int m, int* cycleId);
}

## Usage Example
```csharp
unsafe
{
    int n = 5;
    int m = 4;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(int));
    int* cycleId = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(int));
    try
    {
        for (int i = 0; i < n; i++)
        {
            head[i] = -1;
        }
        for (int i = 0; i < m; i++)
        {
            to[i] = 0;
            next[i] = -1;
            cycleId[i] = -1;
        }
        int count = IAFahim.Graph.Cactus.CactusCycleDecompose.Run(head, to, next, n, m, cycleId);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)cycleId);
    }
}
```"""

# 16. IAFahim.Graph.Centroid
readmes["IAFahim.Graph.Centroid"] = """# IAFahim.Graph.Centroid

## Description
This package provides centroid decomposition for tree structures. It enables divide-and-conquer algorithms on trees by finding tree centroids and building centroid trees.

## Complexity
Building the centroid tree runs in O(N log N) time complexity, where N is the vertex count.

## API Signature
public static class CentroidDecomposition
{
    public static int Build(int n, int* head, int* to, int* next, int* centroid, int* sz, byte* removed);
    public static void Decompose(int n, int* head, int* to, int* next, int u, byte* removed, int* sz, int* centroids, int* centroidCount);
}

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* centroid = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* sz = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    byte* removed = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(byte));
    try
    {
        for (int i = 0; i < n; i++)
        {
            head[i] = -1;
            removed[i] = 0;
        }
        int root = IAFahim.Graph.Centroid.CentroidDecomposition.Build(n, head, to, next, centroid, sz, removed);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)centroid);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)sz);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)removed);
    }
}
```"""

# 17. IAFahim.Graph.Clique
readmes["IAFahim.Graph.Clique"] = """# IAFahim.Graph.Clique

## Description
This package provides algorithms for finding fully connected subgraphs in a graph. It solves the clique search problem by identifying subsets of vertices that are mutually adjacent.

## Complexity
Finding a maximum clique is an NP-hard problem. Exponential-time algorithms are used for general graphs, while polynomial-time bounds apply to specific graph types.

## API Signature
public static class CliqueSearch
{
    public static int FindMaximal(int n, int* head, int* to, int* next, int* outVertices);
}

## Usage Example
```csharp
unsafe
{
    int n = 4;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* outVertices = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        for (int i = 0; i < n; i++)
        {
            head[i] = -1;
            outVertices[i] = -1;
        }
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)outVertices);
    }
}
```"""

# 18. IAFahim.Graph.Connectivity
readmes["IAFahim.Graph.Connectivity"] = """# IAFahim.Graph.Connectivity

## Description
This package provides methods for dynamic graph connectivity. It supports incremental union-find, decremental connectivity, offline dynamic connectivity, dynamic transitive closure, and fully dynamic connectivity.

## Complexity
Incremental connectivity operations run in nearly linear time using inverse Ackermann bounds. Fully dynamic connectivity queries run in O(log^2 V) amortized time.

## API Signature
public static class IncrementalConnectivity
{
    public static void Init(int* parent, int* size, int n);
    public static int Find(int* parent, int i);
    public static bool Union(int* parent, int* size, int i, int j);
    public static bool Connected(int* parent, int i, int j);
}

## Usage Example
```csharp
unsafe
{
    int n = 5;
    int* parent = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* size = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        IAFahim.Graph.Connectivity.IncrementalConnectivity.Init(parent, size, n);
        bool change = IAFahim.Graph.Connectivity.IncrementalConnectivity.Union(parent, size, 0, 1);
        bool connected = IAFahim.Graph.Connectivity.IncrementalConnectivity.Connected(parent, 0, 1);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)parent);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)size);
    }
}
```"""

# 19. IAFahim.Graph.Cut
readmes["IAFahim.Graph.Cut"] = """# IAFahim.Graph.Cut

## Description
This package provides algorithms for graph cuts and flow networks. It solves the minimum cut problem, identifying subsets of edges that partition the graph.

## Complexity
Minimum cut algorithms on planar graphs run in O(N log N) time, while general graphs run in polynomial time matching maximum flow bounds.

## API Signature
public static class MinimumCut
{
    public static int Solve(int n, int* head, int* to, int* next, int* cap, int* outCutEdges);
}

## Usage Example
```csharp
unsafe
{
    int n = 4;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* outCutEdges = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        for (int i = 0; i < n; i++)
        {
            head[i] = -1;
            outCutEdges[i] = -1;
        }
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)outCutEdges);
    }
}
```"""

# Let's validate each of the generated README files using the validator
has_errors = False
for name, md in readmes.items():
    errors = check_text(md, name)
    if errors:
        print(f"Validation errors for {name}:")
        for err in errors:
            print(f"  - {err}")
        has_errors = True

if not has_errors:
    output_file = "/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_3/outputs.json"
    with open(output_file, "w", encoding="utf-8") as f:
        json.dump(readmes, f, indent=2, ensure_ascii=False)
    print("All READMEs validated successfully! Outputs written to outputs.json")
else:
    print("Some READMEs failed validation. Fix them.")
    exit(1)
