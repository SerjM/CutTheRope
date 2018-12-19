using System.Collections.Generic;
using UnityEngine;

namespace Main_menu.Scripts
{
    public class MainMenuLoader : MonoBehaviour
    {

        [SerializeField] private GameObject _scenePrefab;
        [SerializeField] private GameObject _scenesFillPlace;
        [SerializeField] private int _scenesCount;

        private List<GameObject> _allScenes = new List<GameObject>();

        private void Start()
        {
            // PlayerPrefs.DeleteAll();
            FillTheScenesPlace();
            SetSceneSettings();
        }

        private void FillTheScenesPlace()
        {
            for (int i = 0; i < _scenesCount; i++)
            {
                GameObject temporal = Instantiate(_scenePrefab, _scenesFillPlace.transform);
                _allScenes.Add(temporal);
            }

        }
        private void SetSceneSettings()
        {
            for (int i = 0; i < _scenesCount; i++)
            {
                if (i == 0)
                {
                    _allScenes[i].GetComponent<ScenesSettings>().BlockedImage = false;
                }
                _allScenes[i].GetComponent<ScenesSettings>().LevelText.text = (i + 1).ToString();
                _allScenes[i].GetComponent<ScenesSettings>().LevelName = "level_" + (i + 1).ToString();
            }
        }
    }
}
