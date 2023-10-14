//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using PixelCrushers.DialogueSystem;
//using UnityEngine.SceneManagement;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System;
using SWS;

public class OutdoorGameManager : MonoBehaviour
{
    public static OutdoorGameManager instance;

    #region Variables
    [Header("bool")]
    public bool[] isVendorStoolTaken = new bool[3];
    public bool canServeFood;
    public bool kidOneBattingTurn;
    public bool kidTwoBattingTurn;
    public bool canTriggerForTeleport;
    public bool playerCollidedWithVehicle;
    public bool playerCollidedWithNpc;
    public bool playerInConversation;
    [Space(10)]

    [Header("int")]
    public int platesActiveCounter = 0;
    public int plateDisposalCounter = 1;
    public int vendorEntryPointCounter;
    public int choosenColdDrinkIndex;
    [Space(10)]

    [Header("GameObject")]
    public GameObject[] plates = new GameObject[3];
    public GameObject[] npcSittingOnStools = new GameObject[3];
    public GameObject[] kidsBatting;
    public GameObject[] stools;
    public GameObject[] vendorEntryPoint;
    public GameObject cricketBall;
    public GameObject bowler;
    public GameObject bowlerHand;
    public GameObject fielder;
    public GameObject rollingDough;
    public GameObject dippingDoughInOil;
    public GameObject cookingSpoon;
    public GameObject cookingBread;
    public GameObject foodPlateBread;
    public GameObject staticSpoon;
    public GameObject channaPuriVendor;
    public GameObject allPlayers;
    public GameObject activePlayer;
    public GameObject talkBubble;
    public GameObject leftHandPlate;
    public GameObject rightHandPlate;
    public GameObject leftHandPlateServing;
    public GameObject rightHandPlateServing;
    public GameObject leftHandPlateCholeServing;
    public GameObject vendorRightHandSpoon;
    public GameObject controlsButton;
    public GameObject coldDrink;
    public GameObject fakeLoadingPanel;
    public GameObject foodPlatePrefab;
    private GameObject systems;
    public GameObject cursor;
    public GameObject coldDrinkCollider;

    [SerializeField] private GameObject vehiclesParent;
    [Space(10)]


    [Header("Transform")]
    public Transform[] plateDisposePoints = new Transform[3];
    public Transform[] plateResetPoints = new Transform[3];
    public Transform rollingDoughResetPoint;
    [HideInInspector]
    public Transform dippingDoughInOilResetPoint;
    [HideInInspector]
    public Transform cookingSpoonResetPoint;
    [HideInInspector]
    public Transform staticSpoonResetPoint;
    private Transform cricketBallResetPoint;
    public Transform clinicEntrance;
    public Transform outdoorSceneSpawnPoint;
    [Space(10)]

    [Header("Vector3")]
    public Vector3 coldDrinkBoxPosition;
    public Vector3 VendorStandingPosition;
    [Space(10)]

    [Header("Collider")]
    public List<Collider> vehiclesCollider = new List<Collider>();
    [Space(10)]

    [Header("Animator")]
    private Animator cricketBallAnimator;
    private Animator bowlerAnimator;
    private Animator fielderAnimator;
    private Animator channaPuriVendorAnimator;
    //public Animator rollingDoughAnimator;
    [Space(10)]

    [Header("Text")]
    public TMP_Text subtitleText;
    [Space(10)]

    [Header("String")]
    private string path;
    private string PlayerName;
    public string[] subtitle;
    public string[] outsideDogAnimations;
    public string[] bullAnimations;
    public string[] vendorSubtitle;
    public string[] cricketPlayersPathNames;
    private string currentStateCricketBall;
    private string currentStateEatingPlates;
    private string currentStateOfVendor;
    private readonly string[] typesOfThrows = { "Throw 1", "Throw 2" };
    private readonly string[] vendorConversationList = { "Vendor - 1st time visit", "Back from Clinic", "Vendor - 2nd time visit", "Brother - 3rd time visit",
    "Vendor - 4th time visit","Vendor - 5th visit repetitive" };
    [Space(10)]

