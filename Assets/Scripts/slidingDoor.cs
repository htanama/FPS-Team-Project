/*
  Code Author: Juan Contreras
  Date: 12/11/2024
  Class: DEV2
*/

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class slidingDoor : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] [Range(1.0f, 10.0f)] float doorSpeed; 

    Vector3 originalDoorPos;

    bool isOpening;
    bool isClosing;

    private void Start()
    {
        originalDoorPos = door.transform.position;
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger) { return; }     //Prevent triggers from activating triggers

        IOpen open = other.GetComponent<IOpen>();   //Null if object does not have IOpen

        if(open != null)
        {
            //slide on x-axis
            //Size of x scale
            //door.transform.position = new Vector3(originalDoorPos.x + door.transform.localScale.x, originalDoorPos.y, originalDoorPos.z);
            door.transform.position = Vector3.Lerp(door.transform.position, 
                                                    new Vector3(originalDoorPos.x + door.transform.localScale.x, originalDoorPos.y, originalDoorPos.z),
                                                    Time.deltaTime * 10 * doorSpeed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IOpen open = other.GetComponent<IOpen>();

        if(open != null)
        {
            door.transform.position = originalDoorPos;
        }
    }

}
