/*
  Code Author: Juan Contreras
  Date: 12/13/2024
  Class: DEV2
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbManager : MonoBehaviour
{
    [Header("     Orb Options     ")]
    [SerializeField] private Transform orbSpawnPoint;
    [SerializeField] private Transform orbGoalPoint;
    [SerializeField][Range(2.0f, 5.0f)] float orbPickupDistance;   //How close to get to the orb to pick it up
    [SerializeField][Range(2.0f, 10.0f)] float orbAreaSize;     //How close to get to the goal

    private GameObject orb;
    private Transform playerTransform;
    private bool isHoldingOrb = false;
    private Vector3 orbOffset = new Vector3(0, 2.9f, 0); //Adjust orb to not clip the ground        DELETE??
    private int orbsCollected = 0;   //To keep track of score

    //Getters and setters
    public GameObject Orb
    {
        get => orb;
        set => orb = value;
    }

    public Transform OrbSpawnPoint
    { 
        get => orbSpawnPoint;
        set => orbSpawnPoint = value;
    }

    public Transform OrbGoalPoint
    {
        get => orbGoalPoint;
        set => orbGoalPoint = value;
    }

    public int OrbsCollected
    {
        get => orbsCollected;
        set => orbsCollected = value;
    }

    public bool IsHoldingOrb
    {
        get => isHoldingOrb;
        set => isHoldingOrb = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameManager.instance.Player.transform;
        orb = GameManager.instance.Orb;
        //Orb setup
        ResetOrb();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHoldingOrb)
        {
            if (Vector3.Distance(playerTransform.position, orbGoalPoint.transform.position) < orbAreaSize)
            {

                OrbGoalReached();
                GameManager.instance.UpdateCaptures(orbsCollected); //Update capture count to the UI
            }
        }
        else
        {
            //If not holding orb check if close enough to pick it up
            if (Vector3.Distance(playerTransform.position, orb.transform.position) < orbPickupDistance)
                PickupOrb();
        }
    }

    void OrbGoalReached()
    {
        orbsCollected++;
        GameManager.instance.UpdateCaptures(orbsCollected);  //Update the number of captures on the UI

        Debug.Log($"Orb collected! Total collected: {orbsCollected}");

        ReturnOrb();
    }

    void PickupOrb()
    {
        if (orb.transform.parent == null)
        {
            //pick up the orb
            isHoldingOrb = true;
            //playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            orb.transform.SetParent(playerTransform);  //Orb attaches to the player
            orb.transform.localPosition = new Vector3(0, 1, 0); //Set orb position on player
            orb.GetComponent<Collider>().enabled = false; //Turn off orb collider
        }
    }

    public void DropOrb(Transform objectTransform)
    {
        if (orb.transform.parent != null)
        {
            isHoldingOrb = false;
            orb.transform.SetParent(null);     //Detach orb from carrier
            orb.transform.position = objectTransform.position; //Drop orb at carrier's location
            orb.GetComponent<Collider>().enabled = true;   //Enable orb collider for pickup

            Debug.Log("Orb Dropped");
        }
    }

    void ReturnOrb()                //Might not need anymore
    {
        //return the orb to base
        isHoldingOrb = false;
        orb.transform.SetParent(null);
        orb.transform.position = orbSpawnPoint.transform.position + orbOffset; //Respawn/Move orb back at base
        orb.GetComponent<Collider>().enabled = true; //Enable orb collider

        Debug.Log("Orb returned to base");
    }

    void ResetOrb()
    {
        //orb is reset to base if not held
        orb.transform.position = orbSpawnPoint.transform.position + orbOffset;
        orb.GetComponent<Collider>().enabled = true;
    }

    //called when enemy takes orb from player
    public void takeOrb(Transform enemyTransform)
    {
        if(isHoldingOrb)
        {
            //separate orb from player model
            isHoldingOrb= false;
            orb.transform.SetParent(null);

            //attach orb to enemy
            orb.transform.SetParent(enemyTransform);
            orb.transform.localPosition = new Vector3(0, 1, 0);    //Set location on enemy
            orb.GetComponent<Collider>().enabled = false;     //Can't take orb from enemy

            Debug.Log("Orb taken by enemy");
        }
    }
}
