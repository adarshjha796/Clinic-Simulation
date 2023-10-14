using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SendToGoogle : MonoBehaviour
{
    [Header("String")]
    private string Comments;

    [Header("Int")]
    private int StarsCount;

    [SerializeField]
    private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSc4e0PB9RCjMmgQ_m8yF_c5S-0_TKNhaBFzoaHKM7VT9EzsMg/formResponse";// Our google form link


    /// <summary>
    /// Networking related and set the response text entry no. 
    /// </summary>
    /// <param name="comment"></param>
    /// <param name="stars"></param>
    /// <returns></returns>
    IEnumerator Post(string comment, int stars)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.669593122", comment);// in form click on field then oprn inspect and get our entry.no. and past it here
        form.AddField("entry.2143567311", stars);
        UnityWebRequest www = UnityWebRequest.Post(BASE_URL, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }



    /// <summary>
    /// Here we are setting Value in variables
    /// </summary>
    /// <param name="stars"></param>
    /// <param name="comment"></param>
    public void Send(int stars, string comment)
    {
        Comments = comment;
        StarsCount = stars;
        StartCoroutine(Post(Comments, StarsCount));

    }
}
