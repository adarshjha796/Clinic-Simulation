using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine.AI;
using SWS;

[System.Serializable]
public class ElevatorSounds 
{
	public AudioClip ElevatorStartMoving;
	public AudioClip ElevatorStop;
}

[RequireComponent (typeof (AudioSource))]
public class ElevatorController : MonoBehaviour 
{	
	[Header("Scripts")]
	// Reference to this which is attached onto the player gameobject.
	private FirstPersonController firstpersonController;
	// This will reference this script which is attached onto the player gameobject.
	private CrosshairGUI crosshair;

	[Header("float")]
	public float elevatorSpeed = 0.05f;
	
	[Header("Transform")]
	public Transform[] ElevatorFloors;
	private Transform ElevatorFloor;

	public ElevatorSounds ElevatorSounds =  new ElevatorSounds();


	[Header("int")]
	public int floorNumber = 0;
	private int currentFloorNumber = 0;

	[Header("string")]
	private string elevatorDirection;

	[Header("bool")]
	public bool ElevatorMoving;
	private bool ElevatorMax;
	private bool ElevatorMin;
	private bool soundplayed;
	private bool isMoved = false;


	[Header("Audio Source")]
	private AudioSource audioSource;

	[Header("Animator")]
	public Animator doorAnimator;

	[Header("GameObjects")]
	public GameObject characterPrefabs;
	private GameObject player;

    private	void Start ()
	{
		int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");
		player = characterPrefabs.transform.GetChild(selectedCharacter).gameObject;
		audioSource = GetComponent<AudioSource>();
		firstpersonController = player.GetComponent<FirstPersonController>();
		crosshair = player.GetComponent<CrosshairGUI>();
	}



	private void Update()
	{
		ElevatorFloor = ElevatorFloors[floorNumber];

		if ((elevatorDirection == "Pharmacy" || elevatorDirection == "Reception" || elevatorDirection == "Library" || elevatorDirection == "Therapy") && ElevatorMoving)
		{
			/// So that player stays still insde the elevator.
			firstpersonController.walkSpeed = 0;
			firstpersonController.bobSpeed = 0;
			crosshair.allowedToRaycast = false;

			Timing.RunCoroutine(MoveElevator()); // To run in the Update segment.
		}

		/*if(elevatorDirection == "ElevatorDown")
		{
			player.GetComponent<FirstPersonMovement>().speed = 0;
			doorAnimator.SetBool("Opened", false);
			Timing.RunCoroutine(_moveElevator()); // To run in the Update segment.
			ElevatorMoving = true;
		}*/

		if (transform.position.y == ElevatorFloor.position.y && ElevatorMoving)
		{
			doorAnimator.SetBool("Opened", true);
			crosshair.allowedToRaycast = true;

			/// So that player can move again.
			firstpersonController.walkSpeed = 2f;
			firstpersonController.bobSpeed = 10f;

			ElevatorMoving = false;
			soundplayed = false;
		}

		if (transform.position.y == ElevatorFloors[0].position.y)
		{
			ElevatorMax = false;
			ElevatorMin = true;
		}

		if (transform.position.y > ElevatorFloors[0].position.y)
		{
			ElevatorMin = false;
		}

		int MaxFloors = ElevatorFloors.Length - 1;
		if (transform.position.y == ElevatorFloors[MaxFloors].position.y)
		{
			ElevatorMax = true;
			ElevatorMin = false;
		}

		if (transform.position.y < ElevatorFloors[MaxFloors].position.y)
		{
			ElevatorMax = false;
		}

		if (ElevatorMoving && !isMoved)
		{
			if (soundplayed == false)
			{
				audioSource.clip = ElevatorSounds.ElevatorStartMoving;
				audioSource.Play();
				soundplayed = true;
			}
			isMoved = true;
		}

		if (transform.position.y == ElevatorFloors[floorNumber].position.y && !ElevatorMoving && isMoved)
		{
			if (soundplayed == false)
			{
				audioSource.Stop();
				audioSource.PlayOneShot(ElevatorSounds.ElevatorStop);
				soundplayed = true;
			}
			isMoved = false;
		}
	}



