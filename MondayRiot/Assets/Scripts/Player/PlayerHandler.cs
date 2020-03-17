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
using UnityEngine.UI;
using TMPro;
using XboxCtrlrInput;

public class PlayerHandler : MonoBehaviour
{
    [Header("Attributes")]
    public float totalHealth = 100.0f;
    public float defaultMovementSpeed = 200.0f;
    public float rotationSpeed = 300.0f;
    public Transform spawnPoint;

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

    [Header("UI References")]
    public Image heartFillImage;
    public TextMeshProUGUI heartText;

    private bool KBAM = false;
    private XboxController assignedController = (XboxController)(-1);
    private bool isControllable = false;
    private int id = 0;
    private float currentHealth = 0.0f;
    private float currentMovementSpeed = 0.0f;
    private bool isDead = false;
    private InteractableObject equippedObject;
    private float totalDamageDealt = 0.0f;
    private PlayerThrow throwScript;

    private void Awake()
    {
        throwScript = this.GetComponent<PlayerThrow>();
        currentMovementSpeed = defaultMovementSpeed;
        currentHealth = totalHealth;
        UpdateHeartUI();
    }

    private void Update()
    {
        // Exiting the function if the player isn't controllable:
        if (!isControllable)
            return;

        // Check if alive:
        if (currentHealth <= 0)
        {
            isDead = true;
            throwScript.InstantDrop();
            ModelTransform.gameObject.SetActive(false);
            currentHealth = totalHealth;
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
    public InteractableObject EquippedObject
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

    // Returns and sets if the player is controllable:
    public bool IsControllable
    {
        get { return isControllable; }
        set { isControllable = value; }
    }

    // Returns and sets if the player is dead:
    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    // Returns and sets how much damage the player has dealt:
    public float TotalDamageDealt
    {
        get { return totalDamageDealt; }
        set { totalDamageDealt = value; }
    }

    // Returns and sets the spawn point of the player:
    public Transform SpawnPoint
    {
        get { return spawnPoint;  }
    }

    // Returns the player throw script attached to the player:
    public PlayerThrow ThrowScript
    {
        get { return throwScript; }
    }

    // Makes the player take damage:
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHeartUI();
    }

    // Updates the heart UI for the player:
    public void UpdateHeartUI()
    {
        heartText.text = currentHealth.ToString();
        heartFillImage.fillAmount = currentHealth / totalHealth;
    }

    // Respawns the player:
    public void RespawnPlayer()
    {
        currentHealth = totalHealth;
        UpdateHeartUI();
        ModelTransform.gameObject.SetActive(true);
        modelTransform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        isDead = false;
    }
}
