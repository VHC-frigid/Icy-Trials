using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public float sensitivity = 120f;
    public float minClamp = -50, maxClamp = 50;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        transform.Rotate(0, sensitivity * Input.GetAxis("Mouse X") * Time.deltaTime, 0, Space.World);
        transform.Rotate(-sensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime, 0, 0, Space.Self);
        
        float xRot = transform.eulerAngles.x;
        if (xRot > 180) xRot -= 360;
        xRot = Mathf.Clamp(xRot, minClamp, maxClamp);
        transform.eulerAngles = new Vector3(xRot, transform.eulerAngles.y, 0);
    }
}
