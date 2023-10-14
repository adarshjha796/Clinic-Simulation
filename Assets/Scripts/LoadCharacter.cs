//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using TMPro;

public class LoadCharacter : MonoBehaviour
{
	public GameObject characterPrefabs;
	void Awake()
	{
		int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");

        for (int i = 0; i < characterPrefabs.transform.childCount; i++)
        {
			if(i == selectedCharacter)
            {
				// If disabled then only enable it.
				if (!characterPrefabs.transform.GetChild(i).gameObject.activeSelf)
				    characterPrefabs.transform.GetChild(i).gameObject.SetActive(true);
			}
			else
            {
				// If enabled then only disabled it.
				if (characterPrefabs.transform.GetChild(i).gameObject.activeSelf)
					characterPrefabs.transform.GetChild(i).gameObject.SetActive(false);
			}
        }
	}
}
