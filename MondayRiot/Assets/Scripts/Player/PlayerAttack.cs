/*=============================================================================
 * Game:        Monday Riot
 * Version:     Alpha
 * 
 * Class:       PlayerMovement.cs
 * Purpose:     Used to attack other players
 * 
 * Author:      Lachlan Wernert
 *===========================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class PlayerAttack : MonoBehaviour
{
    private PlayerHandler handler;

    private void Awake()
    {
        handler = this.GetComponent<PlayerHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        // Exiting the function if the player isn't controllable:
        if (!handler.IsControllable || handler.IsDead)
            return;

        if (handler.HasAssignedController())
        {
            if (XCI.GetButtonDown(handler.meleeAttackButton, handler.AssignedController))
            {
                TriggerAnimation();
            }
        }
        else
        {
            if(Input.GetKeyDown(handler.meleeAttackKey))
            {
                TriggerAnimation();
            }
        }
    }

    void TriggerAnimation()
    {
        if (handler.EquippedObject != null)
        {
            handler.EquippedObject.wasJustSwung = true;
            if (!handler.EquippedObject.useBothHands)
            {
                handler.RightHandAnimator.SetTrigger("Swing");
            }
            else
            {
                handler.BothHandsAnimator.SetTrigger("Swing");
                StartCoroutine(SwingTimer());
            }
        }
    }

    IEnumerator SwingTimer()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        handler.EquippedObject.wasJustSwung = false;
    }
}
