using System.Linq;
using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Level
{
    public class LevelBorder : MonoBehaviour {

        private static readonly string[] _tags = { "Food" };
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_tags.Contains(collision.tag))
            {
                LevelManager.Instance.OnLose();
                AudioManager.PlaySoundEffect("Lose");
         
            }
        }


    }
}
