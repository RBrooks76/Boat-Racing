using NWH.Common.CoM;
using UnityEngine;

namespace NWH.DWP2.WaterObjects
{
    /// <summary>
    ///     Calculates mass of a Rigidbody from the children that have MassFromVolume script attached.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class MassFromChildren : MonoBehaviour
    {
        private Rigidbody _rb;
        private string    _result;


        public void Calculate()
        {
            _rb = GetComponent<Rigidbody>();
            float massSum = 0;

            _result = "Calculated mass from: ";
            foreach (MassFromVolume mam in GetComponentsInChildren<MassFromVolume>())
            {
                massSum += mam.mass;
                _result += $"{mam.name} ({mam.mass})";
            }

            _result += $". Total mass: {massSum}.";
            Debug.Log(_result);

            if (massSum > 0.001f)
            {
                _rb.mass = massSum;

                VariableCenterOfMass vcom = GetComponent<VariableCenterOfMass>();
                if (vcom != null)
                {
                    vcom.baseMass = massSum;
                }
            }
        }
    }
}