using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneSystem;

using ServiceLocator;
using ServiceLocator.Services;
using GameSystem.State;

using EventSystem;
using Events.InputSystem;
using Events.Tutorial;
using Events.GameSystem;

using TMPro;
namespace Tutorial {

    public enum TutorialProceedMethod {
        CLICK,
        TRIGGER
    }

    [System.Serializable]
    public class TutorialSequenceData {
        public int sequenceIndex;
        public int targetDialoguePanelIndex = 1;
        public TutorialProceedMethod proceedMethod;
        public string tutorialDesc;
        public GameObject[] objectsToActivate;
        public GameObject[] objectsToDeactivate;
    }

    public class TutorialManager : MonoBehaviour, ITutorialSystem {
        [Header("Tutorial Sequence")]
        public TutorialSequenceData[] tutorialSequence;

        [Header("Dialogue Panel")]
        [SerializeField] private GameObject dialoguePanel_1;
        [SerializeField] private GameObject dialoguePanel_2;
        [SerializeField] private TextMeshProUGUI dialoguePanelTxt_1;
        [SerializeField] private TextMeshProUGUI dialoguePanelTxt_2;

        private int currentSequenceIndex = -1;

        // Sequence Delay
        private float sequenceDelay = 0.1f;
        private bool canProceed = false;


        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            if (ServiceRegistry.IsRegistered<ITutorialSystem>()) {
                Debug.LogWarning("[TutorialManager] ITutorialSystem service is already registered.");
                Destroy(gameObject);
                return;
            }

            ServiceRegistry.Register<ITutorialSystem>(this);
        }

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnClick>(OnClickAction);
            EventBus.Subscribe<Evt_OnSequenceTrigger>(OnSequenceTrigger);
            EventBus.Subscribe<Evt_OnGameStateChange>(OnGameStateChange);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnClick>(OnClickAction);
            EventBus.Unsubscribe<Evt_OnSequenceTrigger>(OnSequenceTrigger);
            EventBus.Unsubscribe<Evt_OnGameStateChange>(OnGameStateChange);
        }

        private void Start() {
            canProceed = true;
            ShowNextSequence();
        }

        private void OnDestroy() {
            ServiceRegistry.Unregister<ITutorialSystem>(this);
        }

        // =====================================================================
        //
        //                          Events Methods
        //
        // =====================================================================
        private void OnClickAction(Evt_OnClick evt) {
            // Only proceed if the current sequence is meant to be advanced by a click.
            if (canProceed && currentSequenceIndex >= 0 && currentSequenceIndex < tutorialSequence.Length) {
                if (tutorialSequence[currentSequenceIndex].proceedMethod == TutorialProceedMethod.CLICK) {
                    ShowNextSequence();
                }
            }
        }

        private void OnSequenceTrigger(Evt_OnSequenceTrigger evt) {
            ShowNextSequenceByTrigger();
        }

        private void OnGameStateChange(Evt_OnGameStateChange evt) {
            if (evt.gameState == GameState.PAUSE) {
                canProceed = false;
            }
            else {
                canProceed = true;
            }
        }


        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================

        /// <summary>
        /// Advances the tutorial if the current sequence's proceed method is TRIGGER.
        /// </summary>
        public void ShowNextSequenceByTrigger() {
            if (canProceed && currentSequenceIndex >= 0 && currentSequenceIndex < tutorialSequence.Length) {
                if (tutorialSequence[currentSequenceIndex].proceedMethod == TutorialProceedMethod.TRIGGER) {
                    ShowNextSequence();
                }
            }
        }

        /// <summary>
        /// Advances the tutorial to the next sequence.
        /// </summary>
        private void ShowNextSequence() {
            if (!canProceed) return;

            currentSequenceIndex++;

            if (currentSequenceIndex >= tutorialSequence.Length) {
                if (PlayerPrefs.GetInt("CompletedTutorial") == 0) {
                    SceneLoader.SCENE_TO_LOAD = "Game";
                    SceneManager.LoadScene("Loading");

                    PlayerPrefs.SetInt("CompletedTutorial", 1);
                }
                else {
                    SceneLoader.SCENE_TO_LOAD = "MainMenu";
                    SceneManager.LoadScene("Loading");
                }
                return;
            }

            canProceed = false;
            ApplySequence(tutorialSequence[currentSequenceIndex]);
            StartCoroutine(SequenceDelay());
        }

        private void ApplySequence(TutorialSequenceData data) {
            if (data == null) {
                Debug.LogError("Tutorial sequence data is null.");
                return;
            }

            switch (data.targetDialoguePanelIndex) {
                case 1:
                    dialoguePanel_1.SetActive(true);
                    dialoguePanel_2.SetActive(false);
                    dialoguePanelTxt_1.text = data.tutorialDesc;
                    break;
                case 2:
                    dialoguePanel_1.SetActive(false);
                    dialoguePanel_2.SetActive(true);
                    dialoguePanelTxt_2.text = data.tutorialDesc;
                    break;
                default:
                    dialoguePanel_1.SetActive(false);
                    dialoguePanel_2.SetActive(false);
                    break;
            }

            // Activate objects
            if (data.objectsToActivate != null) {
                foreach (GameObject obj in data.objectsToActivate) {
                    if (obj != null) {
                        obj.SetActive(true);
                    }
                }
            }

            // Deactivate objects
            if (data.objectsToDeactivate != null) {
                foreach (GameObject obj in data.objectsToDeactivate) {
                    if (obj != null) {
                        obj.SetActive(false);
                    }
                }
            }

            EventBus.Publish(new Evt_OnSequenceChange(data.sequenceIndex));
        }

        private IEnumerator SequenceDelay() {
            yield return new WaitForSeconds(sequenceDelay);
            canProceed = true;
        }
    }
}