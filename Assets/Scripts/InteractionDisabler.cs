using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDisabler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sofa") && other.gameObject.layer == 0)
        {
            other.gameObject.transform.parent.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Sofa") && other.gameObject.layer == 0)
        {
            other.gameObject.transform.parent.GetComponent<BoxCollider>().enabled = true;
        }
    }
}
