using System.Collections.Generic;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Rope
{
    public enum SegmentSelectionMode
    {
        RoundRobin,
        Random
    }
    public enum LineOverflowMode
    {
        Round,
        Shrink,
        Extend
    }
    [ExecuteInEditMode]
    [RequireComponent(typeof(CurvedLineRenderer), typeof(SpringJoint2D))]
    public class Rope : MonoBehaviour
    {
        public SpriteRenderer[] SegmentsPrefabs;
        public SegmentSelectionMode SegmentsMode;
        public LineOverflowMode OverflowMode;
        [HideInInspector]
        public bool useBendLimit = true;
        [HideInInspector]
        public int bendLimit = 45;
        //[HideInInspector]
        //public bool HangFirstSegment = false;
        [HideInInspector]
        public Vector2 FirstSegmentConnectionAnchor;
        [HideInInspector]
        public Vector2 LastSegmentConnectionAnchor;
        //[HideInInspector]
        //public bool HangLastSegment = false;
#if UNITY_5_3_OR_NEWER
        [HideInInspector]
        public bool BreakableJoints = false;
        [HideInInspector]
        public float BreakForce = 100;
#endif
        [Range(-0.5f, 0.5f)]
        public float overlapFactor;
        public List<Vector3> nodes = new List<Vector3>(new Vector3[] { new Vector3(-3, 0, 0), new Vector3(3, 0, 0) });
        [HideInInspector]
        public bool EnablePhysics = true;

        [Header("Springs")]
        public bool UseSpringJoints = true;
        public bool autoConfigureDistance = false;
        public float distance = 0.0001f;
        public float dampingRatio = 1f;
        public float frequency = 10f;


        [Header("")]
        public bool Use2DCollider = true;

        //SpringRope
        private LineRenderer _lineRenderer;
        private SpringJoint2D _joint;

        [Header("Stretch")]
        public AnimationCurve ColorAnimationCurve;
        public Color NormalColor = Color.white;
        public Color StrechedColor = new Color(0.8f, 0.2f, 0.2f);
        public float MaxStrechValue = 1.5f;

        public float _radius; //радиус центральной пружины

        [HideInInspector][SerializeField] public Rigidbody2D AttachedObject;

        private CurvedLineRenderer _curvedLineRenderer;

        [HideInInspector]
        public bool IsCut = false;

        private void Awake()
        {
            _curvedLineRenderer = GetComponent<CurvedLineRenderer>();
            _lineRenderer = GetComponent<LineRenderer>();
            _joint = GetComponent<SpringJoint2D>();
            //AttachObject(AttachedObject);
        }

        private void Start()
        {
            RopeUtil.UpdateRope(this);
        }

        public void AttachObject(Rigidbody2D attachedObject)
        {
            print("AttachObject(" + attachedObject + ")");
            if (attachedObject == null) Detach();
            AttachedObject = attachedObject;
            _joint.connectedBody = attachedObject;
            _joint.distance = GetRopeLength();
            //_joint.distance = RopeUtil.GetRopeLength(this);
            RopeUtil.UpdateRope(this);
            //AttachToEnd(attachedObject);
            _curvedLineRenderer.CustomLastPoint = attachedObject.transform;
            UpdateLength();
        }

        private float GetRopeLength()
        {
            float len = 0f;

            for (int i = 0; i + 1 < nodes.Count; i++)
            {
                len += Vector3.Distance(nodes[i], nodes[i + 1]);
            }

            var lastNodeGlobal = transform.TransformPoint(nodes[nodes.Count - 1]);
            if (gameObject.name != "StretchedRope")
            {
                len += Vector3.Distance(AttachedObject.position, lastNodeGlobal);
            }

            return len;
        }

        public void AttachToEnd(Rigidbody2D food)
        {
            /*if (!UseSpringJoints)
            {
                Transform lastSegment = transform.GetChild(transform.childCount - 1);
                HingeJoint2D[] joints = lastSegment.gameObject.GetComponents<HingeJoint2D>();
                HingeJoint2D joint = null;
                if (joints.Length > 1)
                    joint = joints[1];
                else
                    joint = lastSegment.gameObject.AddComponent<HingeJoint2D>();

                joint.connectedAnchor = Vector2.zero;
                joint.connectedBody = food;
            }
            else*/
            {
                Transform lastSegment = transform.GetChild(transform.childCount - 1);
                SpringJoint2D[] joints = lastSegment.gameObject.GetComponents<SpringJoint2D>();
                SpringJoint2D spring = null;
                if (joints.Length > 1)
                    spring = joints[1];
                else
                    spring = lastSegment.gameObject.AddComponent<SpringJoint2D>();

                spring.autoConfigureDistance = autoConfigureDistance;
                spring.distance = distance;
                spring.frequency = frequency;
                spring.dampingRatio = dampingRatio;
                spring.anchor = Vector2.zero;
                spring.connectedAnchor = Vector2.zero;
                spring.connectedBody = food;
            }
        }

        public void Detach()
        {
            print("Detach()");
            AttachedObject = null;
            _joint.connectedBody = null;
            _curvedLineRenderer.CustomLastPoint = null;
            RopeUtil.DetachLast(this);
        }

        public void FixedUpdate()
        {
            UpdateCollider();
            UpdateSpring();
        }

        private void UpdateSpring()
        {
            if (IsCut)
            {
                _joint.enabled = false;
                return;
            }

            if (AttachedObject == null) return;

            
            float dist = Vector2.Distance(transform.position, AttachedObject.position);
            if (dist >= _radius)
            {
                _joint.enabled = true;
                float mappedValue = (dist - _radius) / (_radius * MaxStrechValue - _radius);
                mappedValue = ColorAnimationCurve.Evaluate(mappedValue);
                var color = Color.Lerp(NormalColor, StrechedColor, mappedValue);
                _lineRenderer.endColor = color;
                _lineRenderer.startColor = color;
            }

            else
            {
                _joint.enabled = false;


                _lineRenderer.endColor = NormalColor;
                _lineRenderer.startColor = NormalColor;
            }
        }

        private void UpdateCollider()
        {
            if (!Use2DCollider) return;

            var edgeCollider2D = GetComponent<EdgeCollider2D>();

            if (edgeCollider2D == null)
            {
                edgeCollider2D = gameObject.AddComponent<EdgeCollider2D>();
            }

            var linePositions = _curvedLineRenderer.linePositions;
            var points = new Vector2[linePositions.Length];
            for (int i = 0; i < linePositions.Length; i++)
            {
                points[i] = linePositions[i] - transform.position;
            }
            edgeCollider2D.points = points;

        }

        public void InitVisuals()
        {
            if (_curvedLineRenderer.SetPointsAutomatically)
            {
                _curvedLineRenderer.SetAutoPoints();
            }
        }

        public void Cut(Vector2 pos)
        {
            if (IsCut) return;

            //Get closest segment
            Transform closestSegment = transform.GetChild(0);
            float minDistance = Vector2.Distance(closestSegment.position, pos);
            foreach (Transform child in transform)
            {
                float dist = Vector2.Distance(child.position, pos);

                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestSegment = child;
                }

                //child.GetComponent<Collider2D>().enabled = false; не отключать коллайдер а изменить слой на но селф коллижн
            }
            GetComponent<EdgeCollider2D>().enabled = false;


            //Cut it
            closestSegment.GetComponent<Joint2D>().enabled = false;
            IsCut = true;
            Use2DCollider = false;

            //Add new linerenderer
            _curvedLineRenderer.Cut(closestSegment);

        }

        public void UpdateLength()
        {
            if (AttachedObject != null)
            {
                _radius = GetRopeLength();/*Vector2.Distance(transform.position, AttachedObject.position);*///RopeUtil.GetRopeLength(this);
                _joint.distance = GetRopeLength(); //_radius;
            }
        }
    }
}