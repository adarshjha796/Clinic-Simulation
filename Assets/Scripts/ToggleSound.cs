using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSound : MonoBehaviour
{
    [Header("Bool")]
    [SerializeField] private bool toggleSound;   
    public bool toggleMusic;   
    public bool toggleMusicIndoor;

    public void Toggle()
    {
        if (toggleSound)
        {
            GeneralManager.instance.isSoundMute = !GeneralManager.instance.isSoundMute;
            SfxManager.sfxInstance.ToggleSound();

            if (GeneralManager.instance.isOutdoorSceneActive)
            {
                SfxManager.sfxInstance.ToggleCarSound();
            }         
        }
        if (toggleMusic)
        {
            GeneralManager.instance.isMusicMute = !GeneralManager.instance.isMusicMute;
            SfxManager.sfxInstance.ToggleMusic();
        }
        if (toggleMusicIndoor)
        {
            GeneralManager.instance.isMusicMute = !GeneralManager.instance.isMusicMute;
            SfxManager.sfxInstance.ToggleMusicIndoor();
        }
    }
}
