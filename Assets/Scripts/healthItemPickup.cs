using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthItemPickup : MonoBehaviour
{
    [SerializeField] float rotSpeedX, rotSpeedY, rotSpeedz;
    [SerializeField] float healAmount;

    [SerializeField] AudioClip[] pickupHealSounds;

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


}
