using System;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Managers
{
    public class AudioManager : MonoBehaviour
    {
        #region Singleton
        public static AudioManager Instance;

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

            LoadParameters();
            SetClips();
        }
        #endregion

        public Sound[] Sounds;
        public Sound[] Musics;

        [SerializeField] public float GlobalVolume = 1f;
        [SerializeField] public float MusicVolume = 0.7f;
        [SerializeField] public float SfxVolume = 0.7f;

        private AudioSource _musicAudioSource;

        public void SetClips()
        {
            foreach (var s in Sounds)
            {
                s.Source = gameObject.AddComponent<AudioSource>();
                s.Source.clip = s.clip;
                s.Source.volume = (SfxVolume * GlobalVolume);
                s.Source.pitch = s.pitch;
            }
            foreach (var s in Musics)
            {
                s.Source = gameObject.AddComponent<AudioSource>();
                s.Source.clip = s.clip;
                s.Source.volume = (MusicVolume * GlobalVolume);
                s.Source.pitch = s.pitch;
            }
        }

        public void UpdateSfxVolume(float value)
        {
            SfxVolume = value;
            foreach (var s in Sounds)
            {
                s.Source.volume = (SfxVolume * GlobalVolume);
            }
        }
        public void UpdateMusicVolume(float value)
        {
            MusicVolume = value;
            foreach (var s in Musics)
            {
                s.Source.volume = (MusicVolume * GlobalVolume);
            }
        }
        public void UpdateGlobalVolume(float value)
        {
            GlobalVolume = value;
            foreach (var s in Musics)
            {
                s.Source.volume = (MusicVolume * GlobalVolume);
            }
            foreach (var s in Sounds)
            {
                s.Source.volume = (SfxVolume * GlobalVolume);
            }
        }

        // Use this for initialization
        private void LoadParameters()
        {
            if (PlayerPrefs.HasKey("GlobalVolume"))
                GlobalVolume = PlayerPrefs.GetFloat("GlobalVolume");
            if (PlayerPrefs.HasKey("MusicVolume"))
                GlobalVolume = PlayerPrefs.GetFloat("MusicVolume");
            if (PlayerPrefs.HasKey("SfxVolume"))
                GlobalVolume = PlayerPrefs.GetFloat("SfxVolume");

        }

        private void Start()
        {
            PlayMusic("Main menu");
        }

        public static void PlaySoundEffect(string name, float pitch = 1f)
        {
            Sound s = Array.Find(Instance.Sounds, sound => sound.name == name);

            if (s != null)
            {
                s.Source.pitch = pitch;
                s.Source.Stop();
                s.Source.Play();
            }
            else
            {
                Debug.LogError("no sound for " + name);
            }
        }

        public static void PlayMusic(string name)
        {
            Sound s = Array.Find(Instance.Musics, sound => sound.name == name);

            if (s != null)
            {
                s.Source.Stop(); //TODO fade between musics
                s.Source.Play();
            }
            else
            {
                Debug.LogError("no music for " + name);
            }
        }

    }
}
