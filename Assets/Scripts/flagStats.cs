using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class flagStats : ScriptableObject
{
    public GameObject model;
    public Transform carrier;  // Current object carrying the flag
    public bool isCarried;    // Is the flag currently being carried? 

}
