//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using MEC;
//using UnityEngine.Animations.Rigging;
using SWS;
//using Cinemachine;

public class CollisionChecker : MonoBehaviour
{
    #region Variables
    [Header("scripts")]
    private FirstPersonController firstPersonController;
    private CrosshairGUI crossHairGUI;
    private CameraSwitcher cameraSwitcher;
    private InteractManager interactManager;
    private DoTweenManager doTweenManager;
    //private RigBuilder rigBuilder;

    [Header("Animator")]
    private Animator animator;

    [Header("floats")]
    private float talkAnimationTime = 10f;
    private float idleAnimationTime = 3f;
    private float waitBeforeActivatingMin = 10f;
    private float waitBeforeActivatoingMax = 20f;

    [Header("bools")]
    private bool canDisappear = true;    
    /// To rotate the virtual camera when the player sits on any sofa.   
    private bool rotateCameraForInteraction = false;
    private bool rotateCameraForCollision = false;
    [SerializeField]
    private bool canMakeCollision = true;
    public bool canInteractElevatorButton;

    // This panel has the write survey option.
    public bool showSofaPanel;

    [Header("Rigidbody")]
    private Rigidbody rb;

    [Header("Collider")]
    private CapsuleCollider capsuleCollider;

    [Header("Interaction points (Transforms)")]
    /// This has the position like where player has to sit after interacting with the sofa/ any other sitting object.
    [HideInInspector]
    public Transform newPositionAfterInteraction;
    public Transform receptiontionRoomCenterPoint;
    /// Different center points because there are 6 chairs in the Library which are not at the center of the room. Each set has diff centers.
    public Transform librarianRoomCenterPointSet1;
    public Transform librarianRoomCenterPointSet2;
    public Transform librarianRoomCenterPointSet3;
    /// This is the center point of the Therapy floor.
    public Transform TherapistRoomCenterPoint;
    /// This is center point of the Indian toilet.
    public Transform pharmacyToiletCenterPointIndian;
    public Transform receptionToiletCenterPointIndian;
    public Transform libraryToiletCenterPointIndian;
    public Transform therapistToiletCenterPointIndian;
    // This is center point of the Westren toilet.
    public Transform pharmacyToiletCenterPointWestren;
    public Transform receptionToiletCenterPointWestren;
    public Transform libraryToiletCenterPointWestren;
    public Transform therapistToiletCenterPointWestren;
    // This is Player Laying Camera Position
    public Transform sofaCameraLayingPosition;

    [Header("Quaternions")]
    private Quaternion rotTarget;  

    [Header("GameObjects")]
    public GameObject[] tapWater;
    public GameObject characterPrefabs;
    public GameObject libraryPullInTrigger;

    // This will keep track on which object the player has collided.
    [Header("Strings")]
    private string playerCollidedWith;
    public string playerInteractedWithTag;
    public string playerInteractedWithName;
    private string currentState;

    [Header("int")]
    private int whichFloorToilet = 0;
    #endregion

