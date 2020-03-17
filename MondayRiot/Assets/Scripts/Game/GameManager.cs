using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        START,
        PLAYING,
        BETWEEN_ROUNDS,
        END,
        COUNT
    }

    [Header("Script References")]
    public PlayerManager playerManager;
    public RestoreProps restoreProps;

    [Header("UI References")]
    public List<GameObject> gamestatePanels = new List<GameObject>();
    //public List<TextMeshProUGUI> timerText = new List<TextMeshProUGUI>();
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI timerHeader;
    public List<string> timerHeaderStrings = new List<string>(3);
    public Image timerFill;

    [Header("Attributes")]
    public int amountOfRounds = 3;
    public float timeBeforeGameStarts = 5.0f;
    public float roundTimeInSeconds = 90.0f;
    public float timeBetweenRounds = 5.0f;

    // Timers and state:
    private float beforeGameTimer = 0.0f;
    private float roundTimer = 0.0f;
    private float betweenRoundTimer = 0.0f;
    private GameState currentState;
    private bool hasReset = false;

    private void Start()
    {
        currentState = GameState.START;
        ResetTimers();
    }

    private void Update()
    {
        switch(currentState)
        {
            case GameState.START:
                if (beforeGameTimer > 0.0f)
                {
                    beforeGameTimer -= Time.deltaTime;
                    UpdatePanelTimer(GameState.START, beforeGameTimer);
                }
                else
                {
                    hasReset = false;
                    beforeGameTimer = timeBeforeGameStarts;
                    UpdateState(GameState.PLAYING);
                    SetAllPlayersControllable(true);
                }
                break;

            case GameState.PLAYING:
                if (roundTimer > 0.0f)
                {
                    roundTimer -= Time.deltaTime;
                    UpdatePanelTimer(GameState.PLAYING, roundTimer);
                }
                else
                {
                    roundTimer = roundTimeInSeconds;
                    UpdateState(GameState.BETWEEN_ROUNDS);
                    SetAllPlayersControllable(false);
                }
                break;

            case GameState.BETWEEN_ROUNDS:

                if(amountOfRounds == 0)
                {
                    UpdateState(GameState.END);
                    return;
                }

                if (betweenRoundTimer > 0)
                {
                    betweenRoundTimer -= Time.deltaTime;
                    UpdatePanelTimer(GameState.BETWEEN_ROUNDS, betweenRoundTimer);
                }
                else
                {
                    betweenRoundTimer = timeBetweenRounds;
                    --amountOfRounds;
                    AllPlayersDropObject();
                    restoreProps.RestoreAll();
                    RespawnAllPlayers();
                    SetAllPlayersControllable(true);
                    UpdateState(GameState.PLAYING);
                }
                break;

            case GameState.END:
                if(!hasReset)
                {
                    AllPlayersDropObject();
                    restoreProps.RestoreAll();
                    ResetTimers();
                    SetAllPlayersControllable(false);
                    hasReset = true;
                }
                break;
        }
    }

    void UpdatePanelTimer(GameState state, float time)
    {
        float maxtime =
            state == GameState.PLAYING ? roundTimeInSeconds :
            state == GameState.BETWEEN_ROUNDS ? timeBetweenRounds :
            state == GameState.START ? timeBeforeGameStarts :
            time;

        timerFill.fillAmount = time / maxtime;
        if (time < 10f)
        {
            timerText.fontSize = 40f;
            timerText.text = time.ToString("0.00");
        }
        else
        {
            timerText.fontSize = 60f;
            timerText.text = time.ToString("0");
        }

        timerHeader.text = timerHeaderStrings[(int)state];

        //gamestatePanels[(int)state].SetActive(true);

        //if (time > 60)
        //    timerText[(int)state].text = (time / 60).ToString("0.00") + " mins";
        //else
        //    timerText[(int)state].text = time.ToString("0.00") + " secs";
    }

    void UpdateState(GameState nextState)
    {
        //gamestatePanels[(int)currentState].SetActive(false);
        currentState = nextState;
        if (nextState == GameState.END)
        {
            gamestatePanels[(int)currentState].SetActive(true);
        }
    }

    void RespawnAllPlayers()
    {
        for (int i = 0; i < playerManager.players.Count; ++i)
        {
            playerManager.players[i].RespawnPlayer();
        }
    }

    void AllPlayersDropObject()
    {
        /* dupes item when round is over! doesnt get rid of it */

        for (int i = 0; i < playerManager.players.Count; ++i)
        {
            playerManager.players[i].ThrowScript.InstantDrop();
        }
    }

    void SetAllPlayersControllable(bool value)
    {
        for (int i = 0; i < playerManager.players.Count; ++i)
        {
            playerManager.players[i].IsControllable = value;
        }
    }

    void ResetTimers()
    {
        roundTimer = roundTimeInSeconds;
        betweenRoundTimer = timeBetweenRounds;
        beforeGameTimer = timeBeforeGameStarts;
    }

    public GameState CurrentState
    {
        get { return currentState;  }
        set { currentState = value; }
    }
}
