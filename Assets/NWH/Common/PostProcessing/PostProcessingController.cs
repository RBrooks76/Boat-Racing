using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace NWH.Common.PostProcessing
{
    public class PostProcessingController : MonoBehaviour
    {
        public string postProcessingLayer = "PostProcessing";
        public PostProcessResources resources;
        
        private void Start()
        {
            if (resources == null) return;
            
            int ppLayer = LayerMask.NameToLayer(postProcessingLayer);
            if (ppLayer < 0) return;
            
            foreach (Camera cam in  GameObject.FindObjectsOfType<Camera>(true))
            {
                PostProcessLayer ppl = cam.gameObject.GetComponent<PostProcessLayer>();
                if (ppl == null)
                {
                    ppl = cam.gameObject.AddComponent<PostProcessLayer>();
                }

                cam.gameObject.layer = ppLayer;
                ppl.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                ppl.volumeTrigger = cam.gameObject.transform;
                ppl.volumeLayer = LayerMask.GetMask("PostProcessing");
                ppl.Init(resources);
            }
        }
    }
}

