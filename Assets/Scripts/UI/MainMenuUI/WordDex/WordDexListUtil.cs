using ServiceLocator;
using ServiceLocator.Services;
using System.Collections.Generic;
using UnityEngine;
using WordSystem;

namespace UI.MainMenu.WordDex {
    public class WordDexListUtil : MonoBehaviour {
        /// <summary>
        /// Filter Type
        /// </summary>
        private enum FilterType {
            ALL,
            NOUN,
            VERB,
            ADJECTIVE
        }

        [Header("WordDexCard")]
        [SerializeField] private GameObject wordDexCardPrefab;
        [SerializeField] private Transform wordDexCardParent;

        [Header("Filter Debugger")]
        [SerializeField] private bool enableFilterDebugger = false;
        [SerializeField] private FilterType filterType = FilterType.ALL;


        private List<WordData> wordList = new List<WordData>();
        private List<GameObject> wordDexCardList = new List<GameObject>();


        // Service Dependencies
        private IWordSystem wordManager;


        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================

        private void Start() {
            wordManager = ServiceRegistry.Get<IWordSystem>();

            if (wordManager != null) {
                wordList = wordManager.GetAllWords(alphabeticalOrder: true);
                DeployCards();
            }



            // Filter Debugger
            if (enableFilterDebugger) {
                FilterWord(filterType.ToString()); 
            }
        }


        // =====================================================================
        //
        //                          Private Methods
        //
        // =====================================================================

        /// <summary>
        /// Deploy Cards 
        /// </summary>
        private void DeployCards() {
            foreach (WordData word in wordList) {
                GameObject card = Instantiate(wordDexCardPrefab, wordDexCardParent);
                WordDexCard wordDexCard = card.GetComponent<WordDexCard>();
                card.name = word.word;

                wordDexCard.SetWordData(word);

                wordDexCardList.Add(card);
            }
        }


        /// <summary>
        /// Filter Words
        /// </summary>
        /// <param name="filter"></param>
        private void FilterWord(string filter) {
            foreach (GameObject card in wordDexCardList) {
                WordDexCard wordDexCard = card.GetComponent<WordDexCard>();

                if (filter.ToLower() == "all") {
                    if (card.activeSelf == false) {
                        card.SetActive(true);
                    }
                }
                else if (wordDexCard.GetWordType() != filter.ToLower()) {
                    card.SetActive(false);
                }
            }
        }
    }
}