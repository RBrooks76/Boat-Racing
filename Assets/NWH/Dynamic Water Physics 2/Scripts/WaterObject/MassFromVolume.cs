using UnityEngine;

namespace NWH.DWP2.WaterObjects
{
    /// <summary>
    ///     Script to calculate mass from mesh volume and material density.
    ///     Removes need for guessing mass and ensures that all objects of same density are submerged equally.
    /// </summary>
    [RequireComponent(typeof(WaterObject))]
    public class MassFromVolume : MonoBehaviour
    {
        public WaterObjectMaterial material;
        public float                                   density;
        public float                                   mass;
        public float                                   volume;

        private WaterObject _waterObject;


        private void Awake()
        {
            _waterObject = GetComponent<WaterObject>();
            if (_waterObject == null)
            {
                Debug.LogError("MassFromVolume requires WaterObject.");
            }

            if (material == null)
            {
                SetDefaultAsMaterial();
            }
        }


        public void SetDefaultAsMaterial()
        {
            material = Resources.Load<WaterObjectMaterial>("DWP2/DefaultWaterObjectMaterial");
        }


        private void Reset()
        {
            SetDefaultAsMaterial();
        }


        /// <summary>
        ///     Gets volume of the simulation mesh. Scale-sensitive.
        /// </summary>
        public void CalculateSimulationMeshVolume()
        {
            if (_waterObject == null)
            {
                _waterObject = GetComponent<WaterObject>();
            }

            if (_waterObject.SimulationMesh == null)
            {
                Debug.LogWarning(
                    "No simulation mesh assigned/generated. Make sure that simulation mesh of WaterObject is not empty - " +
                    "if this is the first time setup try clicking 'Update Simulation Mesh' on WaterObject.");
            }

            volume = _waterObject.SimulationMesh == null
                         ? 0.00000001f
                         : Mathf.Clamp(MeshUtility.VolumeOfMesh(_waterObject.SimulationMesh, _waterObject.transform),
                                       0f, Mathf.Infinity);
        }


        /// <summary>
        ///     Sets density of the material and adjusts mass to be correct for the volume of the mesh.
        /// </summary>
        public float CalculateAndApplyFromMaterial()
        {
            return CalculateAndApplyFromDensity(material.density);
        }


        public float CalculateAndApplyFromDensity(float density)
        {
            mass = -1;
            if (material != null)
            {
                if (_waterObject == null)
                {
                    _waterObject = GetComponent<WaterObject>();
                }

                CalculateSimulationMeshVolume();

                mass = density * volume;
                if (_waterObject.targetRigidbody != null && mass > 0)
                {
                    _waterObject.targetRigidbody.mass = mass;
                }
            }

            return mass;
        }
    }
}