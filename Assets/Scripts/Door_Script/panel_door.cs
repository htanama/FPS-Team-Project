using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class panel_door : MonoBehaviour
{
    bool isOpen = false;  // false = closed, true = open
    bool inRange = false;

    Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Action") && inRange)
        {
            ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        animator.SetBool("DoorState", isOpen);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player") inRange = true;
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player") inRange = false;
    }
}
