using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ramRotation : MonoBehaviour
{
    [SerializeField] float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // rotate on the y axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime * 10);
    }
}