    private void Start()
    {
        doTweenManager = transform.GetChild(0).transform.GetChild(0).GetComponent<DoTweenManager>();
        cameraSwitcher = Camera.main.GetComponent<CameraSwitcher>();
        interactManager = GetComponent<InteractManager>();
        crossHairGUI = GetComponent<CrosshairGUI>();
        //rigBuilder = GetComponent<RigBuilder>();

        if(gameObject.CompareTag("NPC"))
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
        }
        else if(gameObject.CompareTag("Player"))
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            firstPersonController = GetComponent<FirstPersonController>();
        }
    }



    private void Update()
    {
        /// This will happen when the player will interact using click.
        if (rotateCameraForInteraction && interactManager.hit.collider.tag != null) 
        {
            var step = 40 * Time.deltaTime; // The step size is equal to speed times frame time.

            if(interactManager.hit.collider.CompareTag("Sofa"))
            {
                rotTarget = Quaternion.LookRotation(receptiontionRoomCenterPoint.position - transform.position);
            }

            /// Example 1, 6 are the number of chairs facing each other in the same line. Respectively for other pairs. 
            else if (interactManager.hit.collider.CompareTag("Library chair"))
            {
                if(interactManager.hit.collider.name.Substring(interactManager.hit.collider.name.Length-1) == "1"
                    || interactManager.hit.collider.name.Substring(interactManager.hit.collider.name.Length - 1) == "6")
                {
                    rotTarget = Quaternion.LookRotation(librarianRoomCenterPointSet1.position - transform.position);
                }
                else if (interactManager.hit.collider.name.Substring(interactManager.hit.collider.name.Length - 1) == "2"
                    || interactManager.hit.collider.name.Substring(interactManager.hit.collider.name.Length - 1) == "5")
                {
                    rotTarget = Quaternion.LookRotation(librarianRoomCenterPointSet2.position - transform.position);
                }
                else
                {
                    rotTarget = Quaternion.LookRotation(librarianRoomCenterPointSet3.position - transform.position);
                }
                transform.SetParent(interactManager.hit.collider.transform);
            }


            /// Both targets belong in the same Therapy room.
            else if (interactManager.hit.collider.CompareTag("Therapist sofa") || interactManager.hit.collider.CompareTag("Therapist lounge"))
            {
                rotTarget = Quaternion.LookRotation(TherapistRoomCenterPoint.position - transform.position);
            }

            /// Rotate our transform a step closer to the target's.
            transform.SetPositionAndRotation(Vector3.MoveTowards(transform.position, newPositionAfterInteraction.position, step), Quaternion.RotateTowards(transform.rotation, rotTarget, step)); /// Move the player a step closer towards the environment.

            /// This will check whether the player has reached the 0 rotation while sitting
            if (transform.rotation == rotTarget && rotateCameraForInteraction)
            {
                rotateCameraForInteraction = false;
                cameraSwitcher.changeCamera = true;
                cameraSwitcher.SwitchCameraSofa();
                showSofaPanel = true;
            }
        }


        /// This will run when player will interact using colision.
        else if (rotateCameraForCollision)
        {
            var step = 60 * Time.deltaTime; // The step size is equal to speed times frame time.

            if (playerCollidedWith == "Indian toilet")
            {
                if(whichFloorToilet == 0)
                {
                    rotTarget = Quaternion.LookRotation(pharmacyToiletCenterPointIndian.position - transform.position);
                }
                else if(whichFloorToilet == 1)
                {
                    rotTarget = Quaternion.LookRotation(receptionToiletCenterPointIndian.position - transform.position);
                }
                else if (whichFloorToilet == 2)
                {
                    rotTarget = Quaternion.LookRotation(libraryToiletCenterPointIndian.position - transform.position);
                }
                else if (whichFloorToilet == 3)
                {
                    rotTarget = Quaternion.LookRotation(therapistToiletCenterPointIndian.position - transform.position);
                }
                /// Reset this value.
                playerCollidedWith = "";
            }

            else if (playerCollidedWith == "Western toilet")
            {
                if (whichFloorToilet == 0)
                {
                    rotTarget = Quaternion.LookRotation(pharmacyToiletCenterPointWestren.position - transform.position);
                }
                else if (whichFloorToilet == 1)
                {
                    rotTarget = Quaternion.LookRotation(receptionToiletCenterPointWestren.position - transform.position);
                }
                else if (whichFloorToilet == 2)
                {
                    rotTarget = Quaternion.LookRotation(libraryToiletCenterPointWestren.position - transform.position);
                }
                else if (whichFloorToilet == 3)
                {
                    rotTarget = Quaternion.LookRotation(therapistToiletCenterPointWestren.position - transform.position);
                }
                /// Reset this value.
                playerCollidedWith = "";
            }

            /// Rotate our transform a step closer to the target's.
            transform.SetPositionAndRotation(Vector3.MoveTowards(transform.position, newPositionAfterInteraction.position, step), Quaternion.RotateTowards(transform.rotation, rotTarget, step)); /// Move the player a step closer towards the environment.

            /// This will check whether the player has reached the 0 rotation while sitting.
            if (transform.rotation == rotTarget && rotateCameraForCollision)
            {
                Timing.RunCoroutine(Squat().CancelWith(gameObject));

                doTweenManager.canMove = true;
                doTweenManager.ReachedDestination();               
                rotateCameraForCollision = false;
            }
        }
    }



    /// <summary>
    /// This handles collisions for toilets of all floors
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Pharmacy toilet main door"))
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.doorSwing);
            whichFloorToilet = 0;
            if (canMakeCollision == false)
            {
                canMakeCollision = true;
            }
        }
        else if (collision.gameObject.CompareTag("Reception toilet main door"))
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.doorSwing);
            whichFloorToilet = 1;
            if (canMakeCollision == false)
            {
                canMakeCollision = true;
            }
        }
        else if (collision.gameObject.CompareTag("Library toilet main door"))
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.doorSwing);
            whichFloorToilet = 2;
            if (canMakeCollision == false)
            {
                canMakeCollision = true;
            }
        }
        else if (collision.gameObject.CompareTag("Therapy toilet main door"))
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.doorSwing);
            whichFloorToilet = 3;
            if (canMakeCollision == false)
            {
                canMakeCollision = true;
            }
        }
        else if (collision.gameObject.CompareTag("Toilet door"))
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.doorSwing);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Elevator disappear"))
        {
            if (gameObject.CompareTag("NPC")) /// If NPC
            {
                if (canDisappear)
                {
                    DisableComponentsNPC();

                    Timing.RunCoroutine(StartPatrolAfterActivation().CancelWith(gameObject));

                    canDisappear = false;
                }
            }
        }

        /// This will check whether the player has reached the 0 rotation while sitting
        if (other.gameObject.CompareTag("Talking point") && gameObject.CompareTag("NPC")) /// Play animation when this point is reached.
        {
            ChangeAnimationState("Male talking animation " + "" + Random.Range(1,3)); /// Play random talking animation.

            Timing.RunCoroutine(StartPatrol().CancelWith(gameObject)); /// Once animation playes for sometime, end it and resume the patrol.
        }

        if (other.gameObject.CompareTag("Elevator"))
        {
            canInteractElevatorButton = true;
            transform.parent = other.gameObject.transform;
        }

        if ((other.gameObject.CompareTag("Indian toilet") || other.gameObject.CompareTag("Western toilet")) && canMakeCollision)
        {
            IndoorGameManager.instance.activePlayer.transform.GetChild(2).gameObject.SetActive(false);
            ChangeAnimationState("Idle");
            playerCollidedWith = other.gameObject.tag;
            newPositionAfterInteraction = other.gameObject.transform.GetChild(0).transform;
            rotateCameraForCollision = true;
            firstPersonController.enabled = false;
            canMakeCollision = false;
        }

        if (other.gameObject.CompareTag("Basin"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Interact");
        }

        if (other.gameObject.CompareTag("Talking point receptionist"))
        {
            print("aa");
            if (IndoorGameManager.instance.isReceptionistSitting && IndoorGameManager.instance.receptionistOnCall == false)
            {
               // IndoorGameManager.instance.talkingPointReceptionistCollider.gameObject.layer = LayerMask.NameToLayer("Interact");
                IndoorGameManager.instance.receptionist.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Interact");
               // IndoorGameManager.instance.receptionist.transform.GetChild(1).GetComponent<PixelCrushers.DialogueSystem.Usable>().enabled = true;

                if (GeneralManager.instance.vendorVisitCounter == 1)
                {
                    IndoorGameManager.instance.receptionist.transform.GetChild(1).GetComponent<PixelCrushers.DialogueSystem.Usable>().enabled = true;
                }
            }
        }
    }



    private void OnTriggerStay(Collider other)
    {
        //if (other.gameObject.CompareTag("Talking point receptionist"))
        //{
        //    if (IndoorGameManager.instance.isReceptionistSitting && IndoorGameManager.instance.receptionistOnCall == false)
        //    {
        //        other.gameObject.layer = LayerMask.NameToLayer("Interact");

        //        if (GeneralManager.instance.vendorVisitCounter == 1)
        //        {
        //            IndoorGameManager.instance.receptionist.transform.GetChild(1).GetComponent<PixelCrushers.DialogueSystem.Usable>().enabled = true;
        //        }
        //    }         
        //}
    }



    public void OnTriggerExit(Collider other)
    {
        print("bb");
        if (other.gameObject.CompareTag("Elevator"))
        {
            canInteractElevatorButton = false;
            transform.parent = characterPrefabs.transform;
            transform.SetAsFirstSibling();
        }

        if (other.gameObject.CompareTag("Talking point receptionist"))
        {
            //IndoorGameManager.instance.talkingPointReceptionistCollider.gameObject.layer = LayerMask.NameToLayer("Default");
            IndoorGameManager.instance.receptionist.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
            //IndoorGameManager.instance.receptionist.transform.GetChild(1).GetComponent<PixelCrushers.DialogueSystem.Usable>().enabled = false;

            if (GeneralManager.instance.vendorVisitCounter == 2)
            {
                IndoorGameManager.instance.receptionist.transform.GetChild(1).GetComponent<PixelCrushers.DialogueSystem.Usable>().enabled = false;
            }
        }

        if (other.gameObject.CompareTag("Basin"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Default");
        }

        if (other.gameObject.CompareTag("LibraryPullinTrigger"))
        {
            Timing.RunCoroutine(interactManager.ResetLiraryChairAnimationState("Push in " + playerInteractedWithName).CancelWith(gameObject));

            IndoorGameManager.instance.librarytrigger.GetComponent<BoxCollider>().isTrigger = false;

            libraryPullInTrigger.SetActive(false);
        }
    }



    /// <summary>
    /// This changes the animation states for this gameobject.
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);

        currentState = newState;
    }

    

    /// <summary>
    /// This Function is responsible for each floor on mouse click.
    /// </summary>
    public void BasinTapTrigger()
    {
        Timing.RunCoroutine(WashingHands().CancelWith(gameObject)); // change to better animation

        if (interactManager.currentFloor == "Pharmacy")
        {
            tapWater[0].SetActive(true);
        }
        else if (interactManager.currentFloor == "Reception")
        {
            tapWater[1].SetActive(true);
        }
        else if (interactManager.currentFloor == "Library")
        {
            tapWater[2].SetActive(true);
        }
        else if (interactManager.currentFloor == "Therapy")
        {
            tapWater[3].SetActive(true);
        }
        SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.tapWaterClip);

        Timing.RunCoroutine(StopTapWater().CancelWith(gameObject));

    }



    /// <summary>
    /// Disables componets when NPC disappears.
    /// </summary>
    private void DisableComponentsNPC()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        capsuleCollider.enabled = false;
    }



    /// <summary>
    /// Enables components when the NPC reappear.
    /// </summary>
    private void EnableComponentsNPC()
    {
        transform.Rotate(new Vector3(0f, 180f, 0f));
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        capsuleCollider.enabled = true;
    }



    /// <summary>
    /// especially for player.
    /// </summary>
    private void DisableComponentsPlayer()
    {
        capsuleCollider.enabled = false;
        rb.useGravity = false;
        firstPersonController.enabled = false;
        crossHairGUI.isStanding = false;
    }



    /// <summary>
    /// especially for player.
    /// </summary>
    private void EnableComponentsPlayer()
    {
        firstPersonController.enabled = true; 
        capsuleCollider.enabled = true;
        rb.useGravity = true;
        //firstPersonController.cameraCanMove = true; /// We need player to rotate the camera when he sits.
        crossHairGUI.isStanding = true;
    }



    /// <summary>
    /// This is assinged to the write survey button on the sofa panel.
    /// </summary>
    public void WriteSurvey()
    {
        crossHairGUI.m_ShowCursor = false;
        cameraSwitcher.changeCamera = true;
        cameraSwitcher.SwitchCameraSurvey();
    }



    /// <summary>
    /// This function will be called when sitting on sofa or chair.
    /// </summary>
    public void Sit()
    {
        IndoorGameManager.instance.sittingTriggerHolder.SetActive(false);

        crossHairGUI.m_ShowCursor = false;
        rotateCameraForInteraction = true; /// First we need to make the player sit then we need to show an option whether he wants to look down or not.
        firstPersonController.cameraCanMove = true;
        DisableComponentsPlayer();
        ChangeAnimationState("Stand to sit");
    }



    /// <summary>
    /// This function will be called on lying down
    /// </summary>
    public void LayDown()
    {
        crossHairGUI.m_ShowCursor = false;
        rotateCameraForInteraction = true; /// First we need to make the player sit then we need to show an option whether he wants to look down or not.
        DisableComponentsPlayer();
        ChangeAnimationState("Stand to sit"); 

        Timing.RunCoroutine(PlayLayingAnimation().CancelWith(gameObject));

    }



    /// <summary>
    /// This function will be called when exiting sofa or chair
    /// </summary>
    public void ExitFromSittingPosition()
    {
        IndoorGameManager.instance.sittingTriggerHolder.SetActive(true);

        EnableComponentsPlayer();

        if(playerInteractedWithTag == "Sofa")
        {
            ChangeAnimationState("Sit to stand");
        }

        else if(playerInteractedWithTag == "Library chair")
        {           
            libraryPullInTrigger.SetActive(true);
            interactManager.CallChangeAnimationStateForLibraryChair("Pull out ");
            ChangeAnimationState("Sit to stand");
            transform.parent = null;
        }

        else if(playerInteractedWithTag == "Therapist sofa")
        {
            ChangeAnimationState("Sit to stand");
        }

        crossHairGUI.allowedToRaycast = true;
    }



    /// <summary>
    /// This will run after talking animation is finished of the NPC.
    /// </summary>
    private IEnumerator<float> StartPatrol()
    {
        yield return Timing.WaitForSeconds(talkAnimationTime);

        ChangeAnimationState("Locomotion");

        yield return Timing.WaitForSeconds(idleAnimationTime);
    }



    /// <summary>
    /// This wil run when the NPC is enabled after disabling.
    /// </summary>
    private IEnumerator<float> StartPatrolAfterActivation()
    {
        yield return Timing.WaitForSeconds(Random.Range(waitBeforeActivatingMin,waitBeforeActivatoingMax));

        EnableComponentsNPC();
        ChangeAnimationState("Locomotion");

        yield return Timing.WaitForSeconds(idleAnimationTime);

        yield return Timing.WaitForSeconds(5f); /// Just to avoid being triggered just after reappearing.

        canDisappear = true;
    }



    /// <summary>
    /// This wil run when the NPC is sitting on the Chasis Lounge and after he sits then the laying animation will be played.
    /// </summary>
    private IEnumerator<float> PlayLayingAnimation()
    {
        yield return Timing.WaitForSeconds(5f);

        ChangeAnimationState("Laying");

        yield return Timing.WaitForSeconds(2f);

        transform.localPosition = new Vector3(-0.5420001f, 16.392f, 6.227f);
        transform.localRotation = Quaternion.Euler(8.425f, 230.129f, -2.052f);
        /// This will bring the camera backwards while the player is laying down.
        cameraSwitcher.sofaCamera.transform.position = sofaCameraLayingPosition.transform.position;

        /// This is a specific value that player has to keep while laying down

        //rigBuilder.enabled = true;
    }



    /// <summary>
    /// Turn off the tap water.
    /// </summary>
    private IEnumerator<float> StopTapWater()
    {
        yield return Timing.WaitForSeconds(5f);

        if (interactManager.currentFloor == "Pharmacy")
        {
            tapWater[0].SetActive(false);
        }
        else if (interactManager.currentFloor == "Reception")
        {
            tapWater[1].SetActive(false);
        }
        else if (interactManager.currentFloor == "Library")
        {
            tapWater[2].SetActive(false);
        }
        else if (interactManager.currentFloor == "Therapy")
        {
            tapWater[3].SetActive(false);
        }
        SfxManager.sfxInstance.audioSource[0].Stop();
    }



    /// <summary>
    /// This will make the player squat after rotation while standing on top of the toilet.
    /// </summary>
    private IEnumerator<float> Squat()
    {
        ChangeAnimationState("Squatting");       

        yield return Timing.WaitForSeconds(4f);

        SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.peeingClip);

        /// Stop the Peeing sound then play the flusing sound.
        yield return Timing.WaitForSeconds(1f);

        doTweenManager.canMove = true;
        doTweenManager.ResetPosition();
        SfxManager.sfxInstance.audioSource[0].Stop();
        SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.flushClip);
        IndoorGameManager.instance.activePlayer.transform.GetChild(2).gameObject.SetActive(true);
        firstPersonController.enabled = true;
    }



    /// <summary>
    /// This function will change camera position and play animation then reset its position 
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> WashingHands()
    {
        doTweenManager.canMove = true;
        doTweenManager.ReachDestinationBasin();

        ChangeAnimationState("Wash Hand Animation");

        yield return Timing.WaitForSeconds(6.7f);

        doTweenManager.canMove = true;
        currentState = null;
        doTweenManager.ResetPositionBasin();


    }



    /// <summary>
    /// By default max time for peeing sound should be 20 secoonds. If player exists the zone before the time ends the stop this coroutine.
    /// </summary>
    //IEnumerator<float> StopPeeingAfterTimeOut()
    //{
    //    yield return Timing.WaitForSeconds(20f);

    //    SfxManager.sfxInstance.audioSource[0].Stop(); 
    //}



    //IEnumerator<float> ResetAllowedToRayCastVariable()
    //{
    //    yield return Timing.WaitForSeconds(6f);

    //    crossHairGUI.allowedToRaycast = true;
    //}
}
