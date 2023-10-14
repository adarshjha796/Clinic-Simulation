//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using SWS;
using PixelCrushers.DialogueSystem;
//using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System;
using TMPro;
public class IndoorGameManager : MonoBehaviour
{
    public static IndoorGameManager instance;

    [Header("GameObjects")]
    [SerializeField]
    private GameObject receptionistChair;
    [SerializeField]
    private GameObject cabinetBox;
    [SerializeField]
    private GameObject pharmacist;
    [SerializeField]
    private GameObject allMedicines;
    [SerializeField]
    private GameObject landLineReciever;
    [SerializeField]
    private GameObject daschundDog;
    public GameObject sittingTriggerHolder;
    public GameObject[] sittingTriggers;
    private GameObject[] selectedMedicines = new GameObject[4];
    public GameObject allPlayers;
    public GameObject activePlayer;
    // vendor[0] will be ground floor vendor and vendor[1] will be 1st floor vendor
    public GameObject[] Vendor;
    public GameObject elevatorDisapperPoint;
    public GameObject receptionist;
    public GameObject clinicEntrance;
    public GameObject elevatorLabel;
    public GameObject[] AllNpc;
    public GameObject[] elevatorButton;
    public GameObject elevatorWarning;
    public GameObject fakeLoadingPanel;
    public GameObject leftHandPlateVendorPharmacy;
    public GameObject rightHandPlateVendorPharmacy;
    public GameObject leftHandPlateVendorReception;
    public GameObject rightHandPlateVendorReception;
    private GameObject systems;
    public GameObject librarytrigger;
    public GameObject talkingPointReceptionistCollider;
    public GameObject cursor;
    public GameObject[] receptionFoodPlate;

    [Header("Text")]
    public TMP_Text subtitleText;

    [Header("Animators")]
    [SerializeField]
    private Animator receptionistChairAnimator;
    [SerializeField]
    private Animator cabinetBoxAnimator;
    [SerializeField]
    public Animator pharmacistAnimator;
    [SerializeField]
    public Animator receptionistAnimator;
    [SerializeField]
    private Animator landLineRecieverAnimator;
    [SerializeField]
    public Animator DaschundDogAnimator;

    [Header("Strings")] 
    // Keep track of animation state of the receptionist chair.
    private string receptionistChairCurrentState;
    // Keep track of the animation state of the 2d cabinet box.
    private string cabinetBoxCurrentState;
    // Kepp track of the animation state of the pharmacist.
    public string pharmacistCurrentState;
    // Keep track of the animation state of the landline reciever.
    private string landLineRecieverCurrentState;
    // make a variable for currentstate of daschund dog.
    private string DaschundDogCurrentState;
    private string dialogueTitle;
    // Keep track of the paths receptionist can move on.
    public string[] receptionistPathNames;
    // This will keep track of all the animation states for the daschund dog.
    public string[] daschundDogAnimationStates;
    // This will hold all Dialogues for NPC.
    public string[] subtitle;
    // This will hold all Dialogues for Vendor.
    public string[] vendorSubtitle;
    private string PlayerName;

    [Header("Booleans")]
    // This will the receptionschair to moves in when she comes.
    public bool moveInReceptionistChair;
    // This will the receptionschair to moves back when she gets up.
    public bool moveOutReceptionistChair;
    // This will allow the 2nd cabinet of the filling cabinet to come out when receptionist intereact with it. 
    public bool moveOutCabinetBox;
    // This will keep track of animations the the pharmacist has to play when an NPC reaches her.
    public bool startPharmacistAnimations;

    public bool furtherAnimationPlayPhatamcist;
    // This will keep track of animations the the pharmacist has to play when an vendor reaches her.
    public bool startPharmacistAnimationsVendor;
    // This will allow is to ring the landline.
    public bool ringTheLandline = false;
    // This will be let the any character rotate nicely and then stop after facing the target.
    private bool isPharmacistRotating = false;
    // This is let the couroutine only run for once.
    private bool canCallCoroutine = false;
    // This will keep track of if the sitting point is occupied by a NPC or not.
    public bool[] isSittingPointForNPCTaken;
    //control music audio source
    public bool canPlayMusic;
    public bool isReceptionistSitting;
    public bool playerInConversation;
    public bool playerInteractedWithDog;
    public bool playerInteractedWithSofa;
    public bool playerInteractedWithReceptionist;
    public bool receptionistOnCall;
    public bool[] elevator;
    private bool conversationCancled;

