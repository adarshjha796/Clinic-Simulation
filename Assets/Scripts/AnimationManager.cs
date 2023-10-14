//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    // Keeps track of which animation state is being played
    public string currentState;
    // Takes the refernce of the animator.
    public Animator dogAnimator;
    // This will change and always have the current animation on which animations are needed to be played.
    private Animator currentAnimator;

    public void DogPat()
    {
        currentAnimator = dogAnimator;
        ChangeAnimationState("Duschand Dog Bed final animation");
    }

    public void DogJump()
    {
        currentAnimator = dogAnimator;
        ChangeAnimationState("Duschand Dog Jump Final animation");
    }

    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        currentAnimator.Play(newState);

        currentState = newState;
    }
}
