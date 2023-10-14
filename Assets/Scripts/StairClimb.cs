using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairClimb : MonoBehaviour
{
    Rigidbody rigidBody;

    [SerializeField] 
    GameObject stepRayUpper;
    [SerializeField] 
    GameObject stepRayLower;
    [SerializeField] 
    float stepHeight = 0.3f;
    [SerializeField]
    float stepSmooth = 2f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Toilet Platform"))
        {
            StepClimb();
        }
    }

    void StepClimb()
    {
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hitLower, 0.1f))
        {
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hitUpper, 0.2f))
            {
                rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
        }

        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out RaycastHit hitLower45, 0.1f))
        {
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1), out RaycastHit hitUpper45, 0.2f))
            {
                rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
        }

        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out RaycastHit hitLowerMinus45, 0.1f))
        {
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1), out RaycastHit hitUpperMinus45, 0.2f))
            {
                rigidBody.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
        }
    }
}
