using UnityEngine;
//using UnityEngine.SceneManagement;
using MEC;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System;

public class CharacterSelection : MonoBehaviour
{
	[Header("GameObjects")]
	public GameObject[] characters;
	public GameObject startButton;
	public GameObject okButton;
	public GameObject fakeLoadingPanel;
	public GameObject warringMessagePanel;

    [Header("InputField")]
	public TMP_InputField playerInputField;



	[Header("int")]
	private int selectedCharacter = 0;


    private void Awake()
    {
		Timing.RunCoroutine(FakeDelayPanelClose().CancelWith(gameObject));
    }


    private void Update()
    {
        if (playerInputField.isFocused)
        {
			return;
        }

        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
			characters[selectedCharacter].SetActive(false);
			selectedCharacter = (selectedCharacter + 1) % characters.Length;
			characters[selectedCharacter].SetActive(true);
		}

		if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
			characters[selectedCharacter].SetActive(false);
			selectedCharacter--;
			if (selectedCharacter < 0)
			{
				selectedCharacter += characters.Length;
			}
			characters[selectedCharacter].SetActive(true);
		}
    }


	
	public void StartGame()
	{
		PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
	}



	/// <summary>
	/// Save player name to Playfab Database
	/// </summary>
	public void Save()
	{
		var request = new UpdateUserDataRequest
		{
			Data = new Dictionary<string, string>
			{
				{"PlayerName" ,playerInputField.text }				
			}
		};
		PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
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
	/// This function will call on every button and this will play click sound
	/// </summary>
	public void ButtonClickSound()
	{
		SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.buttonClick);
	}



	/// <summary>
	/// Warning function if player name field is empty 
	/// </summary>
	public void CheckPlayerNameNull()
    {
		if ( string.IsNullOrEmpty(playerInputField.text))
        {
			warringMessagePanel.SetActive(true);
		}

        else
        {
			startButton.SetActive(true);
			okButton.SetActive(false);
		}
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
}
