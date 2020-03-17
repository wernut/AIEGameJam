using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerManager playerManager;
    public int amountOfRounds = 3;
    public float roundTimeInSeconds = 90.0f;
    public float timeBetweenRounds = 5.0f;
    private float roundTimer = 0.0f;
    private float betweenRoundTimer = 0.0f;
    private bool isRoundPlaying = false;
    private bool gameOver = false;
    private bool inbetweenRounds = false;

    private void Start()
    {
        ResetTimers();
    }

    private void Update()
    {
        if (!gameOver && roundTimer > 0)
        {
            if (inbetweenRounds)
            {
                if (betweenRoundTimer > 0)
                    betweenRoundTimer -= Time.deltaTime;
                else
                {
                    inbetweenRounds = true;
                    betweenRoundTimer = timeBetweenRounds;
                }
            }
            else
                roundTimer -= Time.deltaTime;
        }
        else
        {
            --amountOfRounds;
            roundTimer = roundTimeInSeconds;
            inbetweenRounds = true;
        }
    }

    void ResetTimers()
    {
        roundTimer = roundTimeInSeconds;
        betweenRoundTimer = timeBetweenRounds;
    }
}
