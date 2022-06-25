#if UNITY_EDITOR
using NWH.Common.CoM;
using NWH.DWP2.NUI;
using NWH.DWP2.WaterObjects;
using NWH.Common.Utility;
using UnityEditor;
using UnityEngine;

namespace NWH.DWP2
{
    [CustomEditor(typeof(WaterObjectWizard))]
    [CanEditMultipleObjects]
    public class WaterObjectWizardEditor : DWP_NUIEditor
    {
        private WaterObjectWizard wow;
        private bool              _wizardFinished;


        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            serializedObject.Update();
            wow = (WaterObjectWizard) target;

            Rect logoRect = drawer.positionRect;
            logoRect.height = 60f;
            drawer.DrawEditorTexture(logoRect, "DWP2/Logos/WaterObjectWizardLogo");
            drawer.AdvancePosition(logoRect.height);

            drawer.BeginSubsection("Options");
            drawer.Field("addWaterParticleSystem");
            drawer.EndSubsection();

            if (drawer.Button("Auto-Setup"))
            {
                foreach (WaterObjectWizard wow in targets)
                {
                    RunWizard(wow);
                }
            }

            MeshFilter mf = wow.GetComponent<MeshFilter>();
            if (mf != null)
            {
                if (mf.sharedMesh != null)
                {
                    if (mf.sharedMesh.triangles.Length / 3 > 4000)
                    {
                        drawer.Info(
                            "Large mesh detected. Expect WaterObjectWizard to take a few moments to setup this object.");
                    }
                }
            }

            drawer.EndEditor();

            if (_wizardFinished)
            {
                DestroyImmediate(wow);
            }

            return true;
        }


        private void RunWizard(WaterObjectWizard wow)
        {
            GameObject target = wow.gameObject;

            // Check for existing water object
            if (target.GetComponent<WaterObject>() != null)
            {
                Debug.LogWarning($"WaterObjectWizard: {target.name} already contains WaterObject component.");
            }

            // Check for mesh filter
            MeshFilter mf = target.GetComponent<MeshFilter>();
            if (mf == null)
            {
                Debug.LogError("WaterObjectWizard: MeshFilter not found. WaterObject requires MeshFilter to work.");
                return;
            }

            // Add rigidbody
            Rigidbody parentRigidbody = target.transform.GetComponentInParent<Rigidbody>(true);
            if (parentRigidbody == null)
            {
                Debug.Log("WaterObjectWizard: Parent rigidbody not found. Adding new.");
                parentRigidbody               = target.AddComponent<Rigidbody>();
                parentRigidbody.angularDrag   = 0.15f;
                parentRigidbody.drag          = 0.05f;
                parentRigidbody.interpolation = RigidbodyInterpolation.None;

                VariableCenterOfMass com = target.AddComponent<VariableCenterOfMass>();
            }

            // Add collider
            int colliderCount = parentRigidbody.transform.GetComponentsInChildren<Collider>().Length;
            if (colliderCount == 0)
            {
                Debug.Log(
                    $"WaterObjectWizard: Found 0 colliders on object {parentRigidbody.name}. Adding new mesh collider.");
                MeshCollider mc = target.AddComponent<MeshCollider>();
                mc.convex    = true;
                mc.isTrigger = false;
            }

            // Add water object
            if (target.GetComponent<WaterObject>() == null)
            {
                WaterObject wo = target.AddComponent<WaterObject>();
                wo.convexifyMesh       = true;
                wo.simplifyMesh        = true;
                wo.targetTriangleCount = 64;
                wo.GenerateSimMesh();

                MassFromVolume massFromVolume = target.AddComponent<MassFromVolume>();
                massFromVolume.SetDefaultAsMaterial();
                massFromVolume.CalculateAndApplyFromMaterial();
            }

            // Add Water Particle System and Particle System
            if (wow.addWaterParticleSystem)
            {
                GameObject waterParticleSystemPrefab = Resources.Load<GameObject>("DWP2/DefaultWaterParticleSystem");
                if (waterParticleSystemPrefab == null)
                {
                    Debug.LogError("Could not load WaterParticleSystemPrefab from Resources.");
                }
                else
                {
                    Instantiate(waterParticleSystemPrefab, target.transform);
                }
            }

            _wizardFinished = true;
        }
    }
}

#endif
