//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class DestinationChecker : MonoBehaviour
{
    public Transform[] destinationPoints;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("NPC"))
        {
            transform.position = destinationPoints[Random.Range(0,destinationPoints.Length)].transform.position;
        }
    }
}
