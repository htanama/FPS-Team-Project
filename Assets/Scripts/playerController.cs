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
    //[SerializeField] LayerMask ignoreMask;              //Use when shooting is implemented

    [Header("      STATS      ")]
    [SerializeField][Range(1, 10)] int speed;      //Range adds a slider
    [SerializeField][Range(2, 5)] int sprintMod;
    [SerializeField][Range(1, 5)] int jumpMax;
    [SerializeField][Range(5, 30)] int jumpSpeed;
    [SerializeField][Range(10, 60)] int gravity;
    [SerializeField, Range(5, 25)] public int HP;

    [SerializeField][Range(1, 20)] int uncrouchSpeed;
    [SerializeField][Range(0.1f, 1.0f)] float crouchWalkSpeed;
    [SerializeField][Range(0.1f, 1.0f)] float crouchHeight;

    Vector3 moveDirection;
    Vector3 horizontalVelocity;

    int jumpCount;

    bool isSprinting;

    bool isScaling;                 //To allow to modify crouch speed

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
    }

    public int GetHP() => HP;

    // Update is called once per frame
    void Update()
    {
        //always checking for these
        movement();
        sprint();
        crouch();
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


    void crouch()
    {
        if (Input.GetButtonDown("Crouch")) //When the crouch key is pressed
        {
            isScaling = true;
            currentSpeed = Mathf.RoundToInt(speed * crouchWalkSpeed); //Reduce speed

            targetScale = new Vector3(transform.localScale.x, originalScaleY * crouchHeight, transform.localScale.z);  //Change scale to crouch scale

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
}
