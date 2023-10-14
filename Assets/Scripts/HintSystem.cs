using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HintSystem : MonoBehaviour
{
    public static HintSystem instance;

    public Slider masterVolumeSlider;
    public Slider soundVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider musicIndoorVolumeSlider;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
          //  DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        ChangeMasterVolume(masterVolumeSlider.value);
        ChangeSoundVolume(soundVolumeSlider.value);

        masterVolumeSlider.onValueChanged.AddListener(val => ChangeMasterVolume(val));
        soundVolumeSlider.onValueChanged.AddListener(val => ChangeSoundVolume(val));
        musicVolumeSlider.onValueChanged.AddListener(val => ChangeMusicVolume(val));
        musicIndoorVolumeSlider.onValueChanged.AddListener(val => ChangeMusicVolumeIndoor(val));
    }



    /// <summary>
    /// This will adujest Master Volume
    /// </summary>
    /// <param name="value"></param>
    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }



    /// <summary>
    /// This will adujest sound Volume
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundVolume(float value)
    {
        if (GeneralManager.instance.isOutdoorSceneActive  || GeneralManager.instance.isIndoorSceneActive)
        {
            for (int i = 0; i < SfxManager.sfxInstance.audioSource.Length; i++)
            {
                SfxManager.sfxInstance.audioSource[i].volume = value;
            }
        }

        if (GeneralManager.instance.isOutdoorSceneActive)
        {
            for (int i = 0; i < OutdoorGameManager.instance.CarsSource.Length; i++)
            {
                OutdoorGameManager.instance.CarsSource[i].volume = value;
            }
        }      
    }



    /// <summary>
    /// This will adujest music Volume
    /// </summary>
    /// <param name="value"></param>
    public void ChangeMusicVolume(float value)
    {
        for (int i = 0; i < OutdoorGameManager.instance.MusicPlayer.Length; i++)
        {
            OutdoorGameManager.instance.MusicPlayer[i].volume = value;
        }
    }



    /// <summary>
    /// This will adujest indoor music Volume
    /// </summary>
    /// <param name="value"></param>
    public void ChangeMusicVolumeIndoor(float value)
    {
        for (int i = 0; i < IndoorGameManager.instance.MusicPlayer.Length; i++)
        {
            IndoorGameManager.instance.MusicPlayer[i].volume = value;
        }
    }
}
