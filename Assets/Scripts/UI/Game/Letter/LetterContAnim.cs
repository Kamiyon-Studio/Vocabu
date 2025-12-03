using UnityEngine;

using EventSystem;
using Events.Letter;
using Events.WordContainer;

namespace UI.LetterObj {
	public class LetterContAnim : MonoBehaviour {
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
            EventBus.Subscribe<Evt_OnLetterDrag>(OnLetterDrag);
            EventBus.Subscribe<Evt_OnLetterDrop>(OnLetterDrop);

            EventBus.Subscribe<Evt_OnWordCardDrag>(OnWordDrag);
            EventBus.Subscribe<Evt_OnWordCardDrop>(OnWordDrop);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnLetterDrag>(OnLetterDrag);
            EventBus.Unsubscribe<Evt_OnLetterDrop>(OnLetterDrop);

            EventBus.Unsubscribe<Evt_OnWordCardDrag>(OnWordDrag);
            EventBus.Unsubscribe<Evt_OnWordCardDrop>(OnWordDrop);
        }

        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================
        private void OnLetterDrag(Evt_OnLetterDrag evt) { PlayDownAnim(); }

        private void OnLetterDrop(Evt_OnLetterDrop evt) { PlayUpAnim(); }

        private void OnWordDrag(Evt_OnWordCardDrag evt) { PlayDownAnim(); }

        private void OnWordDrop(Evt_OnWordCardDrop evt) { PlayUpAnim(); }



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