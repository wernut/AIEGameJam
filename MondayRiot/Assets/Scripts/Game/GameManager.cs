using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public TextMeshProUGUI winningText;
    public List<string> timerHeaderStrings = new List<string>(3);
    public Image timerFill;
    public GameObject clipboard;

    [Header("Attributes")]
    public int amountOfRounds = 3;
    public float timeBeforeGameStarts = 5.0f;
    public float roundTimeInSeconds = 90.0f;
    public float timeBetweenRounds = 5.0f;
    public float timeBeforeGameReloads = 5.0f;

    // Timers and state:
    private float beforeGameTimer = 0.0f;
    private float roundTimer = 0.0f;
    private float betweenRoundTimer = 0.0f;
    private float reloadGameTimer = 0.0f;
    private GameState currentState;
    private bool hasReset = false;
    private PlayerHandler playerRoundWinner, playerGameWinner;

    private void Start()
    {
        currentState = GameState.START;
        reloadGameTimer = timeBeforeGameReloads;
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

                    if(CheckForLastAlive())
                    {
                        roundTimer = roundTimeInSeconds;
                        UpdateState(GameState.BETWEEN_ROUNDS);
                        SetAllPlayersControllable(false);
                    }
                }
                else
                {
                    playerRoundWinner = CheckForWinnerViaDamageDealt();
                    if (playerRoundWinner != null)
                    {
                        winningText.color = Color.black;
                        winningText.text = "Player" + playerRoundWinner.ID + " won the round!";
                    }
                    else
                    {
                        winningText.color = Color.black;
                        winningText.text = "It's a tie!";
                    }
                    roundTimer = roundTimeInSeconds;
                    UpdateState(GameState.BETWEEN_ROUNDS);
                    SetAllPlayersControllable(false);
                }
                break;

            case GameState.BETWEEN_ROUNDS:
                clipboard.SetActive(true);
                if (amountOfRounds == 1)
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
                    clipboard.SetActive(false);
                    UpdateState(GameState.PLAYING);
                }
                break;

            case GameState.END:
                clipboard.SetActive(true);
                if(!hasReset)
                {
                    playerGameWinner = CheckForGameWinner();
                    if (playerGameWinner != null)
                    {
                        winningText.text = "Player" + playerGameWinner.ID + " won the game!";
                    }
                    else
                    {
                        winningText.text = "It's a tie!";
                    }
                    AllPlayersDropObject();
                    restoreProps.RestoreAll();
                    ResetTimers();
                    SetAllPlayersControllable(false);
                    hasReset = true;
                }

                if (timeBeforeGameReloads > 0)
                {
                    timeBeforeGameReloads -= Time.deltaTime;
                    UpdatePanelTimer(GameState.END, timeBeforeGameReloads);
                }
                else
                {
                    SceneManager.LoadScene("TitleScene");
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
            state == GameState.END ? timeBeforeGameReloads :
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
        if (currentState == GameState.END)
        {
            gamestatePanels[(int)currentState].SetActive(true);
        }
        if (currentState == GameState.PLAYING)
        {
            clipboard.gameObject.SetActive(false);
        }
    }

    void RespawnAllPlayers()
    {
        for (int i = 0; i < playerManager.ActivePlayers.Count; ++i)
        {
            playerManager.allPlayers[i].RespawnPlayer();
        }
    }

    void AllPlayersDropObject()
    {
        for (int i = 0; i < playerManager.ActivePlayers.Count; ++i)
        {
            if (playerManager.ActivePlayers[i] && playerManager.ActivePlayers[i].EquippedObject)
            {
                playerManager.ActivePlayers[i].ThrowScript.InstantDrop();
            }
        }
    }

    void SetAllPlayersControllable(bool value)
    {
        for (int i = 0; i < playerManager.allPlayers.Count; ++i)
        {
            playerManager.allPlayers[i].IsControllable = value;
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

    // Checks to see if anyone has won the round:
    bool CheckForLastAlive()
    {
        // Find how many players are alive:
        int alivePlayers = 0;
        for (int i = 0; i < playerManager.ActivePlayers.Count; ++i)
        {
            if(!playerManager.ActivePlayers[i].IsDead)
                ++alivePlayers;
        }

        if(alivePlayers == 1)
        {
            for (int i = 0; i < playerManager.ActivePlayers.Count; ++i)
            {
                if (!playerManager.ActivePlayers[i].IsDead)
                {
                    playerManager.ActivePlayers[i].AmountOfRoundsWon++;
                    winningText.text = "Player" + playerManager.ActivePlayers[i].ID + " won the round!";
                    return true;
                }
            }
        }

        return false;
    }

    PlayerHandler CheckForWinnerViaDamageDealt()
    {
        PlayerHandler winner = null;
        for (int i = 0; i < playerManager.ActivePlayers.Count; ++i)
        {
            for (int j = 0; j < playerManager.ActivePlayers.Count; ++j)
            {
                if (i == j)
                    continue;

                if (!playerManager.ActivePlayers[i].IsDead && !playerManager.ActivePlayers[j].IsDead)
                    if (playerManager.ActivePlayers[i].TotalDamageDealt > playerManager.ActivePlayers[j].TotalDamageDealt)
                    {
                        winner = playerManager.ActivePlayers[i];
                        winner.AmountOfRoundsWon++;
                    }
            }
        }

        return winner;
    }

    PlayerHandler CheckForGameWinner()
    {
        PlayerHandler winner = null;
        for (int i = 0; i < playerManager.ActivePlayers.Count; ++i)
        {
            for (int j = 0; j < playerManager.ActivePlayers.Count; ++j)
            {
                if (i == j)
                    continue;

                if (playerManager.ActivePlayers[i].AmountOfRoundsWon > playerManager.ActivePlayers[j].AmountOfRoundsWon)
                {
                    winner = playerManager.ActivePlayers[i];
                }
            }
        }

        return winner;
    }
}
