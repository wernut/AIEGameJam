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

public class PlayerMovement : MonoBehaviour
{
    private PlayerHandler handler;
    private Transform modelTransform;
    private Rigidbody rigidBody;

    [Header("Attributes")]
    public float speed = 50.0f;
    public float rotationSpeed = 10.0f;
    private float _vAxis, _hAxis, _hRotAxis;

    private void Awake()
    {
        // Getting the references from the player handler component:
        handler = this.GetComponent<PlayerHandler>();
        modelTransform = handler.ModelTransform;
        rigidBody = handler.Rigidbody;
    }

    private void FixedUpdate()
    {
        // Obtaining axis values from unity input system:
        _vAxis = Input.GetAxis("Vertical");
        _hAxis = Input.GetAxis("Horizontal");
        _hRotAxis = Input.GetAxis("A_Horizontal");

        // Calculating total movement based on both vectors:
        Vector3 totalMovement = ((Vector3.forward * _vAxis) + (Vector3.right * _hAxis)) * speed * Time.fixedDeltaTime;

        // Calcuating rigidbody velocity based on both axis:
        rigidBody.velocity = totalMovement;

        // Calculating the transforms rotation based on the rotation axis:
        modelTransform.Rotate((modelTransform.up * _hRotAxis) * rotationSpeed * Time.fixedDeltaTime);
    }
}
