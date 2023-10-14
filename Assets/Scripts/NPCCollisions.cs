//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class NPCCollisions : MonoBehaviour
{
    // Keeps track of which animation state is being played
    public string currentState;
    // Takes the refernce of the animator.
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Talking point"))
        {
            ChangeAnimationState("Talking 1");
        }
    }

    /// <summary>
    /// Changes the animations
    /// </summary>
    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);

        currentState = newState;
    }
}
