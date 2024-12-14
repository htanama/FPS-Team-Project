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
*/

using System.Collections;
using System.Collections.Generic;
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
    private int currentSpeed;     //To avoid bugs by modifying speed directly
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

    [SerializeField] Transform shootPos;
    
    [Header("      CAPTURE THE FLAG      ")]
    [SerializeField] private Transform captureFlagBasePosition; // Position of the base
    [SerializeField] private GameObject flagPole;  // this is the flagPole object. 
    private Flag flag; // flag logic that control when to drop the flag at the base. 
    private Transform flagOriginalPosition;

    // Vectors //
    Vector3 moveDirection;
    Vector3 horizontalVelocity;

    // Tracking //
    Color colorOrig;

    int jumpCount;
    int HPOrig;
    // jammie add list pos

    bool isShooting;
    bool isSprinting;
    bool isCrouching;
    //bool isCrouchLerping;                 //To allow to modify crouch speed

    RaycastHit contact;
    

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = speed;
        originalHeight = controller.height;
        originalCenter = controller.center;
        //originalScaleY = controller.transform.localScale.y;
        //originalScale = controller.transform.localScale;

        HPOrig = HP;
        updatePlayerUI();
        
        flag = flagPole.GetComponent<Flag>(); //put all the components of the flagPole from the inspector to the flag object(has info who carry the flag)
        flagOriginalPosition = flag.GetComponentInParent<Transform>(); // store original position of the flag
    }

    //Player moves with platform they're on
    //void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    Debug.Log("Collided with: " + hit.collider.name);
    //    //Check if the player is standing on a moving platform
    //    if (hit.collider.CompareTag("MovingPlatform"))
    //    {
    //        transform.parent = hit.collider.transform; //Attach player to the platform
    //    }
    //    else
    //    {
    //        transform.parent = null; //Detach player from the platform
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        //draw ray
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);

        //if game is not paused
        if(!GameManager.instance.isPaused)
        {
            //always checking for these
            movement();
            // jammie add gun select method

        }

        sprint(); //lecture puts outside of if
        crouch();

        ReachToBase();        
    }

    // Player Movement //
    void movement()
    {
        //Resets number of jumps once player is on the ground
        if (controller.isGrounded)
        {
            jumpCount = 0;
            // falling/ledge
            horizontalVelocity = Vector3.zero;
        }

        // tie movement to camera 
        moveDirection = (transform.right * Input.GetAxis("Horizontal")) +
                        (transform.forward * Input.GetAxis("Vertical"));    //Normalized to handle diagonal movement
        controller.Move(moveDirection * speed * Time.deltaTime);

        jump();

        //gives jump speed (y) a value
        controller.Move(horizontalVelocity * Time.deltaTime);
        //start pulling down immediately after the jump
        horizontalVelocity.y -= gravity * Time.deltaTime;

        //physics fix, under object
        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            horizontalVelocity.y = Vector3.zero.y; // horizontal velocity is lecture player velocity?
        }

        // Shoot Add //
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

        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint") && !isCrouching)  //Won't sprint if crouching
        {
            speed *= sprintMod;
            currentSpeed = speed * sprintMod; // *nice catches here for powerup
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
            currentSpeed = Mathf.RoundToInt(speed * crouchWalkSpeed); //Reduce speed

            //Change height when crouching
            controller.height = originalHeight * crouchHeight;
            controller.center = new Vector3(0, controller.height / 2, 0);

            //isCrouchLerping = true;
        }
        else if (Input.GetButtonUp("Crouch")) //When the crouch key is released
        {
            isCrouching = false;
            currentSpeed = speed; //Restore speed

            //Restore height when releasing crouch button
            controller.height = originalHeight;
            controller.center = originalCenter;

            //isCrouchLerping = true;
        }
        //Adjust speed at which player crouches/uncrouches
        //if (isCrouchLerping)
        //{
        //    controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * uncrouchSpeed);  //Change scale accordingly

        //    isCrouchLerping = false;
        //}
        //Note: The line above is here and not in the if statement b/e of the nature in which Unity checks for button presses,
        //      the line would only execute about half way or so
    }

    // Player UI //
    public void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }


    // Player Damage and Weapons //   
    public void takeDamage(int amount)
    {
        HP -= amount;

        updatePlayerUI();
        StartCoroutine(screenFlashRed());

        if (HP <= 0)
        {
            //death/lose screen
            GameManager.instance.LoseGame();
        }
    }

    // somewhere around this section
    // jammie add get gun stats
    // jammie add select gun scroll wheel (want to do a radial menu eventually)
    // jammie add change gun

    IEnumerator screenFlashRed()
    {   
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }
    
    IEnumerator Shoot()
    {
        //turn on
        isShooting = true;

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

    // Capture the Flag //

    // For capture the flag only
    // checking if player reach to base with the flag and score
    private void ReachToBase()
    {    
     
        if (captureFlagBasePosition != null)
        {
            // Check if Player has reached the base
            if (Vector3.Distance(transform.position, captureFlagBasePosition.position) < 2.0f)
            {
                #if UNITY_EDITOR
                    Debug.Log($"Player Touch Based");
                #endif
                
                if(flag != null && flag.IsCarriedBy(transform))
                {
                    #if UNITY_EDITOR
                        Debug.Log("Player has the flag and reached the base!");
                    #endif
                    GameManager.instance.UpdateFlagCount(+1);
                    flag.ResetFlag();
                }           
                
                
            }
        }
    }

     public void getFlagStatus(flagStats flag)
    {
        //flagPole is the flag object on the scene.  
        flagPole.GetComponent<MeshFilter>().sharedMesh = flag.model.GetComponent<MeshFilter>().sharedMesh;
        flagPole.GetComponent<MeshRenderer>().sharedMaterial = flag.model.GetComponent<MeshRenderer>().sharedMaterial;
        
        flag.model.GetComponent<Transform>().position = flagPole.GetComponent<Transform>().position; // put the flag in the correct position of the player
    }
}
