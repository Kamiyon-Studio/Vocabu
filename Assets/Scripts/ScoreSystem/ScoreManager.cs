using UnityEngine;

using ServiceLocator;
using ServiceLocator.Services;

using GameSystem.State;

using EventSystem;
using Events.DropContainer;
using Events.WordContainer;
using Events.WordSystem;
using Events.ScoreSystem;
using Events.GameSystem;

namespace ScoreSystem {
    public class ScoreManager : MonoBehaviour, IScoreSystem {
        private int totalScore = 0;
        private int initialScore = 0;
        private float scoreMultiplier = 1;

        private int nounCount = 0;
        private int verbCount = 0;
        private int adjCount = 0;

        private int totalCards = 0;
        private int indexToCalculateTotalScore = 0;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            if (ServiceRegistry.IsRegistered<IScoreSystem>()) {
                Debug.LogWarning("[ScoreManager] IScoreSystem service is already registered.");
                Destroy(gameObject);
                return;
            }

            ServiceRegistry.Register<IScoreSystem>(this);
        }

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnCorrectCardDrop>(OnCorrectCardDrop);
            EventBus.Subscribe<Evt_OnIncorrectCardDrop>(OnIncorrectCardDrop);
            EventBus.Subscribe<Evt_OnIncorrectLetterDrop>(OnIncorrectLetterDrop);

            EventBus.Subscribe<Evt_OnCorrectLetterDrop>(OnCorrectLetterDrop);

            EventBus.Subscribe<Evt_TotalWordCards>(TotalWordCount);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnCorrectCardDrop>(OnCorrectCardDrop);
            EventBus.Unsubscribe<Evt_OnIncorrectCardDrop>(OnIncorrectCardDrop);
            EventBus.Unsubscribe<Evt_OnIncorrectLetterDrop>(OnIncorrectLetterDrop);

            EventBus.Unsubscribe<Evt_OnCorrectLetterDrop>(OnCorrectLetterDrop);

            EventBus.Unsubscribe<Evt_TotalWordCards>(TotalWordCount);
        }

        private void Start() {
            indexToCalculateTotalScore = totalCards;
        }

        private void OnDestroy() {
            if (ServiceRegistry.IsRegistered<IScoreSystem>()) {
                ServiceRegistry.Unregister<IScoreSystem>(this);
            }
        }

        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================
        private void TotalWordCount(Evt_TotalWordCards evt) { totalCards = evt.wordCardCount; }

        private void OnCorrectCardDrop(Evt_OnCorrectCardDrop evt) {
            CalculateInitialScore(evt.score, isCardDrop: true, setMultiplier: evt.multiplier, evt.wordType);
        }

        private void OnIncorrectCardDrop(Evt_OnIncorrectCardDrop evt) { ResetScore(isCardDrop: true); }
        private void OnIncorrectLetterDrop(Evt_OnIncorrectLetterDrop evt) { ResetScore(); }
        private void OnCorrectLetterDrop(Evt_OnCorrectLetterDrop evt) { CalculateInitialScore(evt.score); }

        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================
        private void CalculateInitialScore(
            int score,
            bool isCardDrop = false,
            int setMultiplier = 1,
            string wordType = null
            ) {

            initialScore += Mathf.RoundToInt(score + scoreMultiplier);

            if (isCardDrop) {
                scoreMultiplier += setMultiplier;
                indexToCalculateTotalScore--;

                if (wordType != null) {
                    switch (wordType.ToLower()) {
                        case "noun": nounCount++; break;
                        case "verb": verbCount++; break;
                        case "adjective": adjCount++; break;
                    }
                }

                if (indexToCalculateTotalScore == 0) {
                    CalculateTotalScore();
                    indexToCalculateTotalScore = totalCards;
                }

                EventBus.Publish(new Evt_OnMultiplierChange(scoreMultiplier));
            }

            EventBus.Publish(new Evt_OnCorrectAnswer());
            EventBus.Publish(new Evt_OnInitialScoreChange(initialScore));
        }

        private void ResetScore(bool isCardDrop = false) {
            CalculateTotalScore();

            scoreMultiplier = 1;

            if (isCardDrop) {
                indexToCalculateTotalScore--; 
            }

            EventBus.Publish(new Evt_OnIncorrectAnswer());
            EventBus.Publish(new Evt_OnMultiplierChange(scoreMultiplier));
        }


        private void CalculateTotalScore() {
            totalScore += Mathf.RoundToInt(initialScore * scoreMultiplier);
            initialScore = 0;

            EventBus.Publish(new Evt_OnTotalScoreChange(totalScore));
            EventBus.Publish(new Evt_OnInitialScoreChange(initialScore));
        }


        // =====================================================================
        //
        //                          Interface Methods
        //
        // =====================================================================
        public int GetTotalScore() { 
            ResetScore();
            return totalScore;
        }
        public int GetNounCount() { return nounCount; }
        public int GetVerbCount() { return verbCount; }
        public int GetAdjCount() { return adjCount; }
    }
}