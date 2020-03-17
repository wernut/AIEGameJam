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
    public List<PlayerHandler> players = new List<PlayerHandler>();
    public List<Transform> spawnPoints = new List<Transform>();
    public List<Material> playerMaterials = new List<Material>();
    private PlayerInputInformation playerInputInfo;

    [Header("Debug")]
    public bool debugMode = false;

    public List<bool> shouldPlayerUseKBAM = new List<bool>();
    public List<bool> isPlayerActive = new List<bool>();
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
            players[i].gameObject.SetActive(true);
            players[i].ModelTransform.SetPositionAndRotation(spawnPoints[i].position, spawnPoints[i].rotation);
            players[i].ID = i + 1;

            if (!playerInputInfo.GetInputInfo(i).KBAM)
            {
                players[i].AssignedController = playerInputInfo.GetInputInfo(i).assignedController;
            }

        }
    }

    void SetupViaDebugControls()
    {
        for(int i = 0; i < 4; ++i)
        {
            players[i].gameObject.SetActive(true);
            players[i].ModelTransform.SetPositionAndRotation(spawnPoints[i].position, spawnPoints[i].rotation);
            players[i].ID = i + 1;

            if (!isPlayerActive[i])
                players[i].IsDead = true;

            if (!shouldPlayerUseKBAM[i])
                players[i].AssignedController = xboxControllers[i];
        }
    }
}
