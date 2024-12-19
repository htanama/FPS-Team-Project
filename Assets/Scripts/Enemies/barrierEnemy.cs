/*
  Code Author: Juan Contreras
  Date: 12/17/2024
  Class: DEV2
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barrierEnemy : baseEnemy
{
    [Header("     Barrier Enemy Stats     ")]
    [SerializeField][Range(0.5f, 10f)] private GameObject barrierObj;     //place barrier object here
    [SerializeField][Range(0.5f, 10.0f)] private float barrierCooldown;//time between barrier casts
    [SerializeField][Range(2.0f, 20.0f)] private float barrierLifetime;//how long the barrier is up for
    [SerializeField][Range(1.0f, 25.0f)] private float allyDetectionRadius;//radius to find enemy AI's
    [SerializeField][Range(1.0f, 10.0f)] private float followDistance;     //distance to stand behind enemy allies

    private float nextBarrierTime;          //time before allowing to create another barrier using cooldown time
    //set to infinity to guarantee first iteration of the condition is met
    private float closestDistance = Mathf.Infinity; //to temporarily hold distance of closest ally
    private float distance;     //holds distance of an ally to check if closer or not

    Transform closestAlly;      //keep track of closest enemy(ally)

    //overriding from baseEnemy
    protected override void Behavior()
    {
        stayBehindEnemies();
        manageBarriers();
    }

    // Update is called once per frame
    void Update()
    {
        Behavior();
    }

    private void stayBehindEnemies()
    {
        //check for nearby enemies(allies) to support
        Collider[] alliesInRange = Physics.OverlapSphere(transform.position, allyDetectionRadius);

        foreach (Collider ally in alliesInRange)
        {
            //checking if colliders in range or not itself and only enemy types
            if (ally.gameObject != this.gameObject && ally.GetComponent<baseEnemy>() != null)
            {
                //current distance to an enemy(ally) in range
                distance = Vector3.Distance(transform.position, ally.transform.position);
                if (distance < closestDistance)
                {
                    closestAlly = ally.transform;       //closest ally found and stored
                    closestDistance = distance;         //stores closest's ally distance
                }
            }
        }

        //enemy stays behind closest ally
        if (closestAlly != null)
        {
            //find direction of the player
            Vector3 dirToPlayer = (GameManager.instance.Player.transform.position - closestAlly.position).normalized;

            //find a position to stay behind the ally
            Vector3 posBehindAlly = closestAlly.position - dirToPlayer * followDistance;

            //move agent to that position
            agent.SetDestination(posBehindAlly);
        }
    }

    private void manageBarriers()
    {
        //checks to see if the set time has passed to cast next barrier
        if (Time.time >= nextBarrierTime)
        {
            createBarrier();    //spawn barriers on allies
            nextBarrierTime = Time.time + barrierCooldown;  //sets time for next barrier cast
        }
    }

    private void createBarrier()
    {
        //check for nearby enemies(allies) to support
        Collider[] alliesInRange = Physics.OverlapSphere(transform.position, allyDetectionRadius);

        foreach (Collider ally in alliesInRange)
        {
            //checking if colliders in range or not itself and only enemy types
            if (ally.gameObject != this.gameObject && ally.GetComponent<baseEnemy>() != null)
            {
                //instantiate a barrier object on the ally's position
                GameObject barrier = Instantiate(barrierObj, ally.transform.position, Quaternion.identity);

                //attach barrier to ally to follow them
                barrier.transform.SetParent(ally.transform);

                //destroy barrier after lifetime is over
                Destroy(barrier, barrierLifetime);
            }
        }
    }
}
