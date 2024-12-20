/*
  Code Author: Juan Contreras
  Date: 12/15/2024
  Class: DEV2
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

//There will be more enemies so Base Enemy is created for inheritance and reusability
public abstract class baseEnemy : MonoBehaviour, IDamage 
{
    [Header("     Base Enemy Stats     ")]
    [SerializeField] protected NavMeshAgent agent;      //Components shared between all/most enemy types
    [SerializeField] protected Renderer model;
    [SerializeField] protected Animator animator;
    
    [SerializeField] protected Image enemyHPBar;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;

    [SerializeField] protected float fillSpeed;
    [SerializeField] protected Gradient colorGradient;


    protected Vector3 playerDirection;

    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }
    public Image EnemyHPBar
    {
        get { return enemyHPBar; }
        set { enemyHPBar = value; }
    }
    public float FillSpeed
    {
        get { return fillSpeed; }
        set { fillSpeed = value; }
    }
    public Gradient ColorGradient
    {
        get { return colorGradient; }
        set { colorGradient = value; }
    }

    public virtual void takeDamage(float amount)      //All enemies take damage
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
            Destroy(gameObject);        //Dead
    }

    //To be defined in each enemy class
    protected abstract void Behavior();     //For consistency and clarity
}
