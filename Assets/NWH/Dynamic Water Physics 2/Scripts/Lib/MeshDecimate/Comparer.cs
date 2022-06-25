using System.Collections;

namespace NWH.DWP2.MeshDecimation
{
    public class Comparer : IComparer
    {
        private Vert vx;
        private Vert vy;


        public int Compare(object x, object y)
        {
            vx = (Vert) x;
            vy = (Vert) y;
            if (vx == vy)
            {
                return 0;
            }

            if (vx.cost < vy.cost)
            {
                return -1;
            }

            return 1;
        }
    }
}