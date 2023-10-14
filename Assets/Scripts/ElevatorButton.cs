using MEC;
using UnityEngine;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class ElevatorButton : MonoBehaviour {

	[Header("GameObjects")]
	public GameObject m_ElevatorController;

	[Header("Script references")]
	private ElevatorController Elevator;

	[HideInInspector]
	public bool ElevatorMoving;



    void Start()
    {
		Elevator = m_ElevatorController.GetComponent<ElevatorController>();
	}

	private void OnError(PlayFabError obj)
	{
		print("Data Not Saved");
	}

	private void OnDataSend(UpdateUserDataResult result)
	{
		print("Data Save");
		//Application.Quit();
	}

	public void SendCall (string Call) 
	{
		ElevatorMoving = Elevator.ElevatorMoving;
		if(Call == "Pharmacy" && !ElevatorMoving)
		{
			Elevator.ElevatorGO("Pharmacy");
			var request = new UpdateUserDataRequest
			{
				Data = new Dictionary<string, string>
			    {
					{"Floor Name" , Call}				
		     	}
			};
            PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
            //if (GeneralManager.instance.isButtonPressed)
            //{
            //	PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
            //}
        }
		
		if(Call == "Reception" && !ElevatorMoving)
		{
			Elevator.ElevatorGO("Reception");
			var request = new UpdateUserDataRequest
			{
				Data = new Dictionary<string, string>
				{
					{"Floor Name" , Call}
				}
			};
			PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
			//if (GeneralManager.instance.isButtonPressed)
			//{
			//	PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
			//}
		}

		if (Call == "Library" && !ElevatorMoving)
		{
			if(GeneralManager.instance.vendorVisitCounter == 1)
            {
				if (IndoorGameManager.instance.elevator[0])
				{
					Elevator.ElevatorGO("Library");

					IndoorGameManager.instance.elevator[1] = true;
				}

				else
				{
					Timing.RunCoroutine(IndoorGameManager.instance.DisplayAndResetWarningForElevatorFloorButton().CancelWith(gameObject));
				}
			}

            else
            {
				Elevator.ElevatorGO("Library");
			}

			var request = new UpdateUserDataRequest
			{
				Data = new Dictionary<string, string>
				{
					{"Floor Name" , Call}
				 }
			};
			PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
			//if (GeneralManager.instance.isButtonPressed)
			//{
			//	PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
			//}

		}

		if (Call == "Therapy" && !ElevatorMoving)
		{
			if (GeneralManager.instance.vendorVisitCounter == 1)
            {
				if (IndoorGameManager.instance.elevator[0] && IndoorGameManager.instance.elevator[1])
				{
					Elevator.ElevatorGO("Therapy");
				}

				else
				{
					Timing.RunCoroutine(IndoorGameManager.instance.DisplayAndResetWarningForElevatorFloorButton().CancelWith(gameObject));
				}
			}

            else
            {
				Elevator.ElevatorGO("Therapy");
			}

			var request = new UpdateUserDataRequest
			{
				Data = new Dictionary<string, string>
				{
					{"Floor Name" , Call}
				 }
			};
			PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
			//if (GeneralManager.instance.isButtonPressed)
			//{
			//	PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
			//}
		}

		if (Call == "CallElevator" && !ElevatorMoving)
		{
			Elevator.CallElevator();
		}
	}
}
