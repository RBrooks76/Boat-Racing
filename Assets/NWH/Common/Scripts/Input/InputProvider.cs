using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.Common.Input
{
    /// <summary>
    ///     Base class from which all input providers inherit.
    /// </summary>
    public abstract class InputProvider : MonoBehaviour
    {
        /// <summary>
        ///     List of all InputProviders in the scene.
        /// </summary>
        public static List<InputProvider> Instances = new List<InputProvider>();


        public virtual void Awake()
        {
            Instances.Add(this);
        }


        public virtual void OnDestroy()
        {
            // Reset instances list on scene exit.
            Instances = new List<InputProvider>();
        }


        /// <summary>
        ///     Returns combined input of all InputProviders present in the scene.
        ///     Result will be a sum of all inputs of the selected type.
        ///     T is a type of InputProvider that the input will be retrieved from.
        /// </summary>
        public static int CombinedInput<T>(Func<T, int> selector) where T : InputProvider
        {
            int sum = 0;
            foreach (InputProvider ip in Instances)
            {
                if (ip is T)
                {
                    sum += selector(ip as T);
                }
            }

            return sum;
        }


        /// <summary>
        ///     Returns combined input of all InputProviders present in the scene.
        ///     Result will be a sum of all inputs of the selected type.
        ///     T is a type of InputProvider that the input will be retrieved from.
        /// </summary>
        public static float CombinedInput<T>(Func<T, float> selector) where T : InputProvider
        {
            float sum = 0;
            foreach (InputProvider ip in Instances)
            {
                if (ip is T)
                {
                    sum += selector(ip as T);
                }
            }

            return sum;
        }


        /// <summary>
        ///     Returns combined input of all InputProviders present in the scene.
        ///     Result will be positive if any InputProvider has the selected input set to true.
        ///     T is a type of InputProvider that the input will be retrieved from.
        /// </summary>
        public static bool CombinedInput<T>(Func<T, bool> selector) where T : InputProvider
        {
            foreach (InputProvider ip in Instances)
            {
                if (ip is T && selector(ip as T))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        ///     Returns combined input of all InputProviders present in the scene.
        ///     Result will be a sum of all inputs of the selected type.
        ///     T is a type of InputProvider that the input will be retrieved from.
        /// </summary>
        public static Vector2 CombinedInput<T>(Func<T, Vector2> selector) where T : InputProvider
        {
            Vector2 sum = Vector2.zero;
            foreach (InputProvider ip in Instances)
            {
                if (ip is T)
                {
                    sum += selector(ip as T);
                }
            }

            return sum;
        }
    }
}