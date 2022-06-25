using UnityEngine;

namespace NWH.DWP2.ShipController
{
    /// <summary>
    ///     Approximates the behavior of a real anchor by keeping the object near the anchored position, but
    ///     allowing for some movement.
    /// </summary>
    public class Anchor : MonoBehaviour
    {
        /// <summary>
        ///     Should the anchor be dropped at start?
        /// </summary>
        [Tooltip("Should the anchor be dropped at start?")]
        public bool dropOnStart = true;

        /// <summary>
        ///     Coefficient by which the force will be multiplied when the object starts pulling on the anchor.
        /// </summary>
        [Tooltip("Coefficient by which the force will be multiplied when the object starts pulling on the anchor.")]
        public float forceCoefficient = 10f;

        /// <summary>
        ///     Radius around anchor in which the chain/rope is slack and in which no force will be applied.
        /// </summary>
        [Tooltip("Radius around anchor in which the chain/rope is slack and in which no force will be applied.")]
        public float zeroForceRadius = 2f;

        /// <summary>
        ///     Maximum force that can be applied to anchor before it starts to drag.
        /// </summary>
        [Tooltip("Maximum force that can be applied to anchor before it starts to drag.")]
        public float dragForce = 500f;

        /// <summary>
        ///     Point in coordinates local to the object this script is attached to.
        /// </summary>
        [Tooltip("Point in coordinates local to the object this script is attached to.")]
        public Vector3 localAnchorPoint = Vector3.zero;

        private Vector3 _force;
        private Vector3 _distance;
        private Vector3 _prevDistance;

        /// <summary>
        ///     Rigidbody to which the force will be applied.
        /// </summary>
        public Rigidbody ParentRigidbody { get; private set; }

        public Vector3 AnchorPoint
        {
            get { return transform.TransformPoint(localAnchorPoint); }
        }

        /// <summary>
        ///     Position of the anchor.
        /// </summary>
        public Vector3 AnchorPosition { get; set; }

        /// <summary>
        ///     Has the anchor been dropped?
        /// </summary>
        public bool Dropped { get; private set; }

        /// <summary>
        ///     Is the anchor dragging on the floor?
        /// </summary>
        public bool IsDragging { get; private set; }


        private void Start()
        {
            ParentRigidbody = GetComponentInParent<Rigidbody>();
            if (ParentRigidbody == null)
            {
                Debug.LogError(
                    $"No rigidbody found on object {name} or its parents. Anchor script needs rigidbody to function.");
            }

            if (dropOnStart)
            {
                Drop();
            }
        }


        private void FixedUpdate()
        {
            if (!Dropped)
            {
                return;
            }

            _prevDistance = _distance;
            _distance     = AnchorPoint - AnchorPosition;
            _distance.y   = 0;
            float distMag = _distance.magnitude - zeroForceRadius;
            if (distMag < 0)
            {
                return;
            }

            _force = distMag * distMag * 100f * forceCoefficient * -_distance.normalized;

            IsDragging = false;
            if (_force.magnitude > dragForce)
            {
                IsDragging     =  true;
                _force         =  _force.normalized * dragForce;
                AnchorPosition += _distance - _prevDistance;
            }

            ParentRigidbody.AddForceAtPosition(_force, AnchorPoint);
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(AnchorPoint, 0.2f);

            if (Dropped)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(AnchorPoint, AnchorPosition);
            }
        }


        /// <summary>
        ///     Drops the anchor. Opposite of Weigh()
        /// </summary>
        public void Drop()
        {
            if (Dropped)
            {
                return;
            }

            Dropped        = true;
            AnchorPosition = AnchorPoint;
        }


        /// <summary>
        ///     Weighs (retracts) the anchor.
        /// </summary>
        public void Weigh()
        {
            if (!Dropped)
            {
                return;
            }

            Dropped = false;
        }
    }
}