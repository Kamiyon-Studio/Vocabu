using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UI.CointainerUI;
using GameSystem.State;

using EventSystem;
using Events.DropContainer;
using Events.GameSystem;

namespace UI.DropContainerUI {
    /// <summary>
    /// Container type
    /// </summary>
    public enum ContainerType {
        NOUN,
        VERB,
        ADJECTIVE
    }

    public class DropContainer : MonoBehaviour, IDropHandler {
        [SerializeField] private ContainerType containerType;
        [SerializeField] private int correctCardScore = 20;

        private GameState gameState;
        private void OnEnable() {
            EventBus.Subscribe<Evt_OnGameStateChange>(OnGameStateChange);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnGameStateChange>(OnGameStateChange);
        }

        private void OnGameStateChange(Evt_OnGameStateChange evt) {
            gameState = evt.gameState;
        }

        /// <summary>
        /// Called when an object is dropped on the container
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrop(PointerEventData eventData) {
            if (gameState != GameState.PLAYING) return;

            List<RaycastResult> results = new List<RaycastResult>();

            UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);

            if (results.Count > 0 && results[0].gameObject == gameObject) {
                GameObject droppedObject = eventData.pointerDrag;
                Container wordContainer = droppedObject.GetComponent<Container>();

                if (wordContainer != null) {
                    if (wordContainer.wordData.wordType.ToLower() == containerType.ToString().ToLower()) {
                        EventBus.Publish(new Evt_OnCorrectCardDrop(correctCardScore, wordContainer.cardMultiplier, wordContainer.wordData.wordType));
                    }
                    else {
                        EventBus.Publish(new Evt_OnIncorrectCardDrop());
                    }

                    wordContainer.ToggleWord(false, true);
                }
            }
        }
    }
}