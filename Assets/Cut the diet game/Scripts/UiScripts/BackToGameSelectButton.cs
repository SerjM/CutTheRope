using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.UiScripts
{
    public class BackToGameSelectButton : MonoBehaviour {


        public void OnClick () {

            LoadingScreenManager.LoadScene("GameSelect");

        }
	
    }
}
