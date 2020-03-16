using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public KeyCode interactKey;
    public KeyCode cancelKey;
    public float forwardForce = 10.0f;
    public float upForce = 5.0f;

    [Header("Transforms and Offsets")]
    public Transform bothHandTransform;
    public Vector3 bothHandPositionOffset;
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
        if(Input.GetKeyUp(interactKey) && !justThrewObject)
        {
            // Picking up an object if hand is empty:
            if (handler.ObjectInHand == null)
            {
                // Find objects within a certain radius:
                RaycastHit[] objects = Physics.SphereCastAll(modelTransform.position + offset, sphereCastRadius, modelTransform.forward, sphereCastMaxDistance, layer);

                // Finding the first object with a rigid body and picking it up:
                for (int i = 0; i < objects.Length; ++i)
                {
                    // If the object is the model, continue:
                    if (objects[i].transform.gameObject == modelTransform.gameObject)
                        continue;

                    handler.RBOfObjectInHand = objects[i].transform.gameObject.GetComponent<Rigidbody>();
                    if (handler.RBOfObjectInHand != null)
                    {
                        if(handler.RBOfObjectInHand.mass > 5)
                            PickUpObject(objects[i].transform.gameObject, true);
                        else
                            PickUpObject(objects[i].transform.gameObject, false);
                        break;
                    }
                }
            }
            // Throwing object in hand:
            else
            {
                if (handler.RBOfObjectInHand.mass > 5)
                    StartCoroutine(ThrowSequence(-0.30f, true));
                else
                    StartCoroutine(ThrowSequence(0.0f, false));
            }
        }

        // Dropping the object in hand:
        if(Input.GetKeyUp(cancelKey) && handler.ObjectInHand != null)
        {
            if(handler.RBOfObjectInHand.mass > 5)
            {
                bothHandsAnimator.SetFloat("PullbackSpeed", animationSpeed);
                bothHandsAnimator.SetTrigger("Pullback");
                bothHandsAnimator.SetFloat("ThrowSpeed", animationSpeed * 7.0f);
                bothHandsAnimator.SetTrigger("Throw");
            }
            
            StartCoroutine(DropObject());
        }
    }

    void PickUpObject(GameObject pickUp, bool bothHands)
    {
        // Setup references:
        handler.ObjectInHand = pickUp.transform.gameObject;
        objectLayer = handler.ObjectInHand.layer;
        handler.ObjectInHand.layer = LayerMask.NameToLayer("InHand");
        handler.RBOfObjectInHand.isKinematic = true;

        if (bothHands)
        {
            float x = handler.RBOfObjectInHand.mass / (handler.RBOfObjectInHand.mass + 4.0f);
            float y = handler.RBOfObjectInHand.mass / (handler.RBOfObjectInHand.mass + 6.0f);
            animationSpeed = (x - y) * 5.0f;

        }
        else
            animationSpeed = 0.1f;

        Debug.Log(animationSpeed);

        // Set position and parent:
        if(bothHands)
        {
            handler.ObjectInHand.transform.SetParent(bothHandTransform);
            handler.ObjectInHand.transform.localPosition = Vector3.zero + bothHandPositionOffset;
            bothHandsAnimator.SetFloat("PickupSpeed", animationSpeed);
            bothHandsAnimator.SetTrigger("PickUp");
        }
        else
        {
            handler.ObjectInHand.transform.SetParent(rightHandTransform);
            handler.ObjectInHand.transform.localPosition = Vector3.zero + rightHandPositionOffset;
            rightHandAnimator.SetTrigger("Swing");
        }
    }

    IEnumerator ThrowSequence(float padding, bool bothHands)
    {
        if (bothHands)
        {
            bothHandsAnimator.SetFloat("PullbackSpeed", animationSpeed);
            bothHandsAnimator.SetTrigger("Pullback");
            bothHandsAnimator.SetFloat("ThrowSpeed", animationSpeed * 7.0f);
            bothHandsAnimator.SetTrigger("Throw");
            yield return new WaitForSeconds(animationSpeed + padding);
            StartCoroutine(ThrowObject());
        }
        else
        {
            yield return new WaitForSeconds(0.1f + padding);
            rightHandAnimator.SetTrigger("Swing");
            StartCoroutine(ThrowObject());
        }
    }

    IEnumerator ThrowObject()
    {
        if (handler.RBOfObjectInHand)
        {
            // Deattach
            handler.RBOfObjectInHand.isKinematic = false;
            handler.ObjectInHand.transform.SetParent(null);

            // Throw
            handler.RBOfObjectInHand.AddForce(((modelTransform.forward * forwardForce) + (Vector3.up * upForce)) * handler.RBOfObjectInHand.mass, ForceMode.Impulse);

            // Wait before turning the collider on
            yield return new WaitForSecondsRealtime(0.1f);

            // Enable collision with players
            handler.ObjectInHand.layer = objectLayer;

            // Deattach completely
            handler.ObjectInHand = null;
            handler.RBOfObjectInHand = null;

            // Start cooldown
            justThrewObject = true;
            StartCoroutine(ThrowCooldown());
        }
    }

    IEnumerator DropObject()
    {
        if (handler.ObjectInHand)
        {
            // Deattach
            handler.RBOfObjectInHand.isKinematic = false;
            handler.ObjectInHand.transform.SetParent(null);

            // Wait before turning the collider on
            yield return new WaitForSecondsRealtime(0.1f);

            // Enable collision with players
            handler.ObjectInHand.layer = objectLayer;

            // Deattach completely
            handler.ObjectInHand = null;
            handler.RBOfObjectInHand = null;
        }
    }

    IEnumerator ThrowCooldown()
    {
        yield return new WaitForSecondsRealtime(coolDownTime);
        justThrewObject = false;
    }
}
