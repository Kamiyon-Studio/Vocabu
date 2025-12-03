using UnityEngine;
using ServiceLocator;
using ServiceLocator.Services;

using EventSystem;
using Events.ClickToStartUI;
using Events.GameSystem;

using GameSystem.State;

namespace GameSystem {
    public class GameManager : MonoBehaviour, IGameSystem {
        [Header("Timer Settings")]
        [SerializeField] private float timerDuration = 120f;


        private GameState gameState;

        private float timer = 0f;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            if (ServiceRegistry.IsRegistered<IGameSystem>()) {
                Debug.LogWarning("[GameManager] IGameSystem service is already registered.");
                Destroy(gameObject);
                return;
            }

            ServiceRegistry.Register<IGameSystem>(this);
        }

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnClickToStart>(StartCountdown);
            EventBus.Subscribe<Evt_OnPauseAction>(OnPauseAction);
            EventBus.Subscribe<Evt_OnResumeAction>(OnResumeAction);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnClickToStart>(StartCountdown);
            EventBus.Unsubscribe<Evt_OnPauseAction>(OnPauseAction);
            EventBus.Unsubscribe<Evt_OnResumeAction>(OnResumeAction);
        }

        private void Start() {
            gameState = GameState.WAITING;
            timer = timerDuration;
            EventBus.Publish(new Evt_OnGameStateChange(gameState));
        }

        private void Update() {
            switch (gameState) {
                case GameState.TUTORIAL:
                    break;
                case GameState.WAITING:
                    break;
                case GameState.COUNTDOWN:
                    break;
                case GameState.PLAYING:
                    timer -= Time.deltaTime;
                    if (timer <= 0f) {
                        timer = 0f;
                        gameState = GameState.GAMEOVER;
                        EventBus.Publish(new Evt_OnGameStateChange(gameState));
                    }

                    break;
                case GameState.GAMEOVER:
                    break;
                case GameState.PAUSE:
                    break;
            }
        }

        private void OnDestroy() {
            if (ServiceRegistry.IsRegistered<IGameSystem>()) {
                ServiceRegistry.Unregister<IGameSystem>(this);
            }
        }


        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================

        /// <summary>
        /// Starts the game
        /// </summary>
        /// <param name="evt"></param>
        private void StartCountdown(Evt_OnClickToStart evt) {
            //gameState = GameState.COUNTDOWN;
            gameState = GameState.PLAYING;
            EventBus.Publish(new Evt_OnGameStateChange(gameState));
        }

        private void OnPauseAction(Evt_OnPauseAction evt) {
            Time.timeScale = 0f;
            gameState = GameState.PAUSE;
            EventBus.Publish(new Evt_OnGameStateChange(gameState));
        }
        private void OnResumeAction(Evt_OnResumeAction evt) {
            Time.timeScale = 1f;
            gameState = GameState.PLAYING;
            EventBus.Publish(new Evt_OnGameStateChange(gameState));
        }


        // =====================================================================
        //
        //                          Interface Methods
        //
        // =====================================================================
        public GameState GetGameState() => gameState;
        public float GetTimerDuration() => timerDuration;
        public float GetTimer() => timer;
    }
}