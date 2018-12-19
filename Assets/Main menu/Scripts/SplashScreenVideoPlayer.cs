using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;
using UnityEngine.Video;

namespace Main_menu.Scripts
{
    public class SplashScreenVideoPlayer : MonoBehaviour {

        public string NextSceneName = "GameSelect";

        void Update () {
            VideoPlayer video = GetComponent<VideoPlayer>();
            if (!video.isPlaying)
            {
                Skip();
            }
        }

        public void OnClick()
        {
            Skip();
        }

        private void Skip()
        {
            LoadingScreenManager.LoadScene(NextSceneName);
            Destroy(this);
        }
    }
}
