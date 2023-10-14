//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.AI;

public class Teleporter : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject StartTeleportorLeft;
    public GameObject StartTeleportorRight;
    public GameObject TeleportToLeft;
    public GameObject TeleportToRight;
    public GameObject TeleportPoints;
    private GameObject triggeredObject;

    [Header("int")]
    private int spawnIndexPoints;
    private int rotationSpeed = 3;
    private int walkingSpeed = 70;

    [Header("bool")]
    
    private bool isRotatingTowards = false;
    private bool isWalkingTowards = false;

    [Header("Strings")]
    public string currentState;
    private string rotateDirection;
    private string PathName;

    [Header("Animator")]
    private Animator animator;

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            spawnIndexPoints = Random.Range(0, TeleportPoints.transform.childCount); /// Set the index number randomly

            StartTeleportorLeft.transform.parent.position = TeleportPoints.transform.GetChild(spawnIndexPoints).position;
            StartTeleportorRight.transform.parent.position = TeleportPoints.transform.GetChild(spawnIndexPoints).position;
            StartTeleportorLeft.transform.parent.rotation = TeleportPoints.transform.GetChild(spawnIndexPoints).transform.localRotation;
            StartTeleportorRight.transform.parent.rotation = TeleportPoints.transform.GetChild(spawnIndexPoints).transform.localRotation;

            spawnIndexPoints = Random.Range(0, TeleportPoints.transform.childCount); /// Set the index number randomly

            TeleportToLeft.transform.parent.position = TeleportPoints.transform.GetChild(spawnIndexPoints).position;
            TeleportToRight.transform.parent.position = TeleportPoints.transform.GetChild(spawnIndexPoints).position;
            TeleportToLeft.transform.parent.rotation = TeleportPoints.transform.GetChild(spawnIndexPoints).transform.localRotation;
            TeleportToRight.transform.parent.rotation = TeleportPoints.transform.GetChild(spawnIndexPoints).transform.localRotation;
        }
    }


    private void Update()
    {
        if(isRotatingTowards)
        {
            // The step size is equal to speed times frame time.
            float step = rotationSpeed * Time.deltaTime;
            if (rotateDirection == "Right")
            {
                triggeredObject.transform.localRotation = Quaternion.Lerp(triggeredObject.transform.localRotation, Quaternion.Euler(0, 90, 0), step);
            }
            else if (rotateDirection == "Left")
            {
                triggeredObject.transform.localRotation = Quaternion.Lerp(triggeredObject.transform.localRotation, Quaternion.Euler(0, -90, 0), step);
            }
        }

        if(isWalkingTowards)
        {
            triggeredObject.GetComponent<Rigidbody>().isKinematic = false;
            triggeredObject.GetComponent<Rigidbody>().velocity = walkingSpeed * Time.deltaTime * Vector3.right;
        }
    }



    private void OnTriggerEnter(Collider collision)
    {
        if(OutdoorGameManager.instance.canTriggerForTeleport == false)
        {
            return;
        }
        if (StartTeleportorLeft.CompareTag("Teleporter") && collision.gameObject.CompareTag("Outdoor NPC"))
        {
            if (gameObject.name == "Start Right")
            {
                triggeredObject = collision.gameObject;
                int spawnIndexPoints = Random.Range(0, TeleportPoints.transform.childCount); /// Set the index number randomly

                StartTeleportorLeft.transform.parent.SetPositionAndRotation(TeleportPoints.transform.GetChild(spawnIndexPoints).position, TeleportPoints.transform.GetChild(spawnIndexPoints).localRotation);
                animator = collision.gameObject.GetComponent<Animator>();
                PathName = collision.GetComponent<SWS.navMove>().pathContainer.name;
                rotateDirection = "Right";
                DisableComponents(triggeredObject);
                isRotatingTowards = true;

                Timing.RunCoroutine(StopRotation().CancelWith(gameObject));

                ChangeAnimationState("Turn Right");

                Timing.RunCoroutine(ChangePositionOfNPC(triggeredObject, TeleportToLeft).CancelWith(gameObject));
                Timing.RunCoroutine(ResetCanTriggerVariable().CancelWith(gameObject));
            }

            else if ( gameObject.name == "Start Left")
            {
                triggeredObject = collision.gameObject;
                int spawnIndexPoints = Random.Range(0, TeleportPoints.transform.childCount); /// Set the index number randomly

                StartTeleportorRight.transform.parent.SetPositionAndRotation(TeleportPoints.transform.GetChild(spawnIndexPoints).position, TeleportPoints.transform.GetChild(spawnIndexPoints).localRotation);
                animator = collision.gameObject.GetComponent<Animator>();
                PathName = collision.GetComponent<SWS.navMove>().pathContainer.name;
                rotateDirection = "Left";
                DisableComponents(triggeredObject);
                isRotatingTowards = true;

                Timing.RunCoroutine(StopRotation().CancelWith(gameObject));

                ChangeAnimationState("Turn Left");

                Timing.RunCoroutine(ChangePositionOfNPC(triggeredObject, TeleportToRight).CancelWith(gameObject));
                Timing.RunCoroutine(ResetCanTriggerVariable().CancelWith(gameObject));
            }
        }



        // one more else if
        if (TeleportToLeft.CompareTag("SecondTeleporter") && collision.gameObject.CompareTag("Outdoor NPC"))
        {
            if (gameObject.name == "Right To")
            {
                triggeredObject = collision.gameObject;
                int spawnIndexPoints = Random.Range(0, TeleportPoints.transform.childCount); /// Set the index number randomly

                TeleportToLeft.transform.parent.SetPositionAndRotation(TeleportPoints.transform.GetChild(spawnIndexPoints).position, TeleportPoints.transform.GetChild(spawnIndexPoints).transform.localRotation);
                animator = collision.gameObject.GetComponent<Animator>();
                PathName = collision.GetComponent<SWS.navMove>().pathContainer.name;
                rotateDirection = "Right";
                DisableComponents(triggeredObject);
                isRotatingTowards = true;

                Timing.RunCoroutine(StopRotation().CancelWith(gameObject));

                ChangeAnimationState("Turn Right");

                Timing.RunCoroutine(ChangePositionOfNPC(triggeredObject, StartTeleportorLeft).CancelWith(gameObject));
                Timing.RunCoroutine(ResetCanTriggerVariable().CancelWith(gameObject));
            }
            else if (gameObject.name == "Left To")
            {
                triggeredObject = collision.gameObject;
                int spawnIndexPoints = Random.Range(0, TeleportPoints.transform.childCount); /// Set the index number randomly

                TeleportToRight.transform.parent.SetPositionAndRotation(TeleportPoints.transform.GetChild(spawnIndexPoints).position, TeleportPoints.transform.GetChild(spawnIndexPoints).transform.localRotation);
                animator = collision.gameObject.GetComponent<Animator>();
                PathName = collision.GetComponent<SWS.navMove>().pathContainer.name;
                rotateDirection = "Left";
                DisableComponents(triggeredObject);
                isRotatingTowards = true;

                Timing.RunCoroutine(StopRotation().CancelWith(gameObject));

                ChangeAnimationState("Turn Left");

                Timing.RunCoroutine(ChangePositionOfNPC(triggeredObject, StartTeleportorRight).CancelWith(gameObject));
                Timing.RunCoroutine(ResetCanTriggerVariable().CancelWith(gameObject));
            }
        }
       
    }



    /// <summary>
    /// Changes the animations
    /// </summary>
    void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState, 0);
        currentState = newState;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    private void DisableComponents(GameObject npc)
    {
        npc.GetComponent<SWS.navMove>().Stop();
        npc.GetComponent<NavMeshAgent>().enabled = false;
        npc.GetComponent<SWS.navMove>().enabled = false;
        npc.GetComponent<SWS.MoveAnimator>().enabled = false;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    private void EnableComponents(GameObject npc)
    {
        npc.GetComponent<NavMeshAgent>().enabled = true;
        npc.GetComponent<SWS.navMove>().enabled = true;
        npc.GetComponent<SWS.MoveAnimator>().enabled = true;
        if (PathName == "NPC Path 1")
        {
            npc.GetComponent<SWS.navMove>().startPoint = Random.Range(0, 5);
        }
        else if (PathName == "NPC Path 2")
        {
            npc.GetComponent<SWS.navMove>().startPoint = Random.Range(0, 3);
        }

        npc.GetComponent<SWS.navMove>().moveToPath = true;
        npc.GetComponent<SWS.navMove>().StartMove();
    }



    /// <summary>
    /// This will change the NPC position when they have started walking towards the shop.
    /// </summary>
    private IEnumerator<float> ChangePositionOfNPC(GameObject npc , GameObject target)
    {
        yield return Timing.WaitForSeconds(5f);

        isWalkingTowards = false;
        npc.transform.GetChild(0).gameObject.SetActive(false);

        yield return Timing.WaitForSeconds(2f);

        npc.GetComponent<Rigidbody>().isKinematic = true;
        npc.transform.GetChild(0).gameObject.SetActive(true);
        npc.transform.position = target.transform.position;
        EnableComponents(npc);
    }



    /// <summary>
    /// This function will responsible for resetting the canTrigger value so that this object can collide again with the NPCs.
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> ResetCanTriggerVariable()
    {
        OutdoorGameManager.instance.canTriggerForTeleport = false;

        yield return Timing.WaitForSeconds(240f);

        OutdoorGameManager.instance.canTriggerForTeleport = true;
    }



    /// <summary>
    /// This will stop the rotation of the any character.
    /// </summary>
    private IEnumerator<float> StopRotation()
    {
        yield return Timing.WaitForSeconds(2f);

        isRotatingTowards = false;
        isWalkingTowards = true;
        ChangeAnimationState("Walk");
    }
}