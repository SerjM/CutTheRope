using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.UiScripts
{
    public class GameMenuButton : MonoBehaviour
    {
        public string GameScene;

        public void OnClick()
        {
            LoadingScreenManager.LoadScene(GameScene);
        }
    }
}