	/// <summary>
	/// This function is used to call the elevator from the pharmacy floor.
	/// </summary>
	public void CallElevator()
    {
		//This line will set the bool true so it will play the animation.
		doorAnimator.SetBool("Opened", true);
		if (soundplayed == false)
		{
			audioSource.Stop();
			audioSource.PlayOneShot(ElevatorSounds.ElevatorStop);
			soundplayed = true;
		}
	}


	
	/// <summary>
	/// This function is used to set which floor the elevator has to go.
	/// </summary>
	/// <param name="ElevatorDirection"></param>
	public void ElevatorGO (string ElevatorDirection) 
	{
		elevatorDirection = ElevatorDirection;
		FloorNumber();
		soundplayed = false;
	}


	
	/// <summary>
	/// This will decide the floor depending the the elevator direction
	/// </summary>
	private void FloorNumber ()
	{
		floorNumber = Mathf.Clamp(floorNumber, 0, ElevatorFloors.Length - 1);
		if(elevatorDirection == "Pharmacy" && !ElevatorMoving && !ElevatorMin)
		{
			floorNumber = 0;
            //Diabling/Enabling Npc
            for (int i = 0; i < IndoorGameManager.instance.AllNpc.Length; i++)
            {
				if(i == 0 || i == 1 || i == 2)
                {
					IndoorGameManager.instance.AllNpc[i].SetActive(true);
					if (IndoorGameManager.instance.AllNpc[i].TryGetComponent( out NavMeshAgent nma))
                    {
						nma.enabled = true;
					}

					if (IndoorGameManager.instance.AllNpc[i].transform.GetChild(0).TryGetComponent(out CapsuleCollider cc))
					{
						if (cc.enabled == false)
						{	
							cc.enabled = true;
						}
					}

					if (IndoorGameManager.instance.AllNpc[i].TryGetComponent(out navMove nm))
                    {
						nm.ChangeAnimationState("Idle");
						nm.StartMove();
					}
					
				}
				else
				{
					if (IndoorGameManager.instance.AllNpc[i].activeSelf)
					{
						if (IndoorGameManager.instance.AllNpc[i].TryGetComponent(out navMove nm))
						{
							IndoorGameManager.instance.pharmacistCurrentState = null;		
							
							nm.Stop();

							Timing.KillCoroutines("KillRestartMovementForNPC");
						}
					}
					IndoorGameManager.instance.AllNpc[0].transform.rotation = Quaternion.identity;
					IndoorGameManager.instance.AllNpc[i].SetActive(false);
				}
			}

			IndoorGameManager.instance.elevatorLabel.SetActive(true);
			IndoorGameManager.instance.canPlayMusic = true;
			IndoorGameManager.instance.MusicPlayer[0].UnPause();
			IndoorGameManager.instance.MusicPlayer[1].Pause();
			IndoorGameManager.instance.elevatorDisapperPoint.SetActive(true);

			Timing.RunCoroutine(IndoorGameManager.instance.TurnOnAndOffSittingTriggers(floorNumber).CancelWith(gameObject));
		}

		if(elevatorDirection == "Reception" && !ElevatorMoving)
		{
			floorNumber = 1;
			
			//Diabling/Enabling Npc
			for (int i = 0; i < IndoorGameManager.instance.AllNpc.Length; i++)
			{
				if (i == 3 || i == 4)
				{
					IndoorGameManager.instance.AllNpc[i].SetActive(true);
					if (IndoorGameManager.instance.AllNpc[i].TryGetComponent(out NavMeshAgent nma))
					{
						nma.enabled = true;
					}

					if (IndoorGameManager.instance.AllNpc[i].transform.GetChild(0).TryGetComponent(out CapsuleCollider cc))
					{
						if (cc.enabled == false)
						{
							cc.enabled = true;
						}
					}

					if (IndoorGameManager.instance.AllNpc[i].TryGetComponent(out navMove nm))
					{
						nm.ChangeAnimationState("Idle");
						nm.StartMove();
					}
				}
				else
				{
					if(IndoorGameManager.instance.AllNpc[i].activeSelf)
                    {
						if (IndoorGameManager.instance.AllNpc[i].GetComponent<SWS.navMove>() != null)
						{
							IndoorGameManager.instance.AllNpc[i].GetComponent<SWS.navMove>().Stop();
							Timing.KillCoroutines("KillRestartMovementForNPC");
						}
					}
					IndoorGameManager.instance.AllNpc[i].SetActive(false);
				}
			}

			IndoorGameManager.instance.elevatorLabel.SetActive(false);
			IndoorGameManager.instance.canPlayMusic = true;
			IndoorGameManager.instance.MusicPlayer[1].UnPause();
			IndoorGameManager.instance.MusicPlayer[0].Pause();

			if (GeneralManager.instance.playerSceneSwitchTracker == 1 && GeneralManager.instance.vendorVisitCounter == 1 && IndoorGameManager.instance.receptionVendor == 0)
            {
				IndoorGameManager.instance.Vendor[1].SetActive(true);
				IndoorGameManager.instance.receptionVendor = 1;

				SfxManager.sfxInstance.audioSource[1] = IndoorGameManager.instance.Vendor[1].GetComponent<AudioSource>();

				Timing.RunCoroutine(IndoorGameManager.instance.Vendor[1].GetComponent<navMove>().StartMovement(6f).CancelWith(gameObject));
				Timing.RunCoroutine(IndoorGameManager.instance.Vendor[1].GetComponent<navMove>().VendorPlatesOnOrOff(6f).CancelWith(gameObject));
			}

            Timing.RunCoroutine(IndoorGameManager.instance.TurnOnAndOffSittingTriggers(floorNumber).CancelWith(gameObject));
		}

		if (elevatorDirection == "Library" && !ElevatorMoving)
		{
			floorNumber = 2;
			//Diabling/Enabling Npc
			for (int i = 0; i < IndoorGameManager.instance.AllNpc.Length; i++)
			{
				if (i == 5 || i == 6)
				{
					IndoorGameManager.instance.AllNpc[i].SetActive(true);

					if (IndoorGameManager.instance.AllNpc[i].TryGetComponent(out NavMeshAgent nma))
					{
						nma.enabled = true;
					}

					if (IndoorGameManager.instance.AllNpc[i].transform.GetChild(0).TryGetComponent(out CapsuleCollider cc))
					{
						if (cc.enabled == false)
						{
							cc.enabled = true;
						}
					}

					if (IndoorGameManager.instance.AllNpc[i].TryGetComponent(out navMove nm))
					{
						nm.ChangeAnimationState("Idle");
						nm.StartMove();
					}
				}
				else
				{
					if (IndoorGameManager.instance.AllNpc[i].activeSelf)
					{
						if (IndoorGameManager.instance.AllNpc[i].GetComponent<SWS.navMove>() != null)
						{
							IndoorGameManager.instance.AllNpc[i].GetComponent<SWS.navMove>().Stop();
							Timing.KillCoroutines("KillRestartMovementForNPC");
						}
					}

					IndoorGameManager.instance.AllNpc[i].SetActive(false);
				}
			}

			IndoorGameManager.instance.elevatorLabel.SetActive(false);

			Timing.RunCoroutine(IndoorGameManager.instance.TurnOnAndOffSittingTriggers(floorNumber).CancelWith(gameObject));
		}

		if (elevatorDirection == "Therapy" && !ElevatorMoving && !ElevatorMax)
		{
			floorNumber = 3;
			//Diabling/Enabling Npc
			for (int i = 0; i < IndoorGameManager.instance.AllNpc.Length; i++)
			{
				if (i == 7)
				{
					IndoorGameManager.instance.AllNpc[i].SetActive(true);
					if (IndoorGameManager.instance.AllNpc[i].TryGetComponent(out NavMeshAgent nma))
					{
						nma.enabled = true;
					}

					if (IndoorGameManager.instance.AllNpc[i].transform.GetChild(0).TryGetComponent(out CapsuleCollider cc))
					{
						if (cc.enabled == false)
						{
							cc.enabled = true;
						}
					}

					if (IndoorGameManager.instance.AllNpc[i].TryGetComponent(out navMove nm))
					{
						nm.ChangeAnimationState("Idle");
						nm.StartMove();
					}
				}
				else
				{
					if (IndoorGameManager.instance.AllNpc[i].activeSelf)
					{
						if (IndoorGameManager.instance.AllNpc[i].GetComponent<SWS.navMove>() != null)
						{
							IndoorGameManager.instance.AllNpc[i].GetComponent<SWS.navMove>().Stop();
							Timing.KillCoroutines("KillRestartMovementForNPC");
						}
					}

					IndoorGameManager.instance.AllNpc[i].SetActive(false);
				}
			}

			IndoorGameManager.instance.elevatorLabel.SetActive(false);

			Timing.RunCoroutine(IndoorGameManager.instance.TurnOnAndOffSittingTriggers(floorNumber).CancelWith(gameObject));
		}

		ElevatorFloor = ElevatorFloors[floorNumber];
		ElevatorMoving = true;
	}



