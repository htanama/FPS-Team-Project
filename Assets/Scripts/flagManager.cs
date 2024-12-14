/*
  Code Author: Juan Contreras
  Date: 12/06/2024
  Class: DEV2
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flagManager : MonoBehaviour
{
    [Header("     Flag Options     ")]
    [SerializeField] private Transform flagStartBase;
    [SerializeField] private Transform flagGoalBase;
    [SerializeField][Range(2.0f, 5.0f)] float captureDistance;
    [SerializeField][Range(0.1f, 4.0f)] float goalDistance;     //How close to get to the goal

    private GameObject flag;
    private Transform playerTransform;
    private bool isHoldingFlag = false;
    private Vector3 flagOffset = new Vector3(0, 2.9f, 0); //Adjust flag to not clip the ground

    int captureCount = 0;   //To keep track of score

    //Getters and setters
    public GameObject Flag
    {
        get => flag;
        set => flag = value;
    }

    public Transform FlagStartBase
    { 
        get => flagStartBase;
        set => flagStartBase = value;
    }

    public bool IsHoldingFlag
    {
        get => isHoldingFlag;
        set => isHoldingFlag = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameManager.instance.Player.transform;
        flag = GameManager.instance.Flag;
        //Flag setup
        ResetFlag();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHoldingFlag)
        {
            if (Vector3.Distance(playerTransform.position, flagGoalBase.transform.position) < goalDistance)
            {

                //FlagCaptured();
            }
        }
        else
        {
            //If not holding flag check if close enough to pick it up
            if (Vector3.Distance(playerTransform.position, flag.transform.position) < captureDistance)
                PickupFlag();
        }
    }

    void FlagCaptured()
    {
        captureCount++;
        GameManager.instance.UpdateCaptures(captureCount);  //Update the number of captures on the UI

        Debug.Log($"Flag captured! Total captures: {captureCount}");

        ReturnFlag();
    }

    void PickupFlag()
    {
        //pick up the flag
        isHoldingFlag = true;
        //playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        flag.transform.SetParent(playerTransform);  //Flag attaches to the player
        flag.transform.localPosition = new Vector3(0, 1, 0); //Set flag position on player
        flag.GetComponent<Collider>().enabled = false; //Turn off flag collider
    }

    public void DropFlag()
    {
        if (isHoldingFlag)
        {
            isHoldingFlag = false;
            flag.transform.SetParent(null);     //Detach flag from carrier
            flag.transform.position = playerTransform.position; //Drop flag at carrier's location
            flag.GetComponent<Collider>().enabled = true;   //Enable flag collider for pickup

            Debug.Log("Flag Dropped");
        }
    }

    void ReturnFlag()
    {
        //return the flag to base
        isHoldingFlag = false;
        flag.transform.SetParent(null);
        flag.transform.position = flagStartBase.transform.position + flagOffset; //Respawn/Move flag back at base
        flag.GetComponent<Collider>().enabled = true; //Enable flag collider

        Debug.Log("Flag returned to base");
    }

    void ResetFlag()
    {
        //flag is reset to base if not held
        flag.transform.position = flagStartBase.transform.position + flagOffset;
        flag.GetComponent<Collider>().enabled = true;
    }
}
