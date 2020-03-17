/*================================================================================
 * Game:        Monday Riot
 * Version:     Alpha
 * 
 * Class:       PTCAssigner.cs (Player To Controller Assigner)
 * Purpose:     To assign players to Xbox controllers, allowing 1 keyboard player.
 * 
 * Author:      Lachlan Wernert
 *===============================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class PTCAssigner : MonoBehaviour
{
    public PlayerInputInformation playerInputInfo;
    public PlayerInputLayout playerInputLayout;
    public string nameOfSceneToLoad;
    private int connectedControllers = 0;
    private List<XboxController> unassignedControllers = new List<XboxController>();
    private List<XboxController> assignedControllers = new List<XboxController>();

    void Start()
    {
        FindConnectedControllers();
    }

    void FindConnectedControllers()
    {
        // Getting the amound of controllers plugged in:
        connectedControllers = XCI.GetNumPluggedCtrlrs();

        // Printing the amount of connected controllers to the log:
        if (connectedControllers == 0)
            Debug.Log("No Xbox controllers plugged in!");
        else
        {
            Debug.Log(connectedControllers + " Xbox controllers plugged in.");
        }

        // Printing the controller names:
        XCI.DEBUG_LogControllerNames();

        // Adding connected controllers to the avaliable controllers list:
        for(int c = 1; c < connectedControllers + 1; ++c)
        {
            XboxController xboxController = ((XboxController)c);
            if (xboxController == XboxController.All)
                continue;
            unassignedControllers.Add(xboxController);
        }
    }

    void Update()
    {
        // Only allow 1 KBAM input:
        if (!playerInputInfo.KBAMActive)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                PlayerInputInformation.InputMode inputMode;
                inputMode.assignedController = (XboxController)(-1);
                inputMode.KBAM = true;
                playerInputInfo.AddInputInfo(inputMode);
                playerInputInfo.KBAMActive = true;
                playerInputInfo.PlayerCount++;
            }
        }

        // Detect input from all controllers:
        for (int j = 0; j < unassignedControllers.Count + 1; ++j)
        {
            // Storing the xbox controller at this index:
            XboxController xboxController = (XboxController)j;

            // Skipping if the controller is "All"
            if (xboxController == XboxController.All)
                continue;

            // Checking for controller input:
            if (XCI.GetButton(XboxButton.A, xboxController))
            {
                // Add controller to next avaliable slot:
                AddController(xboxController);
                return;
            }
        }

        if(playerInputInfo.PlayerCount > 1)
        {
            if(Input.GetKeyUp(KeyCode.Return) || XCI.GetButtonUp(XboxButton.Start, XboxController.All))
            {
                SceneManager.LoadScene(nameOfSceneToLoad);
            }
        }
    }

    void AddController(XboxController xboxController)
    {
        // Adding input info:
        PlayerInputInformation.InputMode inputMode;
        inputMode.assignedController = xboxController;
        inputMode.KBAM = false;
        playerInputInfo.AddInputInfo(inputMode);
        playerInputInfo.PlayerCount++;

        // Swapping lists:
        unassignedControllers.Remove(xboxController);
        assignedControllers.Add(xboxController);
    }
}
