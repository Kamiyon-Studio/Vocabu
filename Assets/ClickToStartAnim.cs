using UnityEngine;

using EventSystem;
using Events.ScoreSystem;
using System.Collections;
using System;



public class ClickToStartAnim : MonoBehaviour
{   
    private Animator startAnimator;

    private void Awake() {
            startAnimator = GetComponent<Animator>();
            startAnimator.Play("Start");
        }

}
