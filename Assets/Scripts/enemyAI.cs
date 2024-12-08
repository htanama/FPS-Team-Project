using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class enemyAI : MonoBehaviour, IDamage
{       
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] public Transform shootPos;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    
    bool playerInRange;
    bool isShooting; 

    Color colorOrig;

    Vector3 playerDirection;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.UpdateGame(1);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            playerDirection = GameManager.instance.player.transform.position - transform.position;

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
        }
        
    }
    void faceTarget()
    {
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
