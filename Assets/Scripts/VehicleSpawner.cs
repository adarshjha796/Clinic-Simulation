using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] vehicle;
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;

    void Start()
    {
        StartCoroutine(SpawnerVehicle());
    }

    private IEnumerator SpawnerVehicle()
    {
        while(true)
        {
           yield return new WaitForSeconds(Random.Range(minTime, maxTime));
           Destroy(Instantiate(vehicle[Random.Range(0,vehicle.Length)], transform.position, transform.rotation),10f);
        }
    }
}
