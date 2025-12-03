using Events.DropContainer;
using Events.ScoreSystem;
using Events.Tutorial;
using EventSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tutorial {
    /// <summary>
    /// Container type
    /// </summary>
    public enum ContainerType {
        NOUN,
        VERB,
        ADJECTIVE
    }

    public class Tut_DropContainer : MonoBehaviour, IDropHandler {
        [SerializeField] private ContainerType containerType;

        /// <summary>
        /// Called when an object is dropped on the container
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrop(PointerEventData eventData) {
            List<RaycastResult> results = new List<RaycastResult>();

            UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);

            if (results.Count > 0 && results[0].gameObject == gameObject) {
                GameObject droppedObject = eventData.pointerDrag;
                Tut_Container wordContainer = droppedObject.GetComponent<Tut_Container>();

                if (wordContainer != null) {
                    if (wordContainer.wordData.wordType.ToLower() == containerType.ToString().ToLower()) {
                        wordContainer.ToggleWord(false, true);
                        EventBus.Publish(new Evt_OnSequenceTrigger());
                        EventBus.Publish(new Evt_OnCorrectCardDrop(1, 1, null));
                        EventBus.Publish(new Evt_OnCorrectAnswer());
                    }
                    else {
                        EventBus.Publish(new Evt_OnIncorrectAnswer());
                    }
                }
            }
        }
    }
}