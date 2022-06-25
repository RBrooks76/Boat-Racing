using UnityEngine;

namespace NWH.DWP2.MiConvexHull
{
    public class Vertex : IVertex
    {
        public Vertex(double x, double y, double z)
        {
            Position = new double[3] {x, y, z,};
        }


        public Vertex(Vector3 ver)
        {
            Position = new double[3] {ver.x, ver.y, ver.z,};
        }


        public double[] Position { get; set; }


        public Vector3 ToVec()
        {
            return new Vector3((float) Position[0], (float) Position[1], (float) Position[2]);
        }
    }
}