	//void OnDrawGizmos() 
	//{
	//	for(int i = 0; i < ElevatorFloors.Length; i++) 
	//	{
	//		Gizmos.color = Color.red;
	//		float x = ElevatorFloors[i].position.x;
	//		float y = ElevatorFloors[i].position.y;
	//		float z = ElevatorFloors[i].position.z;
	//		Gizmos.DrawWireCube(new Vector3(x, y+1.502f, z), new Vector3(2.4f, 3.0f, 2.2f));
	//	}
	//}



	/// <summary>
	/// Responsible for moving the elevator.
	/// </summary>
	/// <returns></returns>
 	private IEnumerator<float> MoveElevator()
	{
		doorAnimator.SetBool("Opened", false);

		yield return Timing.WaitForSeconds(2f);

		transform.position = Vector3.MoveTowards(transform.position, ElevatorFloor.position, elevatorSpeed);
		if(Mathf.Abs(currentFloorNumber - floorNumber) == 1)
        {
			yield return Timing.WaitForSeconds(4f);
		}
		else if (Mathf.Abs(currentFloorNumber - floorNumber) == 2)
		{
			yield return Timing.WaitForSeconds(6f);
		}
		else if (Mathf.Abs(currentFloorNumber - floorNumber) == 3)
		{
			yield return Timing.WaitForSeconds(8f);
		}
		currentFloorNumber = floorNumber; // Setting the current floor value after elevator stops.
	}

}
