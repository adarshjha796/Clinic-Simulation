using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using SWS;
using MEC;
using TMPro;
using System.Net.Http.Headers;

public class InteractManager : MonoBehaviour 
{
	[Header("float")]
	public float PickupRange;

	[Header("bool")]
	private bool isPressed;

	[Header("GameObjects")]
	private GameObject playerCam;
	private GameObject objectInteract;

	[Header("Ray/Raycasts")]
	public RaycastHit hit;
	private Ray playerAim;

	[Header("Camera script")]
	private CameraSwitcher cameraSwitcher;

	[Header("Scripts")]
	private CrosshairGUI crosshairGUI;
	private CollisionChecker collisionChecker;

	[Header("Animator")]
	// This will change and always have the current animation on which animations are needed to be played.
	private Animator currentAnimator;

	[Header("Strings")]
	public string currentFloor;
	public string UseButton = "Submit";
	// Keeps track of which animation state is being played
	public string currentState;

	private void Start () 
	{
		crosshairGUI = GetComponent<CrosshairGUI>();
		collisionChecker = GetComponent<CollisionChecker>();
		cameraSwitcher = Camera.main.GetComponent<CameraSwitcher>();
		playerCam = Camera.main.gameObject;
		isPressed = false;
		objectInteract = null;
	}
	


	private void Update () 
	{
		if(Input.GetMouseButtonDown(0) && !isPressed)
		{
			Interact();
			isPressed = true;
		}
		if(Input.GetMouseButtonUp(0) && isPressed)
		{
			if(hit.collider != null && hit.collider.CompareTag("Basin"))
            {
				Timing.RunCoroutine(RestIsPressed(6).CancelWith(gameObject));
            }
            else
            {
				isPressed = false;
			}
		}
	}



	/// <summary>
	/// This methods handels interaction between player and environment
	/// </summary>
	private void Interact()
	{
		playerAim = playerCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		
		if (Physics.SphereCast(playerAim,0.01f, out hit,PickupRange) && crosshairGUI.allowedToRaycast)
		{
			objectInteract = hit.collider.gameObject;

			if (hit.collider.gameObject.layer != 6)
			{
				return;
			}

			if (collisionChecker.canInteractElevatorButton)
            {
				if (hit.collider.CompareTag("Pharmacy"))
				{
					InteractUse("Pharmacy");
				}

				else if (hit.collider.CompareTag("Reception"))
				{
					InteractUse("Reception");
				}

				else if (hit.collider.CompareTag("Library"))
				{
					InteractUse("Library");
                    if (IndoorGameManager.instance.elevatorButton[0].layer == LayerMask.NameToLayer("Interact"))
                    {
                        IndoorGameManager.instance.elevatorButton[1].layer = LayerMask.NameToLayer("Interact");
                    }
                }

				else if (hit.collider.CompareTag("Therapy"))
				{
					InteractUse("Therapy");
				}
			}
			
			else if (hit.collider.CompareTag("CallElevator"))
			{
                if (GeneralManager.instance.playerSceneSwitchTracker == 1 && GeneralManager.instance.vendorVisitCounter >= 1)
                {
                    InteractUse("CallElevator");
                }
                else
                {
                    Timing.RunCoroutine(IndoorGameManager.instance.DisplayAndResetWarning().CancelWith(gameObject));
                }
            }

			else if (hit.collider.CompareTag("Dachshund dog"))
			{
				IndoorGameManager.instance.playerInteractedWithDog = true;
				IndoorGameManager.instance.activePlayer.transform.GetChild(2).gameObject.SetActive(false);

				hit.collider.GetComponent<navMove>().Pause(5000f);
				hit.collider.GetComponent<navMove>().CancelPauseInvoke();

				Timing.RunCoroutine(RotateDogToLookAtPlayerAndThenInteract().CancelWith(gameObject));
				
			}

			else if (hit.collider.tag == "Receptionist")
            {
				IndoorGameManager.instance.playerInteractedWithReceptionist = true;
				IndoorGameManager.instance.CancleCoroutineForReceptionistWhileInConversationFunction();
				print("Receptionist");
		
				cameraSwitcher.changeCamera = true;
				cameraSwitcher.SwitchCameraReceptionist();
				TurnOffToolTip();
				Timing.RunCoroutine(GiveDelay().CancelWith(gameObject));
			}

			else if (hit.collider.CompareTag("Sofa"))
            {
				IndoorGameManager.instance.playerInteractedWithSofa = true;

				collisionChecker.newPositionAfterInteraction = hit.collider.gameObject.transform.GetChild(0).transform; // Every sofa 1st child is the sitting position for the player.

				SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.sofaSitingClip);

				collisionChecker.Sit();
				TurnOffToolTip();
			}

			else if (hit.collider.CompareTag("Library chair"))
            {
				IndoorGameManager.instance.playerInteractedWithSofa = true;
				IndoorGameManager.instance.librarytrigger.GetComponent<BoxCollider>().isTrigger = true;

				currentAnimator = hit.collider.GetComponent<Animator>();
				collisionChecker.newPositionAfterInteraction = hit.collider.gameObject.transform.GetChild(0).transform; // Every Library chair 1st child is the sitting positionyer. for the pla

				SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.chairPullOutClip);

				ChangeAnimationState("Pull out " + hit.collider.name.Substring(hit.collider.name.Length - 1));

				Timing.RunCoroutine(ResetLiraryChairAnimationState("Push in " + hit.collider.name.Substring(hit.collider.name.Length - 1)).CancelWith(gameObject));
			
				TurnOffToolTip();

				Timing.RunCoroutine(SitOnLibraryChair().CancelWith(gameObject));
			}

