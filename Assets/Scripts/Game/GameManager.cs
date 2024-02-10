using System;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPun
{
    #region Initialize
    public static GameManager Instance { get; private set; }
    [SerializeField] Countdown countdown;
    [SerializeField] GameObject earPiecePrefab;


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
        countdown.Setup(WaitForFirstRound, 60.0f, "Game starts in");
    }

    void WaitForFirstRound()
    {
        // Once the game starts, make the lobby unjoinable
        LobbyManager.Instance.MakeLobbyUnJoinable();

        // Have players wait
        SetPlayersState("Waiting");

        // Transition to Start Wait
        countdown.Setup(StartRound, 10.0f, "Next round starts in");
    }

    void StartRound()
    {
        // Spawn EARPIECES
        SpawnEarpiecePickups();

        // Allow players to move (This is the main gameplay part)
        SetPlayersState("Moving");

        // at round end, call WaitForRound
        countdown.Setup(WaitForRound, 30.0f, "Round ends in");
    }

    void WaitForRound()
    {
        // Have players wait, in the meantime, check for anyone without an earpiece
        SetPlayersState("Waiting");
        CheckForLoser();

        // Transition to Start Wait
        countdown.Setup(StartRound, 10.0f, "Next round starts in");
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

    int GetPlayerCount()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        return players.Length;
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
                countdown.Setup(LobbyManager.Instance.ShutdownLobby, 30.0f, "A WINNER HAS BEEN DECIDED! Ending game in");
            }
            else
            {
                players[i].GetComponent<Player>().currentState = players[i].GetComponent<Player>().Loser;
                countdown.Setup(StartRound, 25.0f, "NO WINNER! Rematch game starts in");
            }
        }
    }

    void SpawnEarpiecePickups()
    {
        // instantiate pickup prefabs, always make it one less than total players (cuz musical chairs)
        for (int i = 0; i < GetPlayerCount() - 1; i++)
        {
            var position = new Vector3(UnityEngine.Random.Range(-7.29f, 8.41f), 0, UnityEngine.Random.Range(-4.46f, -1.7f));
            Instantiate(earPiecePrefab, position, Quaternion.identity);
        }
    }

    # endregion
}
