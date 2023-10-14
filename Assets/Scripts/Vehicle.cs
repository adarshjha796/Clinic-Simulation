//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] private float speed;

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.forward);
    }

}
