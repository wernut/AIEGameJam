using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippableObject : MonoBehaviour
{
    private Rigidbody rigidbody;

    [Header("Attributes")]
    public float damage = 10.0f;
    public int durablility = 5;
    public bool useBothHands = false;
    public Vector3 bothHandOffset;
    public bool wasJustThrown;
    public bool wasJustSwung;

    // Handler of the player who through the object.
    private PlayerHandler handler;
    private LayerMask defaultLayer;

    private void Awake()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    public Rigidbody Rigidbody
    {
        get { return rigidbody; }
    }

    public void Thrown(PlayerHandler handler, LayerMask defaultLayer)
    {
        this.handler = handler;
        this.defaultLayer = defaultLayer;
        wasJustThrown = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(wasJustThrown || wasJustSwung)
        {
            PlayerHandler victim = collision.gameObject.GetComponentInParent<PlayerHandler>();
            if (victim != null)
            {
                victim.TakeDamage(damage);
                Debug.Log(victim.ID + " - " + damage);
            }
            if(wasJustThrown)
            {
                this.transform.gameObject.layer = defaultLayer;
                wasJustThrown = false;
            }
            wasJustSwung = false;
        }
    }
}
