using DG.Tweening;
using Events.GameSystem;
using Events.ScoreSystem;
using Events.WordContainer;
using Events.WordSystem;
using EventSystem;
using GameSystem.State;
using System.Collections;
using TMPro;
using UI.LetterObj;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WordSystem;

namespace UI.CointainerUI {
    public class Container : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {
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

        [Header("Score")]
        [SerializeField] private int correctLetterScore = 10;

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
        private string missingLetter;

        public WordData wordData { get; private set; }
        public int cardMultiplier { get; private set; }
        private bool isWordComplete = false;

        private GameState gameState;

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
        private void OnGameStateChange(Evt_OnGameStateChange evt) {
            gameState = evt.gameState;

            if (gameState == GameState.GAMEOVER) {
                ToggleWord(false);
            }
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
            Letter letterObj = dropped.GetComponent<Letter>();

            if (letterObj != null) {
                if (letterObj.letter.ToLower() == missingLetter.ToLower()) {
                    // Clear old letters under letterParent
                    foreach (Transform child in letterParent)
                        Destroy(child.gameObject);

                    GameObject obj = Instantiate(wordTxtPrebab, letterParent);
                    TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
                    tmp.text = originalWord;
                    isWordComplete = true;

                    EventBus.Publish(new Evt_OnCorrectLetterDrop(correctLetterScore));
                }
                else {
                    EventBus.Publish(new Evt_OnIncorrectAnswer());
                    EventBus.Publish(new Evt_OnIncorrectLetterDrop());
                }
            }
        }


        /// <summary>
        /// Called when an object starts to be dragged
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData) {
            if (gameState != GameState.PLAYING) return;
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
            if (gameState != GameState.PLAYING) return;
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


        public void OnPointerEnter(PointerEventData eventData)
        {
            //if (!isWordComplete)
            //{
            //    rotationTween?.Kill();

            //    Vector2 localPoint;
            //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //        rectTransform,
            //        eventData.position,
            //        canvas.worldCamera,
            //        out localPoint
            //    );

            //    float normalizedX = Mathf.Clamp(localPoint.x / (rectTransform.rect.width * 0.5f), -1f, 1f);
            //    float normalizedY = Mathf.Clamp(localPoint.y / (rectTransform.rect.height * 0.5f), -1f, 1f);

            //    float tiltX = -normalizedY * maxTilt;
            //    float tiltY = normalizedX * maxTilt;

            //    Sequence seq = DOTween.Sequence();
            //    seq.Append(rectTransform.DOLocalRotate(new Vector3(tiltX, tiltY, 0f), returnSpeed).SetEase(Ease.OutQuad));
            //    rotationTween = seq;
            //}
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //if (!isWordComplete)
            //{
            //    rotationTween?.Kill();

            //    Sequence seq = DOTween.Sequence();
            //    seq.Append(rectTransform.DOLocalRotate(Vector3.zero, returnSpeed).SetEase(Ease.OutQuad));
            //    rotationTween = seq;
            //}
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

        private void SetRandomMultiplier() {
            cardMultiplier = Random.Range(1, maxMultiplier + 1);
            multiplierTxt.text = cardMultiplier.ToString();
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