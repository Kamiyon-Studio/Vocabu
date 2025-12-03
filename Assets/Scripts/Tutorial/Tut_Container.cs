using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

using WordSystem;

using Events.WordSystem;
using Events.WordContainer;
using Events.Tutorial;
using EventSystem;
using Events.ScoreSystem;


namespace Tutorial {
	public class Tut_Container : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
        [Header("References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject selectedParent;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image wordImage;

        [Header("Tweens Settings")]
        [SerializeField] private float rotationMultiplier = 0.15f;
        [SerializeField] private float returnSpeed = 0.3f;
        [SerializeField] private float maxTilt = 8f;
        [SerializeField] private float liftAmount = 25f;
        [SerializeField] private float liftDuration = 0.25f;

        [Header("Card Attribute")]
        [SerializeField] private GameObject cardAttribObject;
        [SerializeField, Min(1)] private int maxMultiplier = 3;
        [SerializeField] private TextMeshProUGUI multiplierTxt;
        [SerializeField] private Image categoryImg;

        [Header("WordTxt References")]
        [SerializeField] private GameObject wordTxtPrebab;
        [SerializeField] private GameObject blankImgPrebab;
        [SerializeField] private Transform letterParent;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private GameObject defaultParent;

        private Vector2 defaultPosition;
        private Vector2 lastPointerPosition;
        private Tween rotationTween;
        private Tween liftTween;

        private string originalWord;
        private string displayWord;
        private string missingLetter;

        public WordData wordData { get; private set; }
        public int cardMultiplier { get; private set; }

        private bool isWordComplete = false;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            defaultPosition = rectTransform.anchoredPosition;
            defaultParent = transform.parent.gameObject;

            ToggleWord(false);
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

        }


        // =====================================================================
        //
        //                          Interface Methods
        //
        // =====================================================================

        /// <summary>
        /// Called when an object is dropped on the container
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrop(PointerEventData eventData) {
            if (isWordComplete) return;

            GameObject dropped = eventData.pointerDrag;
            Tut_Letter letterObj = dropped.GetComponent<Tut_Letter>();

            if (letterObj != null) {
                if (letterObj.letter.ToLower() == missingLetter.ToLower()) {
                    // Clear old letters under letterParent
                    foreach (Transform child in letterParent)
                        Destroy(child.gameObject);

                    GameObject obj = Instantiate(wordTxtPrebab, letterParent);
                    TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
                    tmp.text = originalWord;
                    isWordComplete = true;
                    EventBus.Publish(new Evt_OnSequenceTrigger());
                    EventBus.Publish(new Evt_OnCorrectLetterDrop(1));
                    EventBus.Publish(new Evt_OnCorrectAnswer());
                } else {
                    EventBus.Publish(new Evt_OnIncorrectAnswer());
                }
            }
        }


        /// <summary>
        /// Called when an object starts to be dragged
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData) {
            if (!isWordComplete) return;
            canvasGroup.blocksRaycasts = false;

            transform.SetParent(selectedParent.transform);
            EventBus.Publish(new Evt_OnWordCardDrag());

            lastPointerPosition = eventData.position;

            liftTween?.Kill();
            liftTween = rectTransform.DOAnchorPos(new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + liftAmount), liftDuration).SetEase(Ease.OutQuad);

        }


        /// <summary>
        /// Called when an object is being dragged
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData) {
            if (!isWordComplete) return;

            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

            Vector2 dragDelta = eventData.position - lastPointerPosition;
            lastPointerPosition = eventData.position;

            float targetZ = Mathf.Clamp(-dragDelta.x * rotationMultiplier, -maxTilt, maxTilt);
            float targetX = Mathf.Clamp(dragDelta.y * 0.1f, -5f, 5f);

            rotationTween?.Kill();
            rotationTween = rectTransform.DORotate(new Vector3(0, 0, targetZ), returnSpeed, RotateMode.Fast).SetEase(Ease.OutQuad);
        }


        /// <summary>
        /// Called when an object stops being dragged
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData) {
            if (!isWordComplete) return;
            transform.SetParent(defaultParent.transform);
            rectTransform.anchoredPosition = defaultPosition;
            EventBus.Publish(new Evt_OnWordCardDrop());

            rotationTween?.Kill();
            liftTween?.Kill();

            Sequence resetSeq = DOTween.Sequence();
            resetSeq.Append(rectTransform.DOLocalRotate(Vector3.zero, returnSpeed).SetEase(Ease.OutBack))
                    .Join(rectTransform.DOAnchorPos(defaultPosition, 0.3f).SetEase(Ease.InOutQuad));

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


        // =====================================================================
        //
        //                          Getters and Setters
        //
        // =====================================================================

        /// <summary>
        /// Sets the word
        /// </summary>
        /// <param name="word"></param>
        public void SetWord(WordData word) {
            wordData = word;
            originalWord = word.word;

            int missingIndex = Random.Range(0, originalWord.Length);

            missingLetter = originalWord[missingIndex].ToString().ToLower();

            char[] wordChars = originalWord.ToCharArray();
            wordChars[missingIndex] = '_';

            for (int i = 0; i < originalWord.Length; i++) {
                if (i == missingIndex) {
                    Instantiate(blankImgPrebab, letterParent);
                }
                else {
                    GameObject letterObj = Instantiate(wordTxtPrebab, letterParent);
                    TextMeshProUGUI tmp = letterObj.GetComponent<TextMeshProUGUI>();
                    tmp.text = originalWord[i].ToString();
                }
            }

            descText.text = word.description;
            SetRandomMultiplier();

            // Update image
            Sprite wordImg = Resources.Load<Sprite>(word.image_path);

            string catName = wordData.category.ToLower();
            Sprite catImg = Resources.Load<Sprite>($"Card Attrib/{catName}");

            if (wordImg != null) {
                wordImage.sprite = wordImg;
            }
            else {
                wordImage.sprite = null;
            }

            if (catImg != null) {
                categoryImg.sprite = catImg;
            }
            else {
                categoryImg.sprite = null;
            }
        }

        private void SetRandomMultiplier() {
            cardMultiplier = Random.Range(1, maxMultiplier + 1);
            multiplierTxt.text = cardMultiplier.ToString();
        }

        /// <summary>
        /// Toggles the word50
        /// </summary>
        /// <param name="value"></param>
        public void ToggleWord(bool value, bool invokeEvent = false) {
            if (!value) {
                cardImage.enabled = false;
                wordImage.enabled = false;
                categoryImg.enabled = false;

                isWordComplete = false;
                wordData = null;
                wordImage.sprite = null;

                descText.text = "";
                multiplierTxt.text = "";
                originalWord = "";
                missingLetter = "";
                cardMultiplier = 1;

                cardAttribObject.SetActive(false);

                // Clear old letters under letterParent
                foreach (Transform child in letterParent)
                    Destroy(child.gameObject);

                transform.SetParent(defaultParent.transform);
                rectTransform.anchoredPosition = defaultPosition;

                rotationTween?.Kill();
                liftTween?.Kill();

                Sequence resetSeq = DOTween.Sequence();
                resetSeq.Append(rectTransform.DOLocalRotate(Vector3.zero, returnSpeed).SetEase(Ease.OutBack))
                        .Join(rectTransform.DOAnchorPos(defaultPosition, 0.3f).SetEase(Ease.InOutQuad));

                StartCoroutine(ReenableRaycasts());

                if (invokeEvent) {
                    EventBus.Publish(new Evt_OnWordCardDrop());
                    EventBus.Publish(new Evt_OnWordCompleted());
                }

                return;
            }

            cardImage.enabled = value;
            wordImage.enabled = value;
            categoryImg.enabled = value;
            cardAttribObject.SetActive(value);
        }
    }
}