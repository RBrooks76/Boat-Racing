using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.Common.CoM
{
    public class MassAffector : MonoBehaviour, IMassAffector
    {
        public float mass = 100.0f;
        
        public float GetMass()
        {
            return mass;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public Vector3 GetWorldCenterOfMass()
        {
            return transform.position;
        }
    }
}

