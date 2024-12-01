using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class playerController : MonoBehaviour
{
    // These are the selectable fields in unity to control said item
    [SerializeField] CharacterController controller;

    [SerializeField] int Speed; 
    [SerializeField] int Jump;      //added this here to allow for jump[ing
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;

    // Enviromental factors - gravity etc...
    [SerializeField] int gravity;
    Vector3 movDir;
    Vector3 playerVel;

    int jumpCount;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void movement()
    {
        if (controller.isGrounded){
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        // the below code allows you to move left and right, essentially a transform of position
        
        movDir = (transform.right * Input.GetAxis("Horizontal")) + 
                 (transform.forward * Speed * Input.GetAxis("Vertical"));
        controller.Move(movDir * Speed * Time.deltaTime); // this is for all machines to move correctly

        jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

    }
    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }
}
