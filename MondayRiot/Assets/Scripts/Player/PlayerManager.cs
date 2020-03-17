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

    public bool p2KeyboardAndMouse = false;
    public XboxController p2Controller;

    public bool p3KeyboardAndMouse = false;
    public XboxController p3Controller;

    public bool p4KeyboardAndMouse = false;
    public XboxController p4Controller;

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

            switch (i)
            {
                case 0:
                    if (!p1KeyboardAndMouse)
                        players[i].AssignedController = p1Controller;
                    break;

                case 1:
                    if (!p2KeyboardAndMouse)
                        players[i].AssignedController = p2Controller;
                    break;

                case 2:
                    if (!p3KeyboardAndMouse)
                        players[i].AssignedController = p3Controller;
                    break;

                case 3:
                    if (!p4KeyboardAndMouse)
                        players[i].AssignedController = p4Controller;
                    break;
            }
        }
    }
}
