using UI.MainMenu.Main;
using UnityEngine;
using UnityEngine.UI;

using EventSystem;
using Events.GameSystem;

namespace UI.MainMenu.WordDex {
    public class WordDexUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private GameObject wordDexPanel;
        [SerializeField] private bool isGameMode = false;

        [Header("Buttons")]
        [SerializeField] private Button wordDexButton;
        [SerializeField] private Button backButton;

        public MainMenuUI mainMenuUI { get; set; }

        private void Awake() {
            wordDexButton?.onClick.AddListener(() => {
                ToggleWordDexUI(true);

                if (isGameMode) {
                    EventBus.Publish(new Evt_OnPauseAction());
                }
            });

            backButton.onClick.AddListener(() => {
                mainMenuUI?.ToggleMainMenuUI(true);
                ToggleWordDexUI(false);

                if (isGameMode) {
                    EventBus.Publish(new Evt_OnResumeAction());
                }
            });
        }

        public void ToggleWordDexUI(bool value) {
            wordDexPanel.SetActive(value);
        }
    }
}