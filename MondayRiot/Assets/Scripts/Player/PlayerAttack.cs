using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerHandler _handler;

    [Header("Attributes")]
    public KeyCode attackKey;

    private void Awake()
    {
        _handler = this.GetComponent<PlayerHandler>();
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(attackKey))
       {
            _handler.RightHandAnimator.SetTrigger("Swing");
       }
    }
}
