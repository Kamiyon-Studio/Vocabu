using UnityEngine;

using ServiceLocator;
using ServiceLocator.Services;

using SO;
using System.Collections;

namespace SoundSystem {
    public class SFXSourceScript : MonoBehaviour {
        [SerializeField] private PoolItemSO soundSourcePoolItemSO;

        private AudioSource audioSource;

        // Dependency
        private IPoolSystem poolManager;

        private void Awake() {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start() {
            poolManager = ServiceRegistry.Get<IPoolSystem>();
        }

        public void PlayAudioClip(AudioClip audioClip) {
            audioSource.clip = audioClip;
            audioSource.Play();
            StartCoroutine(ReturnToPool());
        }

        public void SetVolume(float volume) {
            audioSource.volume = volume;
        }

        private IEnumerator ReturnToPool() {
            yield return new WaitForSeconds(audioSource.clip.length);
            poolManager.ReturnToPool(soundSourcePoolItemSO.name, gameObject);
        }
    }
}