    //[Space]
    //[Header("float")]
    // This is used as a timer after which all stools will be free to use again. Its value can be changed.
    //private float timerToResetOccupiedStools = 30f;

    [Header("Materials")]
    public Material coldDrinkMaterial;
    [Space(10)]

    [Header("Color")]
    public Color[] coldDrinkColor;
    [Space(10)]

    [Header("AudioSource")]
    public AudioSource[] MusicPlayer;
    public AudioSource[] CarsSource;
    [Space(10)]

    [Header("NPC Dialogue Trigger")]
    public DialogueSystemTrigger vendorDialogueSystemTrigger;

    [Header("Scripts")]
    public splineMove[] splineMove;
    SavedValue savedValues = new SavedValue();
    #endregion

    //Added by Gyan
    [SerializeField] private GameObject cursorInfo;

    //added by ankit
    System.Action<int> OnColliderAddded;
    public System.Action OnplayerCollidedWithVehicleFalse;
    //

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

                int whichPlay = PlayerPrefs.GetInt("First Play", 0);
                if (whichPlay == 0)
                {
                    controlsButton.SetActive(true);
                }
                else
                {
                    controlsButton.SetActive(false);
                    activePlayer.GetComponent<FirstPersonController>().enabled = true;
                }

                break;
            }
        }

        SfxManager.sfxInstance.audioSource[1] = channaPuriVendor.GetComponent<AudioSource>();

        Timing.RunCoroutine(FakeDelayPanelClose().CancelWith(gameObject));



        OnColliderAddded += (value) =>
        {
            if (value > 1)
            {
                Timing.RunCoroutine(MoveTheCollidedVehiclesOneByOne());
                print("Movement resumed");
            }
        };


        OnplayerCollidedWithVehicleFalse += () =>
        {
            OnColliderAddded?.Invoke(vehiclesCollider.Count);

        };
    }



    private void Start()
    {
        GetUserData();
        GeneralManager.instance.isOutdoorSceneActive = true;
        GeneralManager.instance.isIndoorSceneActive = false;
        GeneralManager.instance.hintButton.SetActive(false);

        if ((GeneralManager.instance.playerSceneSwitchTracker == 0) && GeneralManager.instance.vendorVisitCounter == 0)
        {
            vendorDialogueSystemTrigger.conversation = "Vendor - 1st time visit";
        }
        //else if ((GeneralManager.instance.playerSceneSwitchTracker == 2) && GeneralManager.instance.vendorVisitCounter == 0)
        //{
        //    vendorDialogueSystemTrigger.conversation = "Vendor - 1st time visit";
        //}
        else if (GeneralManager.instance.playerSceneSwitchTracker == 2 && GeneralManager.instance.vendorVisitCounter == 1)
        {
            Timing.PauseCoroutines("VendorConversationIndoor");
            Timing.KillCoroutines("VendorConversationIndoor");

            vendorDialogueSystemTrigger.conversation = "Vendor - 1st time visit";
        }
        else if (GeneralManager.instance.playerSceneSwitchTracker == 2 && GeneralManager.instance.vendorVisitCounter == 2)
        {
            vendorDialogueSystemTrigger.conversation = "Back from Clinic";
        }

        if (GeneralManager.instance.playerSceneSwitchTracker == 2)
        {
            GeneralManager.instance.playerSceneSwitchTracker = 0;
        }

        // Cache all the animators.
        cricketBallAnimator = cricketBall.GetComponent<Animator>();
        bowlerAnimator = bowler.GetComponent<Animator>();
        fielderAnimator = fielder.GetComponent<Animator>();
        channaPuriVendorAnimator = channaPuriVendor.GetComponent<Animator>();

        // Keep track of all the reset points before the game starts.
        rollingDoughResetPoint = rollingDough.transform;
        cricketBallResetPoint = cricketBall.transform;
        dippingDoughInOilResetPoint = dippingDoughInOil.transform;
        cookingSpoonResetPoint = cookingSpoon.transform;
        staticSpoonResetPoint = staticSpoon.transform;

        VendorAllObjectsOff();

        InvokeRepeating(nameof(ChangeAnimationStateBowler), 4f, 21f);

        Timing.PauseCoroutines("KillIndoorSongs");
        Timing.KillCoroutines("KillIndoorSongs");
        Timing.RunCoroutine(PlayMusicVendor().CancelWith(gameObject), "KillOutdoorSongs");
        Timing.RunCoroutine(VendorIdleDialogue().CancelWith(gameObject), "KillOutdoorVendorIdle");

        if (GeneralManager.instance.isMusicMute)
        {
            for (int i = 0; i < MusicPlayer.Length; i++)
            {
                MusicPlayer[i].mute = true;
            }
        }
        if (GeneralManager.instance.isSoundMute)
        {
            for (int i = 0; i < SfxManager.sfxInstance.audioSource.Length; i++)
            {
                SfxManager.sfxInstance.audioSource[i].mute = true;
            }
        }


        systems = GameObject.FindGameObjectWithTag("Systems");

        FindChildGameObjectByName(systems, "Music Slider").SetActive(true);
        FindChildGameObjectByName(systems, "Music Slider Indoor").SetActive(false);
        FindChildGameObjectByName(systems, "Music Toggle").GetComponent<ToggleSound>().toggleMusicIndoor = false;
        FindChildGameObjectByName(systems, "Music Toggle").GetComponent<ToggleSound>().toggleMusic = true;

        HintSystem.instance.ChangeMusicVolume(HintSystem.instance.musicVolumeSlider.value);

        splineMove = new splineMove[vehiclesParent.transform.childCount];
        for (int i = 0; i < vehiclesParent.transform.childCount; i++)
        {
            splineMove[i] = vehiclesParent.transform.GetChild(i).GetComponent<splineMove>();
        }

        //Set volume to 0.1, Temporary fix by Gyan
        for (int i = 0; i < MusicPlayer.Length; i++)
        {
            MusicPlayer[i].volume = 0.1f;
        }
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
        if (result.Data.ContainsKey("PlayerName"))
        {
            PlayerName = result.Data["PlayerName"].Value;
            DialogueLua.SetActorField("Player", "Display Name", PlayerName);
        }
    }
  


    /// <summary>
    /// Find Gameobject by name in gameobject child 
    /// </summary>
    /// <param name="topParentGameObject"></param>
    /// <param name="gameObjectName"></param>
    /// <returns></returns>
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



    private void Update()
    {
        if (vendorEntryPointCounter == 1)
        {
            talkBubble.SetActive(false);
            VendorInteractionOff();
        }

        if (vendorEntryPointCounter == 2)
        {
            vendorEntryPoint[1].SetActive(false);
        }

        if (vendorEntryPointCounter == 3)
        {
            canServeFood = true;
            channaPuriVendor.GetComponent<navMove>().controlSceneSpecificTask = true;
            RestartMovementOfAllThreeNPCSittingOnStool();
            vendorEntryPoint[0].SetActive(false);
            vendorEntryPoint[1].SetActive(false);
            vendorEntryPointCounter = 0;
            Timing.PauseCoroutines("KillOutdoorVendorIdle");
            Timing.KillCoroutines("KillOutdoorVendorIdle");
        }

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

            for (int i = 0; i < CarsSource.Length; i++)
            {
                CarsSource[i].Pause();
            }

            for (int i = 0; i < splineMove.Length; i++)
            {
                splineMove[i].PauseTyreRotation();
            }
            for (int i = 0; i < SfxManager.sfxInstance.audioSource.Length; i++)
            {
                SfxManager.sfxInstance.audioSource[i].Pause();
            }
            Time.timeScale = 0f;
        }
    }



    /// <summary>
    /// This will play some Dialogue when vendor is idle 
    /// </summary>
    public void PlayVendorIdleDialogueFunction()
    {
        Timing.RunCoroutine(VendorIdleDialogue().CancelWith(gameObject), "KillOutdoorVendorIdle");
    }



    /// <summary>
    /// This will Stop some Dialogue when vendor is idle 
    /// </summary>
    public void StopVendorIdleDialogue()
    {
        Timing.PauseCoroutines("KillOutdoorVendorIdle");
        Timing.KillCoroutines("KillOutdoorVendorIdle");
    }



    /// <summary>
    /// This will check if player Is in conversation or not
    /// </summary>
    /// <param name="value"></param>
    public void PlayerInConversationVariableOnOrOff(bool value)
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
    /// This function will enable player controls after viewing controls
    /// </summary>
    public void EnablePlayerMovementAfterToturial()
    {
        activePlayer.GetComponent<FirstPersonController>().enabled = true;
        PlayerPrefs.SetInt("First Play", 1);
        PlayerPrefs.Save();
    }



    /// <summary>
    /// This will call the animation of the plates while disposing them.
    /// </summary>
    public void DisposePlatesAfterEating()
    {
        if (plateDisposalCounter <= 3)
        {
            ChangeAnimationStateEatingPlates("Dispose plate " +
                plateDisposalCounter,
                plates[plateDisposalCounter - 1].GetComponent<Animator>());

            plateDisposalCounter++;
        }
    }



    /// <summary>
    /// This will responsilbe for playing the this given animation everytime after a time interval. InvokeRepeating is used here to call this method.
    /// </summary>
    public void CricketBallContinousMotion()
    {
        ChangeAnimationStateCricketBall(typesOfThrows[UnityEngine.Random.Range(0, 2)]);
    }



    /// <summary>
    /// Changes the animations for cricket ball. 
    /// </summary>
    public void ChangeAnimationStateCricketBall(string newState)
    {
        if (currentStateCricketBall == newState) return;

        cricketBallAnimator.Play(newState);

        currentStateCricketBall = newState;

        // Reset the ball position after everytime animation plays.
        Timing.RunCoroutine(ResetPositionOfCricketBall().CancelWith(gameObject));
    }



    /// <summary>
    /// Changes the animation state of the  
    /// </summary>
    public void ChangeAnimationStateBowler()
    {
        bowlerAnimator.Play("Throw");

        Timing.RunCoroutine(CricketBallRemoveParentAndPlayAnimation().CancelWith(gameObject));
    }



    /// <summary>
    /// Changes the animations for cricket ball. 
    /// </summary>
    public void ChangeAnimationStateFielder()
    {
        fielderAnimator.Play("Catch");
    }



    /// <summary>
    /// This is increase the counter so that in the next visit the next conversation plays.
    /// </summary>
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
        // This will let the vendor walks towards the clinic and the player should follow him.
        if (GeneralManager.instance.vendorVisitCounter == 1)
        {
            Timing.RunCoroutine(VendorSubtitleToPlayerAfterFirstConversationEnds(), "VendorConversation");
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
    }



    /// <summary>
    /// turn on / off vendor talk bubble
    /// </summary>
    public void VendorTalkBubble()
    {
        if (GeneralManager.instance.vendorVisitCounter == 1)
        {
            talkBubble.SetActive(false);
        }
        else
        {
            talkBubble.SetActive(true);
        }
    }



    /// <summary>
    /// This function will pause the flow of dialogue
    /// </summary>
    public void DialoguePauseDuringDrinkServe()
    {
        DialogueTime.isPaused = true;
    }



    /// <summary>
    /// This function will resume the flow of dialogue 
    /// </summary>
    public void DialogueResumeDrinkServe()
    {
        DialogueTime.isPaused = false;
    }



    /// <summary>
    /// play dialogue audio from vedor audio source
    /// </summary>
    /// <param name="vendorDialogue"></param>
    public void PlayDialogueAudio(AudioClip vendorDialogue)
    {
        SfxManager.sfxInstance.audioSource[1].Stop();
        SfxManager.sfxInstance.audioSource[1].PlayOneShot(vendorDialogue);
    }



    /// <summary>
    /// This function will set value and change color of cold drink according to choosen cold drink
    /// </summary>
    /// <param name="coldDrinkIndex"></param>
    public void ChoosenColdDrink(int coldDrinkIndex)
    {
        choosenColdDrinkIndex = coldDrinkIndex;

        if (choosenColdDrinkIndex == 1)
        {
            coldDrinkMaterial.color = coldDrinkColor[0];
        }
        else if (choosenColdDrinkIndex == 2)
        {
            coldDrinkMaterial.color = coldDrinkColor[1];
        }
        else if (choosenColdDrinkIndex == 3)
        {
            coldDrinkMaterial.color = coldDrinkColor[2];
        }
    }



    /// <summary>
    /// Turn of all vendor object at start
    /// </summary>
    public void VendorAllObjectsOff()
    {
        //Vendor Objects
        leftHandPlateServing.SetActive(false);
        rightHandPlateServing.SetActive(false);
        leftHandPlate.SetActive(false);
        rightHandPlate.SetActive(false);
        leftHandPlateCholeServing.SetActive(false);
        vendorRightHandSpoon.SetActive(false);
        rollingDough.SetActive(false);
        dippingDoughInOil.SetActive(false);
        cookingSpoon.SetActive(false);
        cookingBread.SetActive(false);
        foodPlateBread.SetActive(false);
        staticSpoon.SetActive(false);
        coldDrinkCollider.SetActive(false);
    }



    /// <summary>
    /// This is decide which conversation has to played according to the counter value.
    /// </summary>
    public void SetTheNextConversationForVendor(string nextDialogue)
    {
        vendorDialogueSystemTrigger.conversation = nextDialogue;
    }



    /// <summary>
    /// Main function that will call animation in between dialogues
    /// </summary>
    public void VendorServeFoodToPlayer(string state)
    {
        ChangeAnimationStateForVendor(state, channaPuriVendorAnimator);
        if (state == "Serving")
        {
            Timing.RunCoroutine(TurnOffPlate().CancelWith(gameObject));

            IEnumerator<float> TurnOffPlate()
            {
                yield return Timing.WaitForSeconds(2f);

                leftHandPlateServing.SetActive(false);
                rightHandPlateServing.SetActive(false);
            }
        }
    }



    /// <summary>
    /// Null the current state of vendor after serving 
    /// </summary>
    public void NullAnimationStateForVendor()
    {
        currentStateOfVendor = null;
    }


    /// <summary>
    /// Changes the animation of Vendor
    /// </summary>
    /// <param name="newState"></param>
    /// <param name="animator"></param>
    public void ChangeAnimationStateForVendor(string newState, Animator animator)
    {
        if (currentStateOfVendor == newState) return;

        animator.Play(newState);

        currentStateOfVendor = newState;
    }



    /// <summary>
    /// Changes the animations for eating plates. 
    /// </summary>
    public void ChangeAnimationStateEatingPlates(string newState, Animator animator)
    {
        if (currentStateEatingPlates == newState) return;

        animator.Play(newState);

        currentStateEatingPlates = newState;
    }



    /// <summary>
    /// This function will loop through NPC' at once and sync their restart movement along with disposal of plates
    /// </summary>
    private void RestartMovementOfAllThreeNPCSittingOnStool()
    {
        Timing.RunCoroutine(EnableVendorEntryNullStoolsAndNpcSitting(81).CancelWith(gameObject));

        for (int i = 0; i < 3; i++)
        {
            npcSittingOnStools[i].GetComponent<navMove>().RestartMovementOfAllThreeNPCSittingOnStool1by1Function();
        }
    }



    /// <summary>
    /// Changes Interaction layer of vendor to turn off rectangle Crosshair.
    /// </summary>
    public void VendorInteractionOff()
    {
        activePlayer.GetComponent<Selector>().enabled = false;
        channaPuriVendor.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
    }



    /// <summary>
    /// Changes Interaction layer of vendor to turn On rectangle Crosshair.
    /// </summary>
    public void VendorInteractionOn()
    {
        activePlayer.GetComponent<Selector>().enabled = true;
        channaPuriVendor.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Interact");
    }



    /// <summary>
    /// Player Selector will be disable/ enable this will be called on vendor movement start
    /// </summary>
    /// <param name="decision"></param>
    public void PlayerSelectorDisableAndEnable(bool decision)
    {
        if (decision)
        {
            activePlayer.GetComponent<Selector>().enabled = true;
        }
        else
        {
            activePlayer.GetComponent<Selector>().enabled = false;
        }

    }



    /// <summary>
    /// Add Cehicles colliders
    /// </summary>
    /// <param name="_collider"></param>
    public void AddCollider(Collider _collider)
    {
        vehiclesCollider.Add(_collider);
    }



    /// <summary>
    /// Calling the coutrution in simple function so that we can call it on event
    /// </summary>
    public void VendorObjectsOnFunction()
    {
        Timing.RunCoroutine(VendorObjectsOn().CancelWith(gameObject));

        // This will enable objects of vendor when he start cooking
        IEnumerator<float> VendorObjectsOn()
        {
            rollingDough.SetActive(true);

            yield return Timing.WaitForSeconds(10f);

            rollingDough.SetActive(false);
            dippingDoughInOil.SetActive(true);

            yield return Timing.WaitForSeconds(1f);

            cookingSpoon.SetActive(true);

            yield return Timing.WaitForSeconds(2f);

            dippingDoughInOil.SetActive(false);


            yield return Timing.WaitForSeconds(0.5f);

            cookingBread.SetActive(true);

            yield return Timing.WaitForSeconds(4.5f);

            cookingBread.SetActive(false);
            staticSpoon.SetActive(true);
            foodPlateBread.SetActive(true);

            yield return Timing.WaitForSeconds(1f);

            cookingSpoon.SetActive(false);

            yield return Timing.WaitForSeconds(2f);

            staticSpoon.SetActive(false);
            foodPlateBread.SetActive(false);

            if (playerInConversation)
            {
                staticSpoon.SetActive(true);
                foodPlateBread.SetActive(true);
                VendorServeFoodToPlayer("Idle");
            }
        }
    }



    /// <summary>
    /// This will spawn the plates after few seconds because few seconds are taken to transfer the plates from vendor to NPC.
    /// </summary>
    public IEnumerator<float> SpawnPlates()
    {
        yield return Timing.WaitForSeconds(3f);

        // Active the plates when the NPC will eat and increase the counter so that next time next plate should be activated.
        if (platesActiveCounter <= 2)
        {
            // This will make the plates spawn just near the hands of the NPCS. Below points let us reset the elements of the plate.
            plates[platesActiveCounter].transform.position = plateResetPoints[platesActiveCounter].position;
            plates[platesActiveCounter].transform.GetChild(0).gameObject.SetActive(true);
            plates[platesActiveCounter].transform.GetChild(1).gameObject.SetActive(true);
            plates[platesActiveCounter].SetActive(true);
            platesActiveCounter++;
        }

    }



    /// <summary>
    /// This resets the position of the cricket ball so that it can be used again.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> ResetPositionOfCricketBall()
    {
        // Depending on the time taken by throw then only reset the position of the cricket ball.
        if (currentStateCricketBall == "Throw 1")
        {
            yield return Timing.WaitForSeconds(6f);

            cricketBall.transform.SetParent(bowlerHand.transform);
            cricketBall.transform.position = cricketBallResetPoint.position;
        }

        else if (currentStateCricketBall == "Throw 2")
        {
            yield return Timing.WaitForSeconds(12f);

            cricketBall.transform.SetParent(bowlerHand.transform);
            cricketBall.transform.position = cricketBallResetPoint.position;
        }

        currentStateCricketBall = "";
    }



    /// <summary>
    /// This Function will enable the vendor entry point after Npc leave the stall
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> EnableVendorEntryNullStoolsAndNpcSitting(int waitTime)
    {
        yield return Timing.WaitForSeconds(waitTime);

        for (int i = 0; i < 3; i++)
        {
            npcSittingOnStools[i] = null;
        }

        VendorInteractionOn();

        for (int i = 0; i < 3; i++)
        {
            isVendorStoolTaken[i] = false;
        }

        if (!DialogueManager.isConversationActive)
        {
            vendorEntryPoint[0].SetActive(true);
            vendorEntryPoint[1].SetActive(true);
        }
    }



    /// <summary>
    /// This will let the vendor start the subtitle conservation when First Conversation Ends
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> VendorSubtitleToPlayerAfterFirstConversationEnds()
    {
        yield return Timing.WaitForSeconds(5f);

        subtitleText.gameObject.SetActive(true);
        for (int i = 0; i < 1; i++)
        {
            subtitleText.text = vendorSubtitle[i];
            SfxManager.sfxInstance.audioSource[1].PlayOneShot(SfxManager.sfxInstance.VendorDialogueAfterFirstConversation[i]);

            yield return Timing.WaitForSeconds(2f);

            subtitleText.text = null;
        }

        yield return Timing.WaitForSeconds(0.1f);

        subtitleText.gameObject.SetActive(false);

        yield return Timing.WaitForSeconds(0.1f);

        channaPuriVendor.GetComponent<navMove>().moveToPath = false;
        channaPuriVendor.GetComponent<navMove>().SetPath(WaypointManager.Paths["Vendor Clinic Path"]);
        leftHandPlate.SetActive(true);
        rightHandPlate.SetActive(true);

        yield return Timing.WaitForSeconds(8f);

        subtitleText.gameObject.SetActive(true);
        for (int i = 1; i < vendorSubtitle.Length; i++)
        {
            subtitleText.text = vendorSubtitle[i];
            SfxManager.sfxInstance.audioSource[1].PlayOneShot(SfxManager.sfxInstance.VendorDialogueAfterFirstConversation[i]);

            yield return Timing.WaitForSeconds(3f);

            subtitleText.text = null;

            yield return Timing.WaitForSeconds(6f);
        }

        yield return Timing.WaitForSeconds(1f);

        subtitleText.gameObject.SetActive(false);
    }



    /// <summary>
    /// vendor will disapper if player doesn't go inside clinic under 30 sec 
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> VendorGoBackToShopAfterClinic()
    {
        yield return Timing.WaitForSeconds(30f);

        channaPuriVendor.SetActive(false);

        yield return Timing.WaitForSeconds(120f);

        channaPuriVendor.SetActive(true);
        channaPuriVendor.GetComponent<navMove>().SetPath(WaypointManager.Paths["Vendor Path 1"]);
        channaPuriVendor.GetComponent<navMove>().Stop();

        GeneralManager.instance.vendorVisitCounter = 0;

        vendorDialogueSystemTrigger.conversation = "Vendor - 1st time visit";
        channaPuriVendor.transform.SetPositionAndRotation(VendorStandingPosition, Quaternion.Euler(0, 180, 0));
        VendorInteractionOn();
        talkBubble.SetActive(true);

        Timing.RunCoroutine(EnableVendorEntryNullStoolsAndNpcSitting(10).CancelWith(gameObject));
        Timing.RunCoroutine(VendorIdleDialogue().CancelWith(gameObject), "KillOutdoorVendorIdle");
    }



    /// <summary>
    /// This function will play music in outdoor scene from radio 
    /// </summary>
    /// <returns></returns>
    /// 
    private IEnumerator<float> PlayMusicVendor()
    {
        for (int i = 0; i < SfxManager.sfxInstance.Songs.Length; i++)
        {
            MusicPlayer[0].clip = SfxManager.sfxInstance.Songs[i];
            MusicPlayer[0].Play();
            MusicPlayer[1].clip = SfxManager.sfxInstance.Songs[i];
            MusicPlayer[1].Play();
            MusicPlayer[2].clip = SfxManager.sfxInstance.Songs[i];
            MusicPlayer[2].Play();
            MusicPlayer[3].clip = SfxManager.sfxInstance.Songs[i];
            MusicPlayer[3].Play();

            yield return Timing.WaitForSeconds(MusicPlayer[0].clip.length);

            if (i == SfxManager.sfxInstance.Songs.Length - 1)
            {
                Timing.RunCoroutine(PlayMusicVendor());
            }
        }

        SfxManager.sfxInstance.GetCurrentAudioSource(MusicPlayer[0].time);
    }



    /// <summary>
    /// This will play some Dialogue when vendor is idle 
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> VendorIdleDialogue()
    {
        for (int i = 0; i < SfxManager.sfxInstance.vendorIdleDialogue.Length; i++)
        {
            SfxManager.sfxInstance.audioSource[1].clip = SfxManager.sfxInstance.vendorIdleDialogue[i];
            SfxManager.sfxInstance.audioSource[1].Play();

            yield return Timing.WaitForSeconds(SfxManager.sfxInstance.audioSource[1].clip.length);

            if (i == SfxManager.sfxInstance.vendorIdleDialogue.Length - 1)
            {
                Timing.RunCoroutine(VendorIdleDialogue().CancelWith(gameObject), "KillOutdoorVendorIdle");
            }
        }
    }



    /// <summary>
    /// This function will play ball animation and make ball independent
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> CricketBallRemoveParentAndPlayAnimation()
    {
        yield return Timing.WaitForSeconds(3f);

        cricketBall.transform.parent = null;
        CricketBallContinousMotion();

    }



    /// <summary>
    /// Fake loading panel which give enging time to compile/ run all code
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> FakeDelayPanelClose()
    {
        yield return Timing.WaitForSeconds(2f);
        fakeLoadingPanel.SetActive(false);
    }



    /// <summary>
    /// This is responsible to enable the plates on the vendor stalls edge when players come near it
    /// </summary>
    /// <param name="plateInstantiatePoint"></param>
    /// <returns></returns>
    public IEnumerator<float> EnablePlatesAndDisableAfterDelay(GameObject plateInstantiatePoint)
    {
        GameObject plate = Instantiate(foodPlatePrefab);
        plate.transform.parent = plateInstantiatePoint.transform;
        plate.transform.localPosition = Vector3.zero;
        plate.transform.localRotation = Quaternion.identity;

        yield return Timing.WaitForSeconds(4f);

        Destroy(plate);
    }



    /// <summary>
    /// This will be responsible for moving the stuck vehicles one by one
    /// </summary>
    /// <returns></returns>
    public IEnumerator<float> MoveTheCollidedVehiclesOneByOne()
    {
        yield return Timing.WaitForSeconds(1f);

        int t = 0;
        if (vehiclesCollider.Count >= 1)
        {
            for (int i = 0; i < vehiclesCollider.Count; i++)
            {
                StartCoroutine(vehiclesCollider[i].transform.parent.GetComponent<splineMove>().Wait(UnityEngine.Random.Range(t + 0.02f, t + 0.5f)));
            }
        }
        vehiclesCollider.Clear();
    }

}
