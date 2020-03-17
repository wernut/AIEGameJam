using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class PlayerThrow : MonoBehaviour
{
    private PlayerHandler handler;

    // Object in hand variables:
    private Animator rightHandAnimator, bothHandsAnimator;
    private Transform modelTransform;
    private LayerMask objectLayer;
    private bool justThrewObject = false;
    private float animationSpeed = 0.0f;

    [Header("Attributes")]
    public float forwardForce = 10.0f;
    public float upForce = 5.0f;

    [Header("Transforms and Offsets")]
    public Transform bothHandTransform;
    public Transform rightHandTransform;
    public Vector3 rightHandPositionOffset;

    [Header("Timers")]
    public float coolDownTime = 2.0f; 
    public float waitInRealTimeDivider = 6.0f; // How long it takes for the object to be thrown. Mass / waitInRealTimeDivider

    [Header("Sphere Cast Attributes")]
    public float sphereCastRadius = 5.0f;
    public float sphereCastMaxDistance = 5.0f;
    public LayerMask layer;
    public Vector3 offset;

    private void Awake()
    {
        handler = this.GetComponent<PlayerHandler>();
        rightHandAnimator = handler.RightHandAnimator;
        bothHandsAnimator = handler.BothHandsAnimator;
        modelTransform = handler.ModelTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if(handler.HasAssignedController())
        {
            // Xbox controller input:
            if(XCI.GetButtonUp(handler.interactObjectButton, handler.AssignedController))
            {
                PickupOrThrow();
            }
            else if (XCI.GetButtonUp(handler.dropObjectButton, handler.AssignedController))
            {
                Drop();
            }
        }
        else if(Input.GetKeyUp(handler.interactObjectKey) && !justThrewObject)
        {
            PickupOrThrow();
        }
        // Dropping the object in hand:
        else if(Input.GetKeyUp(handler.dropObjectKey) && handler.EquippedObject != null)
        {
            Drop();
        }
    }

    void PickupOrThrow()
    {
        // Picking up an object if hand is empty:
        if (handler.EquippedObject == null)
        {
            // Find objects within a certain radius:
            RaycastHit[] objects = Physics.SphereCastAll(modelTransform.position + offset, sphereCastRadius, modelTransform.forward, sphereCastMaxDistance, layer);

            // Finding the first object with a rigid body and picking it up:
            for (int i = 0; i < objects.Length; ++i)
            {
                // If the object is the model, continue:
                if (objects[i].transform.gameObject == modelTransform.gameObject)
                    continue;

                // Picking up the first found equippable object:
                handler.EquippedObject = objects[i].transform.gameObject.GetComponent<EquippableObject>();
                if (handler.EquippedObject != null)
                {
                    if (handler.EquippedObject.useBothHands)
                        PickUpObject(handler.EquippedObject, true);
                    else
                        PickUpObject(handler.EquippedObject, false);
                    break;
                }
            }
        }
        // Throwing object in hand:
        else
        {
            if (handler.EquippedObject.useBothHands)
                StartCoroutine(ThrowSequence(0.0f, true));
            else
                StartCoroutine(ThrowSequence(0.0f, false));
        }
    }

    void Drop()
    {
        if (handler.EquippedObject != null && handler.EquippedObject.useBothHands)
            bothHandsAnimator.SetTrigger("Drop");

        StartCoroutine(DropObject());
    }

    void PickUpObject(EquippableObject pickUp, bool bothHands)
    {
        // Setup references:
        objectLayer = pickUp.transform.gameObject.layer;
        pickUp.transform.gameObject.layer = LayerMask.NameToLayer("Player" + handler.ID + "Weapon");
        pickUp.Rigidbody.isKinematic = true;
        handler.EquippedObject = pickUp;

        // Get the animation speed via the object's mass:
        if (bothHands)
        {
            float x = handler.EquippedObject.Rigidbody.mass / (handler.EquippedObject.Rigidbody.mass + 4.0f);
            float y = handler.EquippedObject.Rigidbody.mass / (handler.EquippedObject.Rigidbody.mass + 6.0f);
            animationSpeed = (x - y) * 5.0f;

            handler.CurrentSpeed = handler.defaultMovementSpeed / (handler.EquippedObject.Rigidbody.mass * 0.15f);
        }
        else
            animationSpeed = 0.1f;

        // Set position and parent:
        if(bothHands)
        {
            handler.EquippedObject.transform.SetParent(bothHandTransform);
            handler.EquippedObject.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            handler.EquippedObject.transform.localPosition = Vector3.zero + handler.EquippedObject.bothHandOffset;
            bothHandsAnimator.SetFloat("PickupSpeed", animationSpeed);
            bothHandsAnimator.SetTrigger("PickUp");
        }
        else
        {
            handler.EquippedObject.transform.SetParent(rightHandTransform);
            handler.EquippedObject.transform.localPosition = Vector3.zero + rightHandPositionOffset;
            rightHandAnimator.SetTrigger("Swing");
        }
    }

    IEnumerator ThrowSequence(float padding, bool bothHands)
    {
        if (bothHands)
        {
            bothHandsAnimator.SetFloat("ThrowSpeed", 1.0f);
            bothHandsAnimator.SetTrigger("Throw");
            yield return new WaitForSecondsRealtime(0.6f);
            StartCoroutine(ThrowObject());
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.1f + padding);
            rightHandAnimator.SetTrigger("Swing");
            StartCoroutine(ThrowObject());
        }
    }

    IEnumerator ThrowObject()
    {
        if (handler.EquippedObject)
        {
            // Deattach
            handler.EquippedObject.Rigidbody.isKinematic = false;
            handler.EquippedObject.transform.SetParent(null);

            // Throw
            handler.EquippedObject.Rigidbody.AddForce(((modelTransform.forward * forwardForce) + (Vector3.up * upForce)) * handler.EquippedObject.Rigidbody.mass, ForceMode.Impulse);
            handler.EquippedObject.Rigidbody.AddTorque(handler.EquippedObject.transform.right * 5000.0f);

            handler.CurrentSpeed = handler.defaultMovementSpeed;

            // Wait before turning the collider on
            yield return new WaitForSecondsRealtime(0.1f);

            // Calling the thrown function in the object class:
            handler.EquippedObject.Thrown(handler, objectLayer);

            // Deattach completely
            handler.EquippedObject = null;

            // Start cooldown
            justThrewObject = true;
            StartCoroutine(ThrowCooldown());
        }
    }

    IEnumerator DropObject()
    {
        if (handler.EquippedObject)
        {
            // Deattach
            handler.EquippedObject.Rigidbody.isKinematic = false;
            handler.EquippedObject.transform.SetParent(null);

            // Wait before turning the collider on
            yield return new WaitForSecondsRealtime(0.1f);

            // Enable collision with players
            handler.EquippedObject.transform.gameObject.layer = objectLayer;

            // Deattach completely
            handler.EquippedObject = null;
            handler.CurrentSpeed = handler.defaultMovementSpeed;
        }
    }

    IEnumerator ThrowCooldown()
    {
        yield return new WaitForSecondsRealtime(coolDownTime);
        justThrewObject = false;
    }
}
