using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Trap_Swing : MonoBehaviour
{
    public GameObject trapDoor;

    private void OnTriggerEnter(Collider other)
    {
        trapDoor.GetComponent<Animation>().Play("trapAnim");
    }
}
