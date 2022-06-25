#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.Common.CoM
{
    [CustomEditor(typeof(VariableCenterOfMass))]
    [CanEditMultipleObjects]
    public class VariableCenterOfMassEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            VariableCenterOfMass vcom     = (VariableCenterOfMass) target;

            drawer.Field("updateInterval", true, "s");
            
            if (!Application.isPlaying)
            {
                foreach (var o in targets)
                {
                    var t = (VariableCenterOfMass) o;
                    t.affectors = t.GetMassAffectors();
                    t.UpdateAllProperties();
                }
            }
            
            drawer.BeginSubsection("Mass Affectors");
            if (vcom.affectors != null)
            {
                if (!Application.isPlaying)
                {
                    vcom.affectors = vcom.GetMassAffectors();
                }

                for (int i = 0; i < vcom.affectors.Length; i++)
                {
                    IMassAffector affector = vcom.affectors[i];
                    if (affector == null || affector.GetTransform() == null) continue;
                    string positionStr = i == 0 ? "(this)" : $"Position = {affector.GetTransform().localPosition}";
                    drawer.Label($"{affector.GetTransform().name}  |  Mass = {affector.GetMass()}  |  {positionStr}");
                }
            }
            drawer.EndSubsection();

            drawer.BeginSubsection("Mass");
            drawer.Field("baseMass", true, "kg");
            drawer.Field("totalMass", false, "kg");
            drawer.EndSubsection();

            drawer.BeginSubsection("Center Of Mass");
            drawer.Field("centerOfMass",       false, "m");
            drawer.Field("centerOfMassOffset", true,  "m");
            drawer.Space(2);
            drawer.EndSubsection();
            
            drawer.BeginSubsection("Inertia");
            drawer.Field("useDefaultInertia");
            drawer.Field("inertiaTensor", false, "kg m2");
            if (!vcom.useDefaultInertia)
            {
                drawer.Field("dimensions",    true, "m");
                drawer.Field("inertiaScale",  true, "x100%");
            }
            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }
    }
}

#endif
