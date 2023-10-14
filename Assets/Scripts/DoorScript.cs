using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private bool opened = false;
    public  Animator anim;

    void Pressed()
    {
        //This will set the bool the opposite of what it is.
        opened = !opened;

        //This line will set the bool true so it will play the animation.
        anim.SetBool("Opened", !opened);
    }
}