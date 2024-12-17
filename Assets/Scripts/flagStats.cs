using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class flagStats : ScriptableObject
{
    public GameObject model;
    public Transform carrier;  // Current object carrying the orb
    public bool isCarried;    // Is the orb currently being carried? 

}
