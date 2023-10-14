using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaschundDog : MonoBehaviour
{
    private Animator animator;

    private string currentState;

    void Start()
    {
        animator = GetComponent<Animator>();
        ChangeAnimationState("Lay01 animation");
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);

        currentState = newState;
    }
}
