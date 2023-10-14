//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectAnimation : MonoBehaviour
{
    [SerializeField] Vector3 finalPosition;
    Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, finalPosition, 0.1f);
    }

    private void OnDisable()
    {
        transform.position = initialPosition;
    }
}
