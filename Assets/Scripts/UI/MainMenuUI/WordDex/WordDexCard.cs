using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using WordSystem;
using EventSystem;
using Events.WordDex;

namespace UI.MainMenu.WordDex {
	public class WordDexCard : MonoBehaviour, IPointerClickHandler {
		[Header("Card References")]
		[SerializeField] private Image icon;
		[SerializeField] private TextMeshProUGUI wordText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private Image categoryIcon;

        private WordData wordData;

        // =====================================================================
        //
        //                          Inteface Methods
        //
        // =====================================================================
        public void OnPointerClick(PointerEventData eventData) {
            EventBus.Publish(new Evt_OnWordDexCardClicked(wordData));
            EventBus.Publish(new Evt_OnWordDexCardClicked(wordData));
        }


        // =====================================================================
        //
        //                          Public Methods
        //
        // =====================================================================
        /// <summary>
        /// Sets the word data
        /// </summary>
        /// <param name="wordData"></param>
        public void SetWordData(WordData wordData) {
            this.wordData = wordData;
            wordText.text = wordData.word;
            descText.text = wordData.description;

            string catName = wordData.category.ToLower();

            Sprite iconPath = Resources.Load<Sprite>(wordData.image_path);
            Sprite catImg = Resources.Load<Sprite>($"Card Attrib/{catName}");

            if (iconPath != null) {
                icon.sprite = iconPath;
            } else {
                icon.sprite = null;
            }

            if (catImg != null) categoryIcon.sprite = catImg;
        }

        /// <summary>
        /// Returns the word type
        /// </summary>
        /// <returns></returns>
        public string GetWordType() => wordData.wordType.ToLower();
    }
}