using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    [Header("Model References")]
    public Transform modelTransform;
    public Rigidbody modelRigidBody;

    public Transform ModelTransform
    {
        get { return modelTransform; }
    }

    public Rigidbody Rigidbody
    {
        get { return modelRigidBody; }
    }
}
