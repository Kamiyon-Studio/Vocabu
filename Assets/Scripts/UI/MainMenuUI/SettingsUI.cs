using UnityEngine;
using UnityEngine.UI;

using ServiceLocator;
using ServiceLocator.Services;

using EventSystem;
using Events.SoundSystem;

using UI.MainMenu.Main;

namespace UI.MainMenu.settings {
    public class SettingsUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private GameObject settingsPanel;

        [Header("Buttons")]
        [SerializeField] private Button backButton;

        [Header("Slider")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        public MainMenuUI mainMenuUI { get; set; }


        // Dependencies
        private ISoundSystem soundManager;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            backButton.onClick.AddListener(() => {
                mainMenuUI.ToggleMainMenuUI(true);
                ToggleSettingsUI(false);
            });
        }

        private void Start() {
            soundManager = ServiceRegistry.Get<ISoundSystem>();

            masterVolumeSlider.value = soundManager.GetMasterVolume();
            musicVolumeSlider.value = soundManager.GetMusicVolume();
            sfxVolumeSlider.value = soundManager.GetSFXVolume();

            masterVolumeSlider.onValueChanged.AddListener((value) => {
                soundManager.SetMasterVolume(value);
            });

            musicVolumeSlider.onValueChanged.AddListener((value) => {
                soundManager.SetMusicVolume(value);
            });

            sfxVolumeSlider.onValueChanged.AddListener((value) => {
                soundManager.SetSFXVolume(value);
            });
        }


        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================
        public void ToggleSettingsUI(bool value) {
            settingsPanel.SetActive(value);
        }

    }
}