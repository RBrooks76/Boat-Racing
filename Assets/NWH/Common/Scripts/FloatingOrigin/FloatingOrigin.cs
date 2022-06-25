// This script is based on the following script by Peter Stirling: https://wiki.unity3d.com/index.php/Floating_Origin

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace NWH.Common.FloatingOrigin
{
    /// <summary>
    ///     Moves scene objects back to origin when the Camera.main is further than distanceThreshold from world origin
    ///     [0,0,0].
    ///     Works only on current scene.
    /// </summary>
    public class FloatingOrigin : MonoBehaviour
    {
        public static FloatingOrigin Instance;
        
        public float   distanceThreshold = 500f;

        public UnityEvent OnBeforeJump = new UnityEvent();
        public UnityEvent OnAfterJump = new UnityEvent();

        private Vector3                   _totalOffset;
        private Camera                    _cameraMain;
        private Transform                 _cameraTransform;
        private Vector3                   _cameraPosition;
        private ParticleSystem.Particle[] _particles;

        public Vector3 TotalOffset
        {
            get { return _totalOffset; }
        }


        private void Awake()
        {
            Debug.Assert(Instance == null, "Only one FloatingOrigin script can be present in a scene.");
            Instance = this;

            OnBeforeJump.AddListener(BeforeJump);
            OnAfterJump.AddListener(AfterJump);
        }
        

		private List<T> FindObjects<T>() where T : UnityEngine.Object
		{
			return FindObjectsOfType<T>().ToList(); // Not the fastest solution
        }
        

        private void BeforeJump()
        {
            foreach (Rigidbody rb in FindObjects<Rigidbody>())
            {
                rb.sleepThreshold = float.MaxValue;
            }
        }

        private void AfterJump()
        {
            foreach (Rigidbody rb in FindObjects<Rigidbody>())
            {
                rb.sleepThreshold = 0.14f;
            }

            Physics.SyncTransforms();
        }


        private void LateUpdate()
        {
            _cameraMain = Camera.main;
            if (_cameraMain == null)
            {
                return;
            }

            _cameraTransform = _cameraMain.transform;
            _cameraPosition  = _cameraTransform.position;

            if (_cameraPosition.magnitude > distanceThreshold)
            {
                Jump();
            }
        }


        private void Jump()
        {
            OnBeforeJump.Invoke();

            _totalOffset += _cameraPosition;

            // Move root transforms
            for (int i = 0; i < SceneManager.sceneCount; i++) 
            {
                foreach (GameObject g in SceneManager.GetSceneAt(i).GetRootGameObjects()) 
                {
                    g.transform.position -= _cameraPosition;
                }
            }

            // Move particles
            foreach (ParticleSystem ps in FindObjects<ParticleSystem>())
            {
                ParticleSystem.MainModule main = ps.main;

                if (main.simulationSpace != ParticleSystemSimulationSpace.World)
                {
                    continue;
                }

                int maxParticles = main.maxParticles;

                if (maxParticles == 0)
                {
                    continue;
                }

                bool wasPaused = ps.isPaused;
                bool wasPlaying = ps.isPlaying;

                if (!wasPaused)
                {
                    ps.Pause();
                }

                if (_particles == null || _particles.Length < maxParticles)
                {
                    _particles = new ParticleSystem.Particle[maxParticles];
                }

                int num = ps.GetParticles(_particles);

                for (int i = 0; i < num; i++)
                {
                    _particles[i].position -= _cameraPosition;
                }

                ps.SetParticles(_particles, num);

                if (wasPlaying)
                {
                    ps.Play();
                }
            }

            OnAfterJump.Invoke();
        }
    }
}