using UnityEngine;

using ServiceLocator;
using ServiceLocator.Services;

using EventSystem;
using Events.Letter;
using Events.SoundSystem;
using Events.WordContainer;
using Events.ScoreSystem;
using SO;
using Events.DropContainer;
using Events.ClickToStartUI;

namespace SoundSystem {
    [System.Serializable]
    public enum SoundType {
        MASTER,
        MUSIC,
        SFX
    }

	public class SoundManager : MonoBehaviour, ISoundSystem {
		private float masterVolume = 1f;
		private float musicVolume = 0.6f;
		private float sfxVolume = 0.6f;

        [Header("Audio Source SO")]
        [SerializeField] private PoolItemSO soundSourcePoolItemSO;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip correctSFXAudioClip;
        [SerializeField] private AudioClip incorrectSFXAudioClip;
        [SerializeField] private AudioClip musicAudioClip;
        [SerializeField] private AudioClip sfxCardPickup;


        [SerializeField] private AudioClip sfxTotalScoreAudioClip;

        [Header("Audio SpawnPoint")]
        [SerializeField] private Transform sfxSpawnPoint;

        

        // Dependency
        private IPoolSystem poolManager;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            if (ServiceRegistry.IsRegistered<ISoundSystem>()) {
                Debug.LogWarning("[SoundManager] Service is already registered.");
                Destroy(gameObject);
                return;
            }
            ServiceRegistry.Register<ISoundSystem>(this);

            masterVolume = GetFloatPref(SoundType.MASTER, 1f);
            musicVolume = GetFloatPref(SoundType.MUSIC, 0.6f);
            sfxVolume = GetFloatPref(SoundType.SFX, 0.6f);
        }

        private void Start() {
            poolManager = ServiceRegistry.Get<IPoolSystem>();
        }

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnCorrectLetterDrop>(OnCorrectLetterDrop);
            EventBus.Subscribe<Evt_OnCorrectCardDrop>(OnCorrectCardDrop);
            EventBus.Subscribe<Evt_OnWordCardDrag>(OnWordCardDrag);
            EventBus.Subscribe<Evt_OnIncorrectAnswer>(OnIncorrectAnswer);
            EventBus.Subscribe<Evt_OnTotalScoreChange>(OnTotalScoreChange);
            EventBus.Subscribe<Evt_OnClickToStart>(OnClickToStart);
            EventBus.Subscribe<Evt_OnLetterDrag>(OnLetterDrag);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnCorrectLetterDrop>(OnCorrectLetterDrop);
            EventBus.Unsubscribe<Evt_OnCorrectCardDrop>(OnCorrectCardDrop);
            EventBus.Unsubscribe<Evt_OnWordCardDrag>(OnWordCardDrag);
            EventBus.Unsubscribe<Evt_OnIncorrectAnswer>(OnIncorrectAnswer);
            EventBus.Unsubscribe<Evt_OnTotalScoreChange>(OnTotalScoreChange);
            EventBus.Unsubscribe<Evt_OnClickToStart>(OnClickToStart);
            EventBus.Unsubscribe<Evt_OnLetterDrag>(OnLetterDrag);
        }

        private void OnDestroy() {
            ServiceRegistry.Unregister<ISoundSystem>(this);
        }

        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================



        private void OnCorrectLetterDrop(Evt_OnCorrectLetterDrop evt) {
            PlaySFXClip(correctSFXAudioClip);
        }
        

        private void OnCorrectCardDrop(Evt_OnCorrectCardDrop evt) {
            PlaySFXClip(correctSFXAudioClip);
        }

        private void OnIncorrectAnswer(Evt_OnIncorrectAnswer evt) {
            PlaySFXClip(incorrectSFXAudioClip);
        }

        private void OnClickToStart(Evt_OnClickToStart evt) {
            PlaySFXClip(correctSFXAudioClip);
        }

        private void OnWordCardDrag(Evt_OnWordCardDrag evt) {
            PlaySFXClip(sfxCardPickup);
        }

        private void OnLetterDrag(Evt_OnLetterDrag evt) {
            PlaySFXClip(sfxCardPickup);
        }

        private void OnTotalScoreChange(Evt_OnTotalScoreChange evt) {
            PlaySFXClip(sfxTotalScoreAudioClip);
        }

        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================
        private float GetFloatPref(SoundType prefName, float defaultValue = 1f) {
            string prefNameStr = prefName.ToString();

            if (PlayerPrefs.HasKey(prefNameStr)) {
                return PlayerPrefs.GetFloat(prefNameStr);
            } else {
                PlayerPrefs.SetFloat(prefNameStr, defaultValue);
                return defaultValue;
            }
        }

        private void SetFloatPref(SoundType prefNamem, float value) {
            string prefNameStr = prefNamem.ToString();

            PlayerPrefs.SetFloat(prefNameStr, value);
        }


        private void PlaySFXClip(AudioClip clip) {
            GameObject sfx = poolManager.SpawnFromPool(
                soundSourcePoolItemSO.name,
                sfxSpawnPoint.position,
                parent: sfxSpawnPoint
                );

            SFXSourceScript sfxScript = sfx.GetComponent<SFXSourceScript>();
            sfxScript?.SetVolume(GetSFXVolume());
            sfxScript?.PlayAudioClip(clip);
        }


        // =====================================================================
        //
        //                          Interface Methods
        //
        // =====================================================================
        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => masterVolume * musicVolume;
        public float GetSFXVolume() => masterVolume * sfxVolume;

        public void SetMasterVolume(float value) {
            masterVolume = value;
            SetFloatPref(SoundType.MASTER, masterVolume);
            EventBus.Publish(new Evt_OnMasterVolumeChange(masterVolume));
        }

        public void SetMusicVolume(float value) {
            musicVolume = value;
            SetFloatPref(SoundType.MUSIC, musicVolume);
            EventBus.Publish(new Evt_OnMusicVolumeChange(musicVolume));
        }

        public void SetSFXVolume(float value) {
            sfxVolume = value;
            SetFloatPref(SoundType.SFX, sfxVolume);
            EventBus.Publish(new Evt_OnSFXVolumeChange(sfxVolume));
        }
    }
}