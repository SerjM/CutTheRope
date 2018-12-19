using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Level
{
    public class Food : MonoBehaviour
    {
        public bool IsInBubble = false;
        public GameObject DestroyParticle;
        public static Food Instance;
        private Animator _animator;
      
       private Bubble[] bubbles;
       

        // Use this for initialization
        void Start ()
        {
            Instance = this;
            _animator = GetComponent<Animator>();
            try { bubbles = FindObjectsOfType<Bubble>(); } catch (System.Exception) { }
        }

        public void Eat()
        {
           
            _animator.SetTrigger("Eat");
        }

        public void Die()
        {
        
            Destroy(Instantiate(DestroyParticle, transform.position, transform.rotation), 2f);
            Destroy(gameObject);
            LevelManager.Instance.OnLose();
        }

        private void InBubble(Vector2 pos)
        {
            if (bubbles.Length == 0 || bubbles==null)
            {
            return;
            }


            
            for (int i = 0; i < bubbles.Length; i++)
            {
                if (bubbles[i] != null)
                {
                    Vector2 boundsMin = Camera.main.WorldToScreenPoint(bubbles[i].gameObject.GetComponent<CircleCollider2D>().bounds.min);
                    Vector2 boundsMax = Camera.main.WorldToScreenPoint(bubbles[i].gameObject.GetComponent<CircleCollider2D>().bounds.max);
                    if (
                       boundsMin.x <= pos.x && boundsMax.x >= pos.x
                       && boundsMin.y <= pos.y && boundsMax.y >= pos.y
                      )
                    {
                        if (bubbles[i].Catched)
                        {
                            bubbles[i].Pop();
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            InBubble(Camera.main.WorldToScreenPoint(gameObject.transform.position));
        }

    }
}
