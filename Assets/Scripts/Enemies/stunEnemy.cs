using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stunEnemy : baseEnemy
{
    [Header("     Stun Enemy Stats     ")]
    [SerializeField] private float stunDuration;
    [SerializeField] private int enemyHP;
    [SerializeField] private float enemySpeedMult;      //Speed multiplier

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
        Behavior();
    }

    protected override void Behavior()
    {
        //move to player location anywhere on the scene
        agent.SetDestination(GameManager.instance.Player.transform.position);

        //stun and take flag from player
        if(Vector3.Distance(transform.position, GameManager.instance.Player.transform.position) < agent.stoppingDistance)
        {
            stunPlayer();
            takeFlagFromPlayer();
        }
    }

    private void stunPlayer()
    {
        playerController player = GameManager.instance.Player.GetComponent<playerController>();
        //stun player for set duration
        if(player != null)
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
        }
    }
}
