using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using ServiceLocator;
using ServiceLocator.Services;


namespace WordSystem {
    /// <summary>
    /// The difficulty of a word
    /// </summary>
    [System.Serializable]
    public enum WordDifficulty {
        Easy,
        Medium,
        Hard
    }


    /// <summary>
    /// A single word
    /// </summary>
    [System.Serializable]
    public class WordData {
        public string word;
        public string difficulty;
        public string category;
        public string image_path;
        public string description;
        public string wordType;
        public string[] example_usage;
    }


    /// <summary>
    /// A list of words
    /// </summary>
    [System.Serializable]
    internal class WordListWrapper {
        public WordData[] items;
    }



    public class WordManager : MonoBehaviour, IWordSystem {

        private List<WordData> nouns = new List<WordData>();
        private List<WordData> verbs = new List<WordData>();
        private List<WordData> adjectives = new List<WordData>();
        private List<WordData> allWords = new List<WordData>();

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            if (ServiceRegistry.IsRegistered<IWordSystem>()) {
                Debug.LogWarning("[Word Manager] WordManager service is already registered.");
                Destroy(gameObject);
                return;
            }

            ServiceRegistry.Register<IWordSystem>(this);
            LoadWords();
        }

        private void OnDestroy() {
            if (ServiceRegistry.IsRegistered<IWordSystem>()) {
                ServiceRegistry.Unregister<IWordSystem>(this); 
            }
        }

        // =====================================================================
        //
        //                          Inteface Methods
        //
        // =====================================================================

        /// <summary>
        /// Gets a list of random words of a specified difficulty from all categories.
        /// </summary>
        /// <param name="difficulty">The difficulty of words to retrieve (e.g., "easy", "medium", "hard").</param>
        /// <param name="count">The number of words to return.</param>
        /// <returns>A list of WordData objects.</returns>
        public List<WordData> GetNextWords(string difficulty, int count) {
            List<WordData> filteredWords = allWords.Where(w => w.difficulty.ToLower() == difficulty.ToLower()).ToList();

            if (filteredWords.Count < count) {
                Debug.LogWarning($"[WordManager] Not enough words of difficulty '{difficulty}' across all categories to return {count} words. Returning {filteredWords.Count} words.");
                // Shuffle and return what we have
                return filteredWords.OrderBy(x => Random.value).ToList();
            }

            // Randomly select 'count' words without repetition
            return filteredWords.OrderBy(x => Random.value).Take(count).ToList();
        }


        public List<WordData> GetAllWords(bool alphabeticalOrder = false) {
            if (alphabeticalOrder) {
                return allWords.OrderBy(w => w.word).ToList();
            } else {
                return allWords;
            }
        }


        // =====================================================================
        //
        //                          Private Methods
        //
        // =====================================================================

        /// <summary>
        /// Loads the words
        /// </summary>
        private void LoadWords() {
            nouns = LoadWordList("Words JSON/nouns", "noun");
            verbs = LoadWordList("Words JSON/verbs", "verb");
            adjectives = LoadWordList("Words JSON/adjectives", "adjective");

            allWords.Clear();
            allWords.AddRange(nouns);
            allWords.AddRange(verbs);
            allWords.AddRange(adjectives);
        }


        /// <summary>
        /// Loads a word list
        /// </summary>
        /// <param name="path"></param>
        /// <param name="wordType"></param>
        /// <returns></returns>
        private List<WordData> LoadWordList(string path, string wordType) {
            TextAsset jsonAsset = Resources.Load<TextAsset>(path);
            if (jsonAsset == null) {
                Debug.LogError($"[WordManager] Could not load word list from path: {path}");
                return new List<WordData>();
            }

            // JsonUtility doesn't support top-level arrays, so we wrap it in an object.
            string jsonString = "{ \"items\": " + jsonAsset.text + "}";
            WordListWrapper wrapper = JsonUtility.FromJson<WordListWrapper>(jsonString);

            if (wrapper == null || wrapper.items == null) {
                Debug.LogError($"[WordManager] Failed to parse word list from path: {path}");
                return new List<WordData>();
            }

            List<WordData> wordList = new List<WordData>(wrapper.items);
            foreach (var wordData in wordList) {
                wordData.wordType = wordType;
            }

            return wordList;
        }
    }
}