using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class stunEnemy : baseEnemy
{
    [Header("     Stun Enemy Stats     ")]
    [SerializeField] private float stunDuration;
    [SerializeField] private int enemyHP;
    [SerializeField] private float enemySpeedMult;      //Speed multiplier
    [SerializeField] private int fleeDistance = 2;

    private enum EnemyState { Chasing, Fleeing }            //Behavior changes when taking orb
    private EnemyState currentState = EnemyState.Chasing;   //Starts by chasing the player
    bool isFleeing = false;     //Used to set speed once

    // Start is called before the first frame update
    void Start()
    {
        //Initializing stats
        currentHealth = enemyHP;
        agent.speed *= enemySpeedMult;
    }

    // Update is called once per frame
    void Update()
    {
        Behavior();     //the way the enemy acts around the player
    }

    public override void takeDamage(float amount)
    {
        //drop orb right before dying
        if (currentHealth - amount <= 0)
        { GameManager.instance.OrbScripts.DropOrb(transform); }
        //calling base method for damage handling
        base.takeDamage(amount);
    }

    protected override void Behavior()
    {
        switch (currentState)
        {
            case EnemyState.Chasing:
                chasePlayer();
                break;
            case EnemyState.Fleeing:
                fleePlayer();
                break;
        }
    }

    private void chasePlayer()
    {
        //move to player location anywhere on the scene when the player has the orb
        if (GameManager.instance.OrbScripts.IsHoldingOrb)
            agent.SetDestination(GameManager.instance.Player.transform.position);

        //stun and take orb from player
        if (Vector3.Distance(transform.position, GameManager.instance.Player.transform.position) < agent.stoppingDistance)
        {
            stunPlayer();
            takeOrbFromPlayer();
        }
    }

    private void fleePlayer()
    {
        //enemy runs faster
        if (!isFleeing)
        {
            agent.speed = (GameManager.instance.PlayerScript.Speed *
                GameManager.instance.PlayerScript.SprintMod) - 1;       //He runs slightly slower than player sprint speed
            //Prevent speed from being set more than once
            isFleeing = true;
        }

        //find direction away from player
        Vector3 playerPosition = GameManager.instance.Player.transform.position;
        Vector3 fleeDirection = (transform.position - playerPosition).normalized;

        //destination to run to
        Vector3 fleeDestination = transform.position + fleeDirection * agent.stoppingDistance * fleeDistance;

        //move to that destination
        agent.SetDestination(fleeDestination);
    }

    private void stunPlayer()
    {
        playerController player = GameManager.instance.Player.GetComponent<playerController>();
        //stun player for set duration
        if(player != null && GameManager.instance.OrbScripts.IsHoldingOrb)
        {
            player.stun(stunDuration);
        }
    }

    private void takeOrbFromPlayer()
    {
        //accessing orb manager
        orbManager OrbManager = GameManager.instance.OrbScripts;

        //taking orb from player
        if(OrbManager != null)
        {
            OrbManager.takeOrb(transform);    //Passing enemy transform
            currentState = EnemyState.Fleeing;  //Change enemy state when taking the orb
        }
    }
}
