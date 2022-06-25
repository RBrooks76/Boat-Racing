using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NWH.DWP2.WaterObjects;

#if UNITY_EDITOR
using NWH.DWP2.NUI;
using UnityEditor;
#endif

namespace NWH.DWP2.Tests
{
    public class WaterObjectTest : MonoBehaviour
    {
        public GameObject prefab;

        private GameObject _instance;

        public void Instantiate(Vector3 position)
        {
            _instance = GameObject.Instantiate(prefab);
            _instance.transform.position = position;
        }

        public void Activate()
        {
            _instance.SetActive(true);
        }

        public void Deactivate()
        {
            _instance.SetActive(false);
        }

        public void EnableWaterObject()
        {
            WaterObject wo = GetComponentInChildren<WaterObject>();
            if (wo != null)
            {
                wo.enabled = true;
            }
        }
        
        public void DisableWaterObject()
        {
            WaterObject wo = GetComponentInChildren<WaterObject>();
            if (wo != null)
            {
                wo.enabled = false;
            }
        }

        public void Destroy()
        {
            GameObject.Destroy(_instance);
        }

        public void Teleport(Vector3 distance)
        {
            _instance.transform.position += distance;
        }

        public void Rotate(Vector3 eulerAngles)
        {
            _instance.transform.Rotate(eulerAngles, Space.Self);
        }

        public void ToggleIsKinematic()
        {
            Rigidbody rb = _instance.GetComponentInChildren<Rigidbody>();
            rb.isKinematic = !rb.isKinematic;
        }
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(WaterObjectTest))]
    [CanEditMultipleObjects]
    public class WaterObjectTestEditor : DWP_NUIEditor
    {
        private WaterObjectTest _wot;


        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }
            
            _wot = (WaterObjectTest) target;

            drawer.Field("prefab");

            if (drawer.Button("Instantiate"))
            {
                _wot.Instantiate(Vector3.zero);
            }
            
            if (drawer.Button("Activate"))
            {
                _wot.Activate();
            }
            
            if (drawer.Button("Deactivate"))
            {
                _wot.Deactivate();
            }
            
            if (drawer.Button("Destroy"))
            {
                _wot.Destroy();
            }

            if (drawer.Button("Teleport Forward"))
            {
                _wot.Teleport(Vector3.forward);
            }
            
            if (drawer.Button("Teleport Up"))
            {
                _wot.Teleport(Vector3.up);
            }
            
            if (drawer.Button("Teleport Down"))
            {
                _wot.Teleport(Vector3.down);
            }

            if (drawer.Button("Rotate Left"))
            {
                _wot.Rotate(new Vector3(0, -90, 0));
            }

            if (drawer.Button("Rotate Right"))
            {
                _wot.Rotate(new Vector3(0, 90, 0));
            }

            if (drawer.Button("Instantiate & Synchronize"))
            {
                _wot.Instantiate(Vector3.zero);
            }
            
            if (drawer.Button("Instantiate & Synchronize & Teleport"))
            {
                _wot.Instantiate(Vector3.zero);
                _wot.DisableWaterObject();
                _wot.EnableWaterObject();
            }
            
            
            if (drawer.Button("Toggle IsKinematic"))
            {
                _wot.ToggleIsKinematic();
            }
            
            drawer.EndEditor(this);
            return true;
        }
    }
    #endif
}

