#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.ShipController;
using UnityEditor;

namespace NWH.DWP2.WaterObjects
{
    [CustomEditor(typeof(Submarine))]
    [CanEditMultipleObjects]
    public class SubmarineEditor : DWP_NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }
            
            drawer.Info("To make submarine surface faster lower the Rigidbody mass.\n " +
                        "To make submarine dive faster increase the 'maxMassFactor'.");
            
            drawer.BeginSubsection("Depth PID Controller");
            drawer.Field("requestedDepth");
            drawer.Info("Depth should be a positive number.");
            drawer.Field("depthPID_Kp");
            drawer.Field("depthPID_Ki");
            drawer.Field("depthPID_Kd");
            drawer.Field("maxMassFactor");
            drawer.Info("Too low 'maxMassFactor' value will prevent the submarine from diving.");
            drawer.EndSubsection();

            drawer.BeginSubsection("Keep Horizontal");
            drawer.Field("keepHorizontal");
            drawer.Field("keepHorizontalSensitivity");
            drawer.Field("maxMassOffset");
            drawer.Info("Max Mass Offset [m] should not be larger than ~1/3 of the length of the submarine.");
            drawer.EndSubsection();
            
            drawer.BeginSubsection("Input");
            drawer.Field("depthInputSensitivity");
            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }
    }
}

#endif
