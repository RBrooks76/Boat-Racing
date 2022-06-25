using System.Collections.Generic;
using System.Linq;
using NWH.DWP2.MeshDecimation;
using NWH.DWP2.MiConvexHull;
using UnityEngine;

namespace NWH.DWP2.WaterObjects
{
    public static class MeshUtility
    {
	    /// <summary>
	    ///     Create dummy mesh from original mesh
	    /// </summary>
	    public static void GenerateSimMesh(ref Mesh originalMesh, ref Mesh simMesh,
            bool simplifyMesh = false, bool convexifyMesh = false, bool weldColocatedVertices = true,
            float simplificationRatio = 1f)
        {
            simMesh.vertices  = originalMesh.vertices;
            simMesh.triangles = originalMesh.triangles;

            if (simplifyMesh)
            {
                simMesh = GenerateSimplifiedMesh(ref originalMesh, ref simMesh, simplificationRatio);
            }

            if (convexifyMesh)
            {
                simMesh = GenerateConvexMesh(simMesh);
            }

            if (weldColocatedVertices)
            {
                WeldVertices(ref simMesh);
            }

            simMesh.name = "DWP_SIM_MESH";
            simMesh.RecalculateNormals();
            simMesh.RecalculateTangents();
        }


	    /// <summary>
	    ///     Generate mesh from vertices and triangles.
	    /// </summary>
	    /// <param name="vertices">Array of vertices.</param>
	    /// <param name="triangles">Array of triangles (indices).</param>
	    /// <returns></returns>
	    public static Mesh GenerateMesh(Vector3[] vertices, int[] triangles)
        {
            Mesh m = new Mesh();
            m.vertices  = vertices;
            m.triangles = triangles;
            m.RecalculateBounds();
            m.RecalculateNormals();
            m.name = "DWP_SIM_MESH";
            return m;
        }


	    /// <summary>
	    ///     Welds vertices based on distance.
	    /// </summary>
	    public static void WeldVertices(ref Mesh aMesh, float aMaxDelta = 0.001f)
        {
            Vector3[] verts    = aMesh.vertices;
            List<int> newVerts = new List<int>();
            int[]     map      = new int[verts.Length];
            // create mapping and filter duplicates.
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 p         = verts[i];
                bool    duplicate = false;
                for (int i2 = 0; i2 < newVerts.Count; i2++)
                {
                    int a = newVerts[i2];
                    if ((verts[a] - p).sqrMagnitude <= aMaxDelta)
                    {
                        map[i]    = i2;
                        duplicate = true;
                        break;
                    }
                }

                if (!duplicate)
                {
                    map[i] = newVerts.Count;
                    newVerts.Add(i);
                }
            }

            Vector3[] verts2 = new Vector3[newVerts.Count];
            for (int i = 0; i < newVerts.Count; i++)
            {
                int a = newVerts[i];
                verts2[i] = verts[a];
            }

            int[] tris = aMesh.triangles;
            for (int i = 0; i < tris.Length; i++)
            {
                tris[i] = map[tris[i]];
            }

            aMesh.triangles = tris;
            aMesh.vertices  = verts2;
        }


	    /// <summary>
	    ///     Reduces poly count of the mesh while trying to preserve features.
	    /// </summary>
	    /// <param name="om">Mesh to simplify.</param>
	    /// <param name="ratio">Percent of the triangles the new mesh will have</param>
	    /// <returns></returns>
	    private static Mesh GenerateSimplifiedMesh(ref Mesh om, ref Mesh dummyMesh, float ratio)
        {
            MeshDecimate meshDecimate = new MeshDecimate();
            meshDecimate.ratio = ratio;
            meshDecimate.PreCalculate(om);
            meshDecimate.Calculate(om);

            Mesh sm = new Mesh();
            sm.vertices  = meshDecimate.finalVertices;
            sm.triangles = meshDecimate.finalTriangles;
            sm.normals   = meshDecimate.finalNormals;
            sm.name      = "DWP_SIM_MESH";
            return sm;
        }


	    /// <summary>
	    ///     Calculate signed volume of a triangle given by its vertices.
	    /// </summary>
	    public static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float v321 = p3.x * p2.y * p1.z;
            float v231 = p2.x * p3.y * p1.z;
            float v312 = p3.x * p1.y * p2.z;
            float v132 = p1.x * p3.y * p2.z;
            float v213 = p2.x * p1.y * p3.z;
            float v123 = p1.x * p2.y * p3.z;
            return 1.0f / 6.0f * (-v321 + v231 + v312 - v132 - v213 + v123);
        }


	    /// <summary>
	    ///     Calculates volume of the given mesh.
	    ///     Scale-sensitive
	    /// </summary>
	    public static float VolumeOfMesh(Mesh mesh, Transform transform)
        {
            float     volume          = 0;
            Vector3[] vertices        = mesh.vertices;
            int[]     triangles       = mesh.triangles;
            Matrix4x4 transformMatrix = transform.localToWorldMatrix;
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                Vector3 p1 = transformMatrix.MultiplyPoint(vertices[triangles[i + 0]]);
                Vector3 p2 = transformMatrix.MultiplyPoint(vertices[triangles[i + 1]]);
                Vector3 p3 = transformMatrix.MultiplyPoint(vertices[triangles[i + 2]]);
                volume += SignedVolumeOfTriangle(p1, p2, p3);
            }

            return Mathf.Abs(volume);
        }


	    /// <summary>
	    ///     Generates convex mesh.
	    /// </summary>
	    public static Mesh GenerateConvexMesh(Mesh mesh)
        {
            IEnumerable<Vector3> stars = mesh.vertices;
            Mesh                 m     = new Mesh();

            List<int>    triangles = new List<int>();
            List<Vertex> vertices  = stars.Select(x => new Vertex(x)).ToList();

            ConvexHull<Vertex, DefaultConvexFace<Vertex>> result = ConvexHull.Create(vertices);
            m.vertices = result.Points.Select(x => x.ToVec()).ToArray();
            List<Vertex> xxx = result.Points.ToList();

            foreach (DefaultConvexFace<Vertex> face in result.Faces)
            {
                triangles.Add(xxx.IndexOf(face.Vertices[0]));
                triangles.Add(xxx.IndexOf(face.Vertices[1]));
                triangles.Add(xxx.IndexOf(face.Vertices[2]));
            }

            m.triangles = triangles.ToArray();
            m.RecalculateNormals();
            m.name = "DWP_SIM_MESH";
            return m;
        }
    }
}