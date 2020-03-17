﻿/*=============================================================================
 * Game:        Monday Riot
 * Version:     Alpha
 * 
 * Class:       PlayerHandler.cs
 * Purpose:     To hold all references and attributes regarding the player.
 * 
 * Author:      Lachlan Wernert
 *===========================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class PlayerHandler : MonoBehaviour
{
    [Header("Attributes")]
    public float totalHealth = 10.0f;
    private float currentHealth = 10.0f;
    public float defaultMovementSpeed = 200.0f;
    private float currentMovementSpeed = 200.0f;
    public float rotationSpeed = 300.0f;
    private bool isDead = false;

    [Header("Keybinds")]
    public KeyCode interactObjectKey;
    public KeyCode dropObjectKey;
    public KeyCode meleeAttackKey;

    [Header("Controller Binds")]
    public XboxButton interactObjectButton;
    public XboxButton dropObjectButton;
    public XboxButton meleeAttackButton;

    [Header("Model References")]
    [SerializeField]
    private Transform modelTransform;
    [SerializeField]
    private Rigidbody modelRigidBody;
    [SerializeField]
    private Animator rightHandAnimator;
    [SerializeField]
    private Animator bothHandAnimator;
    [SerializeField]
    private Material modelMaterial;
    [SerializeField]
    private GameObject arrow;

    private bool KBAM = false;
    private int id = 0;
    private XboxController assignedController = (XboxController)(-1);
    private EquippableObject equippedObject;

    private void Update()
    {
        // Check if alive:
        if(currentHealth <= 0)
        {
            ModelTransform.gameObject.SetActive(false);
            currentHealth = totalHealth;
            isDead = true;
        }

        // Check if arrow should show:
        if (equippedObject != null && equippedObject.useBothHands)
        {
            arrow.SetActive(true);
        }
        else
        {
            arrow.SetActive(false);
        }
    }

    // Returns the transform of the player model.
    public Transform ModelTransform
    {
        get { return modelTransform; }
    }

    // Returns the rigid body of the player model.
    public Rigidbody Rigidbody
    {
        get { return modelRigidBody; }
    }

    // Returns and sets the Xbox Controller assigned to the player.
    public XboxController AssignedController
    {
        get { return assignedController; }
        set { assignedController = value; }
    }

    // Returns true if the handler has a controller assigned to it.
    public bool HasAssignedController()
    {
        return assignedController > 0;
    }

    // Returns and sets the equippable object the player is currently holding.
    public EquippableObject EquippedObject
    {
        get { return equippedObject; }
        set { equippedObject = value; }
    }

    // Returns the right hand animator:
    public Animator RightHandAnimator
    {
        get { return rightHandAnimator; }
    }

    // Returns the animator of both hands:
    public Animator BothHandsAnimator
    {
        get { return bothHandAnimator; }
    }

    // Returns and sets if the player is using the keyboard:
    public bool KeyboardAndMouse
    {
        get { return KBAM;  }
        set { KBAM = value; }
    }

    // Returns the material currently bound to the player:
    public Material ModelMaterial
    {
        get { return modelMaterial;  }
    }

    // Returns and sets the ID of the player.
    public int ID
    {
        get { return id;  }
        set { id = value; }
    }

    // Returns and sets the current speed multiplier of the player:
    public float CurrentSpeed
    {
        get { return currentMovementSpeed;  }
        set { currentMovementSpeed = value; }
    }

    // Makes the player take damage:
    public void TakeDamage(float damage)
    {
        this.currentHealth -= damage;
    }
}