    [Header("floats")]
    // Adjust the speed for the moveTowards.
    private readonly float speed = 1f;
    private float step;
    private float rotateToWhichDegree = 180f;
    public int receptionVendor = 0;
    private int currentDialogueId;

    [Header("Transforms")]
    [SerializeField]
    private Transform[] medicinesDisplayPosition;
    public Transform[] sittingPointsForNPC;

    [Header("AudioSource")]
    public AudioSource[] MusicPlayer;
    [SerializeField]private AudioSource receptionistAudioSource;

    [Header("Vectors")]
    private Vector3[] medicinesOriginalPosition = new Vector3[4];

    [Header("Scripts")]
    public ElevatorController elevatorController;
    public ElevatorButton elevatorButtonScript;
    private FirstPersonController firstPersonController;
    public CameraSwitcher cameraSwitcher;

    //Added by Gyan    
    [SerializeField] private GameObject cursorInfo;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        for (int i = 0; i < allPlayers.transform.childCount; i++)
        {
            if (allPlayers.transform.GetChild(i).gameObject.activeSelf)
            {
                activePlayer = allPlayers.transform.GetChild(i).gameObject;
                activePlayer.GetComponent<FirstPersonController>().enabled = false;
                break;
            }
        }

        SfxManager.sfxInstance.audioSource[1] = Vendor[0].GetComponent<AudioSource>();

