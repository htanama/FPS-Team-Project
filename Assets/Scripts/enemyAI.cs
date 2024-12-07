using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class enemyAI : MonoBehaviour, IDamage
{       
    [SerializeField] Renderer model;

    [SerializeField] public Transform shootPos;

    [SerializeField] GameObject bullet;

    [SerializeField] float shootRate;


    [SerializeField] NavMeshAgent agent;

    [Header("To Chase the Target")]    
    [SerializeField] public GameObject player; // remove this one later when we have gamemanager setup

    [SerializeField] int HP;

    bool playerInRange;

    bool isShooting; 

    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        //gamemanager.instance.updateGameGoal(1);

        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {       
        if (playerInRange)
        {
            //agent.SetDestination(playerPosition.position);
            if (!isShooting)
            {
                StartCoroutine(shoot());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            MoveToPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void MoveToPlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.transform.position);
            #if UNITY_EDITOR
                Debug.DrawLine(transform.position, player.transform.position, Color.red);
            #endif
        }
    }


    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        if (HP < 0)
        {
            //gamemanager.instance.updateGameGoal(-1);
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
