using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cut_the_diet_game.Scripts.Managers
{
    
    public class LoadingScreenManager : MonoBehaviour
    {
        
        #region Singleton
        public static LoadingScreenManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            _fadeAnimator = GetComponent<Animator>();
        }
        #endregion

        public float FadeDelay = 0.3f;
        private Animator _fadeAnimator;

        public static void ReloadScene()
        {
            Instance.StartCoroutine(ReloadSceneCoroutine());
            
        }
        public static IEnumerator ReloadSceneCoroutine()
        {
            FadeOut();
            yield return new WaitForSeconds(Instance.FadeDelay);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            FadeIn();
        }

        public static void LoadScene(string sceneName)
        {
            Instance.StartCoroutine(LoadSceneCoroutine(sceneName));
        }

        public static IEnumerator LoadSceneCoroutine(string sceneName)
        {
            FadeOut();
            yield return new WaitForSeconds(Instance.FadeDelay);
            SceneManager.LoadScene(sceneName);
            FadeIn();
        }

        public static void FadeIn()
        {
            Instance._fadeAnimator.SetTrigger("In");
        }
        public static void FadeOut()
        {
            Instance._fadeAnimator.SetTrigger("Out");
        }
        /*public static void Level(int world, int levelN)
        {
            var name = ProgressManager.Instance.GetLevel(world, levelN).name;
            LoadScene(name);
        }*/
        public static void NextLevel()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName == "level_20")
            {
                ExitToMenu();
                return;
            }
            int cutCount = (currentSceneName.Length == 7) ? 1 : 2;
            string nextScenesName = "level_" + (System.Convert.ToInt32(currentSceneName.Substring(currentSceneName.Length - cutCount)) + 1).ToString();
            LoadScene(nextScenesName);
        }

        public static void ExitToMenu()
        {
            LoadScene("Menu");
        }

    }
}
