//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Cinemachine cameras")]
    public CinemachineExternalCamera playerCamera; // The fps camera
    public CinemachineVirtualCamera dogCamera; // Camera focuses on only dog
    public CinemachineVirtualCamera receptionCamera; // Camera focuses on only receptionist
    public CinemachineVirtualCamera sofaCamera; // The camera that only looks around when sitting on sofa.  
    public CinemachineVirtualCamera surveyCamera;// The camera that looks onto the survey paper when sitting on the sofa.

    [Header("Boolean")]
    public bool changeCamera;

    [Header("Canvas panels")]
    public GameObject dogPlayPanel;
    public GameObject receptionistPanel;
    public GameObject sofaPanel;
    public GameObject crossHair;

    [Header("Script references")]
    public CrosshairGUI crossHairGUI;
    public CollisionChecker collisionChecker;
    public FirstPersonController firstPersonController;
    public InteractManager interactManager;
    public ElevatorController elevatorController;
    //public FirstPersonLook firstPersonLook; 

    [Header("GameObjects")]
    public GameObject characterPrefabs;



    private void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");
        playerCamera = characterPrefabs.transform.GetChild(selectedCharacter).transform.GetChild(0).transform.GetChild(0).GetComponent<CinemachineExternalCamera>();
        sofaCamera = characterPrefabs.transform.GetChild(selectedCharacter).transform.GetChild(1).transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        surveyCamera = characterPrefabs.transform.GetChild(selectedCharacter).transform.GetChild(1).transform.GetChild(1).GetComponent<CinemachineVirtualCamera>();
        crossHairGUI = characterPrefabs.transform.GetChild(selectedCharacter).GetComponent<CrosshairGUI>();
        collisionChecker = characterPrefabs.transform.GetChild(selectedCharacter).GetComponent<CollisionChecker>();
        firstPersonController = characterPrefabs.transform.GetChild(selectedCharacter).GetComponent<FirstPersonController>();
        interactManager = characterPrefabs.transform.GetChild(selectedCharacter).GetComponent<InteractManager>();
    }



    private void Update()
    {
        if(collisionChecker.showSofaPanel) // This variable is triggered in the collision checker script
        {
            sofaPanel.SetActive(true); // Turn on the panel when the player rotates and sits nicely on the chair.
            if(interactManager.hit.collider.CompareTag("Sofa"))
            {
                if(elevatorController.floorNumber == 3)
                {
                    sofaPanel.transform.GetChild(1).gameObject.SetActive(false);
                    sofaPanel.transform.GetChild(2).gameObject.SetActive(false);
                }
                else
                {
                    sofaPanel.transform.GetChild(1).gameObject.SetActive(true);
                    sofaPanel.transform.GetChild(2).gameObject.SetActive(false);
                }
            }
            else if(interactManager.hit.collider.CompareTag("Library chair"))
            {
                sofaPanel.transform.GetChild(1).gameObject.SetActive(false);
                sofaPanel.transform.GetChild(2).gameObject.SetActive(true);
            }
            collisionChecker.showSofaPanel = false;
        }
    }



    /// <summary>
    /// Switch Player Camera to Reception camera
    /// </summary>
    public void SwitchCameraReceptionist() 
    {
        if (changeCamera)
        {
            crossHair.SetActive(false);
            playerCamera.Priority = 0;
            receptionCamera.Priority = 1;
            receptionistPanel.SetActive(true);
            DisableComponents();
        }
        else
        {
            crossHair.SetActive(true);
            playerCamera.Priority = 1;
            receptionCamera.Priority = 0;
            receptionistPanel.SetActive(false);
            EnableComponents();         
        }

        IndoorGameManager.instance.elevatorButton[0].layer = LayerMask.NameToLayer("Interact");
    }



    /// <summary>
    /// Switch Player Camera to Survey camera
    /// </summary>
    public void SwitchCameraSurvey() 
    {
        if (changeCamera)
        {
            playerCamera.Priority = 0;
            surveyCamera.Priority = 1;          
        }
        else
        {
            playerCamera.Priority = 1;
            surveyCamera.Priority = 0;         
        }
    }



    /// <summary>
    /// Switch Player Camera to Siting camera
    /// </summary>
    public void SwitchCameraSofa() 
    {
        sofaCamera.transform.rotation = Quaternion.identity;
        if (changeCamera)
        {
            playerCamera.Priority = 0;
            sofaCamera.Priority = 1;
        }
        else
        {
            playerCamera.Priority = 1;
            sofaCamera.Priority = 0;
            sofaPanel.SetActive(false);
        }
    }



    /// <summary>
    /// Switch Player Camera to dog camera
    /// </summary>
    public void SwitchCameraDog()
    {
        if (changeCamera)
        {
            crossHair.SetActive(false);
            playerCamera.Priority = 0;
            dogCamera.Priority = 1;
            dogPlayPanel.SetActive(true);
            DisableComponents();
        }
        else
        {
            crossHair.SetActive(true);
            playerCamera.Priority = 1;
            dogCamera.Priority = 0;
            dogPlayPanel.SetActive(false);
            EnableComponents();
        }
    }



    /// <summary>
    /// Switch Dog Camera to Player Camera 
    /// </summary>
    public void ExitDogPlay() 
    {
        changeCamera = !changeCamera;
        interactManager.ResumeMovement();
        SwitchCameraDog();

        IndoorGameManager.instance.playerInteractedWithDog = false;
        IndoorGameManager.instance.activePlayer.transform.GetChild(2).gameObject.SetActive(true);
    }



    /// <summary>
    /// Switch Reception Camera to Player Camera 
    /// </summary>
    public void ExitReceptionistCamera() 
    {
        changeCamera = !changeCamera;
        SwitchCameraReceptionist();

        IndoorGameManager.instance.elevator[0] = true;
        IndoorGameManager.instance.playerInteractedWithReceptionist = false;
    }



    /// <summary>
    /// Switch Siting Camera to Player Camera 
    /// </summary>
    public void ExitSofaCamera() 
    {
        sofaPanel.SetActive(false); // Turn off the panel with exit and write survey button.
        changeCamera = !changeCamera;
        collisionChecker.ExitFromSittingPosition();

        if(sofaCamera.Priority == 1) // If player is not writing the survey and just sitting on the sofa.
        {
            SwitchCameraSofa();
        }

        if (surveyCamera.Priority == 1) // If player is writing the survey then do below.
        {
            SwitchCameraSurvey();
        }

        IndoorGameManager.instance.playerInteractedWithSofa = false;
    }



    /// <summary>
    /// Enable Player Controls
    /// </summary>
    private void EnableComponents()
    {
        firstPersonController.enabled = true;
    }



    /// <summary>
    /// Disable Player Controls
    /// </summary>
    private void DisableComponents()
    {
        firstPersonController.ChangeAnimationState("Idle");
        firstPersonController.enabled = false;
    }

}
