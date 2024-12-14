// Code Author: Erik Segura


using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class platformAttach : MonoBehaviour
{
    [SerializeField]GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IAttach attachPlayer = other.GetComponent<IAttach>();

        if (attachPlayer != null)
        {
            player.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IAttach attachPlayer = other.GetComponent<IAttach>();

        if (attachPlayer != null)
        {
            player.transform.parent = transform;
        }
    }
}
