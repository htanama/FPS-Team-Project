using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class enemyAI : MonoBehaviour, IDamage
{       
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    
    bool playerInRange;
    bool isShooting; 

    Color colorOrig;

    Vector3 playerDirection;
    float angleToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.UpdateGame(1);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && canSeePlayer())
        {
            
        }
        
    }

    bool canSeePlayer()
    {
        // this is the head position of the enemy
        playerDirection = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        // playerDirection = GameManager.instance.player.transform.position - transform.position; // this is the feet position of the enemy, not the head position. 

        // Draw the Raycast inside the debug mode
        #if UNITY_EDITOR
            Debug.DrawRay(headPos.position, playerDirection);
        #endif

        RaycastHit hit;
        // To know the location of the player by using raycasting, do we hit the player
        if (Physics.Raycast(headPos.position, playerDirection, out hit)) // Inside the sphere range.
        {
            // if the Raycast hit the player then do the statement inside the if statement.
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }

                //agent.SetDestination(playerPosition.position);
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
                return true;
            }
            
        }

        return false; // do not see the player, raycast did not hit the player
    }

    void faceTarget()
    {
        // There is bug here to be fix, the center point of the enemy is at its feet and the player center point is at the middle of the capsule.

        Quaternion rot = Quaternion.LookRotation(playerDirection);  //This 'snaps' in the given direction
        //Lerp it, change it over time; first param is what you're lerping, second is destination, last is time with turn speed multiplier
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {        

        if (other.CompareTag("Player"))
        {
            playerInRange = true;     
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void takeDamage(int amount)
    {   
        HP -= amount;
        StartCoroutine(flashRed());
        #if UNITY_EDITOR
            Debug.Log("before HP < 0");
        #endif

        if (HP <= 0)
        {
            #if UNITY_EDITOR
                Debug.Log("dead");
            #endif      
            GameManager.instance.UpdateGame(-1); // code okay problem code cannot kill the enemy
            // I am dead
            Destroy(gameObject);            
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    // adding to shoot
    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

}
