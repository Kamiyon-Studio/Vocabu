using UnityEngine;
using EventSystem;
using Events.WordContainer;


namespace UI.DropContainerUI {
    public class DropContainerAnim : MonoBehaviour {
        [Header("Animations")]
        [SerializeField] private AnimationClip upClip;
        [SerializeField] private AnimationClip downClip;

        private Animator animator;


        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            animator = GetComponent<Animator>();
        }

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnWordCardDrag>(OnWordCardDrag);
            EventBus.Subscribe<Evt_OnWordCardDrop>(OnWordCardDrop);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnWordCardDrag>(OnWordCardDrag);
            EventBus.Unsubscribe<Evt_OnWordCardDrop>(OnWordCardDrop);
        }

        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================
        private void OnWordCardDrag(Evt_OnWordCardDrag evt) {
            animator.Play(upClip.name);
        }

        private void OnWordCardDrop(Evt_OnWordCardDrop evt) {
            animator.Play(downClip.name);
        }
    }
}