// Code Author: Harry Tanama
// Date 12/1/2024
//
// Assisting/Modified by: 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private Transform flagOppositePosition; // Position where the flag resets when delivered
    private Transform carrier;         // Current object carrying the flag
    private bool isCarried = false;    // Is the flag currently being carried?

    [SerializeField] private Vector3 flagOriginalPosition;  // Original spawn position of the flag

    void Start()
    {
        // Save the original position of the flag
        flagOriginalPosition = transform.position;
    }

    void Update()
    {
        if (isCarried && carrier != null)
        {
            // Update the flag's position to follow the carrier with an offset
            transform.position = carrier.position + new Vector3(0.0f, 0, 0.1f); // Adjust for visibility            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collided with: {other.name}, Tag: {other.tag}");
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("The Flag collided with an Enemy!");
        }

        // Handle flag pickup by Player or Enemy
        if (!isCarried)
        {
            OnTriggerPickUpFlag(other);
        }

       
    }

    private void OnTriggerPickUpFlag(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Enemy")) && !isCarried)
        {
            carrier = other.transform; // Set the object as the carrier
            isCarried = true; // Mark the flag as carried            

            Debug.Log($"{carrier.name} picked up the flag!");

        }
    }

    // Set the carrier of the flag
    public void SetCarrier(Transform newCarrier)
    {
        carrier = newCarrier; // Assign the new carrier
        isCarried = true;     // Mark the flag as carried
        
        #if UNITY_EDITOR
            Debug.Log($"{carrier.name} is now carrying the flag.");
        #endif

    }

    public void DropFlag()
    {
        if (isCarried)
        {
            // Detach the flag from the carrier
            carrier = null;
            isCarried = false;

            #if UNITY_EDITOR
                Debug.Log("Flag dropped!");
            #endif
        }
    }

    public void ResetFlag()
    {
        // Reset the flag to its home position
        carrier = null;
        isCarried = false;

        transform.position = flagOriginalPosition; // Reset to (0, 0, 0)        
        
    }

    public void DeliverFlag()
    {
        if (isCarried)
        {
            #if UNITY_EDITOR
                Debug.Log($"{carrier.name} delivered the flag to the base!");
            #endif
            // Reset the flag to its original position
            ResetFlag();
        }
    }

    public bool IsCarriedBy(Transform potentialCarrier)
    {
        return carrier == potentialCarrier;
    }


}
