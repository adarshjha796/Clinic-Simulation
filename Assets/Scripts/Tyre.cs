using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tyre : MonoBehaviour
{
    private float rotationSpeed = 10f;
    public bool canTyreRotate;

    void Update()
    {
        if(canTyreRotate)
        {
            transform.Rotate(new Vector3(0, 0, rotationSpeed));
        }
    }
}
