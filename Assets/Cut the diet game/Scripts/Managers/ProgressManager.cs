using Cut_the_diet_game.Scripts.UiScripts;
using Main_menu.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cut_the_diet_game.Scripts.Managers
{
    public class ProgressManager
    {
        public static int Coins { get; private set; }

        public static void Buy(ShopItem item)
        {
            if (Coins >= item.Price)
            {
                DialogManager.Instance.Show(
                    () =>
                    {
                        Coins -= item.Price;
                    });
            }
            else
            {
                //TODO недостаточно монет
            }
        }

        public static void Reset()
        {
            DialogManager.Instance.Show(DeleteStats);
            
        }

        private static void DeleteStats()
        {
            PlayerPrefs.DeleteAll();
            LoadingScreenManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public static int GetStarsOnFirstGame()
        {
            int result = 0;
            string level = "level_";
            int i = 1;
            Debug.Log("getting stars");
            while (PlayerPrefs.HasKey(level + i + "Stars"))
            {
                result += PlayerPrefs.GetInt(level + i + "Stars");
                Debug.Log("Stars on level [" + level + i + "Stars" + "] = " + PlayerPrefs.GetInt(level + i + "Stars"));
                i++;
            }

            return result;
        }
    }
}
