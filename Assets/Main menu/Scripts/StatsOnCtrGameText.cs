using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Main_menu.Scripts
{
    public class StatsOnCtrGameText : MonoBehaviour {

        // Use this for initialization
        void Start ()
        {
            var text = GetComponent<Text>();
            int stars = ProgressManager.GetStarsOnFirstGame();
            text.text = stars + "/60"; //TODO make it dynamic
        }
    }
}
