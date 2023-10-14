using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class Dialoguemovement : MonoBehaviour
{

    private int currentDialogueId;
    public GameObject Player;
    private string dialogueTitle;

    private void Start()
    {
        GetUserData();
    }
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
        //if (GeneralManager.instance.isButtonPressed)
        //{
            PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
        //}

        DialogueManager.StopConversation();
        Player.GetComponent<FirstPersonController>().enabled = true;
        Player.GetComponent<Selector>().enabled = true;
    }

    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);
    }

    private void OnDataRecieved(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("currentDialogueId") && result.Data.ContainsKey("Dialogue Title"))
        {
            currentDialogueId = int.Parse(result.Data["currentDialogueId"].Value);
            dialogueTitle = result.Data["Dialogue Title"].Value;
            print(currentDialogueId);
        }
    }

    public void DialogueResume()
    {      
        DialogueManager.StartConversation(dialogueTitle, DialogueManager.currentActor, DialogueManager.currentConversant, currentDialogueId);       
    }

    private void OnError(PlayFabError obj)
    {
        print("Data Not Saved");
    }

    private void OnDataSend(UpdateUserDataResult result)
    {
        print("Data Save");
        GetUserData();
    }
}
