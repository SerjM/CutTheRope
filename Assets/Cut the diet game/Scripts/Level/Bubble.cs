using System.Linq;
using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Level
{
    public class Bubble : MonoBehaviour
    {
        public static float MaxVelocityMagnutude = 2f;
        public static float FlyUpLerp = 3f;
        public GameObject PopParticle;

        public bool Catched { get; private set; }

        private static readonly string[] _tags = { "Food" };

        private Rigidbody2D _catchedRigidbody;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_tags.Contains(collision.tag))
            {
                var food = collision.GetComponent<Food>();
                if (food.IsInBubble)
                {
                    //Pop();
                }
                else
                {
                    food.IsInBubble = true;
                    Catch(collision.attachedRigidbody);
                }
            }
        }

        private void Catch(Rigidbody2D rb)
        {
            _catchedRigidbody = rb;
            _catchedRigidbody.gravityScale = 0f;
            Catched = true;
        }

        private void FixedUpdate()
        {
        
            if (Catched)
            {
                if (_catchedRigidbody == null)
                {
                    return;
                }
                var rb = GetComponent<Rigidbody2D>();
                rb.position = Vector2.Lerp(rb.position, _catchedRigidbody.position, 15f * Time.fixedDeltaTime);
                _catchedRigidbody.velocity = Vector2.Lerp(_catchedRigidbody.velocity,
                    new Vector2(0, MaxVelocityMagnutude), FlyUpLerp * Time.fixedDeltaTime);
            }
        }
        /// <summary>
        /// Взорвать шарик
        /// </summary>
        public void Pop()
        {
            _catchedRigidbody.GetComponent<Food>().IsInBubble = false;
            Destroy(Instantiate(PopParticle, transform.position, transform.rotation), 2f);
            _catchedRigidbody.gravityScale = 2f;
            AudioManager.PlaySoundEffect("Bubble pop");
            Destroy(gameObject);
        }
    }
}
