using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//There will be more enemies so Base Enemy is created for inheritance and reusability
public abstract class baseEnemy : MonoBehaviour, IDamage
{
    [Header("     Base Enemy Stats     ")]
    [SerializeField] protected NavMeshAgent agent;      //Components shared between all/most enemy types
    [SerializeField] protected Renderer model;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float HP;
    protected Vector3 playerDirection;

    public virtual void takeDamage(float amount)      //All enemies take damage
    {
        HP -= amount;
        if (HP <= 0)
            Destroy(gameObject);        //Dead
    }

    //To be defined in each enemy class
    protected abstract void Behavior();     //For consistency and clarity
}