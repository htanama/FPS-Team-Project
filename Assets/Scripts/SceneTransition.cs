using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Name or index of the scene to load
    public string sceneToLoad;

    // Called when another object enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the door. Loading scene...");

            // Load the specified scene
            SceneManager.LoadScene(sceneToLoad);
            
        }
    }
}
