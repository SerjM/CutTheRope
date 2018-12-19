using System;
using System.Collections;
using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Rope
{
    public class AutoRopeTrigger : MonoBehaviour
    {

        public string[] Tags = { "Food" };

        public GameObject RopePrefab;


        private void OnTriggerEnter2D(Collider2D other)
        {
            int pos = Array.IndexOf(Tags, other.tag);
            if (pos > -1)
            {
                AttachRope(other);
            }
        }

        private void AttachRope(Collider2D food)
        {
            Rope rope = SpawnRope();
            var circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.enabled = false;
            rope.nodes[0] = Vector3.zero;
            rope.nodes[rope.nodes.Count - 1] = food.transform.position - transform.position;

            //rope.HangFirstSegment = true; //иногда обе приклеиваются к еде
            //rope.HangLastSegment = false;
            //rope.GetComponent<SpringJoint2D>().distance = circleCollider.radius;
            //rope._radius = circleCollider.radius;

            AudioManager.PlaySoundEffect("Auto rope attach");

            StartCoroutine(AttachRopeNextFrame(rope, food));
        }

        private IEnumerator AttachRopeNextFrame(Rope rope, Collider2D food)
        {
            yield return null; //Ждем один кадр чтобы rope успела раздуплиться
            rope.AttachObject(food.attachedRigidbody);
            rope.transform.GetChild(0).GetComponent<HingeJoint2D>().connectedBody = null;

            RopeUtil.UpdateRope(rope);
        }

        private Rope SpawnRope()
        {
            //print("Auto rope spawned");
            var ropeGo = Instantiate(RopePrefab, transform.position, Quaternion.identity);
            var ropeScript = ropeGo.GetComponent<Rope>();
            return ropeScript;
        }
    }
}
