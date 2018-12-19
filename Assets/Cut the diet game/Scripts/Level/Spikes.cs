using System.Linq;
using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Level
{
    public class Spikes : MonoBehaviour {

        private static readonly string[] _tags = { "Food" };
        [SerializeField] private GameObject _shards;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_tags.Contains(collision.tag))
            {
                LevelManager.Instance.OnLose();
                AudioManager.PlaySoundEffect("Lose");
                ShardsBlast(collision.gameObject);
            }
        }

        private void ShardsBlast(GameObject food)
        {
            GameObject temporal = Instantiate(_shards, transform);
            temporal.transform.position = food.transform.position;
            Destroy(food);
        }
    }
}