        Timing.RunCoroutine(FakeDelayPanelClose().CancelWith(gameObject));
    }



    private void Start()
    {
        GetUserData();
        GeneralManager.instance.hintButton.SetActive(false);

        if (GeneralManager.instance.playerSceneSwitchTracker == 1 && GeneralManager.instance.vendorVisitCounter == 1)
        {
            Timing.KillCoroutines("VendorConversation");

            Vendor[0].SetActive(true);
            clinicEntrance.SetActive(false);
           
            elevatorButton[2].layer = LayerMask.NameToLayer("Default");
        }
        else if (GeneralManager.instance.playerSceneSwitchTracker == 1 && GeneralManager.instance.vendorVisitCounter >= 2)
        {
            elevatorButton[0].layer = LayerMask.NameToLayer("Interact");
            elevatorButton[1].layer = LayerMask.NameToLayer("Interact");
        }
        else
        {          
            elevatorButton[2].layer = LayerMask.NameToLayer("Interact");
        }

        GeneralManager.instance.isOutdoorSceneActive = false;
        GeneralManager.instance.isIndoorSceneActive = true;

        firstPersonController = activePlayer.GetComponent<FirstPersonController>();       

        AllNpc[0].SetActive(true);
        AllNpc[1].SetActive(true);
        AllNpc[2].SetActive(true);
        AllNpc[3].SetActive(false);
        AllNpc[4].SetActive(false);
        AllNpc[5].SetActive(false);
        AllNpc[6].SetActive(false);
        AllNpc[7].SetActive(false);

        receptionFoodPlate[0].SetActive(false);
        receptionFoodPlate[1].SetActive(false);

        Timing.PauseCoroutines("KillOutdoorSongs");
        Timing.PauseCoroutines("KillOutdoorVendorIdle");
        Timing.KillCoroutines("KillOutdoorVendorIdle");
        Timing.KillCoroutines("KillOutdoorSongs");
        Timing.RunCoroutine(TurnOnAndOffSittingTriggers(0).CancelWith(gameObject));
        Timing.RunCoroutine(PlayMusicPharmacy().CancelWith(gameObject),"KillIndoorSongs");
        Timing.RunCoroutine(PlayMusicReception().CancelWith(gameObject), "KillIndoorSongs");

        if (GeneralManager.instance.isMusicMute)
        {
            MusicPlayer[0].mute = true;
            MusicPlayer[1].mute = true; 
        }

        if (GeneralManager.instance.isSoundMute)
        {
            for (int i = 0; i < SfxManager.sfxInstance.audioSource.Length; i++)
            {
                SfxManager.sfxInstance.audioSource[i].mute = true;
            }
        }

        if (elevatorController.floorNumber == 0)
        {
            MusicPlayer[1].Pause();
        }


        //leftHandPlateVendorPharmacy.SetActive(false);
        //rightHandPlateVendorPharmacy.SetActive(false);
        leftHandPlateVendorReception.SetActive(false);
        rightHandPlateVendorReception.SetActive(false);

        systems = GameObject.FindGameObjectWithTag("Systems");
        
        FindChildGameObjectByName(systems, "Music Slider").SetActive(false);
        FindChildGameObjectByName(systems, "Music Slider Indoor").SetActive(true);
        FindChildGameObjectByName(systems, "Music Toggle").GetComponent<ToggleSound>().toggleMusicIndoor = true;
        FindChildGameObjectByName(systems, "Music Toggle").GetComponent<ToggleSound>().toggleMusic = false;

        HintSystem.instance.ChangeMusicVolumeIndoor(HintSystem.instance.musicIndoorVolumeSlider.value);

        receptionistAudioSource = receptionist.GetComponent<AudioSource>();
        DialogueLua.SetActorField("Player", "Display Name", PlayerName);
    }



    private GameObject FindChildGameObjectByName(GameObject topParentGameObject, string gameObjectName)
    {
        for (int i = 0; i < topParentGameObject.transform.childCount; i++)
        {
            if (topParentGameObject.transform.GetChild(i).name.ToLower() == gameObjectName.ToLower())
            {
                return topParentGameObject.transform.GetChild(i).gameObject;
            }
            GameObject tmp = FindChildGameObjectByName(topParentGameObject.transform.GetChild(i).gameObject, gameObjectName);
            if (tmp != null)
            {
                return tmp;
            }
        }
        return null;
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && playerInConversation == false && !fakeLoadingPanel.activeSelf)
        {
            GeneralManager.instance.isGamePause = true;

            cursor.SetActive(false);

            if (activePlayer.GetComponent<CrosshairGUI>().m_ShowCursor)
            {
                activePlayer.GetComponent<CrosshairGUI>().CursorControl();
            }
            activePlayer.GetComponent<CrosshairGUI>().CursorControl();
            FindChildGameObjectByName(systems, "Pause Panel BG").SetActive(true);

            for (int i = 0; i < MusicPlayer.Length; i++)
            {
                MusicPlayer[i].Pause();
            }

            if (playerInteractedWithDog)
            {
                cameraSwitcher.dogPlayPanel.SetActive(false);
            }

            if (playerInteractedWithReceptionist)
            {
                cameraSwitcher.receptionistPanel.SetActive(false);
            }

            if (playerInteractedWithSofa)
            {
                cameraSwitcher.sofaPanel.SetActive(false);
            }
            Time.timeScale = 0f;
        }

        //this will call when npc collided and sit on chair 
        if (moveInReceptionistChair)
        {
            ChangeAnimationState("MoveIn", receptionistChairAnimator);
            moveInReceptionistChair = false;
        }

        //this will call when npc go from sit to stand 
        if (moveOutReceptionistChair)
        {
            ChangeAnimationState("MoveOut", receptionistChairAnimator);
            moveOutReceptionistChair = false;
        }
      
        if (moveOutCabinetBox)
        {
            ChangeAnimationState("MoveOut", cabinetBoxAnimator);
            moveOutCabinetBox = false;
        }

        // this will call when npc talking pharmacist
        if(startPharmacistAnimations)
        {
            ChangeAnimationState("Namaste", pharmacistAnimator);
            startPharmacistAnimations = false;

            pharmacistAnimator.SetBool("furtherAnimationPlayPhatamcist", true);
        }

        if (startPharmacistAnimationsVendor)
        {
            ChangeAnimationState("Namaste", pharmacistAnimator);
            startPharmacistAnimationsVendor = false;

            pharmacistAnimator.SetBool("furtherAnimationPlayPhatamcist", false);
        }

        if (pharmacistAnimator.GetCurrentAnimatorStateInfo(0).IsName("Namaste") && canCallCoroutine == false)
        {
            canCallCoroutine = true;
        }

        if (pharmacistAnimator.GetCurrentAnimatorStateInfo(0).IsName("Turn around 1") && canCallCoroutine)
        {
            Timing.RunCoroutine(StartRotation(0.1f).CancelWith(gameObject));

            canCallCoroutine = false;
        }

        if (pharmacistAnimator.GetCurrentAnimatorStateInfo(0).IsName("Pick medicines") && canCallCoroutine == false)
        {
            canCallCoroutine = true;
        }

        if (pharmacistAnimator.GetCurrentAnimatorStateInfo(0).IsName("Turn around 2") && canCallCoroutine)
        {
            Timing.RunCoroutine(StartRotation(0.1f).CancelWith(gameObject));

            canCallCoroutine = false;
        }
        
        if(pharmacistAnimator.GetCurrentAnimatorStateInfo(0).IsName("Show pills") && canCallCoroutine == false)
        {
            Timing.RunCoroutine(PositionMedicines(0.5f).CancelWith(gameObject));

            canCallCoroutine = true;
        }

        //this condiction will check if pharmacist is rotating
        if (isPharmacistRotating)
        {
            // The step size is equal to speed times frame time.
            step = speed * Time.deltaTime;
            pharmacist.transform.localRotation = Quaternion.Lerp(pharmacist.transform.localRotation, Quaternion.Euler(0f,rotateToWhichDegree,0f), step);
        }

        //this condiction will call ring landline function when npc sit on chair
        if(ringTheLandline)
        {
            if (GeneralManager.instance.playerSceneSwitchTracker == 1 && GeneralManager.instance.vendorVisitCounter == 1)
            {
                Timing.RunCoroutine(RingLandLineAndRecieveAndPutDown(50f).CancelWith(gameObject), "RingLandLine");
            }

            else
            {
                Timing.RunCoroutine(RingLandLineAndRecieveAndPutDown(10f).CancelWith(gameObject), "RingLandLine");
            }
        }
    }



    /// <summary>
    /// This will check if the player currently in conversation
    /// </summary>
    /// <param name="value"></param>
    public void PlayerInConversatiVariableOnOrOff(bool value)
    {
        playerInConversation = value;
        cursorInfo.SetActive(value);
        if (value)
        {
            for (int i = 0; i < MusicPlayer.Length; i++)
            {
                MusicPlayer[i].volume = 0.02f;
            }
        }
        else
        {
            for (int i = 0; i < MusicPlayer.Length; i++)
            {
                MusicPlayer[i].volume = 0.1f;
            }
        }

    }



    /// <summary>
    /// This function will call on every button and this will play click sound
    /// </summary>
    public void ButtonClickSound()
    {
        SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.buttonClick);
    }



    /// <summary>
    /// Changes the animations
    /// </summary>
    public void ChangeAnimationState(string newState, Animator animator)
    {
        if(animator == receptionistChairAnimator)
        {
            if (receptionistChairCurrentState == newState) return;
            animator.Play(newState);
            receptionistChairCurrentState = newState;
        }

        if(animator == cabinetBoxAnimator)
        {
            if (cabinetBoxCurrentState == newState) return;
            animator.Play(newState);
            cabinetBoxCurrentState = newState;
        }

        if(animator == pharmacistAnimator)
        {
            if (pharmacistCurrentState == newState) return;
            animator.Play(newState);
            pharmacistCurrentState = newState;
        }

        if(animator == landLineRecieverAnimator)
        {
            if (landLineRecieverCurrentState == newState) return;
            animator.Play(newState);
            landLineRecieverCurrentState = newState;
        }

        if (animator == DaschundDogAnimator)
        {
            if (DaschundDogCurrentState == newState) return;
            animator.Play(newState);
            DaschundDogCurrentState = newState;
        }
    }



    /// <summary>
    /// Diable Player Dialogue / Movement component
    /// </summary>
    public void DisableComponents()
    {
        receptionist.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
        activePlayer.GetComponent<FirstPersonController>().enabled = false;
        activePlayer.GetComponent<Selector>().enabled = false;
    }



    /// <summary>
    /// Enable Player Dialogue / Movement component
    /// </summary>
    public void EnableComponents()
    {
        activePlayer.GetComponent<CrosshairGUI>().allowedToRaycast = true;
        if (!conversationCancled)
        {
            activePlayer.GetComponent<FirstPersonController>().enabled = true;
        }       
        activePlayer.GetComponent<Selector>().enabled = true;
    }

    public void ConversationCancle()
    {
        activePlayer.GetComponent<Selector>().enabled = false;
        conversationCancled = true;
    }



    /// <summary>
    /// Stop Conversation And store the dialogue Id 
    /// </summary>
    public void DialoguePause()
    {
        currentDialogueId = DialogueManager.currentConversationState.subtitle.dialogueEntry.id;
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"currentDialogueId" ,"" + currentDialogueId },
                {"Dialogue Title" ,"" + DialogueManager.lastConversationStarted }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);

        DialogueManager.StopConversation();
        activePlayer.GetComponent<FirstPersonController>().enabled = true;
        activePlayer.GetComponent<Selector>().enabled = true;
    }



    /// <summary>
    /// This function will get the user data present on playfab database
    /// </summary>
    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);
    }



    /// <summary>
    /// This function will be called on succesfully reciving data from playfab
    /// </summary>
    /// <param name="result"></param>
    private void OnDataRecieved(GetUserDataResult result)
    {
        if (result.Data == null)
        {
            return;
        }

        if (result.Data.ContainsKey("Floor Name"))
        {
            elevatorController.ElevatorGO(result.Data["Floor Name"].Value);
        }

        if (result.Data.ContainsKey("currentDialogueId") && result.Data.ContainsKey("Dialogue Title"))
        {
            currentDialogueId = int.Parse(result.Data["currentDialogueId"].Value);
            dialogueTitle = result.Data["Dialogue Title"].Value;
            print(currentDialogueId);
        }
        if (result.Data.ContainsKey("PlayerName"))
        {
            PlayerName = result.Data["PlayerName"].Value;
            print(PlayerName);
        }
    }



    /// <summary>
    /// this function will be called if any error occur during saving a playfab value
    /// </summary>
    /// <param name="obj"></param>
    private void OnError(PlayFabError obj)
    {
        print("Data Not Saved");
    }



    /// <summary>
    /// This Function will be called when saving value to playfab is succesfull
    /// </summary>
    /// <param name="result"></param>
    private void OnDataSend(UpdateUserDataResult result)
    {
        print("Data Save");
        GetUserData();
    }



    /// <summary>
    /// Start conversation from the stored Dialogue Id
    /// </summary>
    public void DialogueResume()
    {
        DialogueManager.StartConversation(dialogueTitle, DialogueManager.currentActor, DialogueManager.currentConversant, currentDialogueId);
    }



    /// <summary>
    /// Play Sound when Dog play Sit animation
    /// </summary>
    public void PlayDaschundDogSitAudio()
    {
        if (allPlayers.transform.GetChild(0).gameObject.name== "PlayerFemale")
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.Baithofemale);
        }
        else if (allPlayers.transform.GetChild(0).gameObject.name == "PlayerTrans")
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.Baithotrans);
        }
        else if (allPlayers.transform.GetChild(0).gameObject.name == "PlayerMale")
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.Baithomale);
        }
    }



    /// <summary>
    /// Play Sound when Dog play Sleep animation
    /// </summary>
    public void PlayDaschundDogSleepAudio()
    {
        if (allPlayers.transform.GetChild(0).gameObject.name == "PlayerFemale")
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.Sojaofemale);
        }
        else if (allPlayers.transform.GetChild(0).gameObject.name == "PlayerTrans")
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.Sojaotrans);
        }
        else if (allPlayers.transform.GetChild(0).gameObject.name == "PlayerMale")
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.Sojaomale);
        }
    }



    /// <summary>
    /// Play Sound when Dog play Lie animation
    /// </summary>
    public void PlayDaschundDogLieAudio()
    {
        if (allPlayers.transform.GetChild(0).gameObject.name == "PlayerFemale")
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.Leytjaofemale);
        }
        else if (allPlayers.transform.GetChild(0).gameObject.name == "PlayerTrans")
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.Leytjaotrans);
        }
        else if (allPlayers.transform.GetChild(0).gameObject.name == "PlayerMale")
        {
            SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.Leytjaomale);
        }
    }



    /// <summary>
    /// This function will be called on the buttons in the dog interaction panel.
    /// </summary>
    public void ChangeAnimationForDaschundDogUsingButton(string newState)
    {
        ChangeAnimationState(newState,DaschundDogAnimator);
        DaschundDogCurrentState = null;
    }



    /// <summary>
    /// This function will turn of usable component so player can not intract with recptionist again after conversation end
    /// </summary>
    public void TurnOffReceptionistUsable()
    {
        receptionist.transform.GetChild(1).GetComponent<Usable>().enabled = false;
    }



    /// <summary>
    /// Setting the vendor visit counter on dialoug end 
    /// </summary>
    /// <param name="index"></param>
    public void IncreaseTheVendorVisitCounter(int index)
    {
        GeneralManager.instance.vendorVisitCounter = index;
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"vendorVisitCounter" ,"" + index }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }



    /// <summary>
    /// This Function will call corutione for receptionis from exit button 
    /// </summary>
    public void CancleCoroutineForReceptionistWhileInConversationFunction()
    {
        Timing.RunCoroutine(CancleCoroutineForReceptionistWhileInConversation().CancelWith(gameObject));
    }



    /// <summary>
    /// This will keep track of which trigger point should be active or not.
    /// </summary>
    /// <param name="turnOnIndex"></param>
    public IEnumerator<float> TurnOnAndOffSittingTriggers(int turnOnIndex)
    {
        yield return Timing.WaitForSeconds(1f);
        for (int i = 0; i < 4; i++)
        {
            if(i == turnOnIndex)
            {
                sittingTriggers[i].SetActive(true);
            }
            else
            {
                sittingTriggers[i].SetActive(false);
            }
        }
    }



    /// <summary>
    /// This will start the rotation of the any character.
    /// </summary>
    private IEnumerator<float> StartRotation(float waitTime)
    {
        yield return Timing.WaitForSeconds(waitTime);

        isPharmacistRotating = true;

        Timing.RunCoroutine(StopRotation(3f).CancelWith(gameObject));
    }
    


    /// <summary>
    /// This will stop the rotation of the any character.
    /// </summary>
    private IEnumerator<float> StopRotation(float waitTime)
    {
        yield return Timing.WaitForSeconds(waitTime);

        isPharmacistRotating = false;
        if(rotateToWhichDegree == 180)
        {
            rotateToWhichDegree = 0;
        }
    }



    /// <summary>
    /// This will be responsible for showing the medicines when the pharmacist will be playing "Show pills" animation.
    /// </summary>
    private IEnumerator<float> PositionMedicines(float waitTime)
    {
        canCallCoroutine = false;
        yield return Timing.WaitForSeconds(waitTime);

        for (int i = 0; i < 4; i++)
        {
            // Select some random medicines from the entire collection.
            selectedMedicines[i] = allMedicines.transform.GetChild(UnityEngine.Random.Range(0, allMedicines.transform.childCount)).gameObject;
            // Keep track of the original position to reset it after the display.
            medicinesOriginalPosition[i] = selectedMedicines[i].transform.position;
            // Assign the position to the selected medicines according to the display positions on the desk in front of pharmacist.
            selectedMedicines[i].transform.position = medicinesDisplayPosition[i].position;

            yield return Timing.WaitForSeconds(1.5f);
        }

        Timing.RunCoroutine(ResetMedicines(5f).CancelWith(gameObject));
    }



    /// <summary>
    /// This will be only used to bring back the medicines once they have been shown.
    /// </summary>
    /// <param name="waitTime"></param>
    private IEnumerator<float> ResetMedicines(float waitTime)
    {
        yield return Timing.WaitForSeconds(waitTime);

        for (int i = 0; i < 4; i++)
        {
            // Reset it.
            selectedMedicines[i].transform.position = medicinesOriginalPosition[i];
        }
    }



    /// <summary>
    /// This will be let the ring the landline after sometime.
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    private IEnumerator<float> RingLandLineAndRecieveAndPutDown(float waitTime)
    {
        yield return Timing.WaitForOneFrame;

        ringTheLandline = false;

        yield return Timing.WaitForSeconds(waitTime);

        receptionistOnCall = true;
        talkingPointReceptionistCollider.layer = LayerMask.NameToLayer("Default");

        receptionist.transform.GetChild(1).GetComponent<Usable>().enabled = false;
        MusicPlayer[2].PlayOneShot(SfxManager.sfxInstance.landLineRing);

        // This wsitTime will be the when it takes for the receptionist to start picking up the landline reciever.
        yield return Timing.WaitForSeconds(12f);

         ChangeAnimationState("Pick up", landLineRecieverAnimator);

        yield return Timing.WaitForSeconds(15f);

        ChangeAnimationState("Put down", landLineRecieverAnimator);
        receptionist.transform.GetChild(1).GetComponent<Usable>().enabled = true;
        receptionistOnCall = false;
    }



    /// <summary>
    /// This will let the vendor start the subtitle conservation when in front of the Pharmacist.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> VendorSubtitleToPlayerInFrontOfPharmacist(int startIndex, int endIndex)
    {
        leftHandPlateVendorPharmacy.SetActive(false);
        rightHandPlateVendorPharmacy.SetActive(false);
        subtitleText.gameObject.SetActive(true);

        for (int i = startIndex; i < endIndex; i++)
        {
            subtitleText.text = vendorSubtitle[i];

            SfxManager.sfxInstance.audioSource[1].PlayOneShot(SfxManager.sfxInstance.VendorDialogueInFrontOfElevator[i]);

            yield return Timing.WaitForSeconds(6f);

            subtitleText.text = null;

            yield return Timing.WaitForSeconds(2f);
        }

        yield return Timing.WaitForSeconds(3f);

        subtitleText.gameObject.SetActive(false);

        yield return Timing.WaitForSeconds(1f);
        leftHandPlateVendorPharmacy.SetActive(true);
        rightHandPlateVendorPharmacy.SetActive(true);      
    }



    /// <summary>
    /// This will let the vendor start the subtitle conservation when in front of the Receptionist.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> VendorInFrontOfReceptionist()
    {
        leftHandPlateVendorReception.SetActive(false);
        rightHandPlateVendorReception.SetActive(false);
        subtitleText.gameObject.SetActive(true);

        subtitleText.text = vendorSubtitle[6];

        SfxManager.sfxInstance.audioSource[1].PlayOneShot(SfxManager.sfxInstance.VendorDialogueInFrontOfElevator[6]);

        yield return Timing.WaitForSeconds(10f);

        subtitleText.text = null;

        yield return Timing.WaitForSeconds(2f);

        subtitleText.gameObject.SetActive(false);

        yield return Timing.WaitForSeconds(2f);

        leftHandPlateVendorReception.SetActive(true);
        rightHandPlateVendorReception.SetActive(true);
    }



    /// <summary>
    /// This will check if player is not near elevator then this function will call in nav move 
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> VendorWaitingInFrontOgElevator()
    {
        yield return Timing.WaitForSeconds(1f);

        Vendor[0].SetActive(false);
        firstPersonController.walkSpeed = 2;
        firstPersonController.bobSpeed = 5;
        elevatorButton[2].layer = LayerMask.NameToLayer("Interact");
    }



    /// <summary>
    /// Play Music for Pharamacy
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> PlayMusicPharmacy()
    {
        print("Music Player : " + MusicPlayer[0].GetComponent<AudioSource>() + "Nameeeee : " + MusicPlayer[0].name);
        for (int i = 0; i < SfxManager.sfxInstance.Songs.Length; i++)
        {           
            MusicPlayer[0].clip = SfxManager.sfxInstance.Songs[i];
            MusicPlayer[0].time = SfxManager.sfxInstance.currentAudioTime;
            MusicPlayer[0].Play();
            
            yield return Timing.WaitForSeconds(MusicPlayer[0].clip.length);

            if (i == SfxManager.sfxInstance.Songs.Length - 1)
            {
                Timing.RunCoroutine(PlayMusicPharmacy().CancelWith(gameObject), "KillIndoorSongs");
            }
        }        
    }



    /// <summary>
    /// Play Music For Reception
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> PlayMusicReception()
    {
        for (int i = 0; i < SfxManager.sfxInstance.ClasicalSongs.Length; i++)
        {
            MusicPlayer[1].clip = SfxManager.sfxInstance.ClasicalSongs[i];
            MusicPlayer[1].Play();

            yield return Timing.WaitForSeconds(MusicPlayer[1].clip.length - 1);

            if (i == SfxManager.sfxInstance.ClasicalSongs.Length - 1)
            {
                Timing.RunCoroutine(PlayMusicReception().CancelWith(gameObject), "KillIndoorSongs");
            }
        }
    }



    /// <summary>
    /// Display warning on elevator and reset after some time
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> DisplayAndResetWarning()
    {
        elevatorWarning.SetActive(true);
        elevatorWarning.transform.GetChild(0).GetComponent<TMP_Text>().text = "Please Interact with channa puri vendor first";

        yield return Timing.WaitForSeconds(4f);

        elevatorWarning.SetActive(false);
    }



    /// <summary>
    /// Display warning on elevator and reset after some time
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> DisplayAndResetWarningForElevatorFloorButton()
    {
        elevatorWarning.SetActive(true);
        elevatorWarning.transform.GetChild(0).GetComponent<TMP_Text>().text = "Floor closed";

        yield return Timing.WaitForSeconds(4f);

        elevatorWarning.SetActive(false);
    }



    /// <summary>
    /// Vendor will disable after some delay after serving food in reception
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> VendorServingFoodInReception()
    {
        yield return Timing.WaitForSeconds(4f);

        leftHandPlateVendorReception.SetActive(false);
        rightHandPlateVendorReception.SetActive(false);
        Vendor[1].SetActive(false);
        receptionFoodPlate[0].SetActive(true);
        receptionFoodPlate[1].SetActive(true);
    }



    /// <summary>
    /// This Function will cancle the corutions for receptionist and landline when player start conversation with receptionist
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> CancleCoroutineForReceptionistWhileInConversation()
    {
        yield return Timing.WaitForSeconds(2f);

        ringTheLandline = false;
        MusicPlayer[2].Stop();

        Timing.PauseCoroutines("RingLandLine");
        Timing.PauseCoroutines("StopRestartMovement");
        Timing.KillCoroutines("RingLandLine");
        Timing.KillCoroutines("StopRestartMovement");
    }



    /// <summary>
    /// Landline answring dialogues
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> ReceptionistDialogue()
    {
        if (GeneralManager.instance.playerSceneSwitchTracker == 1 && GeneralManager.instance.vendorVisitCounter == 1)
        {
            yield return Timing.WaitForSeconds(45f);

            receptionistAudioSource.clip = SfxManager.sfxInstance.receptionistDialogue[0];
            receptionistAudioSource.GetComponent<AudioSource>().Play();

            yield return Timing.WaitForSeconds(7f);

            receptionistAudioSource.clip = SfxManager.sfxInstance.receptionistDialogue[1];
            receptionistAudioSource.Play();
        }

        else
        {
            yield return Timing.WaitForSeconds(25f);

            receptionistAudioSource.clip = SfxManager.sfxInstance.receptionistDialogue[0];
            receptionistAudioSource.GetComponent<AudioSource>().Play();

            yield return Timing.WaitForSeconds(7f);

            receptionistAudioSource.clip = SfxManager.sfxInstance.receptionistDialogue[1];
            receptionistAudioSource.Play();
        }
    }



    /// <summary>
    /// Fake loading panel which give enging time to compile/ run all code
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> FakeDelayPanelClose()
    {
        yield return Timing.WaitForSeconds(5f);

        fakeLoadingPanel.SetActive(false);
        activePlayer.GetComponent<FirstPersonController>().enabled = true;

        if (GeneralManager.instance.vendorVisitCounter == 1)
        {
            firstPersonController.walkSpeed = 0;
            firstPersonController.bobSpeed = 0;
        }
        
    }
}
