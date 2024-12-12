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

    [Header("      WEAPONS      ")]
    //[SerializeField] weaponType; weaponEquipped; ammoCount;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;

    [Header("      STATS      ")]
    [SerializeField][Range(1, 10)] int speed;      //Range adds a slider
    [SerializeField][Range(2, 5)] int sprintMod;
    [SerializeField][Range(1, 5)] int jumpMax;
    [SerializeField][Range(5, 30)] int jumpSpeed;
    [SerializeField][Range(10, 60)] int gravity;
    [SerializeField][Range(1, 10)] public int HP; // turn into Get/Setter

    //[SerializeField][Range(1, 20)] int uncrouchSpeed; //Fixing later
    [SerializeField][Range(0.1f, 1.0f)] float crouchWalkSpeed;
    [SerializeField][Range(0.01f, 1.0f)] float crouchHeight;

    [Header("      Capture the Flag      ")]
    [SerializeField] private Transform captureFlagBasePosition; // Position of the base
    [SerializeField] private Flag flag;
    private Transform flagOriginalPosition;


    Vector3 moveDirection;
    Vector3 horizontalVelocity;
    Color colorOrig;

    int jumpCount;
    int HPOrig;

    bool isSprinting;
    bool isCrouching;
    //bool isCrouchLerping;                 //To allow to modify crouch speed

    bool isShooting;
    RaycastHit contact;
    //bool isReloading; isEquipping;

    //Crouching variables
    private int currentSpeed;     //To avoid bugs by modifying speed directly
    private float originalHeight; //When releasing crouch
    //private float targetHeight;
    private Vector3 originalCenter;

   
    //private float originalScaleY; //For use when crouching
    //private Vector3 originalScale; //Used when releasing crouch
    //private Vector3 targetScale; //For use when releasing crouch

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

        //always checking for these
        movement();
        sprint();
        crouch();

        ReachToBase();
        
            //UpdateCrosshair();
    }

    void movement()
    {
        //Resets number of jumps once player is on the ground
        if (controller.isGrounded)
        {
            jumpCount = 0;

            horizontalVelocity = Vector3.zero;
        }

        moveDirection = transform.right * Input.GetAxis("Horizontal") +
                  transform.forward * Input.GetAxis("Vertical");    //Normalized to handle diagonal movement
        controller.Move(moveDirection * speed * Time.deltaTime);


        jump();

        //gives jump speed (y) a value
        controller.Move(horizontalVelocity * Time.deltaTime);
        //start pulling down immediately after the jump
        horizontalVelocity.y -= gravity * Time.deltaTime;

        // Weapons Add //
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

            currentSpeed = speed * sprintMod;

            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))       //Potential bug with crouching
        {

            speed /= sprintMod;

            currentSpeed = speed;

            isSprinting = false;
        }
    }

    // Paint ball gun effect implementation
    private void OnTriggerEnter(Collider other)
    {
        // Check if the trigger is the sphere
        if (other.CompareTag("Damage-Ball"))
        {
            #if UNITY_EDITOR
                //Debug.Log("Player hit by ball");
            #endif


            // Get the direction vector from the ball (sphere) to the player
            Vector3 pushDirection = (transform.position - other.transform.position).normalized;

            // Define the push distance
            float pushDistance = 13.0f; // knock player backward.

            // Use CharacterController to move the player
            controller.Move(pushDirection * pushDistance);
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



    // Weapons //
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

    public void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }
    
    IEnumerator Shoot()
    {
        //turn on
        isShooting = true;

        //shoot code        
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out contact, shootDistance, ~ignoreMask))
        {
            Debug.Log(contact.collider.name);                   

             //being overridden            
            IDamage dmg = contact.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
            
        }        

        yield return new WaitForSeconds(shootRate);
        
        //turn off
        isShooting = false;
    }

    //public void UpdateCrosshair()
    //{
    //    Crosshair crosshair = FindObjectOfType<Crosshair>();
    //    int crossDefault = crosshair.GetDefaultValue();

    //    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out contact, shootDistance, ~ignoreMask))
    //    {
    //        crosshair.SetDefaultValue(crosshair.GetTargetValue());
    //    }
    //    else
    //    {
    //        crosshair.SetDefaultValue(crossDefault);
    //    }
    //}

    IEnumerator screenFlashRed()
    {   
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }

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
}
