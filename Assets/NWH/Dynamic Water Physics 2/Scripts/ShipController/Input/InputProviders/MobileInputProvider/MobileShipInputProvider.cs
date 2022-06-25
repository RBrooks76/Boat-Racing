using NWH.Common.Input;
using UnityEngine.UI;

namespace NWH.DWP2.ShipController
{
    /// <summary>
    ///     Class for handling mobile user input via touch screen and sensors.
    /// </summary>
    public class MobileShipInputProvider : ShipInputProvider
    {
        // Ship
        public Slider            steeringSlider;
        public Slider            throttleSlider;
        public Slider            throttleSlider2;
        public Slider            throttleSlider3;
        public Slider            throttleSlider4;
        public Slider            sternThrusterSlider;
        public Slider            bowThrusterSlider;
        public Slider            submarineDepthSlider;
        public MobileInputButton engineStartStopButton;
        public MobileInputButton anchorButton;

        // Camera
        public MobileInputButton changeCameraButton;

        // Scene
        public MobileInputButton changeShipButton;


        public override float Steering()
        {
            if (steeringSlider != null)
            {
                return steeringSlider.value;
            }

            return 0;
        }


        public override float Throttle()
        {
            if (throttleSlider != null)
            {
                return throttleSlider.value;
            }

            return 0;
        }
        
        
        public override float Throttle2()
        {
            if (throttleSlider2 != null)
            {
                return throttleSlider2.value;
            }

            return 0;
        }
        
        
        public override float Throttle3()
        {
            if (throttleSlider3 != null)
            {
                return throttleSlider3.value;
            }

            return 0;
        }
        
        
        public override float Throttle4()
        {
            if (throttleSlider4 != null)
            {
                return throttleSlider4.value;
            }

            return 0;
        }


        public override float SternThruster()
        {
            if (sternThrusterSlider != null)
            {
                return sternThrusterSlider.value;
            }

            return 0;
        }


        public override float BowThruster()
        {
            if (bowThrusterSlider != null)
            {
                return bowThrusterSlider.value;
            }

            return 0;
        }


        public override float SubmarineDepth()
        {
            if (submarineDepthSlider != null)
            {
                return submarineDepthSlider.value;
            }

            return 0;
        }


        public override bool EngineStartStop()
        {
            if (engineStartStopButton != null)
            {
                return engineStartStopButton.hasBeenClicked;
            }

            return false;
        }


        public override bool Anchor()
        {
            if (anchorButton != null)
            {
                return anchorButton.hasBeenClicked;
            }

            return false;
        }
    }
}