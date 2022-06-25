using UnityEngine;

namespace NWH.DWP2.ShipController
{
    /// <summary>
    ///     Class for handling input through new InputSystem
    /// </summary>
    public class InputSystemShipInputProvider : ShipInputProvider
    {
        public ShipInputActions shipInputActions;

        private float _steering;
        private float _throttle;
        private float _throttle2;
        private float _throttle3;
        private float _throttle4;
        private float _sternThruster;
        private float _bowThruster;
        private float _submarineDepth;


        public new void Awake()
        {
            base.Awake();
            shipInputActions = new ShipInputActions();
            shipInputActions.Enable();
        }


        public void Update()
        {
            _steering       = shipInputActions.ShipControls.Steering.ReadValue<float>();
            _throttle      = shipInputActions.ShipControls.Throttle.ReadValue<float>();
            _throttle2      = shipInputActions.ShipControls.Throttle2.ReadValue<float>();
            _throttle3      = shipInputActions.ShipControls.Throttle3.ReadValue<float>();
            _throttle4      = shipInputActions.ShipControls.Throttle4.ReadValue<float>();
            _bowThruster    = shipInputActions.ShipControls.BowThruster.ReadValue<float>();
            _sternThruster  = shipInputActions.ShipControls.SternThruster.ReadValue<float>();
            _submarineDepth = shipInputActions.ShipControls.SubmarineDepth.ReadValue<float>();
        }


        // Ship bindings
        public override float Throttle()
        {
            return _throttle;
        }
        
        public override float Throttle2()
        {
            return _throttle2;
        }
        
        public override float Throttle3()
        {
            return _throttle3;
        }
        
        public override float Throttle4()
        {
            return _throttle4;
        }


        public override float Steering()
        {
            return _steering;
        }


        public override float BowThruster()
        {
            return _bowThruster;
        }


        public override float SternThruster()
        {
            return _sternThruster;
        }


        public override float SubmarineDepth()
        {
            return _submarineDepth;
        }


        public override bool EngineStartStop()
        {
            return shipInputActions.ShipControls.EngineStartStop.triggered;
        }


        public override bool Anchor()
        {
            return shipInputActions.ShipControls.Anchor.triggered;
        }


        public override Vector2 DragObjectPosition()
        {
            return Vector2.zero;
        }


        public override bool DragObjectModifier()
        {
            return false;
        }
    }
}