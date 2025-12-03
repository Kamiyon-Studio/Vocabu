using UnityEngine;
using UnityEngine.UI;

using SceneSystem;
using UnityEngine.SceneManagement;


using EventSystem;
using Events.GameSystem;

using GameSystem.State;

namespace UI.PauseUI {
    public class PauseUI_Script : MonoBehaviour {
        [Header("References")]
        [SerializeField] private GameObject pauseUIPanel;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================

        private void Start() {
            pauseButton.onClick.AddListener(() => {
                TogglePanel(true);
                EventBus.Publish(new Evt_OnPauseAction());
            });

            resumeButton.onClick.AddListener(() => {
                TogglePanel(false);
                EventBus.Publish(new Evt_OnResumeAction());
            });

            mainMenuButton.onClick.AddListener(() => {
                SceneLoader.SCENE_TO_LOAD = "MainMenu";
                SceneManager.LoadScene("Loading");
            });
        }

        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================
        private void TogglePanel(bool value) {
            pauseUIPanel.SetActive(value);
        }
    }
}