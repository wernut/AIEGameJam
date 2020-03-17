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

    [Header("Debug Options")]
    public bool debugMode = false;
    public bool p1KeyboardAndMouse = true;
    public XboxController p1Controller;
    public bool p2KeyboardAndMouse = true;
    public XboxController p2Controller;

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

            if (!playerInputInfo.GetInputInfo(i).KBAM)
            {
                players[i].AssignedController = playerInputInfo.GetInputInfo(i).assignedController;
            }

            players[i].ID = i + 1;
        }
    }

    void SetupViaDebugControls()
    {
        players[0].gameObject.SetActive(true);
        players[0].ModelTransform.SetPositionAndRotation(spawnPoints[0].position, spawnPoints[0].rotation);
        if (!p1KeyboardAndMouse)
            players[0].AssignedController = p1Controller;

        players[1].gameObject.SetActive(true);
        players[1].ModelTransform.SetPositionAndRotation(spawnPoints[1].position, spawnPoints[1].rotation);
        if(!p2KeyboardAndMouse)
            players[1].AssignedController = p2Controller;

    }
}
