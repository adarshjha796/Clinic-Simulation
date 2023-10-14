//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager sfxInstance;

    [Header("AudioSource")]
    public AudioSource[] audioSource;

    [Header("Audio clips")]
    public AudioClip buttonClick;
    public AudioClip flushClip;
    public AudioClip peeingClip;
    public AudioClip tapWaterClip;
    public AudioClip landLineRing;
    public AudioClip doorSwing;
    public AudioClip sofaSitingClip;
    public AudioClip chairPullOutClip;
    public AudioClip Baithofemale;
    public AudioClip Baithomale;
    public AudioClip Baithotrans;
    public AudioClip Leytjaofemale;
    public AudioClip Leytjaomale;
    public AudioClip Leytjaotrans;
    public AudioClip Sojaofemale;
    public AudioClip Sojaomale;
    public AudioClip Sojaotrans;
    public AudioClip[] npcCollisionDialogueMale;
    public AudioClip[] npcCollisionDialogueFemale;
    public AudioClip[] VendorDialogueAfterFirstConversation;
    public AudioClip[] VendorDialogueInFrontOfElevator;
    public AudioClip DogBarking;
    //public AudioClip BullSound;
    public AudioClip CarHorn;
    public AudioClip AutoHorn;
    public AudioClip[] Songs;
    public AudioClip[] ClasicalSongs;
    public AudioClip[] vendorIdleDialogue;
    public AudioClip[] receptionistDialogue;

    public float currentAudioTime;

    private void Awake()
    {
        if (sfxInstance != null && sfxInstance != this)
        {
            Destroy(gameObject);
            return;
        }
        sfxInstance = this;
       // DontDestroyOnLoad(this);
    }



    private void Start()
    {
        buttonClick = Resources.Load<AudioClip>("Audios/Click sound");

        //Player collision events
        flushClip = Resources.Load<AudioClip>("Audios/Toilet/Flushing");
        peeingClip = Resources.Load<AudioClip>("Audios/Toilet/Peeing");
        doorSwing = Resources.Load<AudioClip>("Audios/Door/Door Swing");
        tapWaterClip = Resources.Load<AudioClip>("Audios/Water/TapWater");
        landLineRing = Resources.Load<AudioClip>("Audios/Landline/Landline ring");
        sofaSitingClip = Resources.Load<AudioClip>("Audios/Sitting/Sofa Siting Sound");
        chairPullOutClip = Resources.Load<AudioClip>("Audios/Sitting/Chair Pull Out Sound");
        Baithofemale = Resources.Load<AudioClip>("Audios/Dialogues/Female Voice/Baitho female");
        Baithomale = Resources.Load<AudioClip>("Audios/Dialogues/Male Voice/Baitho male");
        Baithotrans = Resources.Load<AudioClip>("Audios/Dialogues/Trans Voice/Baitho trans");
        Leytjaofemale = Resources.Load<AudioClip>("Audios/Dialogues/Female Voice/Leyt jao female");
        Leytjaomale = Resources.Load<AudioClip>("Audios/Dialogues/Male Voice/Leyt jao male");
        Leytjaotrans = Resources.Load<AudioClip>("Audios/Dialogues/Trans Voice/Leyt jao trans");
        Sojaofemale = Resources.Load<AudioClip>("Audios/Dialogues/Female Voice/So jao female");
        Sojaomale = Resources.Load<AudioClip>("Audios/Dialogues/Male Voice/So jao male");
        Sojaotrans = Resources.Load<AudioClip>("Audios/Dialogues/Trans Voice/So jao trans");

        //Animals Sounds
        DogBarking = Resources.Load<AudioClip>("Audios/Animals/Dogs/Dog Bark");
        //BullSound = Resources.Load<AudioClip>("Audios/Animals/Bull/Dog Bark");

        //Car Sounds
        CarHorn = Resources.Load<AudioClip>("Audios/Vehicle/Car Horn Sound Effect");
        AutoHorn = Resources.Load<AudioClip>("Audios/Vehicle/Auto Horn");

        //Npc Collision with player 
        npcCollisionDialogueFemale[0] = Resources.Load<AudioClip>("Audios/Dialogues/Female Voice/Aap kya kar rahe ho female");
        npcCollisionDialogueMale[0] = Resources.Load<AudioClip>("Audios/Dialogues/Male Voice/Aap kya kar rahe ho male");
        npcCollisionDialogueFemale[1] = Resources.Load<AudioClip>("Audios/Dialogues/Female Voice/Dekh ke female");
        npcCollisionDialogueMale[1] = Resources.Load<AudioClip>("Audios/Dialogues/Male Voice/Dekh ke male");
        npcCollisionDialogueFemale[2] = Resources.Load<AudioClip>("Audios/Dialogues/Female Voice/Andhe ho kya female");
        npcCollisionDialogueMale[2] = Resources.Load<AudioClip>("Audios/Dialogues/Male Voice/Andhe ho kya male");
        npcCollisionDialogueFemale[3] = Resources.Load<AudioClip>("Audios/Dialogues/Female Voice/jaldi mei ho kya female");
        npcCollisionDialogueMale[3] = Resources.Load<AudioClip>("Audios/Dialogues/Male Voice/jaldi mei ho kya male");
        npcCollisionDialogueFemale[4] = Resources.Load<AudioClip>("Audios/Dialogues/Female Voice/Tum thik ho female");
        npcCollisionDialogueMale[4] = Resources.Load<AudioClip>("Audios/Dialogues/Male Voice/Tum thik ho male");
        npcCollisionDialogueFemale[5] = Resources.Load<AudioClip>("Audios/Dialogues/Female Voice/Thik se female");
        npcCollisionDialogueMale[5] = Resources.Load<AudioClip>("Audios/Dialogues/Male Voice/Thik se male");
        npcCollisionDialogueFemale[6] = Resources.Load<AudioClip>("Audios/Dialogues/Female Voice/Thik se chalo female");
        npcCollisionDialogueMale[6] = Resources.Load<AudioClip>("Audios/Dialogues/Male Voice/Thik se chalo male");
        npcCollisionDialogueFemale[7] = Resources.Load<AudioClip>("Audios/Dialogues/Female Voice/Aaram se chalo female");
        npcCollisionDialogueMale[7] = Resources.Load<AudioClip>("Audios/Dialogues/Male Voice/Aaram se chalo male");

        // Vendor Outdoor Idle Dialodues
        vendorIdleDialogue[0] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/Aao Jaldi Aao");
        vendorIdleDialogue[1] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/Hlo Bhiya And Didi");

        //Vendor outdoor subtitle Dialogues
        VendorDialogueAfterFirstConversation[0] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/C’mon.Let's go to the clinic");
        VendorDialogueAfterFirstConversation[1] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/Mere peechhe aao");
        VendorDialogueAfterFirstConversation[2] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/Chalo");
        VendorDialogueAfterFirstConversation[3] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/Chana puri will get cold, hurry up");
        VendorDialogueAfterFirstConversation[4] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/What’s taking you so long");

        //Vendor indoor Subtitle Dialogues

        VendorDialogueInFrontOfElevator[0] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/Pharmacy  Shubana");
        VendorDialogueInFrontOfElevator[1] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/Shabana the ClearMind pharmacist over there");
        VendorDialogueInFrontOfElevator[2] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/After you have registered with the clinic you can talk to her about medications");
        VendorDialogueInFrontOfElevator[3] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/Ok, let's get Priya her Channa puri before it gets cold");
        VendorDialogueInFrontOfElevator[4] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/You go ahead and take the elevator to the reception floor");
        VendorDialogueInFrontOfElevator[5] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/I will take the stairs and meet you there");
        VendorDialogueInFrontOfElevator[6] = Resources.Load<AudioClip>("Audios/Dialogues/Vendor/Here we are");

        // Receptionist Dialogues
        receptionistDialogue[0] = Resources.Load<AudioClip>("Audios/Dialogues/Receptionist/Hello");
        receptionistDialogue[1] = Resources.Load<AudioClip>("Audios/Dialogues/Receptionist/Call Back");

        //Songs
        Songs[0] = Resources.Load<AudioClip>("Audios/Songs/Music/Apni To Jaise Taise - Laawaris (1981) 128 Kbps");
        Songs[1] = Resources.Load<AudioClip>("Audios/Songs/Music/I Love You - Gujarati Mein");
        Songs[2] = Resources.Load<AudioClip>("Audios/Songs/Ads/Vicco-Vajradanti");
        Songs[3] = Resources.Load<AudioClip>("Audios/Songs/Ads/Campa-Cola");

        Songs[4] = Resources.Load<AudioClip>("Audios/Songs/Music/My Name Is Lakhan - Ram Lakhan 128 Kbps");
        Songs[5] = Resources.Load<AudioClip>("Audios/Songs/Music/Om Shanti Om - Meri Umar Ke Naujawano - Karz 128 Kbps");
        Songs[6] = Resources.Load<AudioClip>("Audios/Songs/Ads/Maggi 1980s AD");
        Songs[7] = Resources.Load<AudioClip>("Audios/Songs/Ads/Farex Baby Food");

        Songs[8] = Resources.Load<AudioClip>("Audios/Songs/Music/Sharabi-inteha ho gayi");
        Songs[9] = Resources.Load<AudioClip>("Audios/Songs/Music/Tune O Rangeele.m4a 128 Kbps");
        Songs[10] = Resources.Load<AudioClip>("Audios/Songs/Ads/Gagan Vanaspati");
        Songs[11] = Resources.Load<AudioClip>("Audios/Songs/Ads/Glucon C");

        Songs[12] = Resources.Load<AudioClip>("Audios/Songs/Music/Aap Ke Aa Jane Se - Khudgarz 128 Kbps");
        Songs[13] = Resources.Load<AudioClip>("Audios/Songs/Music/Hamen Tumse Pyar Kitna Male.m4a");
        Songs[14] = Resources.Load<AudioClip>("Audios/Songs/Ads/Glucose D Biscuits");
        Songs[15] = Resources.Load<AudioClip>("Audios/Songs/Ads/Pan-Parag");

        Songs[16] = Resources.Load<AudioClip>("Audios/Songs/Music/Chhu Kar Mere Manko - Yaarana 128 Kbps");
        Songs[17] = Resources.Load<AudioClip>("Audios/Songs/Music/Jimmi-Jimmi");
        Songs[18] = Resources.Load<AudioClip>("Audios/Songs/Ads/Prestige-Jo-Biwi-se-Pressure-Cooker");
        Songs[19] = Resources.Load<AudioClip>("Audios/Songs/Ads/Washing-Powder-Nirma");

        Songs[20] = Resources.Load<AudioClip>("Audios/Songs/Music/Disco-Dancer");
        Songs[21] = Resources.Load<AudioClip>("Audios/Songs/Music/01. Tum Ko Dekha To Yeh Khayal Aaya");
        Songs[22] = Resources.Load<AudioClip>("Audios/Songs/Ads/1983_ Limca");

        Songs[23] = Resources.Load<AudioClip>("Audios/Songs/Music/Accident_Ho_Gaya_Rabba_Rabba_");
        Songs[24] = Resources.Load<AudioClip>("Audios/Songs/Music/Disco Station Disco - Reena Roy, Asha Bhosle, Haathkadi Song");



        //Clasical
        ClasicalSongs[0] = Resources.Load<AudioClip>("Audios/Songs/Classical/1");
        ClasicalSongs[1] = Resources.Load<AudioClip>("Audios/Songs/Classical/2");
        ClasicalSongs[2] = Resources.Load<AudioClip>("Audios/Songs/Classical/3");
        ClasicalSongs[3] = Resources.Load<AudioClip>("Audios/Songs/Classical/4");
        ClasicalSongs[4] = Resources.Load<AudioClip>("Audios/Songs/Classical/5");
        ClasicalSongs[5] = Resources.Load<AudioClip>("Audios/Songs/Classical/6");
        ClasicalSongs[6] = Resources.Load<AudioClip>("Audios/Songs/Classical/7");
        ClasicalSongs[7] = Resources.Load<AudioClip>("Audios/Songs/Classical/8");
        ClasicalSongs[8] = Resources.Load<AudioClip>("Audios/Songs/Classical/9");
        ClasicalSongs[9] = Resources.Load<AudioClip>("Audios/Songs/Classical/10");
    }



    /// <summary>
    /// This Will toggle Dialogues Sound which use sfx manager ausio source
    /// </summary>
    public void ToggleSound()
    {
        for (int i = 0; i < audioSource.Length; i++)
        {
            audioSource[i].mute = !audioSource[i].mute;
        }       
    }



    /// <summary>
    /// This Will toggle sound effects of cars / Auto
    /// </summary>
    public void ToggleCarSound()
    {
        for (int i = 0; i < OutdoorGameManager.instance.CarsSource.Length; i++)
        {
            OutdoorGameManager.instance.CarsSource[i].mute = !OutdoorGameManager.instance.CarsSource[i].mute;
        }
    }



    /// <summary>
    /// This Will toggle Music of Outdoor Scene
    /// </summary>
    public void ToggleMusic()
    {
        for (int i = 0; i < OutdoorGameManager.instance.MusicPlayer.Length; i++)
        {
            OutdoorGameManager.instance.MusicPlayer[i].mute = !OutdoorGameManager.instance.MusicPlayer[i].mute;
        }
    }



    /// <summary>
    /// This Will toggle Music of Indoor Scene
    /// </summary>
    public void ToggleMusicIndoor()
    {
        for (int i = 0; i < IndoorGameManager.instance.MusicPlayer.Length; i++)
        {
            IndoorGameManager.instance.MusicPlayer[i].mute = !IndoorGameManager.instance.MusicPlayer[i].mute;
        }
    }



    /// <summary>
    /// This will store song played time
    /// </summary>
    /// <param name="audioSource"></param>
    public void GetCurrentAudioSource(float audioSourceTime)
    {
        currentAudioTime = audioSourceTime;
    }
}
