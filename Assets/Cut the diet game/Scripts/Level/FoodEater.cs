using System.Collections;
using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Level
{
    public class FoodEater : MonoBehaviour
    {
        public float EatSpeed = 5f;

        private void OnTriggerEnter2D(Collider2D food)
        {

            if (!food.CompareTag("Food")) return;
            food.enabled = false;
            StartCoroutine(EatAnimation(food.attachedRigidbody));
            Invoke("End", 1f);
            
        }

        private void End()
        {
            LevelManager.Instance.OnFoodAte();
        }

        private IEnumerator EatAnimation(Rigidbody2D rb)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            Food.Instance.Eat();
            while (Vector2.Distance(rb.position, transform.position) > 0.1f)
            {
                rb.position = Vector2.Lerp(rb.position, transform.position, EatSpeed * Time.deltaTime);
                yield return null;
            }
            AudioManager.PlaySoundEffect("Eat");
            yield return null;
            Destroy(rb.gameObject);
            yield break;
        }
    }
}
