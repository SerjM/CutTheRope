using UnityEngine;

namespace Cut_the_diet_game.Scripts.Rope
{
    [ExecuteInEditMode]
    public class AttachRopeTo : MonoBehaviour
    {
        private Rope _rope;
        public Transform Target;
        public Vector3 offset;
    

        // Update is called once per frame
        void Update () {
            if (!Application.isPlaying)
            {
                if (_rope == null)
                {
                    _rope = GetComponent<Rope>();
                }

                if (Target != null)
                {
                    _rope.nodes[0] = Vector3.zero;
                    _rope.nodes[_rope.nodes.Count - 1] = Target.position - transform.position;
                }
            }
        }
    }
}
