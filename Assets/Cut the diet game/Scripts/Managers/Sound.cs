using UnityEngine;

namespace Cut_the_diet_game.Scripts.Managers
{
    [System.Serializable]
    public class Sound
    {
        public string name;

        public AudioClip clip;

        [Range(0f, 1f)]
        public float pitch = 1f;
        [Range(0.1f, 3f)]
        public float volume = 1f;

        public bool looping = false;

        [HideInInspector] public AudioSource Source;
    }
}
