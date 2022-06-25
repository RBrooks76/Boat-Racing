using System;
using UnityEngine;

namespace NWH.DWP2.WaterObjects
{
    [Serializable]
    public class SerializedMesh
    {
        [SerializeField] public Vector3[] vertices;
        [SerializeField] public int[]     triangles;


        public void Serialize(Mesh mesh)
        {
            vertices  = mesh.vertices;
            triangles = mesh.triangles;
        }


        public Mesh Deserialize()
        {
            if (vertices != null && triangles != null)
            {
                Mesh m = MeshUtility.GenerateMesh(vertices, triangles);
                m.name = "DWP_SIM_MESH";
                return m;
            }

            return null;
        }
    }
}