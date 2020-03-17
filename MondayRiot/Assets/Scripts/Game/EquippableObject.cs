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
    private int currentDurablility = 0;

    // Handler of the player who through the object.
    private PlayerHandler handler;
    private LayerMask defaultLayer;

    private void Awake()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        currentDurablility = durablility;
    }

    public void Setup(PlayerHandler handler, LayerMask defaultLayer)
    {
        this.handler = handler;
        this.defaultLayer = defaultLayer;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(wasJustThrown || wasJustSwung)
        {
            PlayerHandler victim = collision.gameObject.GetComponentInParent<PlayerHandler>();
            if (victim != null)
            {
                victim.TakeDamage(damage);
                handler.TotalDamageDealt += damage;
                Debug.Log("Player" + handler.ID + " just dealt " + damage + " damage to Player" + victim.ID);
            }

            if(wasJustThrown)
            {
                currentDurablility -= 1;
                if(currentDurablility <= 0)
                {
                    this.gameObject.SetActive(false);
                    currentDurablility = durablility;
                }
                this.transform.gameObject.layer = defaultLayer;
                wasJustThrown = false;
            }
            wasJustSwung = false;
        }
    }

    public Rigidbody Rigidbody
    {
        get { return rigidbody; }
    }
}
