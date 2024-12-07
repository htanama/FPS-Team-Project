/*
  Code Author: Juan Contreras
  Date: 12/03/2024
  Class: DEV2
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    [Header("      COMPONENTS      ")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;              //Use when shooting is implemented

    [Header("      STATS      ")]
    [SerializeField][Range(1, 10)] int speed;      //Range adds a slider
    [SerializeField][Range(2, 5)] int sprintMod;
    [SerializeField][Range(1, 5)] int jumpMax;
    [SerializeField][Range(5, 30)] int jumpSpeed;
    [SerializeField][Range(10, 60)] int gravity;

    /*[Header("----- Gun Stats -----")]     //if guns are added
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;*/

    Vector3 moveDirection;
    Vector3 horizontalVelocity;


    int jumpCount;

    bool isShooting;
    bool isSprinting;

    private int currentSpeed;     //To avoid bugs by modifying speed directly


    // Start is called before the first frame update
    void Start()
    {

        currentSpeed = speed;

    }

    // Update is called once per frame
    void Update()
    {

        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);  //Shows shooting path


        movement();
        sprint();   //always checking for sprint
    }

    void movement()
    {
        //Resets number of jumps once player is on the ground
        if (controller.isGrounded)
        {
            jumpCount = 0;

            horizontalVelocity = Vector3.zero;
        }
        //moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += moveDirection * speed * Time.deltaTime;

        moveDirection = transform.right * Input.GetAxis("Horizontal") +
                  transform.forward * Input.GetAxis("Vertical");    //Normalized to handle diagonal movement
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);


        jump();

        //gives jump speed (y) a value
        controller.Move(horizontalVelocity * Time.deltaTime);
        //start pulling down immediately after the jump
        horizontalVelocity.y -= gravity * Time.deltaTime;


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

    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {

        }
    }*/
}
