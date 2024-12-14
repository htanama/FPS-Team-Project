using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    //Fields
    //speed powerup?
    //power?
    //inventory? (hold more than one flag?)
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {        

        if (other.CompareTag("Player"))
        {
            // flag?

            // power ups

            // jammie gun/ammo stuff
            if (other.CompareTag("Player"))
            {
                Debug.Log("pickup");
                
                Destroy(gameObject);
            }
            Destroy(gameObject);
        }
    }
}
