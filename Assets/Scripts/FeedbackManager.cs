using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PixelCrushers.DialogueSystem;

public class FeedbackManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject[] starsCollection;
    public GameObject closeButton;

    [Header("Integers")]
    [SerializeField]
    private int starsCount;

    [Header("InputField")]
    public TMP_InputField commentField;

    [Header("String")]
    [SerializeField]
    private string comment;

    [Header("Bool")]
    private bool canCheck=false;
    private bool feedbackWritten = false;

    [Header("Scripts")]
    private SendToGoogle sendDataToGoogle;

    private void Start()
    {
        sendDataToGoogle = gameObject.GetComponent<SendToGoogle>();
    }


    void Update()
    {
        if(starsCount>0 && feedbackWritten==true && canCheck==true)
        {
            closeButton.SetActive(true);
            canCheck = false;
        }

    }



    /// <summary>
    /// This Functions will set stars count on Button Click and enable/ disable stars image
    /// </summary>
    /// <param name="stars"></param>
    public void SetStarsRating(int stars)
    {
        starsCount = stars;
        for (int i = 0; i < stars; i++)
        {
            if (!starsCollection[i].transform.GetChild(0).gameObject.activeSelf)
            {
                starsCollection[i].transform.GetChild(0).gameObject.SetActive(true);
            }          
        }
        for (int i = stars; i < 5; i++)
        {
            if (starsCollection[i].transform.GetChild(0).gameObject.activeSelf)
            {
                starsCollection[i].transform.GetChild(0).gameObject.SetActive(false);
            }               
        }
    }


    /// <summary>
    /// This Function Call on close button and it resume the dialogue flow , and it set inputfield text to a string 
    /// </summary>
    public void CloseFeedbackForm()
    {
        DialogueTime.isPaused = false;
        comment = commentField.text;
        sendDataToGoogle.Send(starsCount,comment);
    }



    /// <summary>
    /// This Function Call in Dialogue Work Flow when it enable rating panel and it pause Dialogue Flow
    /// </summary>
    public void MakeCanCheckTrue()
    {
        DialogueTime.isPaused = true;
        canCheck = true;
    }


    /// <summary>
    /// This Function Call on input field on value changed and it make the bool true so that Close button will be active
    /// </summary>
    public void FeedbackHasBeenWritten()
    {
        feedbackWritten = true;
    }
        
    
}
