/*
  Code Author: Juan Contreras
  Date: 12/03/2024
  Class: DEV2
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] float sensitivity;
    [SerializeField] float verticalMin, verticalMax;

    private float verticalRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //get input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //Not adding invert option
        verticalRotation -= mouseY;

        //Clamping x rotation of cam
        verticalRotation = Mathf.Clamp(verticalRotation, verticalMin, verticalMax);

        //Rotate camera on x-axis
        transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        //Rotate player on y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
