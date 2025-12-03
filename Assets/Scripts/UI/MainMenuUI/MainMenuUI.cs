using UnityEngine;
using UnityEngine.UI;

using UI.MainMenu.WordDex;
using UI.MainMenu.settings;
using UnityEngine.SceneManagement;
using SceneSystem;

namespace UI.MainMenu.Main {
    public class MainMenuUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private WordDexUI wordDexUI;
        [SerializeField] private SettingsUI settingsUI;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button tutorialButtom;
        [SerializeField] private Button wordDexButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        private void Awake() {
            playButton.onClick.AddListener(() => {
                TutorialHandler();
            });

            tutorialButtom.onClick.AddListener(() => {
                SceneLoader.SCENE_TO_LOAD = "Tutorial";
                SceneManager.LoadScene("Loading");
            });

            wordDexButton.onClick.AddListener(() => {
                wordDexUI.mainMenuUI = this;
                wordDexUI.ToggleWordDexUI(true);
                ToggleMainMenuUI(false);
            });

            settingsButton.onClick.AddListener(() => {
                settingsUI.mainMenuUI = this;
                settingsUI.ToggleSettingsUI(true);
            });

            quitButton.onClick.AddListener(() => {
                Application.Quit();
            });

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        public void ToggleMainMenuUI(bool value) {
            mainMenuPanel.SetActive(value);
        }

        private void TutorialHandler() {
            if (PlayerPrefs.GetInt("CompletedTutorial") == 0) {
                SceneLoader.SCENE_TO_LOAD = "Tutorial";
                SceneManager.LoadScene("Loading");
            }
            else {
                SceneLoader.SCENE_TO_LOAD = "Game";
                SceneManager.LoadScene("Loading");
            }
        }
    }
}