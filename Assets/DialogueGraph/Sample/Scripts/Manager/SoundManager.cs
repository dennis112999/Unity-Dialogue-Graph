using UnityEngine;

namespace Dennis.Tools.DialogueGraph
{
    /// <summary>
    /// Sample Sound Manager
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        [Header("Audio Sources")]
        public AudioSource bgmSource;
        public AudioSource seSource;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void PlayBGM(AudioClip clip)
        {
            if (clip == null) return;
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        public void StopBGM()
        {
            bgmSource.Stop();
        }

        public void PlaySE(AudioClip clip)
        {
            if (clip == null) return;
            seSource.PlayOneShot(clip);
        }

        public void StopSE()
        {
            seSource.Stop();
        }

        public void SetBGMVolume(float volume)
        {
            bgmSource.volume = Mathf.Clamp01(volume);
        }

        public void SetSEVolume(float volume)
        {
            seSource.volume = Mathf.Clamp01(volume);
        }
    }
}
