using TMPro;
using UnityEngine;

using ServiceLocator;
using ServiceLocator.Services;

using GameSystem.State;
using EventSystem;
using Events.GameSystem;
using Events.ScoreSystem;

namespace UI.StatsUI {
    public class StatsUI_Script : MonoBehaviour {
        [Header("references")]
        [SerializeField] private GameObject statsUIPanel;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI timerTxt;
        [SerializeField] private TextMeshProUGUI scoreTxt;
        [SerializeField] private TextMeshProUGUI initialScoreTxt;
        [SerializeField] private TextMeshProUGUI multiplierTxt;

        private GameState gameState;

        //Dependencies
        private IGameSystem gameManager;


        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void OnEnable() {
            EventBus.Subscribe<Evt_OnGameStateChange>(OnGameStateChange);

            EventBus.Subscribe<Evt_OnMultiplierChange>(OnMultiplierChange);
            EventBus.Subscribe<Evt_OnInitialScoreChange>(OnInitialScoreChange);
            EventBus.Subscribe<Evt_OnTotalScoreChange>(OnTotalScoreChange);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnGameStateChange>(OnGameStateChange);

            EventBus.Unsubscribe<Evt_OnMultiplierChange>(OnMultiplierChange);
            EventBus.Unsubscribe<Evt_OnInitialScoreChange>(OnInitialScoreChange);
            EventBus.Unsubscribe<Evt_OnTotalScoreChange>(OnTotalScoreChange);
        }

        private void Start() {
            gameManager = ServiceRegistry.Get<IGameSystem>();

            // Sets default text values
            if (gameManager != null) {
                timerTxt.text = $"{gameManager.GetTimerDuration()}S";
            }

            scoreTxt.text = "0";
            initialScoreTxt.text = "0";
            multiplierTxt.text = "1";
        }

        private void Update() {
            SetTimer();
        }


        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================
        private void OnGameStateChange(Evt_OnGameStateChange evt) {
            gameState = evt.gameState;
            if (evt.gameState == GameState.GAMEOVER) {
                statsUIPanel.SetActive(false); 
            }
        }
        private void OnMultiplierChange(Evt_OnMultiplierChange evt) { multiplierTxt.text = $"{evt.multiplier}"; }
        private void OnInitialScoreChange(Evt_OnInitialScoreChange evt) { initialScoreTxt.text = $"{evt.initialScore}"; }
        private void OnTotalScoreChange(Evt_OnTotalScoreChange evt) {
            scoreTxt.text = evt.totalScore.ToString("N0");
        }

        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================
        private void SetTimer() {
            if (gameState != GameState.PLAYING) return;

            float timerValue = gameManager.GetTimer();

            int displayTime = Mathf.FloorToInt(timerValue);
            timerTxt.text = $"{displayTime}S";
        }

    }
}