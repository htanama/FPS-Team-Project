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

public class playerController : MonoBehaviour, IDamage
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

    [Header("      STATS      ")]
    [SerializeField][Range(1, 10)] int speed;      //Range adds a slider
    [SerializeField][Range(2, 5)] int sprintMod;
    [SerializeField][Range(1, 5)] int jumpMax;
    [SerializeField][Range(5, 30)] int jumpSpeed;
    [SerializeField][Range(10, 60)] int gravity;
    [SerializeField][Range(1, 10)] public int HP; // turn into Get/Setter

    [SerializeField][Range(1, 20)] int uncrouchSpeed;
    [SerializeField][Range(0.1f, 1.0f)] float crouchWalkSpeed;
    [SerializeField][Range(0.1f, 1.0f)] float crouchHeight;

    Vector3 moveDirection;
    Vector3 horizontalVelocity;
    Color colorOrig;

    int jumpCount;
    int HPOrig;

    bool isSprinting;
    bool isScaling;                 //To allow to modify crouch speed

    bool isShooting;
    RaycastHit contact;
    //bool isReloading; isEquipping;

    private int currentSpeed;     //To avoid bugs by modifying speed directly

    private float originalScaleY; //For use when crouching
    private Vector3 originalScale; //Used when releasing crouch
    private Vector3 targetScale; //For use when releasing crouch

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = speed;
        originalScaleY = controller.transform.localScale.y;
        originalScale = controller.transform.localScale;
        HPOrig = HP;
        updatePlayerUI();

    }



    // Update is called once per frame
    void Update()
    {
        //draw ray
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);

        //always checking for these
        movement();
        sprint();
        crouch();

        UpdateCrosshair();
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
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);


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
        if (Input.GetButtonDown("Sprint"))
        {

            speed *= sprintMod;

            currentSpeed = speed * sprintMod;

            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {

            speed /= sprintMod;

            currentSpeed = speed;

            isSprinting = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // Check if the trigger is the sphere
        if (other.CompareTag("Damage-Ball"))
        {
#if UNITY_EDITOR
            Debug.Log("Player hit by ball");
#endif


            // Get the direction vector from the ball (sphere) to the player
            Vector3 pushDirection = (transform.position - other.transform.position).normalized;

            // Define the push distance
            float pushDistance = 11.0f;

            // Use CharacterController to move the player
            controller.Move(pushDirection * pushDistance);
        }
    }


    //IEnumerator shoot()     //needs a yield
    void crouch()
    {
        if (Input.GetButtonDown("Crouch")) //When the crouch key is pressed
        {
            isScaling = true;
            currentSpeed = Mathf.RoundToInt(speed * crouchWalkSpeed); //Reduce speed

            targetScale = new Vector3(transform.localScale.x, originalScaleY * crouchHeight, transform.localScale.z);  //Change scale to crouch scale
            // talk to 
        }
        else if (Input.GetButtonUp("Crouch")) //When the crouch key is released
        {
            isScaling = true;
            currentSpeed = speed; //Restore speed

            targetScale = new Vector3(transform.localScale.x, originalScaleY, transform.localScale.z); //Restore original scale

        }
        //Bool check to prevent incorrect scaling
        if (isScaling)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * uncrouchSpeed);  //Change scale accordingly
        //Note: The line above is here and not in the if statement b/e of the nature in which Unity checks for button presses,
        //      the line would only execute about half way or so

    }

    // Weapons //
    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();        

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
        
        //RaycastHit hit;
        //shoot code
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out contact, shootDistance))
        {         

            Debug.Log(contact.collider.name); //being overridden

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


    public void UpdateCrosshair()
    {
        Crosshair crosshair = FindObjectOfType<Crosshair>();
        int crossDefault = crosshair.GetDefaultValue();

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out contact, shootDistance))
        {
            crosshair.SetDefaultValue(crosshair.GetTargetValue());
        }
        else
        {
            crosshair.SetDefaultValue(crossDefault);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
