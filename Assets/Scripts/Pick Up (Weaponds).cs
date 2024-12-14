using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] weaponStats gun;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            GameManager.instance.playerScript.GetGunStats(gun);
                Destroy(gameObject);

        }
    }
   

}
