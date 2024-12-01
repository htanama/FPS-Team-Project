using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panel_door : MonoBehaviour
{
    GameObject Door;
    bool isClosed;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Action();
    }
    void Action()
    {
        // need to open door on key press
        if (Input.GetButtonDown("Action"))
        {
            Door.SetActive(false);
        }
        else
        {
            Door.SetActive(true);
        }
    }
}
