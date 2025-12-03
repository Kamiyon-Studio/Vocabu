using UnityEngine;

using EventSystem;
using Events.ScoreSystem;
using System.Collections;

namespace UI.StatsUI {
    public class TeacherImgAnim : MonoBehaviour {

        [SerializeField] private AnimationClip idleAnimation;
        [SerializeField] private AnimationClip winAnimation;
        [SerializeField] private AnimationClip loseAnimation;

        [Tooltip("Delay before switching to idle animation")]
        [SerializeField] private float animationPeriod = 1f;

        private Animator teacherAnimator;
        private Coroutine resetAnimCoroutine;


        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // ===================================================================== 
        private void Awake() {
            teacherAnimator = GetComponent<Animator>();
        }

        private void Start() {
            PlayAnimation(idleAnimation.name);
        }

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnCorrectAnswer>(OnCorrectAnswer);
            EventBus.Subscribe<Evt_OnIncorrectAnswer>(OnIncorrectAnswer);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnCorrectAnswer>(OnCorrectAnswer);
            EventBus.Unsubscribe<Evt_OnIncorrectAnswer>(OnIncorrectAnswer);
        }


        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================
        private void OnCorrectAnswer(Evt_OnCorrectAnswer evt) => PlayAnimation(winAnimation.name);
        private void OnIncorrectAnswer(Evt_OnIncorrectAnswer evt) => PlayAnimation(loseAnimation.name);



        // =====================================================================
        //
        //                              Methods
        //
        // =====================================================================
        private void PlayAnimation(string animClip) {
            if (resetAnimCoroutine != null) StopCoroutine(resetAnimCoroutine);

            teacherAnimator.Play(animClip);

            resetAnimCoroutine = StartCoroutine(ResetAnim());
        }

        private IEnumerator ResetAnim() {
            yield return new WaitForSeconds(animationPeriod);
            PlayAnimation(idleAnimation.name);
        }
    }
}