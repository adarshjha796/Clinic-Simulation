using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_DogCollieFull : MonoBehaviour {

    public Camera cam;
    public float CamSpeed = 4f;
    private bool isRotating;    
    private bool isZooming;
    Vector3 sp;
   
    private void Start()
    {
        sp = transform.localEulerAngles;
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            ZoomCamera();
        }

        if (!Input.GetMouseButton(1)) isRotating = false;

        if (isRotating)
        {
            CamRotate();
        }
        else StartPose();

        if (isZooming)
        {
            ZoomCamera();
        }
    }
    void CamRotate()
    {
        transform.RotateAround(transform.position, transform.right, - Input.GetAxis("Mouse Y") * CamSpeed);
        transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Mouse X") * CamSpeed);
    }
    void StartPose()
    {
        transform.localEulerAngles = sp;
    }
    void ZoomCamera()
    {
     if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 2f, Time.deltaTime * 5f);
        }else if(Input.GetAxis("Mouse ScrollWheel") < 0) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 100, Time.deltaTime * 5f);
    }
}
