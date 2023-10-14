//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Header("floats")]
    [SerializeField]
    private float targetTime;
    private float resetTargetTime;

    [Header("Colliders")]
    private BoxCollider boxCollider;

    [Header("booleans")]
    private bool startTimer;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        resetTargetTime = targetTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(startTimer)
        {
            targetTime -= Time.deltaTime;

            if (targetTime <= 0.0f)
            {
                TimerEnded();
                targetTime = resetTargetTime;
                startTimer = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (boxCollider.enabled == false)
        {
            boxCollider.enabled = true;
        }
        else
        {
            boxCollider.enabled = false;
        }
        startTimer = true;
    }


    /// <summary>
    /// This is responsiable to enable the collider once the timer stop.
    /// </summary>
    private void TimerEnded()
    {
        if(boxCollider.enabled == false)
        {
            boxCollider.enabled = true;
        }
        else
        {
            boxCollider.enabled = false;
        }
    }
}
