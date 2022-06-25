#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.ShipController;
using UnityEditor;

namespace NWH.DWP2.WaterObjects
{
    /// <summary>
    ///     Editor for MobileInputProvider.
    /// </summary>
    [CustomEditor(typeof(MobileShipInputProvider))]
    public class MobileShipInputProviderEditor : DWP_NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Info("None of the buttons are mandatory. If you do not wish to use an input leave the field empty.");

            MobileShipInputProvider mip = target as MobileShipInputProvider;
            if (mip == null)
            {
                drawer.EndEditor(this);
                return false;
            }

            drawer.BeginSubsection("Vehicle");
            drawer.Field("steeringSlider");
            drawer.Field("throttleSlider");
            drawer.Field("throttleSlider2");
            drawer.Field("throttleSlider3");
            drawer.Field("throttleSlider4");
            drawer.Field("bowThrusterSlider");
            drawer.Field("sternThrusterSlider");
            drawer.Field("submarineDepthSlider");
            drawer.Field("engineStartStopButton");
            drawer.Field("anchorButton");
            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }


        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}

#endif
