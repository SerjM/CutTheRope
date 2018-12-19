using Cut_the_diet_game.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Main_menu.Scripts
{
    public class SoundSlider : MonoBehaviour
    {
        public Image TargetGraphic;

        public Sprite SoundOnSprite;
        public Sprite SoundOffSprite;

        private enum OptionType
        {
            GlobalVolume,
            MusicVolume,
            SfxVolume
        }

        [SerializeField] private OptionType _optionType;

        private bool _isTurnedOff;

        public void OnValueChanged(float value)
        {
            if (AudioManager.Instance == null) return; //On awake

            switch (_optionType)
            {
                case OptionType.GlobalVolume:
                    AudioManager.Instance.UpdateGlobalVolume(value);
                    PlayerPrefs.SetFloat("GlobalVolume", value);
                    break;
                case OptionType.MusicVolume:
                    AudioManager.Instance.UpdateMusicVolume(value);
                    PlayerPrefs.SetFloat("MusicVolume", value);
                    break;
                case OptionType.SfxVolume:
                    AudioManager.Instance.UpdateSfxVolume(value);
                    PlayerPrefs.SetFloat("SfxVolume", value);
                    break;
            }

            if (value == 0f)
            {
                TargetGraphic.sprite = SoundOffSprite;
            }
            else
            {
                TargetGraphic.sprite = SoundOnSprite;
            }

        }

        public void OnSoundIconClick()
        {
            if (_isTurnedOff)
            {
                OnValueChanged(0.5f);
                GetComponentInChildren<Slider>().value = 0.5f;
            }
            else
            {
                OnValueChanged(0f);
                GetComponentInChildren<Slider>().value = 0f;
            }

            _isTurnedOff = !_isTurnedOff;
        }

        private void Start()
        {
            var slider = GetComponentInChildren<Slider>();
            switch (_optionType)
            {
                case OptionType.GlobalVolume:
                    if (PlayerPrefs.HasKey("GlobalVolume"))
                        slider.value = PlayerPrefs.GetFloat("GlobalVolume");
                    break;
                case OptionType.MusicVolume:
                    if (PlayerPrefs.HasKey("MusicVolume"))
                        slider.value = PlayerPrefs.GetFloat("MusicVolume");
                    break;
                case OptionType.SfxVolume:
                    if (PlayerPrefs.HasKey("SfxVolume"))
                        slider.value = PlayerPrefs.GetFloat("SfxVolume");
                    break;
            }
        }
    }
}
