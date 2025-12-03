using UnityEngine;
using UnityEngine.EventSystems;
using EventSystem;
using Events.ClickToStartUI;
using Events.DropContainer;

namespace UI.ClickToStartUI {
    public class ClickToStartUI_Script : MonoBehaviour, IPointerClickHandler {
        [Header("UI")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Animator panelAnimator;
        [SerializeField] private string playTrigger = "Play";

        private bool clicked;

        private void Start() {
            panel.SetActive(true);
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (clicked) return;
            clicked = true;
            
            // Direct Play is more reliable for UI than triggers
            panelAnimator.Play("panel"); 
            EventBus.Publish(new Evt_OnClickToStart());
            
            StartCoroutine(WaitAndPublish());
        }

        private System.Collections.IEnumerator WaitAndPublish() {
            // CRITICAL: Wait 1 frame for Animator to switch to the "panel" state
            yield return null; 
            
            // Now this grabs the correct length of the "panel" clip
            var state = panelAnimator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(state.length);
            
            EventBus.Publish(new Evt_OnClickToStart());
            
            // Explicitly deactivate if that is the intended end behavior
            gameObject.SetActive(false); 
        }
    }
}