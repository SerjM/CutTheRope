using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cut_the_diet_game.Scripts.Managers
{
    public class LevelManager : MonoBehaviour
    {

        [SerializeField] private Image[] _fullApples;
        [SerializeField] private Image[] _finalFullApples;
        public static LevelManager Instance;

        public Animator ResultScreenAnimator;
        public GameObject ResultScreen;

        public int Apples = 0;
        public bool IsLose { get; private set; }
        // Use this for initialization
        void Awake ()
        {
            Instance = this;

        }

        public void OnFoodAte()
        {
            print("you win, Apples = " + Apples);
            ResultScreen.SetActive(true);
            OpenNextLevel();
            SetStarsCount();
            //res.Show(Stars); TODO
        }

        public void OnLose()
        {
            IsLose = true;
            print("you lost");
            Invoke("Restart", 2f);
        }

        private void Restart()
        {
            LoadingScreenManager.ReloadScene();
        }

        public void PickUpStar()
        {
            Apples++;
            SeeCatchedApple(Apples-1);
        }

        private void SeeCatchedApple(int appleIndex)
        {
            _fullApples[appleIndex].GetComponent<Animation>().Play();
            _finalFullApples[appleIndex].enabled = true;
        }

        private void OpenNextLevel()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            int cutCount = (currentSceneName.Length == 7) ? 1 : 2;
            string nextScenesName ="level_"+ (System.Convert.ToInt32( currentSceneName.Substring(currentSceneName.Length-cutCount))+1).ToString();
            PlayerPrefs.SetString(nextScenesName + "Completed", "");
            Debug.Log(nextScenesName);
        }
        private void SetStarsCount()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (PlayerPrefs.HasKey(currentSceneName + "Stars"))
            {
                if (PlayerPrefs.GetInt(currentSceneName + "Stars") < Apples)
                {
                    PlayerPrefs.SetInt(currentSceneName + "Stars", Apples);
                }
            }
            else 
            {
                PlayerPrefs.SetInt(currentSceneName + "Stars", Apples);
            }
          
        }
    }
}
