using System;
using UnityEngine;

namespace NWH.DWP2.ShipController
{
    /// <summary>
    ///     Struct for storing ship input
    /// </summary>
    [Serializable]
    public struct ShipInputStates
    {
        [Range(-1, 1)]
        public float steering;

        [Range(-1, 1)]
        public float throttle;
        
        [Range(-1, 1)]
        public float throttle2;
        
        [Range(-1, 1)]
        public float throttle3;
        
        [Range(-1, 1)]
        public float throttle4;

        [Range(-1, 1)]
        public float sternThruster;

        [Range(-1, 1)]
        public float bowThruster;

        [Range(0, 1)]
        public float submarineDepth;

        public bool engineStartStop;
        public bool anchor;

        public bool changeShip;
        public bool changeCamera;


        public void Reset()
        {
            throttle        = 0;
            throttle2        = 0;
            throttle3        = 0;
            throttle4        = 0;
            sternThruster   = 0;
            bowThruster     = 0;
            submarineDepth  = 0;
            engineStartStop = false;
            anchor          = false;
        }
    }
}