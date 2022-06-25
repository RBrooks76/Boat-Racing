#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.WaterObjects;
using UnityEditor;
using UnityEngine;

namespace NWH.DWP2.WaterEffects
{
    [CustomEditor(typeof(WaterParticleSystem))]
    [CanEditMultipleObjects]
    public class WaterParticleSystemEditor : DWP_NUIEditor
    {
        private WaterParticleSystem wps;


        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            wps = (WaterParticleSystem) target;

            // Draw logo texture
            Rect logoRect = drawer.positionRect;
            logoRect.height = 60f;
            drawer.DrawEditorTexture(logoRect, "DWP2/Logos/WaterParticleSystemLogo");
            drawer.AdvancePosition(logoRect.height);

            drawer.BeginSubsection("Particle Settings");
            drawer.Field("emit");
            drawer.Field("renderQueue");
            drawer.Field("startSize");
            drawer.Field("sleepThresholdVelocity");
            drawer.Field("initialVelocityModifier");
            drawer.Field("maxInitialAlpha");
            drawer.Field("initialAlphaModifier");
            drawer.Field("emitPerCycle");
            drawer.Field("emitTimeInterval");
            drawer.Field("positionExtrapolationFrames");
            drawer.Field("surfaceElevation");
            drawer.EndSubsection();

            /* // TODO - move this from editor script
            if(!wps.GetComponent<ParticleSystem>())
            {
                GameObject waterParticleSystemPrefab = Resources.Load<GameObject>("DWP2/WaterParticleSystemPrefab");
                if (waterParticleSystemPrefab == null)
                {
                    Debug.LogError("Could not load WaterParticleSystemPrefab from Resources.");
                }
                else
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(waterParticleSystemPrefab
                        .GetComponent<ParticleSystem>());
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(wps.gameObject);
                }
            }
            */

            drawer.EndEditor(this);
            return true;
        }
    }
}

#endif
