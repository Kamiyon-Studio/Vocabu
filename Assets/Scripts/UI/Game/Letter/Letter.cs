using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

using EventSystem;
using Events.Letter;
using Events.GameSystem;

using GameSystem.State;
using Events.WordContainer;

namespace UI.LetterObj {
    public class Letter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [Header("References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private TextMeshProUGUI letterText;
        [SerializeField] private GameObject selectedParent;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        private GameObject defaultParent;
        private Vector2 defaultPosition;
        public string letter { get; private set; }

        private GameState gameState;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            letter = gameObject.name;
            letterText.text = letter;
            defaultPosition = rectTransform.anchoredPosition;
            defaultParent = transform.parent.gameObject;
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
        private void OnGameStateChange(Evt_OnGameStateChange evt) { gameState = evt.gameState; }


        // =====================================================================
        //
        //                          Interface Methods
        //
        // =====================================================================


        /// <summary>
        /// Called when the user starts dragging the letter
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData) {
            if (gameState != GameState.PLAYING) return;

            canvasGroup.blocksRaycasts = false;
            transform.SetParent(selectedParent.transform);

            EventBus.Publish(new Evt_OnLetterDrag());
            // Debug.Log("SEGGGGGGGGGS" + letterText.text);
        }


        /// <summary>
        /// Called when the user drags the letter
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData) {
            if (gameState != GameState.PLAYING) return;

            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }


        /// <summary>
        /// Called when the user stops dragging the letter
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData) {
            if (gameState != GameState.PLAYING) return;

            transform.SetParent(defaultParent.transform);
            rectTransform.anchoredPosition = defaultPosition;
            EventBus.Publish(new Evt_OnLetterDrop());

            StartCoroutine(ReenableRaycasts());
        }

        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================

        /// <summary>
        /// Reenables raycasts
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReenableRaycasts() {
            yield return null;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
