using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class stunEnemy : baseEnemy
{
    [Header("     Stun Enemy Stats     ")]
    [SerializeField] private float stunDuration;
    [SerializeField] private float enemyHP;
    [SerializeField] private float enemySpeedMult;      //Speed multiplier
    [SerializeField] private int fleeDistance = 2;

    private enum EnemyState { Chasing, Fleeing }            //Behavior changes when taking flag
    private EnemyState currentState = EnemyState.Chasing;   //Starts by chasing the player
    bool isFleeing = false;     //Used to set speed once

    // Start is called before the first frame update
    void Start()
    {
        //Initializing stats
        HP = enemyHP;
        agent.speed *= enemySpeedMult;
    }

    // Update is called once per frame
    void Update()
    {
        Behavior();     //the way the enemy acts around the player
    }

    public override void takeDamage(float amount)
    {
        //drop flag right before dying
        if (HP - amount <= 0)
        { GameManager.instance.FlagScript.DropFlag(transform); }
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
        //move to player location anywhere on the scene when the player has the flag
        if (GameManager.instance.FlagScript.IsHoldingFlag)
            agent.SetDestination(GameManager.instance.Player.transform.position);

        //stun and take flag from player
        if (Vector3.Distance(transform.position, GameManager.instance.Player.transform.position) < agent.stoppingDistance)
        {
            stunPlayer();
            takeFlagFromPlayer();
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
        if(player != null && GameManager.instance.FlagScript.IsHoldingFlag)
        {
            player.stun(stunDuration);
        }
    }

    private void takeFlagFromPlayer()
    {
        //accessing flag manager
        flagManager FlagManager = GameManager.instance.FlagScript;

        //taking flag from player
        if(FlagManager != null)
        {
            FlagManager.takeFlag(transform);    //Passing enemy transform
            currentState = EnemyState.Fleeing;  //Change enemy state when taking the flag
        }
    }
}
