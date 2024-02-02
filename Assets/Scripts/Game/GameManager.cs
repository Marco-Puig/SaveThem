using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    #region Initialize
    public static GameManager Instance { get; private set; }
    Countdown countdown;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) { Instance = this; }
    }

    void Start()
    {
        // setup countdown for lobby wait and rounds
        if (PhotonNetwork.IsMasterClient)
        {
            countdown = this.gameObject.GetComponent<Countdown>();
            LobbyWait();
        }
    }

    #endregion

    # region Private Methods
    void LobbyWait()
    {
        // wait for players to join for 60 seconds
        countdown.Setup(WaitForRound, 60.0f, "Game starts in");
    }
    void StartRound()
    {
        SetPlayersState("Moving");

        // at round end, call WaitForRound
        countdown.Setup(WaitForRound, 20.0f, "Round ends in");
    }
    void WaitForRound()
    {
        SetPlayersState("Waiting");

        // Transition to Start Wait
        countdown.Setup(StartRound, 5.0f, "Next round starts in");
    }
    void SetPlayersState(string wantedState)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            switch (wantedState)
            {
                case "Moving":
                    players[i].GetComponent<Player>().currentState = players[i].GetComponent<Player>().Moving;
                    break;
                case "Waiting":
                    players[i].GetComponent<Player>().currentState = players[i].GetComponent<Player>().Waiting;
                    break;
                default:
                    Debug.LogError("Invalid state");
                    break;
            }
        }
    }

    # endregion
}
