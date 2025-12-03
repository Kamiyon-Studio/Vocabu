using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WordSystem;

using EventSystem;
using Events.WordDex;

namespace UI.MainMenu.WordDex {
    public class WordInfoUI : MonoBehaviour {
        [Header("Word Info")]
        [SerializeField] private GameObject wordInfoPanel;
        
        [SerializeField] private Image icon;
        [SerializeField] private Image categoryIcon;
        [SerializeField] private TextMeshProUGUI wordTxt;
        [SerializeField] private TextMeshProUGUI descTxt;
        [SerializeField] private Button backButton;

        private bool isActive = false;

        private void Awake() {
            backButton.onClick.AddListener(() => {
                wordInfoPanel.SetActive(false);
                isActive = false;
            });
        }

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnWordDexCardClicked>(OnWordDexCardClicked);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnWordDexCardClicked>(OnWordDexCardClicked);
        }

        private void OnWordDexCardClicked(Evt_OnWordDexCardClicked evt) {
            SetWordInfo(evt.wordData);
        }

        public void SetWordInfo(WordData word) {
            if (isActive) return;

            isActive = true;
            wordInfoPanel?.SetActive(true);
            wordTxt.text = word.word;
            descTxt.text = word.description;

            Sprite iconSprite = Resources.Load<Sprite>(word.image_path);
            Sprite catSprite = Resources.Load<Sprite>($"Card Attrib/{word.category.ToLower()}");

            if (iconSprite != null) {
                icon.sprite = iconSprite;
            } else {
                icon.sprite = null;
            }

            if (catSprite != null) categoryIcon.sprite = catSprite;
        }
    }
}