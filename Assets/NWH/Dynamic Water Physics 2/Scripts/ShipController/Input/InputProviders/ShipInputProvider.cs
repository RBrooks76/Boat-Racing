using NWH.Common.Input;
using UnityEngine;

namespace NWH.DWP2.ShipController
{
    /// <summary>
    ///     Base abstract class from which all input providers inherit.
    /// </summary>
    public abstract class ShipInputProvider : InputProvider
    {
        // Ship bindings
        public virtual float Steering()
        {
            return 0f;
        }


        public virtual float Throttle()
        {
            return 0f;
        }

        
        public virtual float Throttle2()
        {
            return 0f;
        }
        
        
        public virtual float Throttle3()
        {
            return 0f;
        }
        
        
        public virtual float Throttle4()
        {
            return 0f;
        }


        public virtual float SternThruster()
        {
            return 0f;
        }


        public virtual float BowThruster()
        {
            return 0f;
        }


        public virtual float SubmarineDepth()
        {
            return 0f;
        }


        public virtual bool EngineStartStop()
        {
            return false;
        }


        public virtual bool Anchor()
        {
            return false;
        }


        // Additional scene bindings
        public virtual Vector2 DragObjectPosition()
        {
            return Vector2.zero;
        }


        public virtual bool DragObjectModifier()
        {
            return false;
        }
    }
}