using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;

public class Ball : MonoBehaviour
{   
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Batting Trigger"))
        {
            if (OutdoorGameManager.instance.kidOneBattingTurn == true)
            {
                OutdoorGameManager.instance.kidsBatting[0].GetComponent<navMove>().StartMovementWihtoutDelay();
                OutdoorGameManager.instance.kidsBatting[1].GetComponent<navMove>().StartMovementWihtoutDelay();
            }
            else if (OutdoorGameManager.instance.kidTwoBattingTurn == true)
            {
                OutdoorGameManager.instance.kidsBatting[0].GetComponent<navMove>().StartMovementWihtoutDelay();
                OutdoorGameManager.instance.kidsBatting[1].GetComponent<navMove>().StartMovementWihtoutDelay();
            }
        }
        else if (other.gameObject.CompareTag("Fielder Trigger"))
        {
            OutdoorGameManager.instance.ChangeAnimationStateFielder();
        }
    }
}
