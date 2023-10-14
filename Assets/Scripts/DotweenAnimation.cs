using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DotweenAnimation : MonoBehaviour
{
    [SerializeField] Ease easeType;

    [SerializeField] Vector3 destination;
    [SerializeField] float delay;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMove(destination, delay).SetEase(easeType);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
