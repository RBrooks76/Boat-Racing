using NWH.DWP2.ShipController;
using NWH.Common.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NWH.DWP2.DemoContent
{
    public class GUIHandler : MonoBehaviour
    {
        public Text  speedText;
        public Text  rudderText;
        public Image anchorImage;
        public bool  reset;

        private AdvancedShipController activeShip;


        private void Update()
        {
            activeShip = (AdvancedShipController) VehicleChanger.ActiveVehicle;
            if (activeShip != null)
            {
                float speed = activeShip.SpeedKnots;
                speedText.text = "SPEED: " + $"{speed:0.0}" + "kts";

                if (activeShip.rudders.Count > 0)
                {
                    float rudderAngle = activeShip.rudders[0].Angle;
                    rudderText.text = "RUDDER: " + $"{rudderAngle:0.0}" + "°";
                }

                if (activeShip.Anchor != null)
                {
                    if (activeShip.Anchor.Dropped)
                    {
                        anchorImage.gameObject.SetActive(true);
                    }
                    else
                    {
                        anchorImage.gameObject.SetActive(false);
                    }
                }
            }
        }


        public void ResetScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}