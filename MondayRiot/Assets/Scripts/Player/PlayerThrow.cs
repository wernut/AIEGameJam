using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrow : MonoBehaviour
{
    private PlayerHandler _handler;

    // Object in hand variables:
    private LayerMask _objectLayer;
    private bool _justThrewObject = false;

    [Header("Attributes")]
    public KeyCode interactKey;
    public KeyCode cancelKey;
    public Transform handTransform;
    public Vector3 handPositionOffset;
    public float forwardForce = 10.0f;
    public float upForce = 5.0f;
    public float coolDownTime = 2.0f; 
    public float waitInRealTimeDivider = 6.0f; // How long it takes for the object to be thrown. Mass / waitInRealTimeDivider

    [Header("Sphere Cast Attributes")]
    public float sphereCastRadius = 5.0f;
    public float sphereCastMaxDistance = 5.0f;
    public LayerMask layer;
    public Vector3 offset;

    private void Awake()
    {
        _handler = this.GetComponent<PlayerHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(interactKey) && !_justThrewObject)
        {
            // Picking up an object if hand is empty:
            if (_handler.ObjectInHand == null)
            {
                // Find objects within a certain radius:
                RaycastHit[] objects = Physics.SphereCastAll(_handler.ModelTransform.position + offset, sphereCastRadius, _handler.ModelTransform.forward, sphereCastMaxDistance, layer);

                // Finding the first object with a rigid body and picking it up:
                for (int i = 0; i < objects.Length; ++i)
                {
                    // If the object is the model, continue:
                    if (objects[i].transform.gameObject == _handler.ModelTransform.gameObject)
                        continue;

                    _handler.RBOfObjectInHand = objects[i].transform.gameObject.GetComponent<Rigidbody>();
                    if (_handler.RBOfObjectInHand != null)
                    {
                        PickUpObject(objects[i].transform.gameObject);
                        break;
                    }
                }
            }
            // Throwing object in hand:
            else
            {
                StartCoroutine(ThrowSequence());
            }
        }

        // Dropping the object in hand:
        if(Input.GetKeyUp(cancelKey) && _handler.ObjectInHand != null)
        {
            StartCoroutine(DropObject());
        }
    }

    void PickUpObject(GameObject pickUp)
    {
        // Setup references:
        _handler.ObjectInHand = pickUp.transform.gameObject;
        _objectLayer = _handler.ObjectInHand.layer;
        _handler.ObjectInHand.layer = LayerMask.NameToLayer("InHand");
        _handler.RBOfObjectInHand.isKinematic = true;

        // Set position and parent:
        _handler.ObjectInHand.transform.position = handTransform.position + handPositionOffset;
        _handler.ObjectInHand.transform.SetParent(handTransform);

    }

    IEnumerator ThrowSequence()
    {
        yield return new WaitForSecondsRealtime(_handler.RBOfObjectInHand.mass / waitInRealTimeDivider);

        StartCoroutine(ThrowObject());
    }

    IEnumerator ThrowObject()
    {
        if (_handler.RBOfObjectInHand)
        {
            // Deattach
            _handler.RBOfObjectInHand.isKinematic = false;
            _handler.ObjectInHand.transform.SetParent(null);

            // Throw
            _handler.RBOfObjectInHand.AddForce(((_handler.ModelTransform.forward * forwardForce) + (Vector3.up * upForce)) * _handler.RBOfObjectInHand.mass, ForceMode.Impulse);
            _handler.HandAnimator.SetTrigger("Swing");

            // Wait before turning the collider on
            yield return new WaitForSecondsRealtime(0.1f);

            // Enable collision with players
            _handler.ObjectInHand.layer = _objectLayer;

            // Deattach completely
            _handler.ObjectInHand = null;
            _handler.RBOfObjectInHand = null;

            // Start cooldown
            _justThrewObject = true;
            StartCoroutine(ThrowCooldown());
        }
    }

    IEnumerator DropObject()
    {
        if (_handler.ObjectInHand)
        {
            // Deattach
            _handler.RBOfObjectInHand.isKinematic = false;
            _handler.ObjectInHand.transform.SetParent(null);

            // Wait before turning the collider on
            yield return new WaitForSecondsRealtime(0.1f);

            // Enable collision with players
            _handler.ObjectInHand.layer = _objectLayer;

            // Deattach completely
            _handler.ObjectInHand = null;
            _handler.RBOfObjectInHand = null;
        }
    }

    IEnumerator ThrowCooldown()
    {
        yield return new WaitForSecondsRealtime(coolDownTime);
        _justThrewObject = false;
    }
}
