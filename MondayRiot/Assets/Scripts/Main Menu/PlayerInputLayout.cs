/*=============================================================================
 * Game:        Monday Riot
 * Version:     Alpha
 * 
 * Class:       PlayerInputLayout.cs
 * Purpose:     To control the sprites in the main menu.
 * 
 * Author:      Lachlan Wernert
 *===========================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputLayout : MonoBehaviour
{
    [Header("References")]
    public Sprite controllerSprite;
    public Sprite keyboardSprite;
    public Sprite unnasignedSprite;
    public List<Image> layoutSprites = new List<Image>();
    public PlayerInputInformation playerInputInformation;

    private void Awake()
    {
        for(int i = 0; i < layoutSprites.Count; ++i)
        {
            layoutSprites[i].sprite = unnasignedSprite;
        }
    }

    public void Update()
    {
        for(int i = 0; i < playerInputInformation.PlayerCount; ++i)
        {
            if (playerInputInformation.GetInputInfo(i).KBAM)
                layoutSprites[i].sprite = keyboardSprite;
            else
                layoutSprites[i].sprite = controllerSprite;
        }
    }
}
