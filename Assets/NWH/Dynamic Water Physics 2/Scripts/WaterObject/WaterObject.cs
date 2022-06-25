using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using NWH.Common.Utility;
using NWH.DWP2.WaterData;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NWH.DWP2.WaterObjects
{
    [RequireComponent(typeof(MeshFilter))]
    [DisallowMultipleComponent]
    public class WaterObject : MonoBehaviour
    {
	    /// <summary>
	    /// Coefficient by which the result force will get multiplied.
	    /// </summary>
	    [UnityEngine.Tooltip("Coefficient by which the result force will get multiplied.")]
	    public float finalForceCoefficient = 1.0f;

	    /// <summary>
	    /// Coefficient by which the torque force will get multiplied.
	    /// </summary>
        [UnityEngine.Tooltip("Coefficient by which the torque force will get multiplied.")]
        public float finalTorqueCoefficient = 1.0f;

	    /// <summary>
	    /// World water height to be used when there is no WaterDataProvider present.
	    /// </summary>
        [UnityEngine.Tooltip("World water height to be used when there is no WaterDataProvider present.")]
        public float defaultWaterHeight = 0.0f;

	    /// <summary>
	    /// World water normal to be used when there is no WaterDataProvider present and calculateWaterNormals is enabled.
	    /// </summary>
        [UnityEngine.Tooltip("World water normal to be used when there is no WaterDataProvider present and calculateWaterNormals is enabled.")]
        public Vector3 defaultWaterNormal = Vector3.up;

	    /// <summary>
	    /// World water flow to be used when there is no WaterDataProvider present and calculateWaterFlows is enabled.
	    /// </summary>
        [UnityEngine.Tooltip("World water flow to be used when there is no WaterDataProvider present and calculateWaterFlows is enabled.")]
        public Vector3 defaultWaterFlow = Vector3.zero;

	    /// <summary>
	    /// Should the water heights be queried and calculated?
	    /// </summary>
        [UnityEngine.Tooltip("Should the water heights be queried and calculated?")]
        public bool calculateWaterHeights = true;

	    /// <summary>
	    /// Should the water normals be queried and calculated?
	    /// </summary>
        [UnityEngine.Tooltip("Should the water normals be queried and calculated?")]
        public bool calculateWaterNormals = false;

	    /// <summary>
	    /// Should the water flows be queried and calculated?
	    /// </summary>
        [UnityEngine.Tooltip("Should the water flows be queried and calculated?")]
        public bool calculateWaterFlows = false;

	    /// <summary>
	    /// Density of the fluid the object is in.
	    /// </summary>
        [UnityEngine.Tooltip("Density of the fluid the object is in.")]
        public float fluidDensity = 1030.0f;

	    /// <summary>
	    /// Coefficient by which the the buoyancy forces are multiplied.
	    /// </summary>
        [UnityEngine.Tooltip("Coefficient by which the the buoyancy forces are multiplied.")]
        public float buoyantForceCoefficient = 1.0f;

	    /// <summary>
	    /// Coefficient applied to forces when a face of the object is entering the water.
	    /// </summary>
        [UnityEngine.Tooltip("Coefficient applied to forces when a face of the object is entering the water.")]
        public float slamForceCoefficient = 1.0f;

	    /// <summary>
	    /// Coefficient applied to forces when a face of the object is leaving the water.
	    /// </summary>
        [UnityEngine.Tooltip("Coefficient applied to forces when a face of the object is leaving the water.")]
        public float suctionForceCoefficient = 1.0f;

	    /// <summary>
        /// Coefficient by which the hydrodynamic forces are multiplied..
        /// </summary>
        [UnityEngine.Tooltip("Coefficient by which the hydrodynamic forces are multiplied..")]
        public float hydrodynamicForceCoefficient = 1.0f;

	    /// <summary>
        /// Determines to which power the dot product between velocity and triangle normal will be raised.
        /// Higher values will result in lower hydrodynamic forces for situations in which the triangle is nearly parallel to the
        /// water velocity.
        /// </summary>
        [Range(0.01f, 2f)]
        [UnityEngine.Tooltip("Determines to which power the dot product between velocity and triangle normal will be raised.\r\nHigher values will result in lower hydrodynamic forces for situations in which the triangle is nearly parallel to the\r\nwater velocity.")]
        public float velocityDotPower = 1f;

	    /// <summary>
        /// Coefficient by which the skin drag force is multiplied.
        /// Skin drag is a force caused by water flowing over a surface.
        /// </summary>
        [UnityEngine.Tooltip("Coefficient by which the skin drag force is multiplied.\r\nSkin drag is a force caused by water flowing over a surface.")]
        public float skinDragCoefficient = 0.01f;

	    /// <summary>
        /// Should the simulation mesh be made convex?
        /// Can be useful for half-open hulls and otherwise partially hollow meshes.
        /// </summary>
        [UnityEngine.Tooltip("Should the simulation mesh be made convex?\r\nCan be useful for half-open hulls and otherwise partially hollow meshes.")]
        public bool convexifyMesh = true;

	    /// <summary>
        ///     Should the simulation mesh be simplified / decimated?
        /// </summary>
        [UnityEngine.Tooltip("    Should the simulation mesh be simplified / decimated?")]
        public bool simplifyMesh = true;

	    /// <summary>
        ///     If true vertices with same position will be welded.
        ///     Improves performance and is highly recommended.
        /// </summary>
        [UnityEngine.Tooltip("    If true vertices with same position will be welded.\r\n    Improves performance and is highly recommended.")]
        public bool weldColocatedVertices = true;

	    /// <summary>
        ///     Target triangle count for the simulation mesh.
        ///     Original mesh will be decimated to this number of triangles is "SimplifyMesh" is enabled.
        ///     Otherwise does nothing.
        /// </summary>
        [FormerlySerializedAs("targetTris")] [Range(8, 256)]
        public int targetTriangleCount = 64;

	    /// <summary>
	    /// Cached instance ID of the GameObject.
	    /// </summary>
	    [NonSerialized] public int instanceID;
	    
	    /// <summary>
	    ///     Original mesh of the object, non-simplified and non-convexified.
	    /// </summary>
	    [SerializeField] public Mesh           originalMesh;
	    
	    /// <summary>
	    ///     Mesh used to simulate water physics.
	    /// </summary>
	    [SerializeField] private Mesh           simulationMesh;

	    public Mesh SimulationMesh
	    {
		    get
		    {
			    if (simulationMesh == null && serializedSimulationMesh != null)
			    {
				    simulationMesh = new Mesh
				    {
					    vertices = serializedSimulationMesh.vertices,
					    triangles = serializedSimulationMesh.triangles
				    };
			    }

			    return simulationMesh;
		    }
	    }
	    
	    /// <summary>
	    ///     Serialized version of the simulation mesh, used to prevent having to store the mesh in game files.
	    /// </summary>
	    [SerializeField] public SerializedMesh serializedSimulationMesh;

	    /// <summary>
	    /// Vertices used for simulation, in local space.
	    /// </summary>
	    [NonSerialized] public Vector3[] LocalVertices;
	    
	    /// <summary>
	    /// Result triangle areas.
	    /// </summary>
	    [NonSerialized] public float[] ResultAreas;
	    
	    /// <summary>
	    /// Result triangle centers in world coordinates.
	    /// </summary>
	    [NonSerialized] public Vector3[] ResultCenters;
	    
	    /// <summary>
	    /// Result distances to water surface.
	    /// </summary>
	    [NonSerialized] public float[] ResultDistances;
	    
	    /// <summary>
	    /// Total result force.
	    /// </summary>
	    [NonSerialized] public Vector3 ResultForce;
	    
	    /// <summary>
	    /// Per-triangle result forces.
	    /// </summary>
	    [NonSerialized] public Vector3[] ResultForces;
	    
	    /// <summary>
	    /// Result triangle normals. Not equal to the mesh normals.
	    /// </summary>
	    [NonSerialized] public Vector3[] ResultNormals;
	    
	    /// <summary>
	    /// Result sliced triangle vertices.
	    /// </summary>
	    [NonSerialized] public Vector3[] ResultP0s;

	    /// <summary>
	    /// Result triangle states.
	    ///     0 - Triangle is under water
	    ///     1 - Triangle is partially under water
	    ///     2 - Triangle is above water
	    ///     3 - Triangle's object is disabled
	    ///     4 - Triangle's object is deleted
	    /// </summary>
	    [NonSerialized] public int[] ResultStates;
	    
	    /// <summary>
	    /// Result total torque acting on the Rigidbody.
	    /// </summary>
	    [NonSerialized] public Vector3 ResultTorque;
	    
	    /// <summary>
	    /// Result velocities at triangle centers.
	    /// </summary>
	    [NonSerialized] public Vector3[] ResultVelocities;
	    
	    /// <summary>
	    /// Angular velocity of the target Rigidbody.
	    /// </summary>
	    [NonSerialized] public Vector3 RigidbodyAngVel = new Vector3(0, 0, 0);

	    /// <summary>
	    /// Center of mass of the target Rigidbody.
	    /// </summary>
	    [NonSerialized] public Vector3 RigidbodyCoM = new Vector3(0, 0, 0);
	    
	    /// <summary>
	    /// (Linear) velocity of the target Rigidbody.
	    /// </summary>
	    [NonSerialized] public Vector3 RigidbodyLinearVel = new Vector3(0, 0, 0);
	    
	    /// <summary>
	    /// Triangles indices of the simulation mesh.
	    /// </summary>
	    [NonSerialized] public int[] TriIndices;

	    /// <summary>
	    /// Water flows of the current water data provider. Default is used if no water data provider is present.
	    /// </summary>
	    [NonSerialized] public Vector3[] WaterFlows;
	    
	    /// <summary>
	    /// Water surface heights of the current water data provider. Default is used if no water data provider is present.
	    /// </summary>
	    [NonSerialized] public float[] WaterHeights;
	    
	    /// <summary>
	    /// Water normals of the current water data provider. Default is used if no water data provider is present.
	    /// </summary>
	    [NonSerialized] public Vector3[] WaterNormals;
	    
	    /// <summary>
	    /// Simulation vertices coverted to world coordinates.
	    /// </summary>
	    [NonSerialized] public Vector3[] WorldVertices;
	    
	    /// <summary>
	    /// A list of the WaterDataProviders that the vehicle has entered but has yet to exit.
	    /// </summary>
	    [NonSerialized] private List<WaterDataProvider> availableWaterDataProviders;
	    
	    /// <summary>
	    /// Currently active WaterDataProvider.
	    /// </summary>
	    [NonSerialized] public WaterDataProvider CurrentWaterDataProvider;
	    
	    /// <summary>
	    /// Total number of triangles (not indices) on the simulation mesh.
	    /// </summary>
	    [UnityEngine.Tooltip("Total number of triangles (not indices) on the simulation mesh.")]
	    public int triangleCount;
	    
	    /// <summary>
	    /// Total number of vertices on the simulation mesh.
	    /// </summary>
	    [UnityEngine.Tooltip("Total number of vertices on the simulation mesh.")]
	    public int vertexCount;
	    
	    /// <summary>
	    ///     Rigidbody that the forces will be applied to.
	    /// </summary>
	    [UnityEngine.Tooltip("    Rigidbody that the forces will be applied to.")]
	    public Rigidbody targetRigidbody;
	    
	    private Vector3 _gravity = new Vector3(0, -9.81f, 0);
	    private Vector3 _worldUpVector = new Vector3(0.0f, 1.0f, 0.0f);
	    private Matrix4x4 _localToWorldMatrix;
	    private MeshFilter _meshFilter;
	    private float      _simplificationRatio;
	    
	    /// <summary>
        ///     Is the simulation mesh preview enabled?
        /// </summary>
        public bool PreviewEnabled
        {
            get
            {
                if (_meshFilter == null || _meshFilter.sharedMesh == null)
                {
                    return false;
                }

                return _meshFilter.sharedMesh.name == "DWP_SIM_MESH";
            }
        }

	    private void Awake()
	    {
		    instanceID = gameObject.GetInstanceID();
	        
	        availableWaterDataProviders = new List<WaterDataProvider>();
	        _worldUpVector = Vector3.Normalize(-Physics.gravity);

            targetRigidbody = transform.GetComponentInParent<Rigidbody>(true);
            if (targetRigidbody == null)
            {
                Debug.LogError($"TargetRigidbody on object {name} is null.");
                return;
            }

            _meshFilter = GetComponent<MeshFilter>();
            if (_meshFilter == null)
            {
                Debug.LogError($"MeshFilter on object {name} is null.");
                return;
            }

            if (PreviewEnabled)
            {
                StopSimMeshPreview();
            }

            int colliderCount = targetRigidbody.transform.GetComponentsInChildren<Collider>(true).Length;
            if (colliderCount == 0)
            {
                Debug.LogError($"{targetRigidbody.name} has 0 colliders.");
                return;
            }

            if (!PreviewEnabled)
            {
                originalMesh = _meshFilter.sharedMesh;

                if (originalMesh == null)
                {
                    Debug.LogError($"MeshFilter on object {name} does not have a valid mesh assigned.");
                    ShowErrorMessage();
                    return;
                }

                if (simulationMesh == null)
                {
                    simulationMesh = serializedSimulationMesh.Deserialize();
                    if (simulationMesh == null)
                    {
                        simulationMesh = MeshUtility.GenerateMesh(originalMesh.vertices, originalMesh.triangles);
                    }
                }

                // Serialize only if mesh has been modified
                simulationMesh.name = "DWP_SIM_MESH";
                serializedSimulationMesh.Serialize(simulationMesh);
            }

            Debug.Assert(simulationMesh != null, $"Simulation mesh is null on object {name}.");
            
            Debug.Assert(simulationMesh.GetInstanceID() != originalMesh.GetInstanceID(),
                         $"Simulation mesh and original mesh have same Instance ID on object {name}. !BUG!.");
            
            LocalVertices = simulationMesh.vertices;
            TriIndices = simulationMesh.triangles;

            vertexCount = LocalVertices.Length;
            triangleCount = TriIndices.Length / 3;
            
            WorldVertices = new Vector3[vertexCount];
            WaterHeights = new float[vertexCount];
            WaterNormals = new Vector3[vertexCount];
            WaterFlows = new Vector3[vertexCount];
            ResultStates = new int[triangleCount];
            ResultVelocities = new Vector3[triangleCount];
            ResultP0s = new Vector3[triangleCount * 6];
            ResultForces = new Vector3[triangleCount];
            ResultCenters = new Vector3[triangleCount];
            ResultNormals = new Vector3[triangleCount];
            ResultAreas = new float[triangleCount];
            ResultDistances = new float[triangleCount];

			// Fill in default data
			for (int i = 0; i < vertexCount; i++)
			{
				WaterHeights[i] = defaultWaterHeight;
				WaterNormals[i] = defaultWaterNormal;
				WaterFlows[i] = defaultWaterFlow;
			}

			for (int i = 0; i < triangleCount; i++)
			{
				ResultStates[i] = 2;
			}
        }

	    private void FixedUpdate()
        {
            if (simulationMesh == null) return;
            if (triangleCount == 0) return;
            if (Time.fixedDeltaTime == 0) return;
	
            // Get water data
            if(availableWaterDataProviders != null && availableWaterDataProviders.Count > 0)
            {
	            CurrentWaterDataProvider = availableWaterDataProviders.Last();
            }
            
            if (CurrentWaterDataProvider != null)
            {
	            CurrentWaterDataProvider.GetWaterHeightsFlowsNormals(this, ref WorldVertices, ref WaterHeights, ref WaterFlows, ref WaterNormals,
		            calculateWaterHeights, calculateWaterNormals, calculateWaterFlows);
            }

            // Set simulation data and run
            TickWaterObject(
                targetRigidbody.worldCenterOfMass,
                targetRigidbody.velocity,
                targetRigidbody.angularVelocity,
                transform.localToWorldMatrix,
                Physics.gravity
            );

            // Apply force and torque
            targetRigidbody.AddForce(ResultForce);
            targetRigidbody.AddTorque(ResultTorque);
        }

	    private void OnDisable()
        {
            if (Application.isEditor)
            {
                StopSimMeshPreview();
            }
        }

	    private void OnDestroy()
        {
            OnDisable();
        }


	    private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            #if UNITY_EDITOR
	        
	        if (triangleCount <= 0) return;
	        
            for (int i = 0; i < triangleCount; i++)
            {
                int triIndex  = i;
                int vertIndex = i * 3;
                int pIndex    = i * 6;
                int state     = ResultStates[triIndex];
                
                if(state >= 2) continue;

                Color lineColor = state == 0 ? Color.cyan : state == 1 ? Color.green : Color.white;


                // Draw force point
                Vector3 forcePoint = ResultCenters[triIndex];
                Gizmos.DrawWireSphere(forcePoint, 0.01f);
                //Handles.Label(forcePoint, $"T{i}-S{state}");
                
                // Draw normal
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(forcePoint, forcePoint + ResultNormals[triIndex] * 0.1f);
                
                // Draw P0s
                Gizmos.color = lineColor;
                Vector3 p00 = ResultP0s[pIndex];
                Vector3 p01 = ResultP0s[pIndex + 1];
                Vector3 p02 = ResultP0s[pIndex + 2];
                
                // Draw triangle
                Gizmos.color = Color.Lerp(Color.green, Color.red, (ResultForces[i].magnitude * 0.00002f) / ResultAreas[i]);
                Gizmos.DrawLine(p00, p01);
                Gizmos.DrawLine(p01, p02);
                Gizmos.DrawLine(p02, p00);
                
                // Draw indices
                // Handles.Label(p00, $"{i}:P00");
                // Handles.Label(p01, $"{i}:P01");
                // Handles.Label(p02, $"{i}:P02");

                // Draw P1s
                Gizmos.color = lineColor;
                Vector3 p10 = ResultP0s[pIndex + 3];
                Vector3 p11 = ResultP0s[pIndex + 4];
                Vector3 p12 = ResultP0s[pIndex + 5];
                
                if (state == 1)
                {
                    Gizmos.DrawLine(p10, p11);
                    Gizmos.DrawLine(p11, p12);
                    Gizmos.DrawLine(p12, p10);
                    
                    // Handles.Label(p10, $"{i}:P10");
                    // Handles.Label(p11, $"{i}:P11");
                    // Handles.Label(p12, $"{i}:P12");
                }

                // Draw vertices
                int v0i = TriIndices[vertIndex];
                int v1i = TriIndices[vertIndex + 1];
                int v2i = TriIndices[vertIndex + 2];

                Vector3 v0 = WorldVertices[v0i];
                Vector3 v1 = WorldVertices[v1i];
                Vector3 v2 = WorldVertices[v2i];
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(v0, 0.01f);
                Gizmos.DrawSphere(v1, 0.01f);
                Gizmos.DrawSphere(v2, 0.01f);
                
                Handles.color = Color.yellow;
                // Handles.Label(v0, $"T{i}V0");
                // Handles.Label(v0, $"T{i}V1");
                // Handles.Label(v0, $"T{i}V2");

                // Draw vertex water heights
                // Gizmos.color = Color.magenta;
                // Vector3 up = Vector3.up;
                // Gizmos.DrawLine(v0, v0 - up * (v0.y - wom.WaterHeights[v0i]));
                // Gizmos.DrawLine(v1, v1 - up * (v1.y - wom.WaterHeights[v1i]));
                // Gizmos.DrawLine(v2, v2 - up * (v2.y - wom.WaterHeights[v2i]));

                // Visualize distance to water
                Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
                Gizmos.DrawLine(forcePoint, forcePoint + Vector3.up * ResultDistances[triIndex]);
            }
            #endif
        }

	    public void OnEnterWaterDataProvider(WaterDataProvider wdp)
        {
	        if(availableWaterDataProviders.All(e => e != wdp))
	        {
		        availableWaterDataProviders.Add(wdp);
	        }
        }

	    public void OnExitWaterDataProvider(WaterDataProvider wdp)
        {
	        availableWaterDataProviders.Remove(wdp);
        }

	    private void TickWaterObject(Vector3 rigidbodyCoM, Vector3 rigidbodyLinVel, Vector3 rigidbodyAngVel, Matrix4x4 l2wMatrix, Vector3 gravity)
        {
	        RigidbodyLinearVel = rigidbodyLinVel;
            RigidbodyAngVel = rigidbodyAngVel;
            RigidbodyCoM = rigidbodyCoM;
            _localToWorldMatrix = l2wMatrix;
            _gravity = gravity;
            
            for (int i = 0; i < vertexCount; i++)
            {
	            WorldVertices[i] = _localToWorldMatrix.MultiplyPoint(LocalVertices[i]);
            }
            
            for (int i = 0; i < triangleCount; i++)
            {
	            CalcTri(i);
            }

            // Calculate result force and torque
            Vector3 forceSum;
            forceSum.x = 0;
            forceSum.y = 0;
            forceSum.z = 0;

            Vector3 torqueSum;
            torqueSum.x = 0;
            torqueSum.y = 0;
            torqueSum.z = 0;

            float counter = 0;
            Vector3 resultForce;
            Vector3 worldCoM = targetRigidbody.worldCenterOfMass;
            for (int i = 0; i < triangleCount; i++)
            {
                if (ResultStates[i] < 2)
                {
                    counter ++;
                    resultForce = ResultForces[i];
                    
                    forceSum.x += resultForce.x;
                    forceSum.y += resultForce.y;
                    forceSum.z += resultForce.z;

                    Vector3 resultCenter = ResultCenters[i];
                    
                    Vector3 dir;
                    dir.x = resultCenter.x - worldCoM.x;
                    dir.y = resultCenter.y - worldCoM.y;
                    dir.z = resultCenter.z - worldCoM.z;

                    Vector3 rf = ResultForces[i];
                    Vector3 crossDirForce;
                    crossDirForce.x = dir.y * rf.z - dir.z * rf.y;
                    crossDirForce.y = dir.z * rf.x - dir.x * rf.z;
                    crossDirForce.z = dir.x * rf.y - dir.y * rf.x;
                    
                    torqueSum.x += crossDirForce.x;
                    torqueSum.y += crossDirForce.y;
                    torqueSum.z += crossDirForce.z;
                }
            }
            
            ResultForce.x = forceSum.x * finalForceCoefficient;
            ResultForce.y = forceSum.y * finalForceCoefficient;
            ResultForce.z = forceSum.z * finalForceCoefficient;
            
            ResultTorque.x = torqueSum.x * finalTorqueCoefficient;
            ResultTorque.y = torqueSum.y * finalTorqueCoefficient;
            ResultTorque.z = torqueSum.z * finalTorqueCoefficient;
        }

	    private void CalcTri(int i)
		{
			if (ResultStates[i] >= 3)
			{
				return;
			}

			int baseIndex = i * 3;
			int vertIndex0 = TriIndices[baseIndex];
			int vertIndex1 = TriIndices[baseIndex + 1];
			int vertIndex2 = TriIndices[baseIndex + 2];

			Vector3 P0 = WorldVertices[vertIndex0];
			Vector3 P1 = WorldVertices[vertIndex1];
			Vector3 P2 = WorldVertices[vertIndex2];

			float wh_P0 = WaterHeights[vertIndex0];
			float wh_P1 = WaterHeights[vertIndex1];
			float wh_P2 = WaterHeights[vertIndex2];

			float d0 = P0.y - wh_P0;
			float d1 = P1.y - wh_P1;
			float d2 = P2.y - wh_P2;

			//All vertices are above water
			if (d0 >= 0 && d1 >= 0 && d2 >= 0)
			{
				ResultStates[i] = 2;
				return;
			}

			// All vertices are underwater
			if (d0 <= 0 && d1 <= 0 && d2 <= 0)
			{
				ThreeUnderWater(P0, P1, P2, d0, d1, d2, 0, 1, 2, i);
			}
			// 1 or 2 vertices are below the water
			else
			{
				// v0 > v1
				if (d0 > d1)
				{
					// v0 > v2
					if (d0 > d2)
					{
						// v1 > v2
						if (d1 > d2)
						{
							if (d0 > 0 && d1 < 0 && d2 < 0)
							{
								// 0 1 2
								TwoUnderWater(P0, P1, P2, d0, d1, d2, 0, 1, 2, i);
							}
							else if (d0 > 0 && d1 > 0 && d2 < 0)
							{
								// 0 1 2
								OneUnderWater(P0, P1, P2, d0, d1, d2, 0, 1, 2, i);
							}
						}
						// v2 > v1
						else
						{
							if (d0 > 0 && d2 < 0 && d1 < 0)
							{
								// 0 2 1
								TwoUnderWater(P0, P2, P1, d0, d2, d1, 0, 2, 1, i);
							}
							else if (d0 > 0 && d2 > 0 && d1 < 0)
							{
								// 0 2 1
								OneUnderWater(P0, P2, P1, d0, d2, d1, 0, 2, 1, i);
							}
						}
					}
					// v2 > v0
					else
					{
						if (d2 > 0 && d0 < 0 && d1 < 0)
						{
							// 2 0 1
							TwoUnderWater(P2, P0, P1, d2, d0, d1, 2, 0, 1, i);
						}
						else if (d2 > 0 && d0 > 0 && d1 < 0)
						{
							// 2 0 1
							OneUnderWater(P2, P0, P1, d2, d0, d1, 2, 0, 1, i);
						}
					}
				}
				// v0 < v1
				else
				{
					// v0 < v2
					if (d0 < d2)
					{
						// v1 < v2
						if (d1 < d2)
						{
							if (d2 > 0 && d1 < 0 && d0 < 0)
							{
								// 2 1 0
								TwoUnderWater(P2, P1, P0, d2, d1, d0, 2, 1, 0, i);
							}
							else if (d2 > 0 && d1 > 0 && d0 < 0)
							{
								// 2 1 0
								OneUnderWater(P2, P1, P0, d2, d1, d0, 2, 1, 0, i);
							}
						}
						// v2 < v1
						else
						{
							if (d1 > 0 && d2 < 0 && d0 < 0)
							{
								// 1 2 0
								TwoUnderWater(P1, P2, P0, d1, d2, d0, 1, 2, 0, i);
							}
							else if (d1 > 0 && d2 > 0 && d0 < 0)
							{
								// 1 2 0
								OneUnderWater(P1, P2, P0, d1, d2, d0, 1, 2, 0, i);
							}
						};
					}
					// v2 < v0
					else
					{
						if (d1 > 0 && d0 < 0 && d2 < 0)
						{
							// 1 0 2
							TwoUnderWater(P1, P0, P2, d1, d0, d2, 1, 0, 2, i);
						}
						else if (d1 > 0 && d0 > 0 && d2 < 0)
						{
							// 1 0 2
							OneUnderWater(P1, P0, P2, d1, d0, d2, 1, 0, 2, i);
						}
					}
				}
			}
		}

	    private void CalculateForces(Vector3 p0, Vector3 p1, Vector3 p2,
			float dist0, float dist1, float dist2,
			int index, int i0, int i1, int i2,
			out Vector3 force, out Vector3 center, out float area, out float distanceToSurface)
        {
	        force.x = 0;
	        force.y = 0;
	        force.z = 0;

	        center.x = 0;
	        center.y = 0;
	        center.z = 0;
	        
	        area = 0;
	        distanceToSurface = 0;
	        
			center.x = (p0.x + p1.x + p2.x) / 3.0f;
			center.y = (p0.y + p1.y + p2.y) / 3.0f;
			center.z = (p0.z + p1.z + p2.z) / 3.0f;
			
			Vector3 u;
			u.x = p1.x - p0.x;
			u.y = p1.y - p0.y;
			u.z = p1.z - p0.z;
			
			Vector3 v;
			v.x = p2.x - p0.x;
			v.y = p2.y - p0.y;
			v.z = p2.z - p0.z;

			Vector3 crossUV;
			crossUV.x = u.y * v.z - u.z * v.y;
			crossUV.y = u.z * v.x - u.x * v.z;
			crossUV.z = u.x * v.y - u.y * v.x;
			
			float crossMagnitude = crossUV.x * crossUV.x + crossUV.y * crossUV.y + crossUV.z * crossUV.z;
			if (crossMagnitude < 0.0000001f)
			{
				ResultStates[index] = 2;
				return;
			}

			float invSqrtCrossMag = FastInvSqrt(crossMagnitude);
			crossMagnitude *= invSqrtCrossMag;

			Vector3 normal;
			normal.x = crossUV.x * invSqrtCrossMag;
			normal.y = crossUV.y * invSqrtCrossMag;
			normal.z = crossUV.z * invSqrtCrossMag;
			ResultNormals[index] = normal;

			Vector3 p;
			p.x = center.x - RigidbodyCoM.x;
			p.y = center.y - RigidbodyCoM.y;
			p.z = center.z - RigidbodyCoM.z;

			Vector3 crossAngVelP;
			crossAngVelP.x = RigidbodyAngVel.y * p.z - RigidbodyAngVel.z * p.y;
			crossAngVelP.y = RigidbodyAngVel.z * p.x - RigidbodyAngVel.x * p.z;
			crossAngVelP.z = RigidbodyAngVel.x * p.y - RigidbodyAngVel.y * p.x;
			
			Vector3 velocity;
			velocity.x = crossAngVelP.x + RigidbodyLinearVel.x;
			velocity.y = crossAngVelP.y + RigidbodyLinearVel.y;
			velocity.z = crossAngVelP.z + RigidbodyLinearVel.z;
			
			Vector3 waterNormalVector;
			waterNormalVector.x = _worldUpVector.x;
			waterNormalVector.y = _worldUpVector.y;
			waterNormalVector.z = _worldUpVector.z;

			area = crossMagnitude * 0.5f;
			distanceToSurface = 0.0f;
			if (area > 0.0000001f)
			{
				Vector3 f0;
				f0.x = p0.x - center.x;
				f0.y = p0.y - center.y;
				f0.z = p0.z - center.z;
				
				Vector3 f1;
				f1.x = p1.x  - center.x;
				f1.y = p1.y  - center.y;
				f1.z = p1.z  - center.z;
				
				Vector3 f2;
				f2.x  = p2.x  - center.x;
				f2.y  = p2.y  - center.y;
				f2.z  = p2.z  - center.z;
				
				Vector3 cross12;
				cross12.x = f1.y * f2.z - f1.z * f2.y;
				cross12.y = f1.z * f2.x - f1.x * f2.z;
				cross12.z = f1.x * f2.y - f1.y * f2.x;
				float magCross12 = cross12.x * cross12.x + cross12.y * cross12.y + cross12.z * cross12.z;
				magCross12 *= FastInvSqrt(magCross12);
				
				Vector3 cross20;
				cross20.x = f2.y * f0.z - f2.z * f0.y;
				cross20.y = f2.z * f0.x - f2.x * f0.z;
				cross20.z = f2.x * f0.y - f2.y * f0.x;
				float magCross20 = cross20.x * cross20.x + cross20.y * cross20.y + cross20.z * cross20.z;
				magCross20 *= FastInvSqrt(magCross20);

				float   invDoubleArea = 0.5f / area;
				float   w0 = magCross12 * invDoubleArea;
				float   w1 = magCross20 * invDoubleArea;
				float   w2 = 1.0f - (w0 + w1);
				
				if (calculateWaterNormals)
				{
					Vector3 n0 = WaterNormals[i0];
					Vector3 n1 = WaterNormals[i1];
					Vector3 n2 = WaterNormals[i2];

					float dot0 = n0.x * _worldUpVector.x + n0.y * _worldUpVector.y + n0.z * _worldUpVector.z;
					float dot1 = n1.x * _worldUpVector.x + n1.y * _worldUpVector.y + n1.z * _worldUpVector.z;
					float dot2 = n2.x * _worldUpVector.x + n2.y * _worldUpVector.y + n2.z * _worldUpVector.z;

					distanceToSurface =
						w0 * dist0 * dot0 +
						w1 * dist1 * dot1 +
						w2 * dist2 * dot2;

					waterNormalVector.x = w0 * n0.x + w1 * n1.x + w2 * n2.x;
					waterNormalVector.y = w0 * n0.y + w1 * n1.y + w2 * n2.y;
					waterNormalVector.z = w0 * n0.z + w1 * n1.z + w2 * n2.z;
				}
				else
				{
					distanceToSurface =
						w0 * dist0 +
						w1 * dist1 +
						w2 * dist2;
				}

				if (calculateWaterFlows)
				{
					Vector3 wf0 = WaterFlows[i0];
					Vector3 wf1 = WaterFlows[i1];
					Vector3 wf2 = WaterFlows[i2];
					
					velocity.x += w0 * -wf0.x + w1 * -wf1.x + w2 * -wf2.x;
					velocity.y += w0 * -wf0.y + w1 * -wf1.y + w2 * -wf2.y;
					velocity.z += w0 * -wf0.z + w1 * -wf1.z + w2 * -wf2.z;
				}
			}
			else
			{
				ResultStates[index] = 2;
				return;
			}

			float velocityMagnitude = velocity.x * velocity.x + velocity.y * velocity.y + velocity.z * velocity.z;
			float invSqrtVelMag = FastInvSqrt(velocityMagnitude);
			velocityMagnitude *= invSqrtVelMag;

			ResultVelocities[index] = velocity;
			ResultAreas[index] = area;

			distanceToSurface = distanceToSurface < 0 ? 0 : distanceToSurface;

			float densityArea = fluidDensity * area;

			Vector3 buoyantForce;
			if (buoyantForceCoefficient > 0.0001f)
			{
				float gravity = _gravity.y;
				float dotNormalWaterNormal = Vector3.Dot(normal, waterNormalVector);
				float bfc = distanceToSurface * densityArea * gravity * dotNormalWaterNormal * buoyantForceCoefficient;

				buoyantForce.x = waterNormalVector.x * bfc;
				buoyantForce.y = waterNormalVector.y * bfc;
				buoyantForce.z = waterNormalVector.z * bfc;
			}
			else
			{
				buoyantForce.x = 0;
				buoyantForce.y = 0;
				buoyantForce.z = 0;
			}
			

			Vector3 dynamicForce;
			dynamicForce.x = 0;
			dynamicForce.y = 0;
			dynamicForce.z = 0;
			
			if (velocityMagnitude > 0.0001f)
			{
				Vector3 velocityNormalized;
				velocityNormalized.x = velocity.x * invSqrtVelMag;
				velocityNormalized.y = velocity.y * invSqrtVelMag;
				velocityNormalized.z = velocity.z * invSqrtVelMag;
				
				float dotNormVel = Vector3.Dot(normal, velocityNormalized);
				
				if (hydrodynamicForceCoefficient > 0.001f)
				{
					if (velocityDotPower < 0.999f || velocityDotPower > 1.001f)
					{
						dotNormVel = (dotNormVel > 0f ? 1f : -1f) * Mathf.Pow(dotNormVel > 0 ? dotNormVel : -dotNormVel, velocityDotPower);
					}

					float c = -dotNormVel * velocityMagnitude * densityArea;
					dynamicForce.x = normal.x * c;
					dynamicForce.y = normal.y * c;
					dynamicForce.z = normal.z * c;
				}

				if (skinDragCoefficient > 0.0001f)
				{
					float absDot = dotNormVel < 0 ? -dotNormVel : dotNormVel;
					float c = -(1.0f - absDot) * skinDragCoefficient * densityArea;
					dynamicForce.x += velocity.x * c;
					dynamicForce.y += velocity.y * c;
					dynamicForce.z += velocity.z * c;
				}

				float dfc = hydrodynamicForceCoefficient * (dotNormVel > 0 ? slamForceCoefficient : suctionForceCoefficient);
				dynamicForce.x *= dfc;
				dynamicForce.y *= dfc;
				dynamicForce.z *= dfc;
			}

			force.x = buoyantForce.x + dynamicForce.x;
			force.y = buoyantForce.y + dynamicForce.y;
			force.z = buoyantForce.z + dynamicForce.z;
		}

	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe float FastInvSqrt (float x)
        {
	        float xhalf = 0.5f*x;
	        int i = *(int*)&x;
	        i = 0x5f375a86 - (i>>1);
	        x = *(float*)&i;
	        x = x*(1.5f - xhalf*x*x);
	        return x;
        }

        void ThreeUnderWater(Vector3 p0, Vector3 p1, Vector3 p2,
			float dist0, float dist1, float dist2,
			int i0, int i1, int i2, int index)
		{
			ResultStates[index] = 0;

			int i = index * 6;
			ResultP0s[i] = p0;
			ResultP0s[i + 1] = p1;
			ResultP0s[i + 2] = p2;

			Vector3 zeroVector;
			zeroVector.x = 0;
			zeroVector.y = 0;
			zeroVector.z = 0;
			
			ResultP0s[i + 3] = zeroVector;
			ResultP0s[i + 4] = zeroVector;
			ResultP0s[i + 5] = zeroVector;
			
			CalculateForces(p0, p1, p2, -dist0, -dist1, -dist2, index, i0, i1, i2, 
				out Vector3 f, out Vector3 p, out float a, out float d);
			ResultForces[index] = f;
			ResultCenters[index] = p;
			ResultAreas[index] = a;
			ResultDistances[index] = d;
		}

        void TwoUnderWater(Vector3 p0, Vector3 p1, Vector3 p2,
			float dist0, float dist1, float dist2,
			int i0, int i1, int i2, int index)
		{
			ResultStates[index] = 1;

			Vector3 H, M, L, IM, IL;
			float   hH, hM, hL;
			int     mIndex;

			// H is always at position 0
			H = p0;

			// Find the index of M
			mIndex = i0 - 1;
			if (mIndex < 0)
			{
				mIndex = 2;
			}

			// Heights to the water
			hH = dist0;

			if (i1 == mIndex)
			{
				M = p1;
				L = p2;

				hM = dist1;
				hL = dist2;
			}
			else
			{
				M = p2;
				L = p1;

				hM = dist2;
				hL = dist1;
			}

			float cIM = -hM / (hH - hM);
			IM.x = cIM * (H.x - M.x) + M.x;
			IM.y = cIM * (H.y - M.y) + M.y;
			IM.z = cIM * (H.z - M.z) + M.z;

			float cIL = -hL / (hH - hL);
			IL.x = cIL * (H.x - L.x) + L.x;
			IL.y = cIL * (H.y - L.y) + L.y;
			IL.z = cIL * (H.z - L.z) + L.z;

			int i = index * 6;
			ResultP0s[i] = M;
			ResultP0s[i + 1] = IM;
			ResultP0s[i + 2] = IL;

			ResultP0s[i + 3] = M;
			ResultP0s[i + 4] = IL;
			ResultP0s[i + 5] = L;
			
			CalculateForces(M, IM, IL, -hM, 0, 0, index, i0, i1, i2, 
				out Vector3 f0, out Vector3 c0, out float a0, out float dst0);
			CalculateForces(M, IL, L, -hM, 0, -hL, index, i0, i1, i2, 
				out Vector3 f1, out Vector3 c1, out float a1, out float dst1);

			float weight0 = a0 / (a0 + a1);
			float weight1 = 1 - weight0;

			Vector3 resultForce;
			resultForce.x = f0.x + f1.x;
			resultForce.y = f0.y + f1.y;
			resultForce.z = f0.z + f1.z;

			Vector3 resultCenter;
			resultCenter.x = c0.x * weight0 + c1.x * weight1;
			resultCenter.y = c0.y * weight0 + c1.y * weight1;
			resultCenter.z = c0.z * weight0 + c1.z * weight1;
			
			ResultForces[index] = resultForce;
			ResultCenters[index] = resultCenter;
			ResultDistances[index] = dst0 * weight0 + dst1 * weight1;
			ResultAreas[index] = a0 + a1;
		}

        void OneUnderWater(Vector3 p0, Vector3 p1, Vector3 p2,
			float dist0, float dist1, float dist2,
			int i0, int i1, int i2, int index)
		{
			ResultStates[index] = 1;

			Vector3 H, M, L, JM, JH;
			float   hH, hM, hL;

			L = p2;

			// Find the index of H
			int hIndex = i2 + 1;
			if (hIndex > 2)
			{
				hIndex = 0;
			}

			// Get heights to water
			hL = dist2;

			if (i1 == hIndex)
			{
				H = p1;
				M = p0;

				hH = dist1;
				hM = dist0;
			}
			else
			{
				H = p0;
				M = p1;

				hH = dist0;
				hM = dist1;
			}

			float cJM = -hL / (hM - hL);
			JM.x = cJM * (M.x - L.x) + L.x;
			JM.y = cJM * (M.y - L.y) + L.y;
			JM.z = cJM * (M.z - L.z) + L.z;
			
			float cJH = -hL / (hH - hL);
			JH.x =  cJH * (H.x - L.x) + L.x;
			JH.y =  cJH * (H.y - L.y) + L.y;
			JH.z =  cJH * (H.z - L.z) + L.z;

			int i = index * 6;
			ResultP0s[i] = L;
			ResultP0s[i + 1] = JH;
			ResultP0s[i + 2] = JM;

			Vector3 zeroVector;
			zeroVector.x = 0;
			zeroVector.y = 0;
			zeroVector.z = 0;
			
			ResultP0s[i + 3] = zeroVector;
			ResultP0s[i + 4] = zeroVector;
			ResultP0s[i + 5] = zeroVector;

			// Generate trisPtr
			CalculateForces(L, JH, JM, -hL, 0, 0, index, i0, i1, i2, 
				out Vector3 f, out Vector3 p, out float a, out float d);
			ResultForces[index] = f;
			ResultCenters[index] = p;
			ResultAreas[index] = a;
			ResultDistances[index] = d;
		}

        public float GetWaterHeightSingle(Vector3 point)
		{
			return CurrentWaterDataProvider == null ? defaultWaterHeight : CurrentWaterDataProvider.GetWaterHeightSingle(this, point);
		}

        /// <summary>
        ///     Returns true if object is touching water or false if it is not.
        ///     It is recommended to cache the result.
        /// </summary>
        public bool IsTouchingWater()
        {
            for (int i = 0; i < ResultStates.Length; i++)
            {
                int state = ResultStates[i];
                if (state <= 1)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        ///     Swaps original mesh with simulation mesh on MeshFilter for in-scene preview.
        /// </summary>
        public void StartSimMeshPreview()
        {
            if (PreviewEnabled)
            {
                return;
            }

            if (_meshFilter == null)
            {
	            _meshFilter = GetComponent<MeshFilter>();
            }

            if (simulationMesh == null)
            {
                Debug.LogError("Could not start simulation mesh preview. Simulation mesh is null.");
                return;
            }

            if (originalMesh == null)
            {
                Debug.LogError("Could not start simulation mesh preview. Original mesh is null.");
                return;
            }

            if (simulationMesh != null && _meshFilter != null)
            {
                _meshFilter.mesh = simulationMesh;
            }
        }


        /// <summary>
        ///     Generates simulation mesh according to the settings
        /// </summary>
        public void GenerateSimMesh()
        {
	        originalMesh = GetComponent<MeshFilter>()?.sharedMesh;
			Debug.Assert(originalMesh != null, "Mesh or MeshFilter are null.");
	        
            bool previewWasEnabled = false;

            if (PreviewEnabled)
            {
                StopSimMeshPreview();
                previewWasEnabled = true;
            }

            if (!PreviewEnabled)
            {
                if (simulationMesh == null)
                {
                    simulationMesh = new Mesh
                    {
                        name = "DWP_SIM_MESH",
                    };
                }

                if (simplifyMesh)
                {
                    _simplificationRatio = (targetTriangleCount * 3f + 16) / originalMesh.triangles.Length;
                    if (_simplificationRatio >= 1f && !convexifyMesh)
                    {
                        Debug.Log("Target tri count larger than the original tri count. Nothing to simplify.");
                    }

                    _simplificationRatio = Mathf.Clamp(_simplificationRatio, 0f, 1f);
                }

                if (!simplifyMesh && !convexifyMesh && !weldColocatedVertices)
                {
                    simulationMesh = MeshUtility.GenerateMesh(originalMesh.vertices, originalMesh.triangles);
                }
                else
                {
                    MeshUtility.GenerateSimMesh(ref originalMesh, ref simulationMesh, simplifyMesh, convexifyMesh,
                                                  weldColocatedVertices, _simplificationRatio);
                }
                
                serializedSimulationMesh.Serialize(simulationMesh);
            }
            else
            {
                Debug.LogError("Cannot generate simulation mesh while preview is enabled.");
            }

            if (previewWasEnabled)
            {
                StartSimMeshPreview();
            }
        }


        /// <summary>
        ///     Swaps simulation mesh on MeshFilter with original mesh.
        /// </summary>
        public void StopSimMeshPreview()
        {
            if (!PreviewEnabled)
            {
                return;
            }

            if (_meshFilter == null || originalMesh == null)
            {
                Debug.LogError($"Cannot stop sim mesh preview on object {name}. MeshFilter or original mesh is null.");
                return;
            }

            if (originalMesh != null)
            {
                _meshFilter.sharedMesh = originalMesh;
            }
            else
            {
                Debug.LogError($"Original mesh on object {name} could not be found. !BUG!");
            }
        }


        /// <summary>
        ///     Shows setup error message.
        /// </summary>
        private void ShowErrorMessage()
        {
            Debug.LogWarning($"One or more setup errors have been found. WaterObject {name} will not be " +
                             "simulated until these are fixed. Check manual for more details on WaterObject setup.");
        }
    }
}