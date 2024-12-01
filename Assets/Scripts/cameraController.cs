using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // get input

        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

        // tie the mouse imput to rotX
        if (!invertY)
        {
            rotX -= mouseY;
        }
        else
        {
            rotX += mouseY;
        }

        //clamp the camera rotX
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        //Rotate the cam on the x-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);  // this code is for up and down.

        // rotate the player on the y-axis - this is for the player because camera is child
        transform.parent.Rotate(Vector3.up * mouseX);

    }
}
