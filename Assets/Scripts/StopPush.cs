//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;
using MEC;
using SWS;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
//using System.Linq;

public class StopPush : MonoBehaviour
{
    [Header("Scripts")]
    private navMove navigationMovement;
    private splineMove splineMovement;

    [Header("Int")]
    private int sceneIndex;
    private int randomNumber;

    [Header("String")]
    public string currentState;

    //[Header("Animator")]
    //private Animator animator;

    [Header("bool")]
    private bool canTrigger = true;



    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex == 2)
        {
            if (transform.parent.gameObject.CompareTag("Outdoor NPC"))
            {
                navigationMovement = transform.parent.GetComponent<navMove>();
            }

            if (transform.parent.gameObject.CompareTag("Dog") || transform.parent.gameObject.CompareTag("Bull"))
            {
                navigationMovement = transform.parent.GetComponent<navMove>();
            }

            if (transform.parent.gameObject.CompareTag("Vehicle"))
            {
                splineMovement = transform.parent.GetComponent<splineMove>();
            }
        }
       
        if (sceneIndex == 3)
        {
            if (transform.parent.gameObject.CompareTag("NPC"))
            {
                navigationMovement = transform.parent.GetComponent<navMove>();
            }

            if (gameObject.transform.parent.CompareTag("Dachshund dog"))
            {
                navigationMovement = transform.parent.GetComponent<navMove>();
            }
        }

        //animator = transform.parent.GetComponent<Animator>();
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Outdoor
            if (sceneIndex == 2)
            {
                if (gameObject.transform.parent.CompareTag("Dog") && canTrigger)
                {
                    SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.DogBarking);

                    if (navigationMovement != null)
                    {
                        navigationMovement.Pause(1f);
                        gameObject.transform.parent.GetComponent<navMove>().ChangeAnimationState("Dog Bark Animation");
                    }
                }

                else if (gameObject.transform.parent.CompareTag("Bull") && canTrigger)
                {
                    // SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.BullSound);

                    if (navigationMovement != null)
                    {
                        navigationMovement.Pause(2f);
                        gameObject.transform.parent.GetComponent<navMove>().ChangeAnimationState("Idle 1");
                    }
                }

                else if (gameObject.transform.parent.CompareTag("Vehicle"))
                {
                    OutdoorGameManager.instance.playerCollidedWithVehicle = true;
                    OutdoorGameManager.instance.subtitleText.gameObject.SetActive(true);

                    SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.npcCollisionDialogueMale[randomNumber]);

                    OutdoorGameManager.instance.subtitleText.text = OutdoorGameManager.instance.subtitle[randomNumber];

                    if (gameObject.transform.parent.name == "Black rickshaw SWS" || gameObject.transform.parent.name == "Black rickshaw SWS 2")
                    {                      
                        gameObject.GetComponent<AudioSource>().PlayOneShot(SfxManager.sfxInstance.AutoHorn);
                    }

                    else
                    {                                           
                        gameObject.GetComponent<AudioSource>().PlayOneShot(SfxManager.sfxInstance.CarHorn);
                    }

                    if (splineMovement != null)
                    {
                        splineMovement.Pause(5000);// try to find better way if possiable.
                    }
                }

                else if (gameObject.transform.parent.CompareTag("Outdoor NPC") && canTrigger)
                {
                    OutdoorGameManager.instance.playerCollidedWithNpc = true;

                    randomNumber = Random.Range(0, 8);
                    if (gameObject.name == "Genesis8Female")
                    {
                        SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.npcCollisionDialogueFemale[randomNumber]);
                    }

                    else if (gameObject.name == "Genesis8Male")
                    {
                        SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.npcCollisionDialogueMale[randomNumber]);
                    }
                    OutdoorGameManager.instance.subtitleText.gameObject.SetActive(true);
                    OutdoorGameManager.instance.subtitleText.text = OutdoorGameManager.instance.subtitle[randomNumber];

                    if (navigationMovement != null)
                    {
                        navigationMovement.Pause(5f);
                    }

                }
              
                Timing.RunCoroutine(ResetSubtitle().CancelWith(gameObject));
                Timing.RunCoroutine(ResetCanTriggerVariable().CancelWith(gameObject));

                canTrigger = false;
            }

            // Indoor
            if (sceneIndex == 3)
            {
                if (gameObject.transform.parent.CompareTag("NPC") && canTrigger)
                {
                    randomNumber = Random.Range(0, 8);
                    if (gameObject.name == "Genesis8Female")
                    {
                        SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.npcCollisionDialogueFemale[randomNumber]);
                    }
                    else if (gameObject.name == "Genesis8Male")
                    {
                        SfxManager.sfxInstance.audioSource[0].PlayOneShot(SfxManager.sfxInstance.npcCollisionDialogueMale[randomNumber]);
                    }

                    IndoorGameManager.instance.subtitleText.gameObject.SetActive(true);
                    IndoorGameManager.instance.subtitleText.text = IndoorGameManager.instance.subtitle[randomNumber];

                    if (navigationMovement != null)
                    {
                        navigationMovement.Pause(5f);
                    }

                    Timing.RunCoroutine(ResetSubtitle().CancelWith(gameObject));
                }

                if (gameObject.transform.parent.CompareTag("Dachshund dog"))
                {
                    gameObject.transform.parent.gameObject.layer = LayerMask.NameToLayer("Default");
                    if (navigationMovement != null)
                    {
                        IndoorGameManager.instance.ChangeAnimationState("Idle", IndoorGameManager.instance.DaschundDogAnimator);
                        navigationMovement.Stop();
                    }
                }
            }
           
        }

        if(sceneIndex == 2)
        {         
            if (collision.transform.parent.CompareTag("Vehicle"))
            {
                // If we collided with Vehicle and we are vehicles too.
                if (gameObject.transform.parent.CompareTag("Vehicle"))
                {
                    // Check if playerCollidedWithVehicle is needed or not in this case?
                    //if (OutdoorGameManager.instance.playerCollidedWithVehicle == false)
                    //{
                        Timing.PauseCoroutines("StopCarMovement");
                        Timing.KillCoroutines("StopCarMovement");

                        if (!OutdoorGameManager.instance.vehiclesCollider.Contains(gameObject.GetComponent<Collider>()))
                        {
                            //print(gameObject.GetComponent<Collider>());
                            //OutdoorGameManager.instance.vehiclesCollider.Add(gameObject.GetComponent<Collider>());
                            OutdoorGameManager.instance.AddCollider(gameObject.GetComponent<Collider>());

                            //Timing.RunCoroutine(OutdoorGameManager.instance.MoveTheCollidedVehiclesOneByOne());
                        }
                    //}

                    if (splineMovement != null)
                    {
                        splineMovement.Pause(1000);
                    }
                }
            }
        }

        if (sceneIndex == 3)
        {
            if (collision.transform.parent.CompareTag("NPC"))
            {
                if (gameObject.transform.parent.CompareTag("NPC"))
                {
                    //collision.transform.parent.GetComponent<navMove>().Pause(Random.Range(3, 10)); facing problem 
                    if (navigationMovement != null)
                    {
                        navigationMovement.Pause(Random.Range(3, 10));
                    }
                }
            }
        }              
    }



    private void OnCollisionExit(Collision collision)
    {
        // Outdoor
        if (sceneIndex == 2)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (gameObject.transform.parent.CompareTag("Vehicle"))
                {
                    OutdoorGameManager.instance.playerCollidedWithVehicle = false;
                    OutdoorGameManager.instance.OnplayerCollidedWithVehicleFalse?.Invoke();

                    if (splineMovement != null)
                    {
                        Timing.RunCoroutine(StartCarMovement(), "StopCarMovement");                   
                    }
                }

                if (gameObject.transform.parent.CompareTag("Outdoor NPC"))
                {
                    OutdoorGameManager.instance.playerCollidedWithNpc = false;
                }
            }
        }

        // Indoor
        if (sceneIndex == 3)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (gameObject.transform.parent.CompareTag("Dachshund dog"))
                {
                    gameObject.transform.parent.gameObject.layer = LayerMask.NameToLayer("Interact");
                    if (navigationMovement != null)
                    {
                        IndoorGameManager.instance.ChangeAnimationState("Walk", IndoorGameManager.instance.DaschundDogAnimator);
                        navigationMovement.StartMove();
                    }
                }
            }
        }     
    }



    /// <summary>
    /// This function will reset subtitle after every 2 sec 
    /// </summary>
    private IEnumerator<float> ResetSubtitle()
    {
        yield return Timing.WaitForSeconds(2f);

       if (sceneIndex == 2)
       {
            OutdoorGameManager.instance.subtitleText.text = null;
            OutdoorGameManager.instance.subtitleText.gameObject.SetActive(false);
       }

       if (sceneIndex == 3)
       {
            IndoorGameManager.instance.subtitleText.text = null;
            IndoorGameManager.instance.subtitleText.gameObject.SetActive(false);
       }      
    }



    /// <summary>
    /// Rsets the trigger so that the NPC's can start triggering again.
    /// </summary>
    private IEnumerator<float> ResetCanTriggerVariable()
    {
        yield return Timing.WaitForSeconds(3f);

        canTrigger = true;
    }



    private IEnumerator<float> StartCarMovement()
    {
        yield return Timing.WaitForSeconds(1);

        splineMovement.Pause(5f);
    }
}
