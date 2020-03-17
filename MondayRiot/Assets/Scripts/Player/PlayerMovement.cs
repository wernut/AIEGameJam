/*=============================================================================
 * Game:        Monday Riot
 * Version:     Alpha
 * 
 * Class:       PlayerMovement.cs
 * Purpose:     To control the movement of the player.
 * 
 * Author:      Lachlan Wernert
 *===========================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class PlayerMovement : MonoBehaviour
{
    private PlayerHandler handler;
    private Transform modelTransform;
    private Rigidbody rigidBody;
    private float vAxis, hAxis, hRotAxis;

    private void Awake()
    {
        // Getting the references from the player handler component:
        handler = this.GetComponent<PlayerHandler>();
        modelTransform = handler.ModelTransform;
        rigidBody = handler.Rigidbody;
    }

    private void FixedUpdate()
    {
        // Checking if the player has a controller assigned to them: 
        if(handler.HasAssignedController())
        {
            // Obtaining axis values from xbox controller:
            vAxis    = XCI.GetAxis(XboxAxis.LeftStickY,  handler.AssignedController);
            hAxis    = XCI.GetAxis(XboxAxis.LeftStickX,  handler.AssignedController);
            hRotAxis = XCI.GetAxis(XboxAxis.RightStickX, handler.AssignedController);
        }
        else
        {
            // Obtaining axis values from unity input system:
            vAxis    = Input.GetAxis("Vertical");
            hAxis    = Input.GetAxis("Horizontal");
            hRotAxis = Input.GetAxis("A_Horizontal");
        }

        // Calculating total movement based on both vectors:
        Vector3 totalMovement = ((Vector3.forward * vAxis) + (Vector3.right * hAxis)) * handler.CurrentSpeed * Time.fixedDeltaTime;

        // Calcuating rigidbody velocity based on both axis:
        if(totalMovement.magnitude > 0)
            rigidBody.velocity = totalMovement;

        // Calculating the transforms rotation based on the rotation axis:
        modelTransform.Rotate((modelTransform.up * hRotAxis) * handler.rotationSpeed * Time.fixedDeltaTime);
    }
}
