<<<<<<< Updated upstream
=======
/*
  Code Author: Juan Contreras
  Date: 12/03/2024
  Class: DEV2
*/

>>>>>>> Stashed changes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
<<<<<<< Updated upstream
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [Header("----- Stats -----")]
=======
    [Header("      COMPONENTS      ")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;              //Use when shooting is implemented

    [Header("      STATS      ")]
>>>>>>> Stashed changes
    [SerializeField][Range(1, 10)] int speed;      //Range adds a slider
    [SerializeField][Range(2, 5)] int sprintMod;
    [SerializeField][Range(1, 5)] int jumpMax;
    [SerializeField][Range(5, 30)] int jumpSpeed;
    [SerializeField][Range(10, 60)] int gravity;

<<<<<<< Updated upstream
    [Header("----- Gun Stats -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    Vector3 moveDir;
    Vector3 playerVel;
=======
    /*[Header("----- Gun Stats -----")]     //if guns are added
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;*/

    Vector3 moveDirection;
    Vector3 horizontalVelocity;
>>>>>>> Stashed changes

    int jumpCount;

    bool isShooting;
    bool isSprinting;
<<<<<<< Updated upstream
=======
    private int currentSpeed;     //To avoid bugs by modifying speed directly
>>>>>>> Stashed changes

    // Start is called before the first frame update
    void Start()
    {
<<<<<<< Updated upstream

=======
        currentSpeed = speed;
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
=======
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);  //Shows shooting path
>>>>>>> Stashed changes

        movement();
        sprint();   //always checking for sprint
    }

    void movement()
    {
        //Resets number of jumps once player is on the ground
        if (controller.isGrounded)
        {
            jumpCount = 0;
<<<<<<< Updated upstream
            playerVel = Vector3.zero;
        }
        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += moveDir * speed * Time.deltaTime;

        moveDir = transform.right * Input.GetAxis("Horizontal") +
                  transform.forward * Input.GetAxis("Vertical");
        controller.Move(moveDir * speed * Time.deltaTime);
=======
            horizontalVelocity = Vector3.zero;
        }
        //moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += moveDirection * speed * Time.deltaTime;

        moveDirection = transform.right * Input.GetAxis("Horizontal") +
                  transform.forward * Input.GetAxis("Vertical");    //Normalized to handle diagonal movement
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
>>>>>>> Stashed changes

        jump();

        //gives jump speed (y) a value
<<<<<<< Updated upstream
        controller.Move(playerVel * Time.deltaTime);
        //start pulling down immediately after the jump
        playerVel.y -= gravity * Time.deltaTime;
=======
        controller.Move(horizontalVelocity * Time.deltaTime);
        //start pulling down immediately after the jump
        horizontalVelocity.y -= gravity * Time.deltaTime;
>>>>>>> Stashed changes

        /*if (Input.GetButton("Fire1") && !isShooting)
        {
            //IEnumerators have to be called with StartCoroutine
            StartCoroutine(shoot());
        }*/
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
<<<<<<< Updated upstream
            playerVel.y = jumpSpeed;
=======
            horizontalVelocity.y = jumpSpeed;
>>>>>>> Stashed changes
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
<<<<<<< Updated upstream
            speed *= sprintMod;
=======
            currentSpeed = speed * sprintMod;
>>>>>>> Stashed changes
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
<<<<<<< Updated upstream
            speed /= sprintMod;
=======
            currentSpeed = speed;
>>>>>>> Stashed changes
            isSprinting = false;
        }
    }

    /*IEnumerator shoot()     //needs a yield
    {
        isShooting = true;

        //shoot code
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask)) //~ignoreMask to prevent player shooting themselves
        {
            Debug.Log(hit.collider.name);

            //Get IDamage
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            //if it returns a value we know it has IDamage
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);        //Therefore apply damage
            }
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
<<<<<<< Updated upstream
=======
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {

        }
>>>>>>> Stashed changes
    }*/
}
