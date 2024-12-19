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
    [SerializeField] private Transform orbGoalPoint;
    [SerializeField][Range(2.0f, 5.0f)] float orbPickupDistance;   //How close to get to the orb to pick it up
    [SerializeField][Range(2.0f, 10.0f)] float orbAreaSize;     //How close to get to the goal

    private GameObject orb;
    private Transform playerTransform;
    private bool isHoldingOrb = false;

    //Getters and setters
    public GameObject Orb
    {
        get => orb;
        set => orb = value;
    }

    public Transform OrbGoalPoint
    {
        get => orbGoalPoint;
        set => orbGoalPoint = value;
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
        orb = gameObject;
        //orb goal
        orbGoalPoint = GameObject.FindWithTag("OrbGoal").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //limits to hold one orb at a time
        if (isHoldingOrb)
        {
            GameManager.instance.toggleImage(true);

            if (Vector3.Distance(playerTransform.position, orbGoalPoint.transform.position) < orbAreaSize)
            {
                //drop off the orb at the goal
                OrbGoalReached();
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

        GameManager.instance.UpdateOrbsCollected();  //Update the number of captures on the UI

        Debug.Log($"Orb collected!");

        DestroyOrb();
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

    void DestroyOrb()                //Might not need anymore
    {
        //return the orb to base
        isHoldingOrb = false;
        orb.transform.SetParent(null);
        //orb.transform.position = orbSpawnPoint.transform.position + orbOffset; //Respawn/Move orb back at base
        //orb.GetComponent<Collider>().enabled = true; //Enable orb collider
        Object.Destroy(orb);

        Debug.Log("Orb returned to base");
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
