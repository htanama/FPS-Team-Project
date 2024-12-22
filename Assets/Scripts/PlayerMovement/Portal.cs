using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform Player, destination;
    public GameObject playerGameObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerGameObject.SetActive(false);
            Player.position = destination.position;
            playerGameObject.SetActive(true);
        }
    }
}