using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DevionGames.UIWidgets;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CrosshairGUI : MonoBehaviour {

    [Header("Script references")]
    private CameraSwitcher cameraSwitcher;
	private FirstPersonController firstPersonController;

	[Header("Sprites")]
    public Sprite roundCrosshair;
    public Sprite rectangleCrosshair;

    [Header("Canvas objects")]
    public Image crossHairHolder;

    [HideInInspector]
    [Header("Canvas objects")]
    public GameObject rayCastedLastObject;

    [Header("Layer")]
    public LayerMask LayerInteract;

    [Header("Bools")]
    public bool m_ShowCursor;
    private bool canShowTooltip;
	public bool isStanding;
    public bool allowedToRaycast;
    private bool changeCursor;
    private bool m_DefaultReticle = true;
    private bool m_UseReticle;
	private bool canUpdateCrosshair;

	[Header("Ray and hit")]
	private Ray playerAim;
	private RaycastHit hit;

	[Header("Camera")]
	private Camera playerCamera;

	[Header("float")]
	public float RayLength;

	[Header("int")]
	private int currentSceneIndex;

	[Header("Vectors")]
	private Vector2 smallCrosshairSize;
	private Vector2 bigCrosshairSize;

	void Start()
    {
		firstPersonController = GetComponent<FirstPersonController>();
		currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		playerCamera = transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
		if (currentSceneIndex == 3)
        {
		    cameraSwitcher = Camera.main.GetComponent<CameraSwitcher>();
		}
		canShowTooltip = true;
		firstPersonController.cameraCanMove = false;
		m_ShowCursor = true;
	}



	void Update ()
	{
		// RayCast methods
		playerAim = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.SphereCast(playerAim,0.01f,out hit,RayLength,LayerInteract) && isStanding)
		{
			// Outdoor scene
			if (currentSceneIndex == 2)
			{
				if(canShowTooltip)
                {
					if (hit.collider.CompareTag("Vendor"))
					{
						BoolTriggerOn();
					}
				}
			}

			// Indoor scene
			if (currentSceneIndex == 3)
            {
				if (canShowTooltip)
				{
					if (hit.collider.CompareTag("Dachshund dog") || hit.collider.CompareTag("Receptionist") || hit.collider.CompareTag("Sofa")
						|| hit.collider.CompareTag("Library chair") || hit.collider.CompareTag("Therapist sofa") || hit.collider.CompareTag("Therapist lounge") 
						|| hit.collider.CompareTag("Toilet door") || hit.collider.CompareTag("Basin")) // Maybe we have to remove Toilet door.
					{
						rayCastedLastObject = hit.collider.gameObject;

						if (rayCastedLastObject.GetComponent<TooltipTrigger>() != null)
						{
							rayCastedLastObject.GetComponent<TooltipTrigger>().ShowTooltip();  /// Trigger the tooltip attached on the dog.
						}
						BoolTriggerOn();
					}
					else if (hit.collider.CompareTag("Pharmacy") || hit.collider.CompareTag("Reception") ||
				    hit.collider.CompareTag("Library") || hit.collider.CompareTag("Therapy") || hit.collider.CompareTag("CallElevator"))
					{
						BoolTriggerOn();
					}
				}
			}

			allowedToRaycast = true;
		}

		else
		{
			if(!canShowTooltip)
            {
				if (rayCastedLastObject != null)
				{
					if (rayCastedLastObject.GetComponent<TooltipTrigger>() != null)
					{
						rayCastedLastObject.GetComponent<TooltipTrigger>().CloseTooltip(); // Trigger the tooltip attached on the dog.
					}
				}
				BoolTriggerOff();
				rayCastedLastObject = null; // Doing this to make this if statement run only once.
				allowedToRaycast = false;
			}
		}

		// Input methods and checks
		if (Input.GetKeyDown(KeyCode.Alpha1) && GeneralManager.instance.isGamePause == false)
        {
            CursorControl();
        }

        if (changeCursor) // Doing this so that below code doesn't run always.
        {
			if (m_ShowCursor)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;

				if (currentSceneIndex == 3)
                {
					// Doing this so that player cannot rotate camera when the cursor is active.
					cameraSwitcher.sofaCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = 0f;
					cameraSwitcher.sofaCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = 0f;
				}

				changeCursor = false;
			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;

				if (currentSceneIndex == 3)
                {
					// Now player can rotate the camera when the cursor is not active.
					cameraSwitcher.sofaCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = 100f;
					cameraSwitcher.sofaCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = 100f;
				}

				changeCursor = false;
			}
		}

		if (canUpdateCrosshair)
        {
			ChangeCrosshair();
		}

		if (crossHairHolder.rectTransform.sizeDelta == smallCrosshairSize || crossHairHolder.rectTransform.sizeDelta == bigCrosshairSize)
        {
			canUpdateCrosshair = false;
        }
	}

    public void CursorControl()
    {
        m_ShowCursor = !m_ShowCursor;
        firstPersonController.cameraCanMove = !firstPersonController.cameraCanMove;
        changeCursor = true;
    }



    /// <summary>
    ///  This Function turn on reticle crosshair when cursor is on interactable object.
    /// </summary>
    private void BoolTriggerOn()
	{
		m_DefaultReticle = false;
		m_UseReticle = true;
		canShowTooltip = false;
		canUpdateCrosshair = true;
	}



	/// <summary>
	/// This Function turn off reticle crosshair when cursor not on interactable object.
	/// </summary>
	private void BoolTriggerOff()
	{
		m_DefaultReticle = true;
		m_UseReticle = false;
		canShowTooltip = true;
		canUpdateCrosshair = true;
	}



	/// <summary>
	/// This functions changes the crosshair shape.
	/// </summary>
	private void ChangeCrosshair() 
    {
		if(m_DefaultReticle)
        {
			smallCrosshairSize.x = smallCrosshairSize.y = 15;
			crossHairHolder.rectTransform.DOSizeDelta(smallCrosshairSize, 0.1f, false);
			crossHairHolder.sprite = roundCrosshair;
        }

	    if(m_UseReticle)
        {
			bigCrosshairSize.x = bigCrosshairSize.y = 70;
			crossHairHolder.rectTransform.DOSizeDelta(bigCrosshairSize, 0.1f, false);
			crossHairHolder.sprite = rectangleCrosshair;
        }
	}
}