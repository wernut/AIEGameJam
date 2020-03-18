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
    public PlayerInputInformation playerInputInfoPrefab;
    public PlayerInputLayout playerInputLayout;
    public string nameOfSceneToLoad;
    public AudioSource join, start;
    public CanvasGroup fade;
    private int connectedControllers = 0;
    private List<XboxController> unassignedControllers = new List<XboxController>();
    private List<XboxController> assignedControllers = new List<XboxController>();
    private bool fading;
    private PlayerInputInformation existingPII;

    void Start()
    {
        PlayerInputInformation infoObject = FindObjectOfType<PlayerInputInformation>();
        if (infoObject != null)
            Destroy(infoObject.gameObject);

        existingPII = Instantiate(playerInputInfoPrefab);

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
        if (!existingPII.KBAMActive)
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                join.Play();
                PlayerInputInformation.InputMode inputMode;
                inputMode.assignedController = (XboxController)(-1);
                inputMode.KBAM = true;
                existingPII.AddInputInfo(inputMode);
                existingPII.KBAMActive = true;
                existingPII.PlayerCount++;
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
                join.Play();
                // Add controller to next avaliable slot:
                AddController(xboxController);
                return;
            }
        }

        if(existingPII.PlayerCount > 1)
        {
            StartCoroutine(CheckForStart());
            //if(!fading && Input.GetKeyUp(KeyCode.Return) || XCI.GetButtonUp(XboxButton.Start, XboxController.All))
            //{
            //    fading = true;
            //    StartCoroutine(PlayStart(start, start.clip.length));
            //}
        }
    }

    void AddController(XboxController xboxController)
    {
        // Adding input info:
        PlayerInputInformation.InputMode inputMode;
        inputMode.assignedController = xboxController;
        inputMode.KBAM = false;
        existingPII.AddInputInfo(inputMode);
        existingPII.PlayerCount++;

        // Swapping lists:
        unassignedControllers.Remove(xboxController);
        assignedControllers.Add(xboxController);
    }

    IEnumerator CheckForStart()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        if (!fading && Input.GetKeyUp(KeyCode.Return) || XCI.GetButtonUp(XboxButton.Start, XboxController.All))
        {
            fading = true;
            StartCoroutine(PlayStart(start, start.clip.length));
        }
    }

    public IEnumerator PlayStart(AudioSource a, float fadetime)
    {
        a.Play();
        // For keeping track of the fade
        float timeAtStart = Time.time;
        float timeSinceStart;
        float percentageComplete = 0;

        while (percentageComplete < 1) // Keeps looping until the lerp is complete
        {
            timeSinceStart = Time.time - timeAtStart;
            percentageComplete = timeSinceStart / fadetime;

            float currentValue = Mathf.Lerp(0, 1, percentageComplete);

            fade.alpha = currentValue;
            yield return new WaitForEndOfFrame();
        }
        SceneManager.LoadScene(nameOfSceneToLoad);
    }

    public PlayerInputInformation GetExistingPII()
    {
        return existingPII;
    }
}
