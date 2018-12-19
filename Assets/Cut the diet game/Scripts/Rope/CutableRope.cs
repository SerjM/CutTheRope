using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Rope
{
    public class CutableRope : MonoBehaviour
    {

        public GameObject[] Segments;
        public Collider2D[] SegmentsColliders;
        public Animator[] SegmentsAnimators;

        public void Cut(Transform cutterHit)
        {
            cutterHit.GetComponent<Joint2D>().enabled = false;
            cutterHit.GetComponent<Animator>().SetTrigger("Out");

            StartCoroutine(AnimateRopeDisaperar(cutterHit));
        }

        //Анимирует исчезание веревки в прозрачность поэлемнтно от отчки разреза
        private IEnumerator AnimateRopeDisaperar(Transform cutterHit)
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<Animator>().SetTrigger("Out");
            }
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }

        private void AnimateDisaperar(ref List<Transform> list)
        {
            if (list.Count == 0) return;
            var el = list[0];
            el.GetComponent<Animator>().SetTrigger("Out");
            list.RemoveAt(0);
        }

        void Start () {

        }
    }
}
