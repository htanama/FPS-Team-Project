using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChanger : MonoBehaviour
{
    public static SpriteChanger instance;

    public Image uiImage;          // Assign the Image component in the Inspector

    void Start()
    {
        instance = this;    
    }

    public void GetChangedWeaponSprite(Sprite newSprite)
    {
        uiImage.sprite = newSprite;
    }

}
