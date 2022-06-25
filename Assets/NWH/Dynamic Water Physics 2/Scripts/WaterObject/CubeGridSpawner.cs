using NWH.DWP2.WaterObjects;
using UnityEngine;

namespace NWH.DWP2.DemoContent
{
    public class CubeGridSpawner : MonoBehaviour
    {
        public int   xResolution = 10;
        public int   yResolution = 10;
        public int   zResolution = 10;
        public float width       = 1.1f;
        public float height      = 1.1f;
        public float depth       = 1.1f;


        private void Start()
        {
            for (int x = 0; x < xResolution; x++)
            {
                for (int y = 0; y < yResolution; y++)
                {
                    for (int z = 0; z < zResolution; z++)
                    {
                        GameObject spawnedObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        spawnedObject.transform.position = new Vector3(x * width, y * height, z * depth);
                        Rigidbody rb = spawnedObject.AddComponent<Rigidbody>();
                        rb.mass = 200f;
                        spawnedObject.AddComponent<WaterObject>();
                    }
                }
            }
        }
    }
}