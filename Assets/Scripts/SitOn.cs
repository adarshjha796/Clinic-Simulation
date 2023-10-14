using SWS;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MEC;

public class SitOn : MonoBehaviour
{
    [Header("GameObject")]
    // This will have the data of the NPC that will collide with this object.
    GameObject NPCTriggered;

    [Header("Transform")]
    Transform benchMovePoint;
    Transform benchSitPoint;

    [Header("Float")]
    private float stoppingDistanceOffset = 0.5f;

    [Header("Bool")]
    private bool isWalkingTowards = false;
    // This will allow that if npc can trigger on it or not. Enabling it after once used afters sometime only to just frequent collisions.
    private bool canTrigger = true;

    [Header("Animator")]
    private Animator anim;

    [Header("String")]
    public string currentState;

    [Header("References")]
    private NavMeshAgent NPCTriggeredAgent;

    private void Start()
    {
        benchMovePoint = gameObject.transform.GetChild(0);
        benchSitPoint = gameObject.transform.GetChild(1);
    }



    private void Update()
    {
        if(isWalkingTowards)
        {
            if ((benchSitPoint.transform.position - NPCTriggered.transform.position).sqrMagnitude <= NPCTriggeredAgent.stoppingDistance + stoppingDistanceOffset)
            {
                DisableComponents();
                ChangeAnimationState("Stand to sit");
                NPCTriggered.transform.SetPositionAndRotation(benchSitPoint.transform.position, benchMovePoint.transform.rotation);

                Timing.RunCoroutine(RestartTheMovement().CancelWith(gameObject));

                isWalkingTowards = false;
            }          
        }
    }



    private void OnTriggerEnter(Collider other)
    {   
        if(!canTrigger)
        {
            return;
        }

        if (other.gameObject.CompareTag("Outdoor NPC"))
        {
            canTrigger = false;
            NPCTriggered = other.gameObject;
            NPCTriggeredAgent = NPCTriggered.GetComponent<NavMeshAgent>();
            NPCTriggeredAgent.SetDestination(benchMovePoint.transform.position);

            isWalkingTowards = true;
        }
    }



    private void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim = NPCTriggered.GetComponent<Animator>();
  
        anim.Play(newState);

        currentState = newState;
    }



    private void DisableComponents()
    {
        NPCTriggeredAgent.updatePosition = false;
        NPCTriggeredAgent.updateRotation = false;
        NPCTriggered.GetComponent<navMove>().Stop();
        NPCTriggered.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = false;
    }



    private void EnableComponents()
    {
        // This will fix the NPC position while getting up so that he doesn't stick inside the stool
        NPCTriggered.GetComponent<navMove>().moveToPath = true;
        NPCTriggered.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;
        ChangeAnimationState("Sit to stand");

        Timing.RunCoroutine(ResetRotaionAndPosition().CancelWith(gameObject));
    }



    private IEnumerator<float> RestartTheMovement()
    {
        yield return Timing.WaitForSeconds(10f);

        EnableComponents();

        yield return Timing.WaitForSeconds(3f);

        NPCTriggered.GetComponent<navMove>().StartMove();
        ChangeAnimationState("Walk");

        Timing.RunCoroutine(ResetCanTrigger().CancelWith(gameObject));
    }



    private IEnumerator<float> ResetCanTrigger()
    {
        yield return Timing.WaitForSeconds(20f);

        canTrigger = true;
    }



    /// <summary>
    /// This will the agent rotaion and position and will again allow them to follow postion and rotation from the navmeshagaent componenet.
    /// </summary>
    private IEnumerator<float> ResetRotaionAndPosition()
    {
        yield return Timing.WaitForSeconds(3f);

        NPCTriggeredAgent.updatePosition = true;
        NPCTriggeredAgent.updateRotation = true;
        NPCTriggered.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;
    }
}
