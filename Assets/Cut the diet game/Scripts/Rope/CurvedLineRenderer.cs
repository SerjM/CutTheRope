using System.Linq;
using CurvedLine_2016_03_11.CurvedLine;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Rope
{
    [RequireComponent(typeof(LineRenderer))]
    public class CurvedLineRenderer : MonoBehaviour
    {
        public GameObject CutPrefab;
        
        //PUBLIC
        public float lineSegmentSize = 0.15f;
        public float lineWidth = 0.1f;
        public Transform CustomLastPoint;
        [Header("Gizmos")]
        public bool showGizmos = true;
        public float gizmoSize = 0.1f;
        public Color gizmoColor = new Color(1, 0, 0, 0.5f);
        //PRIVATE
        public CurvedLinePoint[] linePoints = new CurvedLinePoint[0];
        public CurvedLinePoint[] linePointsCutHalf = new CurvedLinePoint[0];
        public Vector3[] linePositions = new Vector3[0];
        public Vector3[] linePositionsCutHalf = new Vector3[0];
        private Vector3[] linePositionsOld = new Vector3[0];
        private Vector3[] linePositionsOldCutHalf = new Vector3[0];
        private Rope _rope;
        private LineRenderer _line;
        private LineRenderer _lineCutHalf; //второй рендерер для отрезаного куска веревки
        [HideInInspector]
        public bool SetPointsAutomatically;
        [HideInInspector]
        [Range(0f, 1f)] public float AutoPointsFrequency = 1f;

        public void SetAutoPoints()
        {
            //Destroy existing
            DeleteAutoPoints();

            //Add new
            if (AutoPointsFrequency == 0f) return;

            float n = 1f;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (n >= 1f || i + 1 == transform.childCount)
                {
                    n -= 1f;
                    transform.GetChild(i).gameObject.AddComponent<CurvedLinePoint>();
                }

                n += AutoPointsFrequency;
            }
        }

        public void DeleteAutoPoints()
        {
            var points = GetComponentsInChildren<CurvedLinePoint>();
            foreach (var p in points)
            {
                DestroyImmediate(p);
            }
        }

        // Update is called once per frame
        public void Update()
        {
            _rope = GetComponent<Rope>();
            _line = GetComponent<LineRenderer>();
            GetPoints();
            SetPointsToLine();
            //SetTexture();
        }

        void GetPoints()
        {
            if (_rope == null) return;
            linePoints = GetComponentsInChildren<CurvedLinePoint>().Where(x => !x.CutPart).ToArray();
            PointsToPositions(linePoints, out linePositions);
            if (_rope.IsCut)
            {
                linePointsCutHalf = GetComponentsInChildren<CurvedLinePoint>().Where(x => x.CutPart).ToArray();
                PointsToPositions(linePointsCutHalf, out linePositionsCutHalf);

                if (CustomLastPoint != null)
                {
                    if (linePointsCutHalf.Length == 0) return;
                    linePositionsCutHalf[linePositionsCutHalf.Length - 1] = CustomLastPoint.transform.position;
                }
                return;
            }
            if (CustomLastPoint != null)
            {
                linePositions[linePositions.Length - 1] = CustomLastPoint.transform.position;
            }
        }

        private void PointsToPositions(CurvedLinePoint[] linePoints, out Vector3[] linePositions)
        {
            if (linePoints.Length == 00)
            {
                linePositions = new Vector3[1];
                linePositions[0] = transform.position;
                return;
            }
            if (linePoints.Length == 1)
            {
                linePositions = new Vector3[2];
                var tr = linePoints[0].transform;
                linePositions[0] = tr.position + tr.up * tr.lossyScale.y / 2f;
                linePositions[1] = tr.position - tr.up * tr.lossyScale.y / 2f;
                return;
            }

            linePositions = new Vector3[linePoints.Length + 1];
            for (int i = 0; i < linePoints.Length; i++)
            {
                linePositions[i] = linePoints[i].transform.position;
            }


            var firstTransform = linePoints[0].transform;
            linePositions[0] = firstTransform.position - firstTransform.up * firstTransform.lossyScale.y / 2f;

            var lastTransform = linePoints[linePoints.Length - 1].transform;
            linePositions[linePositions.Length - 1] =
                lastTransform.position + lastTransform.up * lastTransform.localScale.y / 2f;

        }

        void SetPointsToLine()
        {
            SetPointsToLine(linePositionsOld, linePositions, _line);
            if (_rope.IsCut)
            {
                SetPointsToLine(linePositionsOldCutHalf, linePositionsCutHalf, _lineCutHalf);
            }
        }

        void SetPointsToLine(Vector3[] linePositionsOld, Vector3[] linePositions, LineRenderer _line)
        {
            //create old positions if they dont match
            if (linePositionsOld.Length != linePositions.Length)
            {
                linePositionsOld = new Vector3[linePositions.Length];
            }

            //check if line points have moved
            bool moved = false;
            for (int i = 0; i < linePositions.Length; i++)
            {
                //compare
                if (linePositions[i] != linePositionsOld[i])
                {
                    moved = true;
                }
            }

            //update if moved
            if (moved == true)
            {
                if (_line == null)
                    _line = GetComponent<LineRenderer>();
                //get smoothed values
                Vector3[] smoothedPoints = LineSmoother.SmoothLine(linePositions, lineSegmentSize);

                //set line settings

                _line.positionCount = (smoothedPoints.Length);
                _line.SetPositions(smoothedPoints);
                _line.startWidth = (lineWidth);
                _line.endWidth = (lineWidth);
            }
        }

        void OnDrawGizmosSelected()
        {
            Update();
        }

        void OnDrawGizmos()
        {
            if (linePoints == null || linePoints.Length == 0)
            {
                GetPoints();
            }

            //settings for gizmos
            foreach (CurvedLinePoint linePoint in linePoints)
            {
                if (linePoint == null) continue;
                linePoint.showGizmo = showGizmos;
                linePoint.gizmoSize = gizmoSize;
                linePoint.gizmoColor = gizmoColor;
            }
        }

        public void Cut(Transform cutTransform)
        {
            GetComponent<Animator>().SetTrigger("Cut");
            //каждый сегмет веревки - теперь часть лайна
            SetPointsAutomatically = true;
            AutoPointsFrequency = 1f;
            SetAutoPoints();
            SetPointsToLine();

            bool find = false;
            foreach (Transform child in transform)
            {
                if (child == cutTransform)
                {
                    find = true;
                }
                //часть веревки ДО cutTransform помечаем как true, остальные как false
                child.GetComponent<CurvedLinePoint>().CutPart = find;
            }
            //var newseg = Instantiate(cutTransform.gameObject, cutTransform.position, cutTransform.rotation, transform);
            //newseg.GetComponent<Joint2D>().enabled = true;
            //newseg.GetComponent<CurvedLinePoint>().CutPart = false;

            var newGo = Instantiate(CutPrefab);
            _lineCutHalf = newGo.GetComponent<LineRenderer>();
            Destroy(newGo, 2f);
            Destroy(gameObject, 2f);
        }
    }
}
