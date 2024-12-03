using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [Header("----- Stats -----")]
    [SerializeField][Range(1, 10)] int speed;      //Range adds a slider
    [SerializeField][Range(2, 5)] int sprintMod;
    [SerializeField][Range(1, 5)] int jumpMax;
    [SerializeField][Range(5, 30)] int jumpSpeed;
    [SerializeField][Range(10, 60)] int gravity;

    [Header("----- Gun Stats -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;

    bool isShooting;
    bool isSprinting;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        movement();
        sprint();   //always checking for sprint
    }

    void movement()
    {
        //Resets number of jumps once player is on the ground
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }
        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += moveDir * speed * Time.deltaTime;

        moveDir = transform.right * Input.GetAxis("Horizontal") +
                  transform.forward * Input.GetAxis("Vertical");
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();

        //gives jump speed (y) a value
        controller.Move(playerVel * Time.deltaTime);
        //start pulling down immediately after the jump
        playerVel.y -= gravity * Time.deltaTime;

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
            playerVel.y = jumpSpeed;
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
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
    }*/
}
