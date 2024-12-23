/*
  Code Author: Juan Contreras
  Date: 12/03/2024
  Class: DEV2

  Edited by: Lemons (Weapons)
            - Added fields shoot damage, distance, rate
            - Also field _HP
            - uncommented layer mask
            - Added bool/orb isShooting
            - update, added draw ray (raycast)
            - movement, added "fire"
            - added take damage 
            - added shoot
            -------------------------------------------
            - added derive from IDamage
            - workin on a feedback crosshair

        Edited: Erik Segura
            - Added _HP Bar functionality
            - Added audio to movement, gun firing, jump
*/

using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class playerController : MonoBehaviour, IDamage, IOpen
{

    [Header("      COMPONENTS      ")]
    [SerializeField] Renderer model;
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;              //Use when shooting is implemented

    [Header("      STATS      ")]
    [SerializeField][Range(0, 20)] private float playerMaxHealth;
    [SerializeField][Range(0, 20)] private float playerCurrentHealth;
    [SerializeField] Image playerHealthBar;
    //[SerializeField] float fillSpeed;
    //SerializeField] Gradient colorGradient;

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
    private Vector3 originalCenter;

    [Header("      WEAPONS      ")]
    // notes - weaponType; weaponEquipped; ammoCount; bool isReloading; isEquipping;
    // jammie will add gun list from lecture
    // jammie will add gun model from lecture
    // [SerializeField] GameObject bullet; // not using the Damage bullet like the enemy
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] int currentAmmo;
    [SerializeField] GameObject gunModel;
    [SerializeField] List<weaponStats> gunList = new List<weaponStats>();
    // [SerializeField] Transform shootPos; // not using this variable like the enemy 
    

    [Header("      Player Audio      ")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] [Range (0, 1)] float audJumpVol;
    [SerializeField] AudioClip[] audStep;
    [SerializeField] [Range(0, 1)] float audStepVol;
    [SerializeField] AudioClip[] audDamage;
    [SerializeField] [Range(0, 10)] float audDamageVol;
    
    // Vectors //
    Vector3 moveDirection;
    Vector3 horizontalVelocity;

    // Tracking //
    Color colorOrig;

    int jumpCount;
    int gunListpos;

    bool isShooting;
    bool isSprinting;
    bool isPlayingStep;
    bool isCrouching;
    bool isReloading;

    RaycastHit contact;

    // Properties //
    // Health //
    public float PlayerMaxHealth
    {
        get { return playerMaxHealth; }
        set { playerMaxHealth = value; }
    }
    public float PlayerCurrentHealth
    {
        get { return playerCurrentHealth; }
        set { playerCurrentHealth = value; }
    }

    //getters and setters (used to calculate stun enemy speed)
    public int Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public int SprintMod
    {
        get { return sprintMod; }
        set { sprintMod = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = speed;
        originalHeight = controller.height;
        originalCenter = controller.center;
        
        // Health and Health Bar //
        playerCurrentHealth = playerMaxHealth;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {        
        //draw ray
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);

        //if game is not paused
        if (!GameManager.instance.IsPaused)
        {
            //always checking for these
            movement();
            selectGun();
            crouch();              
        }

        sprint(); //Outside of condition to prevent infinite sprint glitch


        //if(Input.GetButton("Gun Info"))
        //{
        //    displayAllWeaponInfo();
        //}
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

        
        if (Input.GetButton("Fire1") && gunList.Count > 0 && !isShooting)
        {
            if (gunList[gunListpos].ammoCurrent > 0 && !isShooting) 
            {
                StartCoroutine(Shoot());                
            }
        }

        if (Input.GetButton("Reload") && gunList.Count > 0 && !isReloading)
        {
            if (gunList[gunListpos].ammoCurrent < gunList[gunListpos].ammoMax)
                StartCoroutine(Reload());
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
            isCrouching = !isCrouching;
        }
        if(isCrouching)
        {
            currentSpeed = Mathf.RoundToInt(speed * crouchWalkSpeed); //Reduce speed

            //Change height when crouching
            controller.height = originalHeight * crouchHeight;
            controller.center = new Vector3(0, controller.height / 2, 0);
        }
        else if (!isCrouching) //When the crouch key is released
        {
            currentSpeed = speed; //Restore speed

            //Restore height when releasing crouch button
            controller.height = originalHeight;
            controller.center = originalCenter;
        }
    }

    // Player UI //
    public void updatePlayerUI()
    {
        //update player health bar
        GameManager.instance.playerHpBar.fillAmount = playerCurrentHealth / playerMaxHealth;

        //float targetFillAmount = playerCurrentHealth / playerMaxHealth;
        //playerHealthBar.fillAmount = Mathf.Lerp(playerHealthBar.fillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
        //playerHealthBar.color = colorGradient.Evaluate(targetFillAmount);

        //variable to pass to game manager method total orbs collected from the list
        //int playerOrbsCollected = 0;
        //counting orbs collected
        //foreach(orbManager orbScript in GameManager.instance.OrbScripts) { playerOrbsCollected += orbScript.OrbsCollected; }
        //show counted orb captures to the UI
        //GameManager.instance.UpdateOrbsCollected(playerOrbsCollected);

        GameManager.instance.UpdateLivesUI(); //Show currentn lives on the UI
    }

    //public void displayAllWeaponInfo()
    //{
    //    if (gunList.Count != 0)
    //    {
    //        for (int i = 0; i < gunList.Count - 1; i++)
    //        {
    //            Debug.Log($"GetGunStat weapon: {gunList[i].model.name} and Index= {gunListpos}");
    //        }
    //    }
    //}

    public void GetGunStats(weaponStats gun)
    {
        gunList.Add(gun);        

        shootDamage = gun.damage;
        shootDistance = gun.weaponRange;
        shootRate = gun.shootRate;
        //currentAmmo = gun.ammoCurrent;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;

        // name of the weapon when it is changing 
        Debug.Log($"Add weapon: {gunList[gunListpos].model.name} and Index= {gunListpos} and length {gunList.Count}");
    }


    void selectGun()
    {     

        //int prevGunPos = gunListpos;

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListpos < gunList.Count - 1)
        {            
            gunListpos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListpos > 0)
        {
            gunListpos--;
            changeGun();
        }

        //if(gunListpos != prevGunPos) changeGun();

        Debug.Log($"gunListpos: {gunListpos}");           
    }


  void changeGun()
    {
        shootDamage = gunList[gunListpos].damage;
        shootDistance = gunList[gunListpos].weaponRange;
        shootRate = gunList[gunListpos].shootRate;

        // keep track current ammo do not pull from the default weapon status. 
        //currentAmmo = gunList[gunListpos].ammoCurrent;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListpos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListpos].model.GetComponent<MeshRenderer>().sharedMaterial;

        // name of the weapon when it is changing 
        Debug.Log($"Change weapon: {gunList[gunListpos].model.name} and Index= {gunListpos}");
    }

    // Player Damage and Weapons //   
    public void takeDamage(float amount)
    {
        playerCurrentHealth -= amount;
        playerCurrentHealth = Mathf.Clamp(playerCurrentHealth, 0, playerMaxHealth);

        StartCoroutine(screenFlashRed());               

        //updatePlayerUI();

        if (playerCurrentHealth <= 0)
        {
            GameManager.instance.Respawn();
        }

        updatePlayerUI();
        
        aud.PlayOneShot(audDamage[Random.Range(0, audDamage.Length)], audDamageVol);              

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
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }
    
    IEnumerator Shoot()
    {
        //turn on
        isShooting = true;

        aud.PlayOneShot(gunList[gunListpos].shootingSounds[Random.Range(0, gunList[gunListpos].shootingSounds.Length)], gunList[gunListpos].weaponSoundVolume);
                       
        if (gunList[gunListpos].ammoCurrent > 0)
        {
            gunList[gunListpos].ammoCurrent--;
            Debug.Log($"Shooting weapon: {gunList[gunListpos].model.name}. Ammo left: {gunList[gunListpos].ammoCurrent}/{gunList[gunListpos].ammoMax}");
            currentAmmo = gunList[gunListpos].ammoCurrent;

            //shoot code        
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out contact, shootDistance, ~ignoreMask))
            {
                Debug.Log(contact.collider.name);

                IDamage dmg = contact.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    dmg.takeDamage(shootDamage);
                }

                if (gunList[gunListpos].hitEffect != null)
                {
                    Instantiate(gunList[gunListpos].hitEffect, contact.point, Quaternion.identity);
                }

            }
        }
        else
        {
            Debug.Log($"Out of ammo for {gunList[gunListpos].model.name}. Reload required.");
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

    IEnumerator Reload()
    {
        weaponStats gun = gunList[gunListpos];

        if (isReloading || gun.ammoCurrent == gun.ammoMax) yield break;

        isReloading = true;

        aud.PlayOneShot(gun.reloadSounds[gunListpos], gun.weaponSoundVolume);

        yield return new WaitForSeconds(gun.reloadTime);       

        gun.ammoCurrent = gun.ammoMax;

        //gunList[gunListpos].ammoCurrent = gunList[gunListpos].ammoMax;
        //currentAmmo = gunList[gunListpos].ammoCurrent;

        Debug.Log($"Reloading weapon: {gunList[gunListpos].model.name}. Ammo: {gunList[gunListpos].ammoCurrent}/{gunList[gunListpos].ammoMax}");

        // Simulate reload time
        isReloading = false;
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

    public void HealthItemPickup(healthItemPickup item)
    {
        playerCurrentHealth += item.HealAmount;
    }

}
