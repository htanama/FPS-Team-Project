using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthItemPickup : MonoBehaviour
{
    [SerializeField] float rotSpeedX, rotSpeedY, rotSpeedz;
    [SerializeField] float healAmount;
    healthItemPickup healthItem;
    
    AudioSource healthItemAudio;
    [SerializeField] AudioClip[] pickupHealSounds;
    [SerializeField][Range(0, 10)] float pickupHealSoundsVol;

    public float HealAmount
    {
        get { return healAmount; }
        set { healAmount = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotSpeedX, rotSpeedY, rotSpeedz);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.PlayerScript.HealthItemPickup(healthItem);
            //play sound
            healthItemAudio.PlayOneShot(pickupHealSounds[Random.Range(0, pickupHealSounds.Length)], pickupHealSoundsVol);
            Destroy(gameObject);
        }
    }


}
