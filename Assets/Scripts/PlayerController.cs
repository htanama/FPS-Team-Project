using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller; // Reference to the CharacterController component
    [SerializeField] private float speed = 5.0f;             // Player movement speed
    [SerializeField] private float gravity = -9.81f;         // Gravity value
    [SerializeField] private float jumpHeight = 2.0f;        // Jump height

    private Vector3 moveDirection;                          // Movement vector
    private float verticalVelocity;                         // Vertical movement due to gravity

    // Start is called before the first frame update
    void Start()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
            if (controller == null)
            {
                Debug.LogError("CharacterController not found! Please attach one to the player.");
            }
        }

        Debug.Log("PlayerController initialized.");
    }


    // Update is called once per frame
    void Update()
    {
    // Get input for horizontal and vertical axes
    float horizontal = Input.GetAxis("Horizontal");     // A/D or Left/Right Arrow
    float vertical = Input.GetAxis("Vertical");         // W/S or Up/Down Arrow

    // Create the movement vector based on input
    moveDirection = new Vector3(horizontal, 0, vertical);
    moveDirection = transform.TransformDirection(moveDirection); // Make movement relative to the player's rotation
    moveDirection *= speed;

    // Check if the player is grounded
    if (controller.isGrounded)
    {
        verticalVelocity = -1f; // Reset vertical velocity to a small value to keep the player grounded

        // Jumping
        if (Input.GetButtonDown("Jump"))
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    else
    {
        // Apply gravity when not grounded
        verticalVelocity += gravity * Time.deltaTime;
    }

    // Add vertical movement (gravity/jump) to the movement vector
    moveDirection.y = verticalVelocity;

    // Move the player using the CharacterController
    controller.Move(moveDirection * Time.deltaTime);
}
}
