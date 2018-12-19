using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Main_menu.Scripts
{
    public class ScenesSettings : MonoBehaviour
    {

        [SerializeField] private Image _blocked;
        [SerializeField] private GameObject _starsPlace;
        [SerializeField] private Text _levelText;
        [SerializeField] private Sprite _fullStar;
        [SerializeField] private string _levelName;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => LoadScene(_levelName));
            _blocked.enabled = IsBlocked();
            SetStars();
        }

        public Text LevelText
        {
            get
            {
                return _levelText;
            }
            set
            {
                _levelText = value;
            }
        }
        public bool BlockedImage
        {
            get
            {
                return _blocked;
            }
            set
            {
                _blocked.enabled = value;
            }
        }

        public GameObject StartsPlace
        {
            get
            {
                return _starsPlace;
            }
        }
        public string LevelName
        {
            get
            {
                return _levelName;
            }
            set
            {
                _levelName = value;
            }
        }

        private void LoadScene(string sceneName)
        {
            if (!_blocked.enabled)
            {
                LoadingScreenManager.LoadScene(sceneName);
          
            
            }
        }


        private bool IsBlocked()
        {
            if (LevelName == "level_1")
            {
                return false;
            }
            if(PlayerPrefs.HasKey(LevelName+ "Completed"))
            {
                return false;
            }

            return true;
        }

        private void SetStars()
        {
            if (PlayerPrefs.HasKey(LevelName + "Stars"))
            {
                for (int i = 0; i < _starsPlace.transform.childCount; i++)
                {
                    if(i<PlayerPrefs.GetInt(LevelName + "Stars"))
                    {
                        _starsPlace.transform.GetChild(i).GetComponent<Image>().sprite = _fullStar;
                    }
                }
            }
        }


    }
}
