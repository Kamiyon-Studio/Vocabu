using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

using EventSystem;
using Events.Letter;
using Events.Tutorial;

namespace Tutorial {
    public class Tut_Letter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [Header("References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private TextMeshProUGUI letterText;
        [SerializeField] private GameObject selectedParent;
        [SerializeField] private int sequenceNumberToEnable;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        private GameObject defaultParent;
        private Vector2 defaultPosition;

        private bool canDrag = false;

        public string letter { get; private set; }


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
            EventBus.Subscribe<Evt_OnSequenceChange>(OnSequenceChange);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnSequenceChange>(OnSequenceChange);
        }


        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================
        private void OnSequenceChange(Evt_OnSequenceChange evt) {
            if (evt.sequenceNumber == sequenceNumberToEnable) {
                canDrag = true;
            }
        }

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
            if (!canDrag) return;

            canvasGroup.blocksRaycasts = false;
            transform.SetParent(selectedParent.transform);

            EventBus.Publish(new Evt_OnLetterDrag());
        }


        /// <summary>
        /// Called when the user drags the letter
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData) {
            if (!canDrag) return;
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }


        /// <summary>
        /// Called when the user stops dragging the letter
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData) {

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