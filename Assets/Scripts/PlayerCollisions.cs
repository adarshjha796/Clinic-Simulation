//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using TMPro;

public class PlayerCollisions : MonoBehaviour
{
    [Header("Script references")]
    public LoadingScreenBarSystem loadingScreenBarSystem;

    #region Private methods

    private void Start()
    {
        if (GeneralManager.instance.wherePlayerWas == "Indoor" && SceneManager.GetActiveScene().buildIndex == 2)
        {
            OutdoorGameManager.instance.activePlayer.transform.SetPositionAndRotation(OutdoorGameManager.instance.outdoorSceneSpawnPoint.position, OutdoorGameManager.instance.outdoorSceneSpawnPoint.rotation);
            GeneralManager.instance.wherePlayerWas = "Outdoor";
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Clinic entrance"))
        {
            if (SceneManager.GetActiveScene().buildIndex == 2) // if Outdoor , Go Indoor
            {
                SfxManager.sfxInstance.GetCurrentAudioSource(OutdoorGameManager.instance.MusicPlayer[0].time);

                loadingScreenBarSystem.LoadingScreen(3);

                GeneralManager.instance.wherePlayerWas = "Indoor";
                GeneralManager.instance.hintDuration = GeneralManager.instance.resetHintDuration;
            }

            else if (SceneManager.GetActiveScene().buildIndex == 3) // if Indoor , Go Outdoor
            {
                loadingScreenBarSystem.LoadingScreen(2);

                GeneralManager.instance.hintDuration = GeneralManager.instance.resetHintDuration;
            }

            GeneralManager.instance.playerSceneSwitchTracker++;
        }
    }
    #endregion
}
