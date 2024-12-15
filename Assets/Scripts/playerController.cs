/*
  Code Author: Juan Contreras
  Date: 12/03/2024
  Class: DEV2

  Edited by: Lemons (Weapons)
            - Added fields shoot damage, distance, rate
            - Also field HP
            - uncommented layer mask
            - Added bool/flag isShooting
            - update, added draw ray (raycast)
            - movement, added "fire"
            - added take damage 
            - added shoot
            -------------------------------------------
            - added derive from IDamage
            - workin on a feedback crosshair

        Edited: Erik Segura
            - Added HP Bar functionality
            - Added audio to movement, gun firing, jump
*/

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.TextCore.Text;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class playerController : MonoBehaviour, IDamage, IOpen
{

    [Header("      COMPONENTS      ")]
    [SerializeField] Renderer model;
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;              //Use when shooting is implemented
    
    [Header("      STATS      ")]
    [SerializeField][Range(1, 10)] public int HP; /// turn into Get/Setter

    [SerializeField][Range(1,  10)] int speed;      //Range adds a slider
    [SerializeField][Range(2,  5)]  int sprintMod;
    [SerializeField][Range(1,  5)]  int jumpMax;
    [SerializeField][Range(5,  30)] int jumpSpeed;
    [SerializeField][Range(10, 60)] int gravity;

    // Crouching //
    //[SerializeField][Range(1, 20)] int uncrouchSpeed; //Fixing later
    [SerializeField][Range(0.1f, 1.0f)] float crouchWalkSpeed;
    [SerializeField][Range(0.01f, 1.0f)] float crouchHeight;

    // Crouching variables
    private int currentSpeed;     //To avoid bugs by modifying enemySpeedMult directly
    private float originalHeight; //When releasing crouch
    //private float targetHeight;
    private Vector3 originalCenter;

    //private float originalScaleY; //For use when crouching
    //private Vector3 originalScale; //Used when releasing crouch
    //private Vector3 targetScale; //For use when releasing crouch

    [Header("      WEAPONS      ")]
    // notes - weaponType; weaponEquipped; ammoCount; bool isReloading; isEquipping;
    // jammie will add gun list from lecture
    // jammie will add gun model from lecture
    [SerializeField] GameObject bullet;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] GameObject gunModel;
    [SerializeField] List<weaponStats> gunList = new List<weaponStats>();
    int gunListpos; 

    [SerializeField] Transform shootPos;

    [Header("      Player Audio      ")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] [Range (0, 1)] float audJumpVol;
    [SerializeField] AudioClip[] audStep;
    [SerializeField] [Range(0, 1)] float audStepVol;
    [SerializeField] AudioClip[] audDamage;
    [SerializeField] [Range(0, 10)] float audDamageVol;
    [SerializeField] AudioClip[] audShootSound;
    [SerializeField] [Range(0, 1)] float audShootSoundVol;

    // Vectors //
    Vector3 moveDirection;
    Vector3 horizontalVelocity;

    // Tracking //
    Color colorOrig;

    int jumpCount;
    int origHP;
    // jammie add list pos

    bool isShooting;
    bool isSprinting;
    bool isPlayingStep;
    bool isCrouching;
    //bool isCrouchLerping;                 //To allow to modify crouch enemySpeedMult

    RaycastHit contact;
    
    //getters and setters (used to calculate stun enemy speed)
    public int Speed
    {
        get => speed;
        set => speed = value;
    }

    public int SprintMod
    {
        get => sprintMod;
        set => sprintMod = value;
    }

    public int OrigHP
    {
        get => origHP;
        set => origHP = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = speed;
        originalHeight = controller.height;
        originalCenter = controller.center;
        //originalScaleY = controller.transform.localScale.y;
        //originalScale = controller.transform.localScale;

        origHP = HP;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        //draw ray
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);

        //if game is not paused
        if(!GameManager.instance.IsPaused)
        {
            //always checking for these
            movement();
            // jammie add gun select method

        }

        sprint(); //Outside of condition to prevent infinite sprint glitch
        crouch();      
    }

    // Player Movement
    void movement()
    {
        //Resets number of jumps once player is on the ground
        if (controller.isGrounded)
        {
            if (moveDirection.magnitude > 0.3f && !isPlayingStep) // check for step movement.
            {
                StartCoroutine(playStep());
            }

            jumpCount = 0;
            // falling/ledge
            horizontalVelocity = Vector3.zero;
        }

        //tie movement to camera 
        moveDirection = (transform.right * Input.GetAxis("Horizontal")) +
                        (transform.forward * Input.GetAxis("Vertical"));    //Normalized to handle diagonal movement
        controller.Move(moveDirection * speed * Time.deltaTime);

        jump();

        //gives jump enemySpeedMult (y) a value
        controller.Move(horizontalVelocity * Time.deltaTime);
        //start pulling down immediately after the jump
        horizontalVelocity.y -= gravity * Time.deltaTime;

        //physics fix, under object
        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            horizontalVelocity.y = Vector3.zero.y; // horizontal velocity is lecture player velocity?
        }

        
        if (Input.GetButton("Fire1") && !isShooting)
        {
            StartCoroutine(Shoot());
        }

    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            horizontalVelocity.y = jumpSpeed;
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint") && !isCrouching)  //Won't sprint if crouching
        {
            speed *= sprintMod;
            currentSpeed = speed; // *nice catches here for powerup
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))               //Potential bug with crouching
        {
            speed /= sprintMod;
            currentSpeed = speed;
            isSprinting = false;
        }
    }

    void crouch()
    {
        if (Input.GetButtonDown("Crouch")) //When the crouch key is pressed
        {
            isCrouching = true;
            currentSpeed = Mathf.RoundToInt(speed * crouchWalkSpeed); //Reduce enemySpeedMult

            //Change height when crouching
            controller.height = originalHeight * crouchHeight;
            controller.center = new Vector3(0, controller.height / 2, 0);

            //isCrouchLerping = true;
        }
        else if (Input.GetButtonUp("Crouch")) //When the crouch key is released
        {
            isCrouching = false;
            currentSpeed = speed; //Restore enemySpeedMult

            //Restore height when releasing crouch button
            controller.height = originalHeight;
            controller.center = originalCenter;

            //isCrouchLerping = true;
        }
        //Adjust enemySpeedMult at which player crouches/uncrouches
        //if (isCrouchLerping)
        //{
        //    controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * uncrouchSpeed);  //Change scale accordingly

        //    isCrouchLerping = false;
        //}
        //Note: The line above is here and not in the if statement b/c of the nature in which Unity checks for button presses,
        //      the line would only execute about half way or so
    }

    // Player UI //
    public void updatePlayerUI()
    {
        GameManager.instance.PlayerHPBar.fillAmount = (float)HP / origHP;
        GameManager.instance.UpdateCaptures(GameManager.instance.FlagScript.CaptureCount);  //Show flag captures on UI
        GameManager.instance.UpdateLives(); //Show lives on the UI
    }

    public void GetGunStats(weaponStats gun)
    {
        gunList.Add(gun);
        shootDamage = gun.damage;
        shootDistance = gun.weaponRange;
        shootRate = gun.shootRate;
        gunModel. GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    // somewhere around this section
    // jammie add get gun stats
    // jammie add select gun scroll wheel (want to do a radial menu eventually)
    // jammie add change gun

    // Player Damage and Weapons //   
    public void takeDamage(int amount)
    {
        HP -= amount;

        updatePlayerUI();
        StartCoroutine(screenFlashRed());
        aud.PlayOneShot(audDamage[Random.Range(0, audDamage.Length)], audDamageVol);

        if (HP <= 0)
        {
            //death/lose screen in Respawn() method
            GameManager.instance.Respawn();
        }
    }

    //When the player is stunned this is called
    public void stun(float duration)
    {
        StartCoroutine(StunCoroutine(duration));        //In it's own method for simplification
    }

    IEnumerator StunCoroutine(float duration)
    {
        Debug.Log("Stun started!");

        //disable movement
        GetComponent<playerController>().enabled = false;
        //stun duration
        yield return new WaitForSeconds(duration);
        //enableMovement();
        GetComponent<playerController>().enabled = true;

        Debug.Log("Stun ended!");
    }

    IEnumerator screenFlashRed()
    {   
        GameManager.instance.PlayerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.PlayerDamageScreen.SetActive(false);
    }
    
    IEnumerator Shoot()
    {
        //turn on
        isShooting = true;
        aud.PlayOneShot(audShootSound[Random.Range(0, audShootSound.Length)], audShootSoundVol);

        //shoot code        
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out contact, shootDistance, ~ignoreMask))
        {
            Debug.Log(contact.collider.name);                   
         
            IDamage dmg = contact.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
            
            // jammie add gunlist if statement

        }

        //**************To be added when pickup is implemented******************

        //if (gunList[gunListPos].explosionRadius > 0)      //Check if gun has AoE damage
        //{
        //    //Find all colliders in the area of effect (in weaopon stats)
        //    Collider[] affectedObjects = Physics.OverlapSphere(hit.point, gunList[gunListPos].explosionRadius);
              //Loop through each object in the radius
        //    foreach (Collider obj in affectedObjects)
        //    {
                  //Gets IDamage component in case child is hit but not parent
        //        IDamage dmg = obj.GetComponentInParent<IDamage>();
                  //Checks for IDamage
        //        if (dmg != null)
        //        {
                      //Applies splash damage
        //            dmg.takeDamage(gunList[gunListPos].splashDamage);
        //        }
        //    }
        //}

        //**************To be added when pickup is implemented******************

        yield return new WaitForSeconds(shootRate);
        
        //turn off
        isShooting = false;
    }

    // code for walking audio

    IEnumerator playStep()
    {
        isPlayingStep = true;
        aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);

        if (!isSprinting)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }
        isPlayingStep = false;
    }

    // Triggers //
    // Paint ball gun effect implementation
    //    private void OnTriggerEnter(Collider other)
    //    {
    //        // Check if the trigger is the sphere
    //        if (other.CompareTag("Damage-Ball"))
    //        {
    //#if UNITY_EDITOR
    //            //Debug.Log("Player hit by ball");
    //#endif

    //            // Get the direction vector from the ball (sphere) to the player
    //            Vector3 pushDirection = (transform.position - other.transform.position).normalized;

    //            // Define the push distance
    //            float pushDistance = 13.0f; // knock player backward.

    //            // Use CharacterController to move the player
    //            controller.Move(pushDirection * pushDistance);
    //        }
    //        // is there an exit? ontriggerenter ontriggerexit?
    //    }
}
