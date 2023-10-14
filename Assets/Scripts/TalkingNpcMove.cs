using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using MEC;

public class TalkingNpcMove : MonoBehaviour
{
    [Header("Int")]
    [SerializeField]
    private int npcCounter = 0;
    public Animator[] animator;
    public GameObject[] TriggerNpc;

    System.Action OnBothPlayersNeedToTalk;


    private void Start()
    {
        OnBothPlayersNeedToTalk += () =>
        {
            TriggerNpc[0].GetComponent<navMove>().updateRotation = false;
            TriggerNpc[1].GetComponent<navMove>().updateRotation = false;

            animator[0].SetBool("IsTalking", true);
            animator[1].SetBool("IsTalking", true);

            Timing.RunCoroutine(RestartMovement());
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Talking NPC"))
        {
            if (other.transform.GetChild(0).gameObject.name == "Genesis8Female")
            {
                npcCounter++;
                TriggerNpc[0] = other.gameObject; 
                animator[0] = other.gameObject.GetComponent<Animator>();


            }
            if (other.transform.GetChild(0).gameObject.name == "Genesis8Male")
            {
                npcCounter++;
                TriggerNpc[1] = other.gameObject;
                animator[1] = other.gameObject.GetComponent<Animator>();
            }

            if (npcCounter == 2)
            {
                OnBothPlayersNeedToTalk?.Invoke();
            }
        }                 
    }



    private IEnumerator<float> RestartMovement()
    {
        yield return Timing.WaitForSeconds(1);

        TriggerNpc[0].transform.localRotation = Quaternion.identity;
        TriggerNpc[1].transform.localRotation = Quaternion.Euler(0, -180, 0);

        yield return Timing.WaitForSeconds(120);

        TriggerNpc[0].GetComponent<navMove>().updateRotation = true;
        TriggerNpc[1].GetComponent<navMove>().updateRotation = true;

        animator[0].SetBool("IsTalking", false);
        animator[1].SetBool("IsTalking", false);

        npcCounter = 0;

        TriggerNpc[0].GetComponent<navMove>().StartMove();
        TriggerNpc[1].GetComponent<navMove>().StartMove();

        Debug.Log("Restart movement");

    }


}
