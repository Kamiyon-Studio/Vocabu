using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    public class UIHoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [Header("Target Settings")]
        [Tooltip("Drag the child object 'chest' here")]
        [SerializeField] private Animator targetAnimator;

        [Header("Animation Parameters")]
        [Tooltip("The exact name of the Bool parameter in your Animator Controller")]
        [SerializeField] private string hoverBoolName = "IsHovering";

        public void OnPointerEnter(PointerEventData eventData) {
            if (targetAnimator != null) {
                targetAnimator.SetBool(hoverBoolName, true);
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (targetAnimator != null) {
                targetAnimator.SetBool(hoverBoolName, false);
            }
        }
    } 
}