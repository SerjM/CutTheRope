using UnityEngine;
using UnityEngine.UI;

namespace Cut_the_diet_game.Scripts.UiScripts
{
    public class LevelUi : MonoBehaviour
    {
        public Text LevelNtext;
        public Image[] Stars;
        public Button Button;

        public void Draw(int levelN, int stars)
        {
            for (int i = 0; i < 3; i++)
            {
                Stars[i].enabled = i < stars;
            }
            LevelNtext.text = levelN.ToString();
        }
        
    }
}
