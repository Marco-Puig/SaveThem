using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    #region Initialize
    public static GameManager Instance { get; private set; }
    [SerializeField] Countdown countdown;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) { Instance = this; }
    }

    void Start()
    {
        // setup countdown for lobby wait and rounds
        if (photonView.IsMine)
        {
            countdown = this.gameObject.GetComponent<Countdown>();
            photonView.RPC("LobbyWait", RpcTarget.AllBuffered);
        }
    }

    #endregion

    # region Private Methods
    [PunRPC]
    void LobbyWait()
    {
        // wait for players to join for 60 seconds
        countdown.Setup(WaitForRound, 60.0f, "Game starts in");
        LobbyManager.Instance.MakeLobbyUnJoinable();
    }
    void StartRound()
    {
        SetPlayersState("Moving");

        // TODO: INSTANTIATE EARPIECES


        // at round end, call WaitForRound
        countdown.Setup(WaitForRound, 30.0f, "Round ends in");
    }
    void WaitForRound()
    {
        SetPlayersState("Waiting");
        CheckForLoser();

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
    void CheckForLoser()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int playersLeft = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().hasEarpiece)
            {
                playersLeft++;
            }
            else
            {
                players[i].GetComponent<Player>().currentState = players[i].GetComponent<Player>().Loser;
            }
        }

        if (playersLeft <= 1)
        {
            EndGame();
        }
    }
    void EndGame()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().hasEarpiece)
            {
                players[i].GetComponent<Player>().currentState = players[i].GetComponent<Player>().Winner;
                countdown.Setup(LobbyManager.Instance.ShutdownLobby, 60.0f, "A WINNER HAS BEEN DECIDED! Ending game in");
            }
            else
            {
                players[i].GetComponent<Player>().currentState = players[i].GetComponent<Player>().Loser;
                countdown.Setup(StartRound, 25.0f, "NO WINNER! Rematch game starts in");
            }
        }
    }

    # endregion
}
