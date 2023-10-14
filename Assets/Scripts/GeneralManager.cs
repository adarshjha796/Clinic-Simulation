using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.SceneManagement;

public class GeneralManager : MonoBehaviour
{
    public static GeneralManager instance;

    [Header("Int")]
    public int playerSceneSwitchTracker;
    // This will keep track of how many time the player has visit the vendor for conversation.
    public int vendorVisitCounter;

    [Header("Float")]
    public float hintDuration;
    public float resetHintDuration;
    [HideInInspector]
    public float playerX;
    [HideInInspector]
    public float playerY;
    [HideInInspector]
    public float playerZ;
    public Vector3 playerPastPosition;

    [Header("GameObject")]
    public GameObject hintButton;
    public GameObject playerTaskPanel;

    [Header("Text")]
    public TMP_Text task;

    [Header("bool")]
    public bool isOutdoorSceneActive;
    public bool isIndoorSceneActive;
    public bool startTimer;
    public bool isGamePause;
    public bool isMusicMute;
    public bool isSoundMute;
    public bool isButtonPressed;

    [Header("String")]
    public string wherePlayerWas;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            //DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        //if (/*GeneralManager.instance.wherePlayerWas == "Indoor" && */SceneManager.GetActiveScene().buildIndex == 3)
        //{
        //    if (GeneralManager.instance.isIndoorSceneActive)
        //    {
        //        print("AAAAAAAAA");
        //        IndoorGameManager.instance.elevatorButtonScript.SendCall("Pharmacy");
        //    }
        //}
        resetHintDuration = hintDuration;
    }


    void Update()
    {
        if (startTimer)
        {
            hintDuration -= Time.deltaTime;
            if (hintDuration <= 0.0f && vendorVisitCounter == 0 && isOutdoorSceneActive)
            {
                hintDuration = resetHintDuration;
                hintButton.SetActive(true);
                task.text = "Go to Channa Puri Stall & Interact with Channa Puri Vendor";
                startTimer = false;
            }

            if (hintDuration <= 0.0f && vendorVisitCounter == 1 && isOutdoorSceneActive)
            {
                hintDuration = resetHintDuration;
                hintButton.SetActive(true);
                task.text = "Go Inside the clinic";
                startTimer = false;
            }

            if (hintDuration <= 0.0f && vendorVisitCounter == 1 && isIndoorSceneActive)
            {
                hintDuration = resetHintDuration;
                hintButton.SetActive(true);
                task.text = "Call Elevator Then Go to reception floor then interact with receptionist";
                startTimer = false;
            }

            if (hintDuration <= 0.0f && vendorVisitCounter == 2 && isIndoorSceneActive)
            {
                hintDuration = resetHintDuration;
                hintButton.SetActive(true);
                task.text = "Go outside the clinic";
                startTimer = false;
            }
        }
    }



    /// <summary>
    /// This will open / close panel
    /// </summary>
    public void PlayerTaskOpen()
    {
        startTimer = true;
        if (!playerTaskPanel.gameObject.activeSelf)
        {
            playerTaskPanel.SetActive(true);
        }
        else
        {
            playerTaskPanel.SetActive(false);
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
    /// This will resume the game and set time scale to 1
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1.0f;
        isGamePause = false;

        if (isOutdoorSceneActive)
        {
            OutdoorGameManager.instance.activePlayer.GetComponent<CrosshairGUI>().CursorControl();
            OutdoorGameManager.instance.cursor.SetActive(true);

            //var randomMusic = UnityEngine.Random.Range(0, OutdoorGameManager.instance.MusicPlayer.Length - 1);
            for (int i = 0; i < OutdoorGameManager.instance.MusicPlayer.Length; i++)
            {
                OutdoorGameManager.instance.MusicPlayer[i].Play();
            }
            for (int i = 0; i < OutdoorGameManager.instance.CarsSource.Length; i++)
            {
                OutdoorGameManager.instance.CarsSource[i].Play();
            }

            for (int i = 0; i < OutdoorGameManager.instance.splineMove.Length; i++)
            {
                OutdoorGameManager.instance.splineMove[i].ResumeTyreRotation();
            }

            for (int i = 0; i < SfxManager.sfxInstance.audioSource.Length; i++)
            {
                SfxManager.sfxInstance.audioSource[i].Play();
            }
        }

        if (isIndoorSceneActive)
        {
            IndoorGameManager.instance.activePlayer.GetComponent<CrosshairGUI>().CursorControl();
            IndoorGameManager.instance.cursor.SetActive(true);

            if (IndoorGameManager.instance.elevatorController.floorNumber == 0)
            {                                                                            
                IndoorGameManager.instance.MusicPlayer[0].Play();                        
            }                                                                            
                                                                                         
            if (IndoorGameManager.instance.elevatorController.floorNumber == 1)
            {
                IndoorGameManager.instance.MusicPlayer[1].Play();
            }

            if (IndoorGameManager.instance.playerInteractedWithDog)
            {
                IndoorGameManager.instance.cameraSwitcher.dogPlayPanel.SetActive(true);
            }

            if (IndoorGameManager.instance.playerInteractedWithReceptionist)
            {
                IndoorGameManager.instance.cameraSwitcher.receptionistPanel.SetActive(true);
            }

            if (IndoorGameManager.instance.playerInteractedWithSofa)
            {
                IndoorGameManager.instance.cameraSwitcher.sofaPanel.SetActive(true);
            }

            for (int i = 0; i < SfxManager.sfxInstance.audioSource.Length; i++)
            {
                SfxManager.sfxInstance.audioSource[i].Play();
            }
        }
    }

    public void SavePlayerLocation()
    {
        if (isOutdoorSceneActive)
        {
            playerPastPosition = OutdoorGameManager.instance.activePlayer.transform.position;
        }
        else if (isIndoorSceneActive)
        {
            playerPastPosition = IndoorGameManager.instance.activePlayer.transform.position;
        }
       

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"PlayerX" ,"" + playerPastPosition.x },
                {"PlayerY" ,"" + playerPastPosition.y },
                {"PlayerZ" ,"" + playerPastPosition.z },
                {"Outdoor" , "" + isOutdoorSceneActive },
                {"Indoor"  , "" + isIndoorSceneActive },
            }
        };
        if (isButtonPressed == true)
        {
            PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
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
        if (result.Data != null && result.Data.ContainsKey("Outdoor") && result.Data.ContainsKey("Indoor"))
        {
            if (result.Data["Outdoor"].Value == "True")
            {               
               playerPastPosition.x = float.Parse(result.Data["PlayerX"].Value);
               playerPastPosition.y = float.Parse(result.Data["PlayerY"].Value);
               playerPastPosition.z = float.Parse(result.Data["PlayerZ"].Value);

                Vector3 loadPosition = playerPastPosition;
                if (result.Data.ContainsKey("vendorVisitCounter"))
                {
                    vendorVisitCounter = int.Parse(result.Data["vendorVisitCounter"].Value);
                    playerSceneSwitchTracker = 0;
                }
                print(loadPosition);

                SceneManager.LoadScene(2);

                StartCoroutine(WaitForActivePlayerOutdoor(loadPosition));                
            }
            else if (result.Data["Indoor"].Value == "True")
            {
                playerPastPosition.x = float.Parse(result.Data["PlayerX"].Value);
                playerPastPosition.y = float.Parse(result.Data["PlayerY"].Value);
                playerPastPosition.z = float.Parse(result.Data["PlayerZ"].Value);

                Vector3 loadPosition = playerPastPosition;
                if (result.Data.ContainsKey("vendorVisitCounter"))
                {
                    vendorVisitCounter = int.Parse(result.Data["vendorVisitCounter"].Value);
                    playerSceneSwitchTracker = 1;
                }

                SceneManager.LoadScene(3);
                StartCoroutine(WaitForActivePlayerIndoor(loadPosition));
            }
        }
       
        else
        {
            SceneManager.LoadScene(1);
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

    static bool ReadyToQuit = false;



    public static bool WantsToQuit()
    {
        instance.StartCoroutine(instance.ExampleCoroutine());
        return ReadyToQuit;
    }



    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        Application.wantsToQuit += WantsToQuit;
    }

    public IEnumerator ExampleCoroutine()
    {
        isButtonPressed = true;
        SavePlayerLocation();
        //Exit();
        yield return new WaitForSeconds(1.5f);
        ReadyToQuit = true;
        Application.Quit();
    }

    /// <summary>
    /// This Function will be called when saving value to playfab is succesfull
    /// </summary>
    /// <param name="result"></param>
    private void OnDataSend(UpdateUserDataResult result)
    {
        print("Data Save");
        Application.Quit();
    }

    /// <summary>
    /// This function quit the game
    /// </summary>
    public void Exit()
    {
        Time.timeScale = 1f;
        StartCoroutine(ExampleCoroutine());
        //isButtonPressed = true;
        //SavePlayerLocation();
        //Application.Quit();
    }

    private IEnumerator WaitForActivePlayerOutdoor(Vector3 loadPosition)
    {
        while (OutdoorGameManager.instance == null)
        {
            yield return null;
        }
        OutdoorGameManager.instance.activePlayer.transform.SetPositionAndRotation(loadPosition, Quaternion.identity);
    }

    private IEnumerator WaitForActivePlayerIndoor(Vector3 loadPosition)
    {
        while (IndoorGameManager.instance == null)
        {
            yield return null;
        }
        IndoorGameManager.instance.activePlayer.transform.SetPositionAndRotation(loadPosition, Quaternion.identity);
    }
}
