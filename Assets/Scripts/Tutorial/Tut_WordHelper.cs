using System.Collections.Generic;
using UnityEngine;

using System.Collections;

using ServiceLocator;
using ServiceLocator.Services;

using EventSystem;
using Events.WordSystem;
using Events.GameSystem;

using GameSystem.State;
using WordSystem;

namespace Tutorial {
    public class Tut_WordHelper : MonoBehaviour {
        [Header("Word Settings")]
        [SerializeField] private List<GameObject> words;
        [SerializeField] private WordDifficulty Difficulty;
        [SerializeField] private float wordPopUpDelay = 0.5f;
        [SerializeField] private bool deployOnStart = false;

        private List<WordData> wordList = new List<WordData>();

        private int wordCount = 0;
        private int activeWord = 0;
        private bool isFirstDeck = true;

        private GameState gameState;
        // Services
        private IWordSystem wordManager;
        private IGameSystem gameSystem;


        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            wordCount = words.Count;
        }

        private void Start() {
            wordManager = ServiceRegistry.Get<IWordSystem>();

            GetWords();

            if (deployOnStart) {
                DeployNewWords();
            }

            EventBus.Publish(new Evt_TotalWordCards(wordCount));
        }

        private void OnEnable() {
        }

        private void OnDisable() {
        }

        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================
        /// <summary>
        /// Called when the game state changes
        /// </summary>
        /// <param name="evt"></param>
        private void OnGameStateChange(Evt_OnGameStateChange evt) {
            if (evt.gameState == GameState.PLAYING && isFirstDeck) {
                isFirstDeck = false;
                DeployNewWords();
            }
        }

        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================

        /// <summary>
        /// Retrieves the words
        /// </summary>
        private void GetWords() {
            string wordDifficulty = Difficulty.ToString();
            wordList = wordManager.GetNextWords(wordDifficulty, wordCount);

        }


        /// <summary>
        /// Deploys the new words
        /// </summary>
        private void DeployNewWords() {
            int count = 0;
            foreach (GameObject word in words) {
                if (word.TryGetComponent<Tut_Container>(out Tut_Container w)) {
                    w.SetWord(wordList[count]);
                    w.ToggleWord(true, true);

                    count++;
                    activeWord++;
                }
            }
        }

        /// <summary>
        /// Delays the word deployment
        /// </summary>
        /// <returns></returns>
        private IEnumerator WordDelay() {
            yield return new WaitForSeconds(wordPopUpDelay);
            DeployNewWords();
        }
    }

}