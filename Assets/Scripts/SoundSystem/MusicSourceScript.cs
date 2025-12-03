using UnityEngine;

using ServiceLocator;
using ServiceLocator.Services;

using EventSystem;
using Events.SoundSystem;

namespace SoundSystem {
    public class MusicSourceScript : MonoBehaviour {
        // Dependency
        private ISoundSystem soundManager;

        private AudioSource audioSource;

        private void Awake() {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start() {
            soundManager = ServiceRegistry.Get<ISoundSystem>();

            SetVolume();
        }

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnMasterVolumeChange>(OnMasterVolumeChange);
            EventBus.Subscribe<Evt_OnMusicVolumeChange>(OnMusicVolumeChange);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnMasterVolumeChange>(OnMasterVolumeChange);
            EventBus.Unsubscribe<Evt_OnMusicVolumeChange>(OnMusicVolumeChange);
        }

        private void OnMasterVolumeChange(Evt_OnMasterVolumeChange evt) { SetVolume(); }
        private void OnMusicVolumeChange(Evt_OnMusicVolumeChange evt) { SetVolume(); }

        private void SetVolume() {
            audioSource.volume = soundManager.GetMusicVolume();
        }
    }
}