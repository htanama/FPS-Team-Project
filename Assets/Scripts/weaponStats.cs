/*
  Code Author: Juan Contreras
  Date: 12/13/2024
  Class: DEV2
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Able to make with right click
[CreateAssetMenu]       //Can make an instance of this

public class weaponStats : ScriptableObject //Acts as data storage
{
    public GameObject model; //Drag weapon model here
    public int damage;
    public float shootRate;
    public int weaponRange;
    public int ammoCurrent;
    public int ammoMax;

    [Header("Area Damage")]
    public float areaOfEffectRadius;
    public int splashDamage;

    [Header("FX")]
    public ParticleSystem hitEffect;//Drag weapon hit effect here
    public AudioClip[] shootingSounds;  //As an array to choose different sounds
    public float weaponSoundVolume;
    public AudioClip[] reloadSounds;
    public AudioClip[] emptySounds;

}
