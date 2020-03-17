using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class PlayerInputInformation : MonoBehaviour
{
    public struct InputMode
    {
        public bool KBAM;
        public XboxController assignedController;
    };

    private List<InputMode> playerInputModes = new List<InputMode>();
    private int playerCount = 0;
    private bool kbamActive = false;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void AddInputInfo(InputMode inputMode)
    {
        playerInputModes.Add(inputMode);
    }

    public InputMode GetInputInfo(int index)
    {
        return playerInputModes[index];
    }

    public int PlayerCount 
    {
        get { return playerCount;  }
        set { playerCount = value; }
    }

    public bool KBAMActive 
    {
        get { return kbamActive;  }
        set { kbamActive = value; }
    }
}
