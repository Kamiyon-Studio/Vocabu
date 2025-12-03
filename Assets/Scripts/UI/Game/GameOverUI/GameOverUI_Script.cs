using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using ServiceLocator;
using ServiceLocator.Services;

using SceneSystem;
using GameSystem.State;

using EventSystem;
using Events.GameSystem;
using TMPro;

namespace UI.GameOverUI {
	public class GameOverUI_Script : MonoBehaviour {
		[Header("References")]
		[SerializeField] private GameObject panel;
		[SerializeField] private Button retryButton;
        [SerializeField] private Button wordDexButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI totalScoreTxt;
        [SerializeField] private TextMeshProUGUI nounCountTxt;
        [SerializeField] private TextMeshProUGUI verbCountTxt;
        [SerializeField] private TextMeshProUGUI adjCountTxt;

        // Dependencies
        private IScoreSystem scoreManager;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
			retryButton.onClick.AddListener(() => {
                SceneLoader.SCENE_TO_LOAD = "Game";
                SceneManager.LoadScene("Loading");
			});

			mainMenuButton.onClick.AddListener(() => {
                SceneLoader.SCENE_TO_LOAD = "MainMenu";
                SceneManager.LoadScene("Loading");
			});

            panel.SetActive(false);
        }

        private void Start() {
            scoreManager = ServiceRegistry.Get<IScoreSystem>();
        }

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnGameStateChange>(OnGameStateChange);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnGameStateChange>(OnGameStateChange);
        }

        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================
        private void OnGameStateChange(Evt_OnGameStateChange evt) {
            if (evt.gameState == GameState.GAMEOVER) {
                totalScoreTxt.text = scoreManager.GetTotalScore().ToString("N0");
                nounCountTxt.text = scoreManager.GetNounCount().ToString("N0");
                verbCountTxt.text = scoreManager.GetVerbCount().ToString("N0");
                adjCountTxt.text = scoreManager.GetAdjCount().ToString("N0");
                panel.SetActive(true);
            }
        }
    }
}