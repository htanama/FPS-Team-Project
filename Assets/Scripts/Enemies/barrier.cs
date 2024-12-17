using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barrier : MonoBehaviour, IDamage       //barrier can take damage and be destroyed
{
    [SerializeField] [Range(1.0f, 200f)] private float barrierHealth;

    public void takeDamage(float amount)
    {
        barrierHealth -= amount;        //subtract damage taken from health

        //when health reaches zero
        if(barrierHealth <= 0)
        {
            //barrier destroyed
            Destroy(gameObject);
        }
    }
}
