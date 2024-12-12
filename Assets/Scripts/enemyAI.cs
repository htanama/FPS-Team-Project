using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class enemyAI : MonoBehaviour, IDamage, IOpen
{       
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist; // sphere distance of roaming
    [SerializeField] int roamTimer; // how long to wait before move again
    [SerializeField] int animSpeedTransition;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    
    bool playerInRange;
    bool isShooting;
    bool isRoaming;

    Color colorOrig;

    Vector3 playerDirection;
    Vector3 startingPos;
    float angleToPlayer;
    float stoppingDistOrig; // to remember our original stopping distance. 
    Coroutine coroutine;

    Vector3 lastPlatformPosition;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.UpdateGame(1);
        startingPos = transform.position; // to remember the starting position 
        stoppingDistOrig = agent.stoppingDistance; // to remember our original stopping distance. 
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && !canSeePlayer())
        {
            // check the timer && check distance if it is closer to the distance by 0.01f
            if (!isRoaming && agent.remainingDistance < 0.01f)
                coroutine = StartCoroutine(roam());
        }
        else if (!playerInRange) // the enemy is not in player range
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)            
                coroutine = StartCoroutine(roam());
            
        }
    }

    IEnumerator roam()
    {
        isRoaming = true;

        yield return new WaitForSeconds(roamTimer); // wait for second before continuing. 

        agent.stoppingDistance = 0; // only for roaming to make sure the AI reach its destination

        Vector3 randomPos = Random.insideUnitSphere * roamDist; // how big is our roaming distance        
        randomPos += startingPos;

        NavMeshHit hit; // get info using similar like raycast
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1); // remember where the hit is at. 
        agent.SetDestination(hit.position);

        isRoaming = false;
    }

    bool canSeePlayer()
    {
        // this is the head position of the enemy
        playerDirection = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        // playerDirection = GameManager.instance.player.transform.position - transform.position; // this is the feet position of the enemy, not the head position. 

        // Draw the Raycast inside the debug mode
        #if UNITY_EDITOR
            //Debug.DrawRay(headPos.position, playerDirection);
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
        agent.stoppingDistance = stoppingDistOrig; // get the distance closer to the player but not very close to the player face. 
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
            agent.stoppingDistance = 0; // agent cannot see player set stopping distance at zero
        }

    }

    public void takeDamage(int amount)
    {   
        HP -= amount;
        StartCoroutine(flashRed());
       
        if (HP <= 0)
        {
            #if UNITY_EDITOR
                //Debug.Log("dead");
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
