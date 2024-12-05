// Code Author: Harry Tanama
// Date 12/1/2024
//
// Assisting/Modified by: 

using UnityEngine;
using UnityEngine.AI;

public class AICaptureFlag : MonoBehaviour
{
    [SerializeField] private Transform flagPosition; // Position of the flag
    [SerializeField] private Transform basePosition; // Position of the base
    [SerializeField] private NavMeshAgent agent;

    private Flag flag; // Reference to the flag
    
    private AIState currentState;

    // Enum for AI states
    public enum AIState
    {
        Idle,
        MovingToFlag,
        MovingToBase
    }

    void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();

        //Searches the Entire Scene to looks for the first active GameObject that has a Flag component attached to it.
        flag = FindObjectOfType<Flag>();

        currentState = AIState.MovingToFlag;

    }

    void Update()
    {
        // Check if the flag is carried by this enemy
        if (flag != null && flag.IsCarriedBy(transform))
        {
            MoveToBase();
        }
        else
        {
            //Debug.Log("Cannot return to base without the flag!");
            // Add alternative behavior if the flag is not attached
        }

        switch (currentState)
        {
            case AIState.Idle:
                // Do nothing
                break;

            case AIState.MovingToFlag:
                MoveToFlag();
                break;

            case AIState.MovingToBase:
                MoveToBase();
                break;
        }
    }

    private void MoveToFlag()
    {
        if (flagPosition != null)
        {
            agent.SetDestination(flagPosition.position);

            Debug.Log("Move to find flag");
            // Check if AI has reached the flag
            if (Vector3.Distance(transform.position, flagPosition.position) < 0.5f)
            {
                flag.SetCarrier(transform); // Attach the flag to the AI
                Debug.Log("Flag captured! Moving to base...");
                currentState = AIState.MovingToBase; // Switch to base state
            }
        }
    }

    private void MoveToBase()
    {
        Debug.Log("In the MoveToBase() function");
        if (basePosition != null)
        {
            Debug.Log("Moving to base...");
            agent.SetDestination(basePosition.position);

            // Check if AI has reached the base
            if (Vector3.Distance(transform.position, basePosition.position) < 2.0f)
            {
                Debug.Log("Flag delivered to base!");

                // Reset the flag to its original position
                if (flag != null && flag.IsCarriedBy(transform))
                {
                    flag.ResetFlag();
                    Debug.Log("Flag successfully reset after delivery.");
                    currentState = AIState.MovingToFlag;
                }

            }
        }
    }

    // Public methods to control AI state
    public void StartMovingToFlag()
    {
        currentState = AIState.MovingToFlag;
    }

    public void StartMovingToBase()
    {
        currentState = AIState.MovingToBase;
    }

    public void SetState(AIState newState)
    {
        currentState = newState;
        Debug.Log($"AI state changed to: {newState}");
    }
}
