using UnityEngine;

using EventSystem;
using Events.Tutorial;

namespace Tutorial {
    public class LetterContAnim : MonoBehaviour {
        [Header("Animations")]
        [SerializeField] private AnimationClip upClip;
        [SerializeField] private AnimationClip downClip;

        [SerializeField] private int sequenceNumberToShow;
        [SerializeField] private int sequenceNumberToHide;

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
            if (evt.sequenceNumber == sequenceNumberToShow) PlayUpAnim();
            else if (evt.sequenceNumber == sequenceNumberToHide) PlayDownAnim();
        }


        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================
        private void PlayUpAnim() {
            animator.Play(upClip.name);
        }

        private void PlayDownAnim() {
            animator.Play(downClip.name);
        }
    }
}