			else if (hit.collider.CompareTag("Therapist sofa"))
			{
				collisionChecker.newPositionAfterInteraction = hit.collider.gameObject.transform.GetChild(0).transform; // Every Sofa 1st child is the sitting position for the player.
				collisionChecker.Sit();
				TurnOffToolTip();
			}

			else if (hit.collider.CompareTag("Therapist lounge"))
			{
				collisionChecker.newPositionAfterInteraction = hit.collider.gameObject.transform.GetChild(0).transform; // Every Lounge 1st child is the sitting position for the player.
				collisionChecker.LayDown();
				TurnOffToolTip();
			}

			else if (hit.collider.CompareTag("Basin"))
            {
				collisionChecker.BasinTapTrigger();
				TurnOffToolTip();
            }

			if (hit.collider != null)
            {
				collisionChecker.playerInteractedWithTag = hit.collider.tag;
				collisionChecker.playerInteractedWithName = hit.collider.name;
			}

			crosshairGUI.allowedToRaycast = false;
		}
	}

	IEnumerator<float> GiveDelay()
	{
		crosshairGUI.allowedToRaycast = false;
        yield return Timing.WaitForSeconds(2);
		crosshairGUI.allowedToRaycast = true;
	}



    /// <summary>
    /// This works when we are not hovering over a gameoject, so this turns off the tooltip.
    /// </summary>
    private void TurnOffToolTip() 
    {
		if (crosshairGUI.rayCastedLastObject != null)
		{
			if (crosshairGUI.rayCastedLastObject.GetComponent<TooltipTrigger>() != null)
			{
				crosshairGUI.rayCastedLastObject.GetComponent<TooltipTrigger>().CloseTooltip(); // Trigger the tooltip attached on the dog.
			}
		}
	}
	


	public void ReceptionistInteractionPanelOnafterConversation()
    {
		IndoorGameManager.instance.playerInteractedWithReceptionist = true;
		IndoorGameManager.instance.CancleCoroutineForReceptionistWhileInConversationFunction();
		print("Receptionist");

		cameraSwitcher.changeCamera = true;
		cameraSwitcher.SwitchCameraReceptionist();
		TurnOffToolTip();
	}



	/// <summary>
	/// This is used on the elevator when the player presses a button
	/// </summary>
	/// <param name="Call"></param>
    private void InteractUse(string Call)
    {
		print("INteract USE");
		if(Call == "Pharmacy" || Call == "Reception" || Call == "Library" || Call == "Therapy")
        {
			currentFloor = Call;
		}
		GeneralManager.instance.SavePlayerLocation();
		objectInteract.GetComponent<ElevatorButton>().SendCall(Call);        
    }



	/// <summary>
	/// This is to resume movement of the NPC or Dog after interaction.
	/// </summary>
	public void ResumeMovement()
    {
		if(hit.collider.transform.parent.CompareTag("Dachshund dog"))
        {
			print("bb");
			hit.collider.transform.parent.GetComponent<navMove>().Resume();
			hit.collider.transform.parent.GetComponent<navMove>().InvokeRepeatStartForPause();
		}
    }



	/// <summary>
	/// This changes the animation states for this gameobject.
	/// </summary>
	/// <param name="newState"></param>
	private void ChangeAnimationState(string newState)
	{
		if (currentState == newState) return;

		currentAnimator.Play(newState, 0);

		currentState = newState;
	}



	/// <summary>
	/// Changing the animation state to playerInteracted with object
	/// </summary>
	/// <param name="newState"></param>
    public void CallChangeAnimationStateForLibraryChair(string newState)
    {
        ChangeAnimationState(newState + collisionChecker.playerInteractedWithName);
	}



	/// <summary>
	/// This is used to cause a delay before playing the sitting aniamtion on chairs.
	/// </summary>
	/// <returns></returns>
    private IEnumerator<float> SitOnLibraryChair()
    {
		yield return Timing.WaitForSeconds(1f);
		collisionChecker.Sit();
	}



	/// <summary>
	/// This is used to reset the state of the library chair so that they can be interacted again.
	/// </summary>
	/// <param name="newState"></param>
	/// <returns></returns>
	public IEnumerator<float> ResetLiraryChairAnimationState(string newState)
	{
		yield return Timing.WaitForSeconds(3f);

		SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.chairPullOutClip);
		ChangeAnimationState(newState);

	}



	/// <summary>
	/// by this function we cannot acess basian frequently
	/// </summary>
	/// <param name="delayTime"></param>
	/// <returns></returns>
	private IEnumerator<float> RestIsPressed(int delayTime)
    {
		yield return Timing.WaitForSeconds(delayTime);

		isPressed = false;
	}



	/// <summary>
	/// Dog ROtate towards player and then player can start interacting with dog 
	/// </summary>
	/// <returns></returns>
	private IEnumerator<float> RotateDogToLookAtPlayerAndThenInteract()
    {
		//play the dog rotating animation here 
		hit.collider.transform.LookAt(IndoorGameManager.instance.activePlayer.transform);

		yield return Timing.WaitForSeconds(1f);

		cameraSwitcher.changeCamera = true;
		cameraSwitcher.SwitchCameraDog();
		TurnOffToolTip();		
	}
}
