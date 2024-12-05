using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Script Author: Erik Segura
// Assisted by:

public class platformMovement : MonoBehaviour
{
    public enum MovementDirection
    {
        Horizontal,
        Vertical
    }

    [SerializeField] MovementDirection direction = MovementDirection.Horizontal;
    [SerializeField] float speed;
    [SerializeField] float distance;

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (direction == MovementDirection.Horizontal)
        {
            float newX = Mathf.PingPong(Time.time * speed, distance);
            transform.position = new Vector3(startPosition.x +  newX, startPosition.y, startPosition.z);
        }
        else
        {
            float newY = Mathf.PingPong(Time.time * speed, distance);
            transform.position = new Vector3(startPosition.x, startPosition.y + newY, startPosition.z);
        }
    }
}
