/*
  Code Author: Juan Contreras
  Date: 12/11/2024
  Class: DEV2
*/

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class slidingDoor : MonoBehaviour
{
    enum Direction      //Which direction to open the door
    { Left, Right }

    [SerializeField] GameObject door;
    [SerializeField] [Range(1, 10)] int doorSpeed;
    [SerializeField] Direction direction;

    Vector3 originalDoorPos;
    Vector3 openDoorPos;
    Vector3 slideDirection;

    bool isOpening;
    bool isClosing;

    private void Start()
    {
        originalDoorPos = door.transform.position;      //Store original position for closing
    }

    private void Update()
    {
        if(isOpening)
        {
            //Opens the door based on doorSpeed
            door.transform.position = Vector3.MoveTowards(door.transform.position, openDoorPos, Time.deltaTime * doorSpeed);
            
            if(door.transform.position == openDoorPos) {isOpening = false;}     //Stop moving once fully opened
        }
        else if(isClosing)
        {
            //Closes the door based on doorSpeed
            door.transform.position = Vector3.MoveTowards(door.transform.position, originalDoorPos, Time.deltaTime * doorSpeed);

            if (door.transform.position == originalDoorPos) { isClosing = false; }     //Stop moving once fully closed
        }

        if (direction == Direction.Right)
        {
            slideDirection = door.transform.right; //Moving right on local X-axis
        }
        else if (direction == Direction.Left)
        {
            slideDirection = -door.transform.right; //Moving left on local X-axis
        }

        //Calculating open door position based on left or right choice
        openDoorPos = originalDoorPos + (slideDirection * door.transform.localScale.x);     //Accounts for door width
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger) { return; }     //Prevent triggers from activating triggers

        IOpen open = other.GetComponent<IOpen>();   //Null if object does not have IOpen

        if(open != null)
        {
            isOpening = true;
            isClosing = false;      //Changes to open state even if closing
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IOpen open = other.GetComponent<IOpen>();

        if(open != null)
        {
            isClosing = true;
            isOpening = false;      //Closes even if in the middle of opening
        }
    }

}
