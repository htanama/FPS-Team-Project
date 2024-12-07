// Code Author: Harry Tanama
// Date 12/1/2024
//
// Assisting/Modified by: 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentLinkJump : MonoBehaviour
{
    public float jumpHeight = 2f;     // Height of the jump
    public float jumpDuration = 1f;  // Duration of the jump

    private NavMeshAgent agent;
    private bool isJumping = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.isOnOffMeshLink && !isJumping)
        {
            StartCoroutine(ParabolicJump(agent));
        }
    }

    IEnumerator ParabolicJump(NavMeshAgent agent)
    {
        isJumping = true;

        // Disable automatic OffMeshLink traversal
        agent.isStopped = true;

        // Get the start position and initial end position
        OffMeshLinkData linkData = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = linkData.endPos + Vector3.up * agent.baseOffset;

        // Check and adjust the landing position using a raycast
        RaycastHit hit;
        if (Physics.Raycast(linkData.endPos + Vector3.up, Vector3.down, out hit, Mathf.Infinity))
        {
            endPos = hit.point + Vector3.up * agent.baseOffset;
    
            #if UNITY_EDITOR
               Debug.Log($"Landing position adjusted to: {endPos}");
            #endif

        }
        else
        {
            #if UNITY_EDITOR
                Debug.LogWarning("Raycast failed! Using default end position.");
            #endif
        }


        // Perform a parabolic jump
        float elapsedTime = 0f;
        while (elapsedTime < jumpDuration)
        {
            float normalizedTime = elapsedTime / jumpDuration; // Normalized time
            float height = Mathf.Sin(normalizedTime * Mathf.PI) * jumpHeight; // Parabolic height
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + Vector3.up * height;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to the adjusted end position
        agent.transform.position = endPos;

        // Complete the OffMeshLink traversal
        agent.CompleteOffMeshLink();
        agent.isStopped = false;

        isJumping = false;
    }
}
