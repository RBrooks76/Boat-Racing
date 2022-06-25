using System.Collections.Generic;
using NWH.Common.Input;
using UnityEngine;

namespace NWH.Common.SceneManagement
{
    public class VehicleChanger : MonoBehaviour
    {
        /// <summary>
        ///     Is vehicle changing character based? When true changing vehicles will require getting close to them
        ///     to be able to enter, opposed to pressing a button to switch between vehicles.
        /// </summary>
        [Tooltip(
            "Is vehicle changing character based? When true changing vehicles will require getting close to them\r\nto be able to enter, opposed to pressing a button to switch between vehicles.")]
        public bool characterBased;

        /// <summary>
        ///     Index of the current vehicle in vehicles list.
        /// </summary>
        [Tooltip("    Index of the current vehicle in vehicles list.")]
        public int currentVehicleIndex;

        /// <summary>
        ///     Should the vehicles that the player is currently not using be put to sleep to improve performance?
        /// </summary>
        [Tooltip(
            "    Should the vehicles that the player is currently not using be put to sleep to improve performance?")]
        public bool putOtherVehiclesToSleep = true;

        /// <summary>
        ///     List of all of the vehicles that can be selected and driven in the scene.
        /// </summary>
        [Tooltip("List of all of the vehicles that can be selected and driven in the scene. " +
                 "If set to 0 script will try to auto-find all the vehicles in the scene with a tag define by VehiclesTag parameter.")]
        [SerializeField]
        public List<Vehicle> vehicles = new List<Vehicle>();

        /// <summary>
        ///     Tag that the script will search for if vehicles list is empty. Can be left empty if vehicles have already been
        ///     assigned manually.
        /// </summary>
        [Tooltip(
            "Tag that the script will search for if vehicles list is empty. Can be left empty if vehicles have already been assigned manually.")]
        public string vehicleTag = "Vehicle";

        /// <summary>
        ///     While true all the vehicles are asleep.
        /// </summary>
        [UnityEngine.Tooltip("    While true all the vehicles are asleep.")]
        public bool deactivateAll;

        public static VehicleChanger Instance { get; private set; }

        public static Vehicle ActiveVehicle { get; private set; }


        private void Awake()
        {
            Instance = this;
            RemoveNullVehicles();
        }


        private void Start()
        {
            if (characterBased && CharacterVehicleChanger.Instance == null)
            {
                Debug.LogError(
                    "CharacterBased is set to true but there is no CharacterVehicleChanger present in the scene.");
            }

            if (vehicles.Count == 0)
            {
                FindVehicles();
            }

            if (characterBased || deactivateAll)
            {
                DeactivateAllIncludingActive();
            }
            else
            {
                DeactivateAllExceptActive();
            }
        }


        private void Update()
        {
            if (!characterBased)
            {
                bool changeVehicle = InputProvider.CombinedInput<SceneInputProviderBase>(i => i.ChangeVehicle());

                if (changeVehicle)
                {
                    NextVehicle();
                }
            }

            if (vehicles.Count > 0)
            {
                ActiveVehicle = deactivateAll ? null : vehicles[currentVehicleIndex];
            }
            else
            {
                ActiveVehicle = null;
            }

            if (deactivateAll)
            {
                for (int i = 0; i < vehicles.Count; i++)
                {
                    if (vehicles[i].IsAwake)
                    {
                        vehicles[i].Sleep();
                    }
                }
            }
        }


        private void RemoveNullVehicles()
        {
            for (int i = vehicles.Count - 1; i >= 0; i--)
            {
                if (vehicles[i] == null)
                {
                    Debug.LogWarning("There is a null reference in the vehicles list. Removing. Make sure that" +
                                     " vehicles list does not contain any null references.");
                    vehicles.RemoveAt(i);
                }
            }
        }


        /// <summary>
        ///     Changes vehicle to requested vehicle.
        /// </summary>
        /// <param name="index">Index of a vehicle in Vehicles list.</param>
        public void ChangeVehicle(int index)
        {
            currentVehicleIndex = index;
            if (currentVehicleIndex >= vehicles.Count)
            {
                currentVehicleIndex = 0;
            }

            DeactivateAllExceptActive();
        }


        /// <summary>
        ///     Finds nearest vehicle on the vehicles list.
        /// </summary>
        public Vehicle NearestVehicleFrom(GameObject go)
        {
            Vehicle nearest = null;

            int   minIndex = -1;
            float minDist  = Mathf.Infinity;
            if (vehicles.Count > 0)
            {
                for (int i = 0; i < vehicles.Count; i++)
                {
                    if (!vehicles[i].gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    float distance = Vector3.Distance(go.transform.position, vehicles[i].transform.position);
                    if (distance < minDist)
                    {
                        minIndex = i;
                        minDist  = distance;
                    }
                }

                nearest = vehicles[minIndex];
            }

            return nearest;
        }


        /// <summary>
        ///     Changes vehicle to a vehicle with the requested name if there is such a vehicle.
        /// </summary>
        public void ChangeVehicle(Vehicle ac)
        {
            int vehicleIndex = vehicles.IndexOf(ac);

            if (vehicleIndex >= 0)
            {
                ChangeVehicle(vehicleIndex);
            }
        }


        /// <summary>
        ///     Changes vehicle to a next vehicle on the Vehicles list.
        /// </summary>
        public void NextVehicle()
        {
            if (vehicles.Count == 1)
            {
                return;
            }

            ChangeVehicle(currentVehicleIndex + 1);
        }


        public void PreviousVehicle()
        {
            if (vehicles.Count == 1)
            {
                return;
            }

            int previousIndex = currentVehicleIndex == 0 ? vehicles.Count - 1 : currentVehicleIndex - 1;


            ChangeVehicle(previousIndex);
        }


        public void DeactivateAllExceptActive()
        {
            for (int i = 0; i < vehicles.Count; i++)
            {
                if (i == currentVehicleIndex)
                {
                    vehicles[i].Wake();
                }
                else if (putOtherVehiclesToSleep)
                {
                    vehicles[i].Sleep();
                }
            }
        }


        public void DeactivateAllIncludingActive()
        {
            for (int i = 0; i < vehicles.Count; i++)
            {
                vehicles[i].Sleep();
            }
        }


        public void FindVehicles()
        {
            GameObject[] candidateGOs = GameObject.FindGameObjectsWithTag(vehicleTag);
            if (vehicles == null)
            {
                vehicles = new List<Vehicle>();
            }

            foreach (GameObject go in candidateGOs)
            {
                Vehicle ac = go.GetComponent<Vehicle>();
                if (ac != null)
                {
                    vehicles.Add(ac);
                }
            }
        }
    }
}