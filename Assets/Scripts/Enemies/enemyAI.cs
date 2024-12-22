using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Unity.VisualScripting;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class enemyAI : baseEnemy, IOpen
{
    [Header("      TRANSFORMS/POSITIONS      ")]
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [Header("      ENEMY STATS      ")]
    float angleToPlayer;
    float stoppingDistOrig;
    
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;  // sphere distance of roaming
    [SerializeField] int roamTimer; // how long to wait before move again

    [Header("      ANIMATION      ")]
    //[SerializeField] Animator animator;
    [SerializeField] int animSpeedTransition;

    [Header("      DMG STATS      ")]
    Color colorOrig;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    
    // Flags/Bools
    bool playerInRange;
    bool isShooting;
    bool isRoaming;

    // Stop Roam Temp //
    Coroutine coroutine;

    // Vectors //
    //Vector3 playerDirection;
    Vector3 startingPos;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateEnemyUI();

        currentHealth = MaxHealth;
        colorOrig = model.material.color; // for flash red
        startingPos = transform.position; // to remember the starting position for roaming
        stoppingDistOrig = agent.stoppingDistance; // remember for roam/idle reset
    }

    // Update is called once per frame
    void Update()
    {
        Behavior();
    }

    protected override void Behavior()
    {
        // animation
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animationSpeed = animator.GetFloat("Speed");
        animator.SetFloat("Speed", Mathf.MoveTowards(animationSpeed, agentSpeed, Time.deltaTime * animSpeedTransition));

        //if player in range but can't see
        if (playerInRange && !canSeePlayer())
        {
            // check the timer && check distance if it is closer to the distance by 0.01f
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                // start to roam
                coroutine = StartCoroutine(roam());
            }
        }
        // if player not in range
        else if (!playerInRange)
        {
            // not currently roaming and almost to position (running towards last known player position)
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                //start roam
                coroutine = StartCoroutine(roam());
            }
        }
    }
    // Enemy Roaming //
    IEnumerator roam()
    {
        // turn on 
        isRoaming = true;

        // IEnums must have yield
        yield return new WaitForSeconds(roamTimer); // wait for second before continuing. 

        // only for roaming to make sure the AI reaches destination
        agent.stoppingDistance = 0;

        // how big is our roaming distance 
        Vector3 randomPos = Random.insideUnitSphere * roamDist;        
        randomPos += startingPos;

        // Enemy is Hit by Player //
        NavMeshHit hit; // get info using similar like raycast
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1); // remember where the hit is at. 
        agent.SetDestination(hit.position); // player last known position

        // turn off
        isRoaming = false;
    }
    
    // Enemy Damage Feedback //
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    // Enemy Shoot //
    IEnumerator shoot()
    {

        // turn on
        isShooting = true;
        
        // how to keep shooting at player's waists?
        // Adjust shooting direction (e.g., aim slightly upward)
        //float angleX = -15f; // Adjust as needed
        //float angleY = 0f;
        //float angleZ = 0f;
        //Vector3 adjustedDirection = Quaternion.Euler(angleX, angleY, angleZ) * transform.forward;
        //create bullet        
        //Instantiate(bullet, shootPos.position, Quaternion.LookRotation(adjustedDirection));

        // animation
        animator.SetTrigger("Shoot");

        // create bullet
        Instantiate(bullet, shootPos.position, transform.rotation);

        // enemySpeedMult
        yield return new WaitForSeconds(shootRate);

        // turn off
        isShooting = false;

    }

    // Enemy Sees Player //
    bool canSeePlayer()
    {
        // this is the head position of the enemy
        playerDirection = GameManager.instance.Player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        // To know the location of the player by using raycasting, do we hit the player
        RaycastHit hit;
        // Player inside the sphere range and in FOV.
        if (Physics.Raycast(headPos.position, playerDirection, out hit)) 
        {
            // reset stopping distance
            agent.stoppingDistance = stoppingDistOrig;

            // if player seen
            if (hit.collider.CompareTag("Player"))
            {
                // run towards player
                agent.SetDestination(GameManager.instance.Player.transform.position);

                // turn towards player
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }

                // if not shooting, begin shooting
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
                // true = can see
                return true;
            }            
        }
        // reset to roam distance
        agent.stoppingDistance = 0;

        // do not see the player, raycast did not hit the player
        return false; 
    }

  
    // Triggers //
    // Player Enters Sphere //
    private void OnTriggerEnter(Collider other)
    {        

        if (other.CompareTag("Player"))
        {
            playerInRange = true;     
        }

    }
    // Player Exits Sphere //
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0; // agent cannot see player set stopping distance at zero
        }

    }

    // Methods //
    // Turn Towards Player //
    void faceTarget()
    {
        //temp for smooth turn
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));

        //Lerp it, change it over time; first param is what you're lerping, second is destination, last is time with turn enemySpeedMult multiplier
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public override void takeDamage(float amount)
    {
        base.takeDamage(amount);    //Calling base class method
        
        if(currentHealth > 0)
        { 
            if(coroutine != null) StopCoroutine(coroutine);

            isRoaming = false;
            StartCoroutine(flashRed());
            agent.SetDestination(GameManager.instance.Player.transform.position);
            UpdateEnemyUI();
            //UpdateHealth(-amount);                                                //*****
        }
    }

    public void UpdateEnemyUI()
    {
        EnemyHPBar.fillAmount = currentHealth / maxHealth;
    }

    //public void UpdateHealth(float amount)
    //{
    //    currentHealth += amount;
    //    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    //    UpdateEnemyHealthBar();
    //}
//    private void UpdateEnemyHealthBar()
//    {
//        float targetFillAmount = currentHealth / maxHealth;
//        enemyHPFill.fillAmount = Mathf.Lerp(enemyHPFill.fillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
//        enemyHPFill.color = colorGradient.Evaluate(targetFillAmount);
//    }
}
