/*=============================================================================
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

    [Header("Model References")]
    [SerializeField]
    private Transform _modelTransform;
    [SerializeField]
    private Rigidbody _modelRigidBody;
    [SerializeField]
    private Animator _handAnimator;

    private XboxController assignedController = (XboxController)(-1);

    private GameObject _gameObjectInHand;
    private Rigidbody _rbOfGameObjectInHand;

    // Returns the transform of the player model.
    public Transform ModelTransform
    {
        get { return _modelTransform; }
    }

    // Returns the rigid body of the player model.
    public Rigidbody Rigidbody
    {
        get { return _modelRigidBody; }
    }

    // Returns the Xbox Controller assigned to the player.
    public XboxController AssignedController
    {
        get { return assignedController;  }
        set { assignedController = value; }
    }

    // Returns true if the handler has a controller assigned to it.
    public bool HasAssignedController()
    {
        return (assignedController > 0);
    }

    // Returns the game object the player is currently holding.
    public GameObject ObjectInHand
    {
        get { return _gameObjectInHand; }
        set { _gameObjectInHand = value; }
    }

    // Returns the rigid body of the game object the player is currently holding.
    public Rigidbody RBOfObjectInHand
    {
        get { return _rbOfGameObjectInHand; }
        set { _rbOfGameObjectInHand = value; }
    }

    // Returns the hand animator:
    public Animator HandAnimator
    {
        get { return _handAnimator; }
    }
}
