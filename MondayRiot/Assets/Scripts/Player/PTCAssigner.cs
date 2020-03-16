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
using XboxCtrlrInput;

public class PTCAssigner : MonoBehaviour
{
    private int _connectedControllers = 0;
    private bool _controllerFound = false;
    private List<XboxController> _availableControllers = new List<XboxController>();

    void Start()
    {
        FindConnectedControllers();
    }

    void FindConnectedControllers()
    {
        // Getting the amound of controllers plugged in:
        _connectedControllers = XCI.GetNumPluggedCtrlrs();

        // Printing the amount of connected controllers to the log:
        if (_connectedControllers == 0)
            Debug.Log("No Xbox controllers plugged in!");
        else
        {
            Debug.Log(_connectedControllers + " Xbox controllers plugged in.");
            _controllerFound = true;
        }

        // Printing the controller names:
        XCI.DEBUG_LogControllerNames();

        // Adding connected controllers to the avaliable controllers list:
        for(int c = 1; c < _connectedControllers + 1; ++c)
        {
            XboxController xboxController = ((XboxController)c);
            if (xboxController == XboxController.All)
                continue;
            _availableControllers.Add(xboxController);
        }
    }
}
