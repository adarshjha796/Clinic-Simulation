using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoTweenManager : MonoBehaviour
{
    [SerializeField] private Transform resetPosition;
    [SerializeField] private Transform toiletPosition;
    [SerializeField] private Transform resetPositionBasin;
    [SerializeField] private Transform basinPosition;

    // This will be assigned in runtime.
    //[SerializeField]
    private Transform target;

    [SerializeField] private float speed;
  
    public bool canMove;

    private void Update()
    {
        if(canMove)
        {
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            if(transform.position == target.position)
            {
                canMove = false;
            }
        }
    }



    /// <summary>
    /// Moves the object from one point to another.
    /// </summary>
    public void ReachedDestination()
    {
        target = toiletPosition;
    }



    /// <summary>
    /// Resets the position of the object.
    /// </summary>
    public void ResetPosition()
    {
        target = resetPosition;
    }



    public void ReachDestinationBasin()
    {
        target = basinPosition;
    }



    public void ResetPositionBasin()
    {
        target = resetPositionBasin;
    }
}
