#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.ShipController;
using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.DWP2.WaterObjects
{
    [CustomPropertyDrawer(typeof(Engine))]
    public class EngineDrawer : DWP_NUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            if (drawer.Button("Set Defaults"))
            {
                (SerializedPropertyHelper.GetTargetObjectOfProperty(property) as Engine)?.SetDefaults();
            }
            drawer.Space();
            
            drawer.Field("name");
            drawer.Field("isOn");

            drawer.BeginSubsection("Engine");
            drawer.Field("throttleBinding");
            drawer.Field("startOnThrottle");
            drawer.Field("minRPM");
            drawer.Field("maxRPM");
            drawer.Field("maxThrust",  true, "N");
            drawer.Field("spinUpTime", true, "N");
            drawer.Field("startingRPM");
            drawer.Field("startDuration", true, "s");
            drawer.Field("stopDuration",  true, "s");
            drawer.EndSubsection();

            drawer.BeginSubsection("Propeller");
            drawer.Field("thrustPosition");
            drawer.Field("thrustDirection");
            drawer.Field("applyThrustWhenAboveWater");
            drawer.Field("reverseThrustCoefficient");
            drawer.Field("maxSpeed", true, "m/s");
            drawer.Field("thrustCurve");
            drawer.Field("rudderTransform");
            drawer.EndSubsection();

            drawer.BeginSubsection("Animation");
            drawer.Field("propellerTransform");
            drawer.Field("propellerRpmRatio");
            drawer.EndSubsection();

            drawer.BeginSubsection("Sound");
            drawer.Field("volume");
            drawer.Field("volumeRange");
            drawer.Field("pitch");
            drawer.Field("pitchRange");
            drawer.Field("runningSource");
            drawer.Field("startingSource");
            drawer.Field("stoppingSource");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}

#endif
