using Cut_the_diet_game.Scripts.Rope;
using UnityEngine;

namespace CurvedLine_2016_03_11.CurvedLine
{
    public class CurvedLinePoint : MonoBehaviour 
    {
        [HideInInspector] public bool showGizmo = true;
        [HideInInspector] public float gizmoSize = 0.1f;
        [HideInInspector] public Color gizmoColor = new Color(1,0,0,0.5f);
        private bool _cutPart;

        public bool CutPart
        {
            get
            {
                return _cutPart;
            }
            set
            {
                _cutPart = value;
            }
        }

        void OnDrawGizmos()
        {
            if( showGizmo == true )
            {
                Gizmos.color = gizmoColor;

                Gizmos.DrawSphere( this.transform.position, gizmoSize );
            }
        }

        //update parent line when this point moved
        void OnDrawGizmosSelected()
        {
            CurvedLineRenderer curvedLine = this.transform.parent.GetComponent<CurvedLineRenderer>();

            if( curvedLine != null )
            {
                curvedLine.Update();
            }
        }
    }
}
