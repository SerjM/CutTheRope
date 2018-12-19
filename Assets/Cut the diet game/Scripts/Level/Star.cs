using System.Linq;
using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Level
{
    public class Star : MonoBehaviour
    {
        public GameObject PickUpParticle;
        public AudioClip PickUpSound;

        private static readonly string[] _tags = {"Food", "Player"};

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_tags.Contains(collision.tag))
            {
                GetComponent<Collider2D>().enabled = false;
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponentInChildren<SpriteRenderer>().enabled = false;
                LevelManager.Instance.PickUpStar();
                Destroy(Instantiate(PickUpParticle, transform.position, transform.rotation), 2f);
                float pitch = 1f + LevelManager.Instance.Apples * 0.5f;
                AudioManager.PlaySoundEffect("Star pick up", pitch);
                Destroy(gameObject);
            }
        }



    }
}
