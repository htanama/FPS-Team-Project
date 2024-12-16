using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

<<<<<<< Updated upstream:Assets/Scripts/enemyAI.cs
public class enemyAI : MonoBehaviour, IDamage, IOpen
{
    [Header("      ENEMY      ")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
=======
public class enemyController : MonoBehaviour, IDamage, IOpen
{
    [Header("      ENEMY      ")]
    [SerializeField] NavMeshAgent agent;      //Components shared between all/most enemy types
    [SerializeField] Renderer model;
    [SerializeField] Animator animator;
>>>>>>> Stashed changes:Assets/Scripts/Enemies/enemyController.cs

    [Header("      TRANSFORMS/POSITIONS      ")]
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Transform healthBarPos;

    [Header("      ENEMY STATS      ")]
<<<<<<< Updated upstream:Assets/Scripts/enemyAI.cs
    int HPOrig;
    float angleToPlayer;
    float stoppingDistOrig;
    [SerializeField] int HP;
=======
    float angleToPlayer;
    float stoppingDistOrig;

    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    [SerializeField] private HealthBars enemyHealthBar;

    private float fillSpeed;
    private Gradient colorGradient;

>>>>>>> Stashed changes:Assets/Scripts/Enemies/enemyController.cs
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;  // sphere distance of roaming
    [SerializeField] int roamTimer; // how long to wait before move again

    [Header("      ANIMATION      ")]
    [SerializeField] Animator animator;
    [SerializeField] int animSpeedTransition;

    [Header("      DMG STATS      ")]
    Color colorOrig;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;


    // Flags/Bools //
    bool playerInRange;
    bool isShooting;
    bool isRoaming;

    // Stop Roam Temp //
    Coroutine co;

    // Vectors //
<<<<<<< Updated upstream:Assets/Scripts/enemyAI.cs
=======

>>>>>>> Stashed changes:Assets/Scripts/Enemies/enemyController.cs
    Vector3 playerDirection;
    Vector3 startingPos;
    Vector3 lastPlatformPosition;

    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }



    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        UpdateEnemyHealthBar();

        colorOrig = model.material.color; // for flash red
        startingPos = agent.transform.position; // to remember the starting position for roaming
        stoppingDistOrig = agent.stoppingDistance; // remember for roam/idle reset
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream:Assets/Scripts/enemyAI.cs
        // animation
=======
        //Behavior();
>>>>>>> Stashed changes:Assets/Scripts/Enemies/enemyController.cs
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animationSpeed = animator.GetFloat("Speed");

        animator.SetFloat("Speed", Mathf.MoveTowards(animationSpeed, agentSpeed, Time.deltaTime * animSpeedTransition));

        //gets animation to work
        //animator.SetFloat("Speed", agent.velocity.normalized.magnitude);

        if (playerInRange && !canSeePlayer())
        {
            //timer or close
            if (!isRoaming && agent.remainingDistance < 0.001f)
            {
                co = StartCoroutine(roam());
            }
        }
        else if (!playerInRange)
        {
            //timer or close
            if (!isRoaming && agent.remainingDistance < 0.001f)
            {
                co = StartCoroutine(roam());
            }
        }
    }
<<<<<<< Updated upstream:Assets/Scripts/enemyAI.cs
    
    // Enemy Roaming //
=======

>>>>>>> Stashed changes:Assets/Scripts/Enemies/enemyController.cs
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

        // animation
        animator.SetTrigger("Shoot");

        // create bullet
        Instantiate(bullet, shootPos.position, agent.transform.rotation);

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
        angleToPlayer = Vector3.Angle(playerDirection, agent.transform.forward);

        // To know the location of the player by using raycasting, do we hit the player
        RaycastHit hit;
        // Player inside the sphere range and in FOV.
        if (Physics.Raycast(headPos.position, playerDirection, out hit) && angleToPlayer <= FOV)
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
        agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    // Enemy Takes Damage //
<<<<<<< Updated upstream:Assets/Scripts/enemyAI.cs
    public void takeDamage(int amount)
    {   
        // decrease HP
        HP -= amount;

        // update UI/health bar....in progress

        // stop Roaming
        StopCoroutine(coroutine);
        isRoaming = false;
        
        // visual feedback flash red
        StartCoroutine(flashRed());
        
        //run toward player's last known position
        agent.SetDestination(GameManager.instance.Player.transform.position);

        // no HP left
        if (HP <= 0)
        {
           /// this is only if the goal is killing enemies, want to make -1 to enemycount    
           // GameManager.instance.UpdateGame(-1); // code okay problem code cannot kill the enemy
           
            // I am dead
            Destroy(gameObject);            
=======
    public void takeDamage(float amount)
    {
        // decrease _HP
        currentHealth -= amount;

        if (currentHealth > 0)
        {
            StopCoroutine(co);
            isRoaming = false;
            StartCoroutine(flashRed());
            agent.SetDestination(GameManager.instance.Player.transform.position);
            UpdateHealth(-amount);
        }
        else
        {
            Destroy(gameObject);
>>>>>>> Stashed changes:Assets/Scripts/Enemies/enemyController.cs
        }
    }

    // Health //
    public void UpdateHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateEnemyHealthBar();
    }
    private void UpdateEnemyHealthBar()
    {
        enemyHealthBar.UpdateHealthBar(currentHealth, maxHealth);
    }
}
