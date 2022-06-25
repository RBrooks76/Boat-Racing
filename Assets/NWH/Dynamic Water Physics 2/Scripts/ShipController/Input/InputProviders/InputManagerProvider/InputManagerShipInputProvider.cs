using UnityEngine;

namespace NWH.DWP2.ShipController
{
    /// <summary>
    ///     Class for handling desktop user input via mouse and keyboard through InputManager.
    /// </summary>
    [DisallowMultipleComponent]
    public class InputManagerShipInputProvider : ShipInputProvider
    {
        #if ENABLE_LEGACY_INPUT_MANAGER
        private static int _warningCount;


        public override float Steering()
        {
            return TryGetAxis("Steering");
        }


        public override float Throttle()
        {
            return TryGetAxis("Throttle");
        }
        
        
        public override float Throttle2()
        {
            return TryGetAxis("Throttle2");
        }
        
        
        public override float Throttle3()
        {
            return TryGetAxis("Throttle3");
        }
        
        
        public override float Throttle4()
        {
            return TryGetAxis("Throttle4");
        }


        public override float SternThruster()
        {
            return TryGetAxis("SternThruster");
        }


        public override float BowThruster()
        {
            return TryGetAxis("BowThruster");
        }


        public override float SubmarineDepth()
        {
            return TryGetAxis("SubmarineDepth");
        }


        public override bool EngineStartStop()
        {
            return TryGetButtonDown("EngineStartStop", KeyCode.E);
        }


        public override bool Anchor()
        {
            return TryGetButtonDown("Anchor", KeyCode.T);
        }


        public override Vector2 DragObjectPosition()
        {
            return new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
        }


        public override bool DragObjectModifier()
        {
            return TryGetButton("DragObjectModifier", KeyCode.LeftControl);
        }


        /// <summary>
        ///     Tries to get the button value through input manager, if not falls back to hardcoded default value.
        /// </summary>
        private static bool TryGetButton(string buttonName, KeyCode altKey, bool showWarning = true)
        {
            try
            {
                return UnityEngine.Input.GetButton(buttonName);
            }
            catch
            {
                // Make sure warning is not spammed as some users tend to ignore the warning and never set up the input,
                // resulting in bad performance in editor.
                if (_warningCount < 100 && showWarning)
                {
                    Debug.LogWarning(buttonName +
                                     " input binding missing, falling back to default. Check Input section in manual for more info.");
                    _warningCount++;
                }

                return UnityEngine.Input.GetKey(altKey);
            }
        }


        /// <summary>
        ///     Tries to get the button value through input manager, if not falls back to hardcoded default value.
        /// </summary>
        private static bool TryGetButtonDown(string buttonName, KeyCode altKey, bool showWarning = true)
        {
            try
            {
                return UnityEngine.Input.GetButtonDown(buttonName);
            }
            catch
            {
                if (_warningCount < 100 && showWarning)
                {
                    Debug.LogWarning(buttonName +
                                     " input binding missing, falling back to default. Check Input section in manual for more info.");
                    _warningCount++;
                }

                return UnityEngine.Input.GetKeyDown(altKey);
            }
        }


        /// <summary>
        ///     Tries to get the axis value through input manager, if not returns 0.
        /// </summary>
        private static float TryGetAxis(string axisName, bool showWarning = true)
        {
            try
            {
                return UnityEngine.Input.GetAxis(axisName);
            }
            catch
            {
                if (_warningCount < 100 && showWarning)
                {
                    Debug.LogWarning(axisName +
                                     " input binding missing. Check Input section in manual for more info.");
                    _warningCount++;
                }
            }

            return 0;
        }


        /// <summary>
        ///     Tries to get the axis value through input manager, if not returns 0.
        /// </summary>
        private static float TryGetAxisRaw(string axisName, bool showWarning = true)
        {
            try
            {
                return UnityEngine.Input.GetAxisRaw(axisName);
            }
            catch
            {
                if (_warningCount < 100 && showWarning)
                {
                    Debug.LogWarning(axisName +
                                     " input binding missing. Check Input section in manual for more info.");
                    _warningCount++;
                }
            }

            return 0;
        }
        #endif
    }
}