using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.UiScripts
{
    public class ResultScreen : MonoBehaviour {

        public void OnRestartClick()
        {
            LoadingScreenManager.ReloadScene();
            gameObject.SetActive(false);
        }

        public void OnNextLevelClick()
        {
            LoadingScreenManager.NextLevel();
        }

        public void OnMenuClick()
        {
            LoadingScreenManager.LoadScene("Menu");
        }
    }
}
