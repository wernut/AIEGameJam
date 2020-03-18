/*=============================================================================
 * Game:        Monday Riot
 * Version:     Alpha
 * 
 * Class:       PlayerManager.cs
 * Purpose:     Managers all the players in the game scene.
 * 
 * Author:      Lachlan Wernert
 *===========================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class PlayerManager : MonoBehaviour
{
    public List<PlayerHandler> allPlayers = new List<PlayerHandler>();
    public List<Transform> spawnPoints = new List<Transform>();
    public List<Material> playerMaterials = new List<Material>();
    private PlayerInputInformation playerInputInfo;
    private List<PlayerHandler> activePlayers = new List<PlayerHandler>();

    [Header("Debug")]
    public bool debugMode = false;

    [Range(1, 4)]
    public int playerUsingKeyboard = 1;
    [Range(1, 4)]
    public int activePlayerCount = 4;
    public List<XboxController> xboxControllers = new List<XboxController>();


    private void Awake()
    {
        if (!debugMode)
        {
            playerInputInfo = GameObject.Find("PlayerInputInformation").GetComponent<PlayerInputInformation>();
            SetupPlayerControls();
        }
        else
            SetupViaDebugControls();

    }

    void SetupPlayerControls()
    {
        for (int i = 0; i < playerInputInfo.PlayerCount; i++)
        {
            activePlayers.Add(allPlayers[i]);
            activePlayers[i].gameObject.SetActive(true);
            activePlayers[i].ModelTransform.SetPositionAndRotation(spawnPoints[i].position, spawnPoints[i].rotation);
            activePlayers[i].ID = i + 1;
            activePlayers[i].heartUIObject.SetActive(true);

            if (!playerInputInfo.GetInputInfo(i).KBAM)
            {
                activePlayers[i].AssignedController = playerInputInfo.GetInputInfo(i).assignedController;
            }


        }
    }

    void SetupViaDebugControls()
    {
        for(int i = 0; i < activePlayerCount; ++i)
        {
            activePlayers.Add(allPlayers[i]);
            activePlayers[i].gameObject.SetActive(true);
            activePlayers[i].ModelTransform.SetPositionAndRotation(spawnPoints[i].position, spawnPoints[i].rotation);
            activePlayers[i].ID = i + 1;
            activePlayers[i].heartUIObject.SetActive(true);

            if (i != (playerUsingKeyboard - 1))
                activePlayers[i].AssignedController = xboxControllers[i];

        }
    }

    // Returns the players currently active:
    public List<PlayerHandler> ActivePlayers
    {
        get { return activePlayers; }
    }
}
